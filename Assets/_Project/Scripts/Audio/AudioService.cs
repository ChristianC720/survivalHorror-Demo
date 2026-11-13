using System.Collections.Generic;
using Heartbeat.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace Heartbeat.Audio
{
    /// <summary>
    /// S-11 audio service, lives on the Bootstrap object (registered by Bootstrap). Pooled AudioSources with priority
    /// voice stealing (<see cref="VoicePicker"/>). Gameplay reaches it only through events: <see cref="Sfx"/> requests,
    /// plus the event → cue table in <see cref="OnEnable"/>. Every play logs "[Heartbeat][Audio] CUE_X" and raises
    /// CuePlayed (UI.SoundCaptionView shows captions from it).
    /// </summary>
    public sealed class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioCue[] cues = new AudioCue[0];
        [SerializeField] private int voices = 24;
        [Tooltip("Alert cues only for enemies this close (same scope as the AlertDirector).")]
        [SerializeField] private float alertCueRadius = 25f;
        [SerializeField] private AudioBalance balance;

        [Header("Mixer (Audio/Mixer/Heartbeat.mixer)")]
        [SerializeField] private AudioMixer mixer;
        [Tooltip("Signals (P1) duck Ambience and Music by this many dB (M-10: signals stay readable).")]
        [SerializeField] private float duckDb = -8f;
        [SerializeField] private float duckHoldSeconds = 0.6f;
        [SerializeField] private float duckReleaseSeconds = 1.5f;

        private float volAmbience = 1f, volMusic = 1f;
        private float duckUntil = -10f;

        private readonly Dictionary<string, AudioCue> byName = new Dictionary<string, AudioCue>();
        private AudioSource[] pool;
        private bool[] busy;
        private int[] prio;
        private float[] started;
        private Transform[] follow;
        private float[] baseVolume;
        private bool[] occluded;
        private float nextOcclusion;
        private AudioListener listener, fallbackListener;
        private float nextListenerCheck;
        private float[] fadeGain, fadeRate;   // fadeRate 0 = no fade; reaching silence stops the voice
        private bool[] fadesAtEnd;
        private AudioSource surveillanceLoop;
        private AlertState signaled;


        private void Awake()
        {
            foreach (var c in cues) if (c != null) byName[c.name] = c;
            pool = new AudioSource[voices];
            busy = new bool[voices];
            prio = new int[voices];
            started = new float[voices];
            follow = new Transform[voices];
            baseVolume = new float[voices];
            occluded = new bool[voices];
            fadeGain = new float[voices];
            fadeRate = new float[voices];
            fadesAtEnd = new bool[voices];
            for (int i = 0; i < voices; i++)
            {
                var go = new GameObject($"Voice_{i:00}");
                go.transform.SetParent(transform, false);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.rolloffMode = AudioRolloffMode.Linear;
                src.minDistance = 1f;
                pool[i] = src;
            }
            fallbackListener = gameObject.AddComponent<AudioListener>();
            if (balance != null)
            {
                gameObject.AddComponent<ThreatAudio>().Init(this, balance);
                gameObject.AddComponent<LabHeartbeat>().Init(this, balance);
            }
            else HbLog.Warn("Audio", "No AudioBalance: heartbeats disabled");
        }

        private int degradation;

        private void OnDegradation(int level) => degradation = level;

        private void Start() => OnOptions();

        private void OnOptions()
        {
            if (!Services.TryGet(out Save.SaveService saves)) return;
            var g = saves.LoadGlobal();
            ApplyVolumes(g);
        }

        /// <summary>Options volumes (0..1) → exposed mixer params. Ambience/Music go through the duck in Update.</summary>
        public void ApplyVolumes(Save.GlobalData g)
        {
            if (mixer == null || g == null) return;
            mixer.SetFloat("vol_master", MixMath.LinearToDb(g.volMaster));
            mixer.SetFloat("vol_sfx", MixMath.LinearToDb(g.volSfx));
            mixer.SetFloat("vol_voices", MixMath.LinearToDb(g.volVoices));
            volAmbience = g.volAmbience;
            volMusic = g.volMusic;
        }

        private void OnEnable()
        {
            GameEvents.CueRequested += OnCueRequested;
            GameEvents.CueStopRequested += StopCue;
            GameEvents.OptionsChanged += OnOptions;
            GameEvents.DegradationChanged += OnDegradation;
            GameEvents.ShotFired += OnShot;
            GameEvents.EnemyAlertChanged += OnEnemyAlert;
            GameEvents.HighestAlertChanged += OnHighestAlert;
            GameEvents.RecycleWarning += OnRecycleWarning;
            GameEvents.StaminaExhausted += OnStaminaExhausted;
            GameEvents.Autosaved += OnAutosaved;
            GameEvents.ItemRegistered += OnItemRegistered;
            GameEvents.EnemyKilled += OnEnemyKilled;
            GameEvents.PlayerAttacked += OnPlayerAttacked;
            GameEvents.GrabResolved += OnGrabResolved;
            GameEvents.PowerRestored += OnPowerRestored;
            GameEvents.PlaySubStateChanged += OnSubState;
        }

        private void OnDisable()
        {
            GameEvents.CueRequested -= OnCueRequested;
            GameEvents.CueStopRequested -= StopCue;
            GameEvents.OptionsChanged -= OnOptions;
            GameEvents.DegradationChanged -= OnDegradation;
            GameEvents.ShotFired -= OnShot;
            GameEvents.EnemyAlertChanged -= OnEnemyAlert;
            GameEvents.HighestAlertChanged -= OnHighestAlert;
            GameEvents.RecycleWarning -= OnRecycleWarning;
            GameEvents.StaminaExhausted -= OnStaminaExhausted;
            GameEvents.Autosaved -= OnAutosaved;
            GameEvents.ItemRegistered -= OnItemRegistered;
            GameEvents.EnemyKilled -= OnEnemyKilled;
            GameEvents.PlayerAttacked -= OnPlayerAttacked;
            GameEvents.GrabResolved -= OnGrabResolved;
            GameEvents.PowerRestored -= OnPowerRestored;
            GameEvents.PlaySubStateChanged -= OnSubState;
        }

        // ---------- API ----------

        public bool TryGetCue(string id, out AudioCue cue) => byName.TryGetValue(id, out cue);

        /// <summary>One-shot. <paramref name="position"/> null = 2D (also forced for non-spatial cues).</summary>
        public AudioSource Play(AudioCue cue, Vector3? position = null) => Begin(cue, position, null, cue != null && cue.loop);

        /// <summary>Looping cue that follows <paramref name="target"/> (null = 2D). Stop it with <see cref="Stop"/>.</summary>
        public AudioSource StartLoop(AudioCue cue, Transform target = null) => Begin(cue, target != null ? target.position : (Vector3?)null, target, true);

        public void Stop(AudioSource voice)
        {
            if (voice == null) return;
            int i = System.Array.IndexOf(pool, voice);
            if (i < 0) return;
            voice.Stop();
            busy[i] = false;
            follow[i] = null;
        }

        /// <summary>Fades out every voice playing one of the cue's clips (<see cref="Sfx.Stop"/>).</summary>
        public void StopCue(string id)
        {
            if (!byName.TryGetValue(id, out var cue)) return;
            for (int i = 0; i < pool.Length; i++)
                if (busy[i] && pool[i].clip != null && System.Array.IndexOf(cue.clips, pool[i].clip) >= 0) fadeRate[i] = -1f / CueStopFadeSeconds;
        }

        private const float CueStopFadeSeconds = 0.2f;   // just long enough not to click

        /// <summary>Music with a fade-in, crossfading from <paramref name="previous"/>. <paramref name="keepPosition"/> continues at the
        /// same point of the bar: the boss loops have the same length, so a phase change links on the beat (D-87).</summary>
        public AudioSource StartMusic(AudioCue cue, AudioSource previous = null, bool keepPosition = false)
        {
            CutCarriedMusic();   // any new music is an action that cuts an ending's music
            var src = Play(cue);
            int previousSamples = previous != null ? previous.timeSamples : 0;
            FadeOutMusic(previous);
            if (src == null) return null;
            int i = System.Array.IndexOf(pool, src);
            fadeGain[i] = 0f;
            fadeRate[i] = 1f / FadeSeconds;
            fadesAtEnd[i] = !src.loop;
            src.volume = 0f;
            if (keepPosition && src.clip != null) src.timeSamples = Mathf.Min(previousSamples, src.clip.samples - 1);
            return src;
        }

        /// <summary>
        /// An ending's music keeps playing into the main menu until it ends or new music cuts it (user 2026-11-09). The menu
        /// waits for <see cref="CarriedMusicPlaying"/> to turn false, then for <see cref="MenuMusicDelay"/>, before its theme.
        /// </summary>
        public void CarryMusic(AudioSource voice) { carried = voice; carriedClip = voice != null ? voice.clip : null; }

        public bool CarriedMusicPlaying => carried != null && carried.clip == carriedClip && IsBusy(carried);   // a reused voice is not the ending

        public float MenuMusicDelay => balance != null ? balance.menuMusicDelayAfterEnding : 2f;

        public void CutCarriedMusic()
        {
            var c = carried;
            carried = null;
            if (c != null && c.clip == carriedClip && IsBusy(c)) FadeOutMusic(c);
        }

        private AudioSource carried;
        private AudioClip carriedClip;

        private bool IsBusy(AudioSource voice)
        {
            int i = System.Array.IndexOf(pool, voice);
            return i >= 0 && busy[i] && voice.isPlaying;
        }

        /// <summary>Fades the voice to silence, then frees it.</summary>
        public void FadeOutMusic(AudioSource voice)
        {
            if (voice == null) return;
            int i = System.Array.IndexOf(pool, voice);
            if (i >= 0 && busy[i]) fadeRate[i] = -1f / FadeSeconds;
        }

        private float FadeSeconds => Mathf.Max(0.01f, balance != null ? balance.musicFadeSeconds : 2f);

        private void UpdateFade(int i)
        {
            var src = pool[i];
            if (fadesAtEnd[i] && src.clip != null && fadeRate[i] >= 0f && src.clip.length - src.time <= FadeSeconds) fadeRate[i] = -1f / FadeSeconds;
            if (fadeRate[i] == 0f) return;
            fadeGain[i] = Mathf.Clamp01(fadeGain[i] + fadeRate[i] * Time.unscaledDeltaTime);
            src.volume = baseVolume[i] * fadeGain[i];
            if (fadeGain[i] >= 1f) fadeRate[i] = 0f;
            else if (fadeGain[i] <= 0f) Stop(src);
        }

        private AudioSource Begin(AudioCue cue, Vector3? position, Transform target, bool loop)
        {
            if (cue == null) return null;
            int i = VoicePicker.Pick(busy, prio, started, (int)cue.priority);
            if (i < 0)
            {
                HbLog.Dev("Audio", $"Dropped {cue.name} (all voices busy with higher priority)");
                return null;
            }

            var src = pool[i];
            src.Stop();
            bool spatial = cue.spatial && position.HasValue;
            src.transform.position = position ?? Vector3.zero;
            src.spatialBlend = spatial ? 1f : 0f;
            src.maxDistance = cue.maxDistance;
            src.outputAudioMixerGroup = cue.bus;
            baseVolume[i] = cue.volume;
            // M-10 signals are never masked (D-29): only ordinary positional sounds are occluded.
            occluded[i] = spatial && cue.priority != CuePriority.Signal;
            src.volume = occluded[i] ? cue.volume * Occlusion(src.transform.position) : cue.volume;
            src.pitch = Random.Range(cue.pitchRange.x, cue.pitchRange.y);
            // M-18 vocal distortion: voices drop and waver with degradation; signals are never touched (D-29).
            if (cue.name.StartsWith("CUE_Vocal_")) src.pitch *= Mathf.Max(0.75f, 1f - 0.025f * degradation) * Random.Range(1f - 0.01f * degradation, 1f);
            src.loop = loop;
            src.ignoreListenerPause = cue.priority == CuePriority.Music;   // reading or pausing must not cut the music dead
            src.clip = cue.PickClip();
            fadeGain[i] = 1f;
            fadeRate[i] = 0f;
            fadesAtEnd[i] = false;
            busy[i] = true;
            prio[i] = (int)cue.priority;
            started[i] = Time.unscaledTime;
            follow[i] = target;
            if (src.clip != null) src.Play();

            if (cue.priority == CuePriority.Signal) duckUntil = Mathf.Max(duckUntil, Time.unscaledTime + duckHoldSeconds);
            LogCue(cue.name);
            GameEvents.RaiseCuePlayed(cue.name, spatial ? position : null);
            return src;
        }

        /// <summary>Share of a sound at <paramref name="source"/> that reaches the listener through walls and closed doors.</summary>
        public float Occlusion(Vector3 source)
        {
            return listener == null ? 1f : OcclusionBetween(source, listener.transform.position);
        }

        /// <summary>Same rule between any two points (also the noise radius enemies perceive, S-02).</summary>
        public float OcclusionBetween(Vector3 a, Vector3 b)
        {
            if (balance == null) return 1f;
            SoundOcclusion.Count(a, b, out int walls, out int doors);
            return OcclusionModel.Factor(walls, doors, balance.wallTransmission, balance.doorTransmission);
        }

        private void Play(string id, Vector3? position)
        {
            if (byName.TryGetValue(id, out var cue)) Play(cue, position);
            else HbLog.Warn("Audio", $"Unknown cue {id}");
        }

        private void Update()
        {
            float now = Time.unscaledTime;
            if (now >= nextListenerCheck)
            {
                nextListenerCheck = now + 0.2f;
                RefreshListener();
            }
            for (int i = 0; i < pool.Length; i++)
            {
                if (!busy[i]) continue;
                if (follow[i] != null) pool[i].transform.position = follow[i].position;
                UpdateFade(i);
                // One-shots free their voice when done; empty-clip one-shots hold it ~0.5 s so stealing stays realistic.
                if (!pool[i].loop && !pool[i].isPlaying && now - started[i] > 0.5f) busy[i] = false;
            }

            if (balance != null && now >= nextOcclusion)
            {
                nextOcclusion = now + balance.occlusionInterval;
                for (int i = 0; i < pool.Length; i++)
                    if (busy[i] && occluded[i]) pool[i].volume = baseVolume[i] * Occlusion(pool[i].transform.position);
            }

            if (mixer != null)
            {
                float duck = MixMath.Duck(now - duckUntil, duckDb, duckReleaseSeconds);
                mixer.SetFloat("vol_ambience", MixMath.LinearToDb(volAmbience) + duck);
                mixer.SetFloat("vol_music", MixMath.LinearToDb(volMusic) + duck);
            }
        }

        /// <summary>Only the player camera has a listener; menus and cinematics need this one or Unity outputs silence.</summary>
        private void RefreshListener()
        {
            AudioListener camera = null;
            foreach (var l in FindObjectsByType<AudioListener>())
                if (l != fallbackListener && l.isActiveAndEnabled) camera = l;
            fallbackListener.enabled = camera == null;
            listener = camera != null ? camera : fallbackListener;
        }

        // ---------- Event → cue table (AUDIO_CUES.md) ----------

        private void OnCueRequested(string id, Vector3? pos) => Play(id, pos);
        private void OnShot(Vector3 origin) => Play("CUE_Revolver_Shot", origin);
        private void OnRecycleWarning(Vector3 pos) => Play("CUE_Recycle_Warning", pos);
        private void OnStaminaExhausted() => Play("CUE_Lukas_Gasp", null);
        private void OnAutosaved() => Play("CUE_Autosave_Sync", null);
        // Capsule_Reconstruction is not on this table: CycleController, IntroDirector and MotherDirector play it with their shot and stop it (C-67).
        private void OnItemRegistered(string _, int __, bool isNew) { if (isNew) Play("CUE_Terminal_Register", null); }
        private void OnEnemyKilled(MonoBehaviour _, Vector3 pos) => Play("CUE_Enemy_Death", pos);
        private void OnPlayerAttacked(AttackInfo a) => Play(a.IsGrab ? "CUE_Enemy_Grab" : "CUE_Enemy_Attack", a.From);
        private void OnGrabResolved(bool countered) { if (countered) Play(Sfx.SpikeDischarge, null); }
        private void OnPowerRestored() => Play(Sfx.PowerStartup, null);

        /// <summary>Alert cues sound only when an enemy raises the highest nearby state (M-10, D-23).</summary>
        private void OnEnemyAlert(MonoBehaviour enemy, AlertState state, float distance)
        {
            if (state == AlertState.Patrol || state <= signaled || distance > alertCueRadius) return;
            signaled = state;
            Play("CUE_Alert_" + state, enemy.transform.position);
        }

        private void OnHighestAlert(AlertState s) => signaled = s;

        private void OnSubState(PlaySubState from, PlaySubState to)
        {
            if (to == PlaySubState.Surveillance && surveillanceLoop == null && byName.TryGetValue("CUE_Surveillance_Static", out var c))
                surveillanceLoop = StartLoop(c);
            else if (from == PlaySubState.Surveillance) { Stop(surveillanceLoop); surveillanceLoop = null; }
        }

        // Same "[Heartbeat][Audio] CUE_X" line playtests assert on, but stripped from release builds: every footstep
        // logging with a stack trace was the biggest per-frame GC source (E13-profiling).
        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEBUG")]
        private static void LogCue(string name) => HbLog.Info("Audio", name);
    }
}
