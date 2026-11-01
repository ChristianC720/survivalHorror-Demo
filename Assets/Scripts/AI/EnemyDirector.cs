using System.Collections.Generic;
using Heartbeat.Config;
using Heartbeat.Level;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.AI
{
    /// <summary>
    /// Servicio único de la IA: registro de enemigos, cupo por sala, arbitraje de beeps
    /// (con varios enemigos suena el estado más alto) y modo debug F1 que colorea la
    /// cápsula según el estado. El debug arranca desactivado.
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public class EnemyDirector : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;

        [Header("Depuración")]
        [Tooltip("Registra en consola las transiciones, los beeps y el cupo por sala.")]
        [SerializeField] private bool logEvents = true;

        private const int BeepSampleRate = 44100;

        private struct BeepRequest
        {
            public EnemyState State;
            public Vector3 Position;
            public string SourceName;
        }

        private static readonly List<EnemyAI> Registered = new List<EnemyAI>();
        private static readonly Dictionary<int, AudioClip> ToneClips = new Dictionary<int, AudioClip>();

        private readonly List<BeepRequest> beepRequests = new List<BeepRequest>();

        private AudioSource beepSource;
        private float beepWindowStart;
        private float motherAttention01;
        private bool validatedRooms;

        /// <summary>Instancia activa en la escena.</summary>
        public static EnemyDirector Instance { get; private set; }

        /// <summary>Modo debug de percepción (F1). Apagado por defecto.</summary>
        public static bool DebugEnabled { get; private set; }

        public IReadOnlyList<EnemyAI> Enemies => Registered;

        private void Awake()
        {
            Instance = this;
            DebugEnabled = false;

            // Los estáticos sobreviven entre sesiones de Play cuando no se recarga el dominio:
            // se limpian al arrancar para que el registro no arrastre enemigos ya destruidos.
            Registered.Clear();

            beepSource = GetComponent<AudioSource>();
            if (beepSource == null) beepSource = gameObject.AddComponent<AudioSource>();

            beepSource.playOnAwake = false;
            beepSource.loop = false;
            beepSource.spatialBlend = 1f;
            beepSource.rolloffMode = AudioRolloffMode.Linear;
            beepSource.maxDistance = config != null ? config.EnemyBeepMaxDistance : 25f;
            beepSource.minDistance = 1f;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            HandleDebugToggle();
            FlushBeeps();

            if (!validatedRooms) ValidateRoomCap();
        }

        // ---------------------------------------------------------------- registro

        public void Register(EnemyAI enemy)
        {
            if (enemy == null) return;

            // Un enemigo destruido sigue "contenido" por identidad reciclada: se purga antes.
            Registered.RemoveAll(IsDestroyed);

            if (Registered.Contains(enemy)) return;
            Registered.Add(enemy);
        }

        private static bool IsDestroyed(EnemyAI enemy)
        {
            return enemy == null;
        }

        public void Unregister(EnemyAI enemy)
        {
            if (enemy == null) return;
            Registered.Remove(enemy);
        }

        /// <summary>Enemigos activos que están ahora mismo dentro de la sala.</summary>
        public int EnemiesInRoom(RoomVolume room)
        {
            if (room == null) return 0;

            int count = 0;
            for (int i = 0; i < Registered.Count; i++)
            {
                EnemyAI enemy = Registered[i];
                if (enemy == null || !enemy.isActiveAndEnabled) continue;
                if (enemy.CurrentRoom == room) count++;
            }
            return count;
        }

        /// <summary>¿Cabe otro enemigo en la sala? Nunca se permite la zona segura.</summary>
        public bool CanEnterRoom(RoomVolume room, EnemyAI requester)
        {
            if (room == null) return true;
            if (room.IsSafeZone) return false;

            int max = config != null ? config.EnemyMaxPerRoom : 2;
            int count = 0;

            for (int i = 0; i < Registered.Count; i++)
            {
                EnemyAI enemy = Registered[i];
                if (enemy == null || enemy == requester || !enemy.isActiveAndEnabled) continue;
                if (enemy.CurrentRoom == room) count++;
            }

            return count < max;
        }

        public bool IsSafeZoneAt(Vector3 worldPosition)
        {
            RoomVolume room = RoomVolume.FindContaining(worldPosition);
            return room != null && room.IsSafeZone;
        }

        /// <summary>
        /// Estado más alto de los enemigos vivos de la sala (y, si se pide, de sus adyacentes).
        /// Sin enemigos devuelve Patrulla. Lo usa la vista de vigilancia para degradar la señal.
        /// </summary>
        public EnemyState HighestStateIn(RoomVolume room, bool includeAdjacent)
        {
            EnemyState highest = EnemyState.Patrulla;
            if (room == null) return highest;

            for (int i = 0; i < Registered.Count; i++)
            {
                EnemyAI enemy = Registered[i];
                if (enemy == null || !enemy.isActiveAndEnabled) continue;

                RoomVolume enemyRoom = enemy.CurrentRoom;
                bool inScope = enemyRoom == room
                    || (includeAdjacent && enemyRoom != null && room.IsAdjacentTo(enemyRoom));

                if (!inScope) continue;
                if (EnemyStateUtil.Priority(enemy.State) > EnemyStateUtil.Priority(highest)) highest = enemy.State;
            }

            return highest;
        }

        // ------------------------------------------------- atención de The Mother

        /// <summary>Atención de The Mother (0-1): medidor oculto y único para el nivel.</summary>
        public float MotherAttention01 => motherAttention01;

        /// <summary>
        /// Sube mientras se usa la vista de vigilancia y baja al soltarla. Al llenarse, los
        /// enemigos en Patrulla de la sala del jugador y de las adyacentes pasan a Sospecha
        /// y luego a Búsqueda hacia su posición; el medidor vuelve a cero.
        /// </summary>
        public void TickMotherAttention(bool surveillanceActive, Vector3 playerPosition, float deltaTime)
        {
            if (config == null || deltaTime <= 0f) return;

            float rate = surveillanceActive
                ? 1f / Mathf.Max(0.01f, config.MotherAttentionFillDuration)
                : -1f / Mathf.Max(0.01f, config.MotherAttentionDrainDuration);

            motherAttention01 = Mathf.Clamp01(motherAttention01 + rate * deltaTime);
            if (motherAttention01 < 1f) return;

            motherAttention01 = 0f;
            RaiseMotherAttention(playerPosition);
        }

        private void RaiseMotherAttention(Vector3 playerPosition)
        {
            RoomVolume room = RoomVolume.FindContaining(playerPosition);
            int alerted = 0;

            for (int i = 0; i < Registered.Count; i++)
            {
                EnemyAI enemy = Registered[i];
                if (enemy == null || !enemy.isActiveAndEnabled) continue;
                if (enemy.State != EnemyState.Patrulla) continue;

                RoomVolume enemyRoom = enemy.CurrentRoom;
                bool inScope = room == null
                    || enemyRoom == room
                    || (enemyRoom != null && room.IsAdjacentTo(enemyRoom));

                if (!inScope) continue;

                enemy.AlertByMother(playerPosition);
                alerted++;
            }

            if (logEvents && alerted > 0)
            {
                Debug.Log($"[IA] The Mother presta atención: {alerted} enemigo(s) pasan a Sospecha en {name}.");
            }
        }

        private void ValidateRoomCap()
        {
            if (Registered.Count == 0) return;
            validatedRooms = true;

            int max = config != null ? config.EnemyMaxPerRoom : 2;

            for (int i = 0; i < RoomVolume.All.Count; i++)
            {
                RoomVolume room = RoomVolume.All[i];
                if (room == null || room.IsSafeZone) continue;

                int count = EnemiesInRoom(room);
                if (count > max && logEvents)
                {
                    Debug.LogWarning($"[IA] {room.RoomId} tiene {count} enemigos; el máximo por sala es {max}.");
                }
            }
        }

        // ------------------------------------------------------------------ beeps

        /// <summary>Un enemigo pide pitar. Se guarda el de mayor estado y se reproduce al cerrar la ventana.</summary>
        public void RequestBeep(EnemyState state, Vector3 position, string sourceName)
        {
            if (config == null) return;

            if (beepRequests.Count == 0) beepWindowStart = Time.time;

            beepRequests.Add(new BeepRequest
            {
                State = state,
                Position = position,
                SourceName = sourceName
            });
        }

        private void FlushBeeps()
        {
            if (config == null || beepRequests.Count == 0) return;
            if (Time.time - beepWindowStart < config.EnemyBeepPriorityWindow) return;

            int best = 0;
            for (int i = 1; i < beepRequests.Count; i++)
            {
                if (EnemyStateUtil.Priority(beepRequests[i].State) > EnemyStateUtil.Priority(beepRequests[best].State)) best = i;
            }

            BeepRequest winner = beepRequests[best];

            if (logEvents && beepRequests.Count > 1)
            {
                Debug.Log($"[IA] Beep prioritario: {EnemyStateUtil.DisplayName(winner.State)} ({winner.SourceName}); descartados {beepRequests.Count - 1}.");
            }

            PlayTone(winner.State, winner.Position);
            beepRequests.Clear();
        }

        private void PlayTone(EnemyState state, Vector3 position)
        {
            if (beepSource == null || config == null) return;

            int tone = config.GetEnemyBeepTone(state);
            AudioClip clip = GetTone(tone);
            if (clip == null) return;

            beepSource.transform.position = position;
            beepSource.PlayOneShot(clip, config.EnemyBeepVolume);
        }

        private AudioClip GetTone(int frequency)
        {
            if (ToneClips.TryGetValue(frequency, out AudioClip cached) && cached != null) return cached;

            float duration = config != null ? config.EnemyBeepDuration : 0.12f;
            int sampleCount = Mathf.Max(1, Mathf.RoundToInt(BeepSampleRate * duration));
            float rampSamples = BeepSampleRate * 0.01f;

            float[] data = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float time = (float)i / BeepSampleRate;
                float attack = Mathf.Clamp01(i / rampSamples);
                float release = Mathf.Clamp01(1f - (float)i / sampleCount);
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * time) * attack * release * 0.5f;
            }

            AudioClip clip = AudioClip.Create($"Beep_{frequency}", sampleCount, 1, BeepSampleRate, false);
            clip.SetData(data, 0);

            ToneClips[frequency] = clip;
            return clip;
        }

        // ------------------------------------------------------------ debug (F1)

        private void HandleDebugToggle()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (!keyboard.f1Key.wasPressedThisFrame) return;

            DebugEnabled = !DebugEnabled;
            if (logEvents) Debug.Log($"[IA] Debug de percepción {(DebugEnabled ? "activado" : "desactivado")} (F1).");
        }
    }
}
