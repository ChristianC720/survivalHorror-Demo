using System.Collections.Generic;
using Heartbeat.Config;
using Heartbeat.Level;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Heartbeat.AI
{
    /// <summary>
    /// Cadáver del enemigo: tumba la malla en el suelo y arranca la cuenta atrás del
    /// reciclaje. Pasado <see cref="GameConfig.CorpseLifetime"/> empieza a temblar y a
    /// pitar durante <see cref="GameConfig.CorpseWarningDuration"/>; al terminar el aviso,
    /// si el jugador no está en la sala del cuerpo, lo devuelve a otra sala que ninguna
    /// cámara esté viendo, más rápido y con menos disparos encima.
    /// Un cuerpo rematado por el jugador deja de avisar y no se recicla nunca.
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    public class EnemyCorpse : MonoBehaviour
    {
        private const int ToneSampleRate = 44100;

        private static readonly List<EnemyCorpse> Registry = new List<EnemyCorpse>();
        private static readonly Dictionary<int, AudioClip> ToneClips = new Dictionary<int, AudioClip>();

        [Header("Depuración")]
        [Tooltip("Registra la caída, el aviso, la desaparición y el reciclaje en consola.")]
        [SerializeField] private bool logEvents = true;

        private GameConfig config;
        private EnemyHealth health;
        private EnemyAI ai;
        private Transform visual;
        private AudioSource source;
        private RoomVolume corpseRoom;
        private PlayerHealth playerHealth;

        private Vector3 visualBasePosition;
        private Quaternion visualBaseRotation;
        private float bodyRadius = 0.5f;
        private float age;
        private float warningElapsed;
        private float warnTimer;
        private bool allowRecycle;
        private bool warning;
        private bool finished;

        /// <summary>Cadáveres vivos en la escena (los rematados salen de la lista).</summary>
        public static IReadOnlyList<EnemyCorpse> All => Registry;

        /// <summary>Posición del cuerpo en el suelo.</summary>
        public Vector3 Position => transform.position;

        /// <summary>Centro del cuerpo tumbado: a esto apunta el remate.</summary>
        public Vector3 AimPoint => transform.position + Vector3.up * bodyRadius;

        /// <summary>El cuerpo ya fue rematado: se queda en el suelo y no vuelve.</summary>
        public bool IsFinished => finished;

        /// <summary>Se puede rematar (todavía no lo está).</summary>
        public bool CanBeFinished => !finished;

        /// <summary>Enemigo al que pertenece el cuerpo.</summary>
        public EnemyAI Enemy => ai;

        private void OnEnable()
        {
            Purge();
            if (!Registry.Contains(this)) Registry.Add(this);
        }

        private void OnDisable()
        {
            Registry.Remove(this);
        }

        private static void Purge()
        {
            for (int i = Registry.Count - 1; i >= 0; i--)
            {
                if (Registry[i] == null) Registry.RemoveAt(i);
            }
        }

        /// <summary>Crea (o reutiliza) el cadáver del enemigo y lo tumba en el suelo.</summary>
        public static EnemyCorpse Create(EnemyHealth health, EnemyAI ai, GameConfig config, bool allowRecycle)
        {
            if (health == null) return null;

            EnemyCorpse corpse = health.GetComponent<EnemyCorpse>();
            if (corpse == null) corpse = health.gameObject.AddComponent<EnemyCorpse>();

            corpse.Initialize(health, ai, config, allowRecycle);
            return corpse;
        }

        private void Initialize(EnemyHealth newHealth, EnemyAI newAI, GameConfig newConfig, bool newAllowRecycle)
        {
            health = newHealth;
            ai = newAI;
            config = newConfig;
            allowRecycle = newAllowRecycle;

            age = 0f;
            warningElapsed = 0f;
            warnTimer = 0f;
            warning = false;
            finished = false;

            FindVisual();
            ResolveAudio();
            TiltBody();

            corpseRoom = RoomVolume.FindContaining(transform.position + Vector3.up * 0.5f);
            if (!Registry.Contains(this)) Registry.Add(this);

            if (!logEvents || config == null) return;

            string type = ai != null ? EnemyTypeUtil.DisplayName(ai.Type) : "Enemigo";
            Debug.Log($"[Cadáver] {name} ({type}) cae; aviso de reciclaje a los {config.CorpseLifetime:F0} s"
                + (allowRecycle ? "." : "; cuerpo ya gastado, esta vez no vuelve."));
        }

        private void FindVisual()
        {
            if (visual != null) return;

            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer == null) return;

            visual = renderer.transform;
            visualBasePosition = visual.localPosition;
            visualBaseRotation = visual.localRotation;
        }

        private void ResolveAudio()
        {
            if (source == null) source = GetComponent<AudioSource>();
            if (source == null) source = gameObject.AddComponent<AudioSource>();
            if (config == null) return;

            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1f;
            source.maxDistance = config.EnemyBeepMaxDistance;
        }

        /// <summary>La cápsula cae de costado: 90° sobre X y el centro a la altura del radio.</summary>
        private void TiltBody()
        {
            if (ai != null)
            {
                EnemyProfile profile = ai.Profile;
                bodyRadius = Mathf.Max(0.1f, profile.capsuleRadius);
            }

            if (visual == null) return;

            visual.localRotation = Quaternion.Euler(90f, 0f, 0f);
            visual.localPosition = LyingPosition;
        }

        /// <summary>Posición local del cuerpo tumbado (centro a la altura del radio).</summary>
        private Vector3 LyingPosition => new Vector3(visualBasePosition.x, bodyRadius, visualBasePosition.z);

        private void Update()
        {
            if (config == null || finished) return;

            float deltaTime = Time.deltaTime;

            if (!warning)
            {
                age += deltaTime;
                if (age < config.CorpseLifetime) return;

                if (!allowRecycle)
                {
                    // Cuerpo gastado: se retira en silencio al cumplir su tiempo.
                    if (logEvents) Debug.Log($"[Cadáver] {name}: cuerpo gastado, se retira.");
                    Destroy(gameObject);
                    return;
                }

                warning = true;
                warningElapsed = 0f;
                warnTimer = 0f;
                if (logEvents) Debug.Log($"[Cadáver] {name}: empieza el aviso de reciclaje.");
            }

            warningElapsed += deltaTime;
            Shake();

            warnTimer -= deltaTime;
            if (warnTimer > 0f) return;

            warnTimer = config.CorpseWarningInterval;
            PlayWarningTone();

            if (warningElapsed >= config.CorpseWarningDuration) TryRecycle();
        }

        private void Shake()
        {
            if (visual == null) return;

            float period = Mathf.Max(0.01f, config.CorpseWarningInterval);
            float offset = Mathf.Sin(Time.time * Mathf.PI * 2f / period) * config.CorpseShakeAmplitude;
            visual.localPosition = LyingPosition + Vector3.up * offset;
        }

        private void PlayWarningTone()
        {
            if (config == null) return;
            if (source == null) ResolveAudio();
            if (source == null) return;

            AudioClip clip = GetTone(config.CorpseWarningTone);
            if (clip == null) return;

            source.PlayOneShot(clip, config.CorpseWarningVolume);
        }

        private AudioClip GetTone(int frequency)
        {
            if (ToneClips.TryGetValue(frequency, out AudioClip cached) && cached != null) return cached;
            if (config == null) return null;

            float duration = config.EnemyBeepDuration;
            int sampleCount = Mathf.Max(1, Mathf.RoundToInt(ToneSampleRate * duration));
            float rampSamples = ToneSampleRate * 0.01f;

            float[] data = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float time = (float)i / ToneSampleRate;
                float attack = Mathf.Clamp01(i / rampSamples);
                float release = Mathf.Clamp01(1f - (float)i / sampleCount);
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * time) * attack * release * 0.5f;
            }

            AudioClip clip = AudioClip.Create($"AvisoReciclaje_{frequency}", sampleCount, 1, ToneSampleRate, false);
            clip.SetData(data, 0);

            ToneClips[frequency] = clip;
            return clip;
        }

        /// <summary>
        /// Remate: el cuerpo deja de avisar y ya no se recicla. Se queda en el suelo.
        /// Devuelve false si ya estaba rematado.
        /// </summary>
        public bool Finish(bool stomp)
        {
            if (finished) return false;

            finished = true;
            warning = false;
            Registry.Remove(this);

            if (visual != null) visual.localPosition = LyingPosition;

            if (logEvents)
            {
                Debug.Log($"[Remate] {name}: {(stomp ? "pisotón" : "punzón")}; el cuerpo queda inerte y no se recicla.");
            }

            return true;
        }

        // ---------------------------------------------------------------- reciclaje

        private bool TryRecycle()
        {
            if (!allowRecycle || finished || config == null) return false;
            if (health == null || ai == null) return false;

            // El salto no puede verse: con el jugador en la sala del cuerpo no se intenta.
            RoomVolume playerRoom = PlayerRoom();
            if (playerRoom != null && playerRoom == corpseRoom) return false;

            if (!TryFindUnseenPoint(out Vector3 point)) return false;

            RestoreBody();

            int shots = config.GetRecycledShotsToKill(ai.Type);
            health.ResetForRecycle(shots);
            ai.BecomeRecycled(point, config.RecycledSpeedMultiplier);

            if (logEvents)
            {
                RoomVolume room = RoomVolume.FindContaining(point + Vector3.up * 0.5f);
                Debug.Log($"[Reciclaje] {name} vuelve en {(room != null ? room.RoomId : "el nivel")}"
                    + $" a {config.RecycledSpeedMultiplier:F2}× con {shots} disparo(s).");
            }

            Destroy(this);
            return true;
        }

        private RoomVolume PlayerRoom()
        {
            if (playerHealth == null) playerHealth = FindAnyObjectByType<PlayerHealth>();
            if (playerHealth == null) return null;

            return RoomVolume.FindContaining(playerHealth.transform.position);
        }

        /// <summary>Punto de NavMesh en otra sala que ninguna cámara activa esté viendo.</summary>
        private bool TryFindUnseenPoint(out Vector3 point)
        {
            point = Vector3.zero;

            IReadOnlyList<RoomVolume> rooms = RoomVolume.All;
            if (rooms.Count == 0) return false;

            EnemyDirector director = EnemyDirector.Instance;
            int start = Random.Range(0, rooms.Count);

            for (int i = 0; i < rooms.Count; i++)
            {
                RoomVolume room = rooms[(start + i) % rooms.Count];
                if (room == null || room.IsSafeZone) continue;
                if (room == corpseRoom) continue;
                if (director != null && !director.CanEnterRoom(room, ai)) continue;
                if (!TrySamplePoint(room, out Vector3 candidate)) continue;
                if (IsSeenByAnyCamera(candidate)) continue;

                point = candidate;
                return true;
            }

            return false;
        }

        private bool TrySamplePoint(RoomVolume room, out Vector3 point)
        {
            point = Vector3.zero;
            if (config == null) return false;

            BoxCollider box = room.GetComponent<BoxCollider>();
            if (box == null) return false;

            for (int i = 0; i < config.EnemyPatrolSampleAttempts; i++)
            {
                Vector3 local = new Vector3(
                    Random.Range(-box.size.x * 0.4f, box.size.x * 0.4f),
                    0f,
                    Random.Range(-box.size.z * 0.4f, box.size.z * 0.4f));

                Vector3 candidate = room.transform.TransformPoint(box.center + local);
                if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, config.EnemyPatrolSampleRadius, NavMesh.AllAreas)) continue;

                RoomVolume hitRoom = RoomVolume.FindContaining(hit.position);
                if (hitRoom != room) continue;
                if (hitRoom.IsSafeZone) continue;

                point = hit.position;
                return true;
            }

            return false;
        }

        private bool IsSeenByAnyCamera(Vector3 worldPoint)
        {
            if (config == null) return true;

            Camera[] cameras = Camera.allCameras;
            Vector3 probe = worldPoint + Vector3.up * (config.PlayerHeight * 0.5f);

            for (int i = 0; i < cameras.Length; i++)
            {
                Camera cam = cameras[i];
                if (cam == null) continue;

                Vector3 viewport = cam.WorldToViewportPoint(probe);
                if (viewport.z <= 0f) continue;
                if (viewport.x < 0f || viewport.x > 1f) continue;
                if (viewport.y < 0f || viewport.y > 1f) continue;

                // Dentro del encuadre: sólo cuenta como visto si no hay nada de por medio.
                if (!Physics.Linecast(cam.transform.position, probe, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore)) return true;
                if (hit.collider == null) return true;
            }

            return false;
        }

        private void RestoreBody()
        {
            if (visual == null) return;

            visual.localPosition = visualBasePosition;
            visual.localRotation = visualBaseRotation;
        }
    }
}
