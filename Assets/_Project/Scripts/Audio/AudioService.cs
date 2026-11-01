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
    /// CuePlayed; cues without clips show their debug caption in development builds.
    /// </summary>
    public sealed class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioCue[] cues = new AudioCue[0];
        [SerializeField] private int voices = 24;
        [Tooltip("Alert cues only for enemies this close (same scope as the AlertDirector).")]
        [SerializeField] private float alertCueRadius = 25f;
        [SerializeField] private bool showDebugCaptions = true;

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
        private AudioSource surveillanceLoop;
        private AlertState signaled;

        private readonly List<(string text, float until)> captions = new List<(string, float)>();

        private void Awake()
        {
            foreach (var c in cues) if (c != null) byName[c.name] = c;
            pool = new AudioSource[voices];
            busy = new bool[voices];
            prio = new int[voices];
            started = new float[voices];
            follow = new Transform[voices];
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
        }

        private void Start()
        {
            if (Services.TryGet(out Save.SaveService saves)) ApplyVolumes(saves.LoadGlobal());
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
            GameEvents.ShotFired += OnShot;
            GameEvents.EnemyAlertChanged += OnEnemyAlert;
            GameEvents.HighestAlertChanged += OnHighestAlert;
            GameEvents.RecycleWarning += OnRecycleWarning;
            GameEvents.StaminaExhausted += OnStaminaExhausted;
            GameEvents.Autosaved += OnAutosaved;
            GameEvents.Reconstructed += OnReconstructed;
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
            GameEvents.ShotFired -= OnShot;
            GameEvents.EnemyAlertChanged -= OnEnemyAlert;
            GameEvents.HighestAlertChanged -= OnHighestAlert;
            GameEvents.RecycleWarning -= OnRecycleWarning;
            GameEvents.StaminaExhausted -= OnStaminaExhausted;
            GameEvents.Autosaved -= OnAutosaved;
            GameEvents.Reconstructed -= OnReconstructed;
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
            src.volume = cue.volume;
            src.pitch = Random.Range(cue.pitchRange.x, cue.pitchRange.y);
            src.loop = loop;
            src.clip = cue.PickClip();
            busy[i] = true;
            prio[i] = (int)cue.priority;
            started[i] = Time.unscaledTime;
            follow[i] = target;
            if (src.clip != null) src.Play();
            else if (showDebugCaptions && Debug.isDebugBuild && !string.IsNullOrEmpty(cue.debugCaption))
                captions.Add((cue.debugCaption, Time.unscaledTime + 2f));

            if (cue.priority == CuePriority.Signal) duckUntil = Mathf.Max(duckUntil, Time.unscaledTime + duckHoldSeconds);
            HbLog.Info("Audio", cue.name);
            GameEvents.RaiseCuePlayed(cue.name, spatial ? position : null);
            return src;
        }

        private void Play(string id, Vector3? position)
        {
            if (byName.TryGetValue(id, out var cue)) Play(cue, position);
            else HbLog.Warn("Audio", $"Unknown cue {id}");
        }

        private void Update()
        {
            float now = Time.unscaledTime;
            for (int i = 0; i < pool.Length; i++)
            {
                if (!busy[i]) continue;
                if (follow[i] != null) pool[i].transform.position = follow[i].position;
                // One-shots free their voice when done; empty-clip one-shots hold it ~0.5 s so stealing stays realistic.
                if (!pool[i].loop && !pool[i].isPlaying && now - started[i] > 0.5f) busy[i] = false;
            }
            captions.RemoveAll(c => c.until < now);

            if (mixer != null)
            {
                float duck = MixMath.Duck(now - duckUntil, duckDb, duckReleaseSeconds);
                mixer.SetFloat("vol_ambience", MixMath.LinearToDb(volAmbience) + duck);
                mixer.SetFloat("vol_music", MixMath.LinearToDb(volMusic) + duck);
            }
        }

        private void OnGUI()
        {
            if (captions.Count == 0) return;
            var style = new GUIStyle(GUI.skin.box) { fontSize = 14, alignment = TextAnchor.MiddleLeft };
            for (int i = 0; i < captions.Count; i++)
                GUI.Box(new Rect(Screen.width - 330, 10 + i * 26, 320, 24), "♪ " + captions[i].text, style);
        }

        // ---------- Event → cue table (AUDIO_CUES.md) ----------

        private void OnCueRequested(string id, Vector3? pos) => Play(id, pos);
        private void OnShot(Vector3 origin) => Play("CUE_Revolver_Shot", origin);
        private void OnRecycleWarning(Vector3 pos) => Play("CUE_Recycle_Warning", pos);
        private void OnStaminaExhausted() => Play("CUE_Lukas_Gasp", null);
        private void OnAutosaved() => Play("CUE_Autosave_Sync", null);
        private void OnReconstructed(int _) => Play("CUE_Capsule_Reconstruction", null);
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
    }
}
