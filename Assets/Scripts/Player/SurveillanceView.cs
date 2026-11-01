using Heartbeat.AI;
using Heartbeat.Config;
using Heartbeat.Level;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Heartbeat.Player
{
    /// <summary>
    /// Vista de vigilancia (Q, sólo en salas con cámara de seguridad y sin enemigos en Caza).
    /// Cambia la vista del jugador por una cámara isométrica fija de la sala, con ruido encima
    /// y unos 0.3 s de retraso servidos desde un búfer circular de RenderTextures. Mientras
    /// está activa el jugador sólo se queda quieto o camina muy despacio: correr, apuntar,
    /// disparar, empujar, salir de la sala o recibir daño la cancelan.
    /// La señal se degrada con el estado más alto de los enemigos de la sala y de las
    /// adyacentes (Búsqueda = más ruido y más retraso; Caza = se corta y queda bloqueada).
    /// Usarla alimenta la atención de The Mother, un medidor oculto que sólo se ve con F1.
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    public class SurveillanceView : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private PlayerMotor motor;
        [SerializeField] private PlayerAim aim;
        [SerializeField] private PlayerWeapon weapon;
        [SerializeField] private PlayerHealth health;

        [Tooltip("Cámara OTS que se apaga mientras dura la vista.")]
        [SerializeField] private Camera viewCamera;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private const string ActionMapName = "Player";
        private const int CanvasSortingOrder = 20;
        private const int NoiseTextureSize = 64;
        private const int NoiseTiling = 5;

        private InputAction surveillanceAction;

        private Camera feedCamera;
        private GameObject feedCameraObject;
        private RenderTexture[] feedBuffers;
        private int writeIndex;
        private int writtenSamples;
        private float sampleTimer;

        private Canvas feedCanvas;
        private RawImage feedImage;
        private RawImage noiseImage;
        private Texture2D noiseTexture;

        private RoomVolume currentRoom;
        private EnemyState signal = EnemyState.Patrulla;
        private float noiseAmount;
        private float feedDelay;
        private bool isActive;
        private bool blocked;

        /// <summary>La vista está mostrando el feed de la sala.</summary>
        public bool IsActive => isActive;

        /// <summary>Cortada por un enemigo en Caza; se levanta cuando dejan de perseguir.</summary>
        public bool IsBlocked => blocked;

        /// <summary>Estado más alto que llega a la señal (sala + adyacentes).</summary>
        public EnemyState Signal => signal;

        /// <summary>Ruido actual del feed (0-1).</summary>
        public float NoiseAmount => noiseAmount;

        /// <summary>Retraso actual de la imagen en segundos.</summary>
        public float FeedDelay => feedDelay;

        private void Awake()
        {
            if (motor == null) motor = GetComponent<PlayerMotor>();
            if (aim == null) aim = GetComponent<PlayerAim>();
            if (weapon == null) weapon = GetComponent<PlayerWeapon>();
            if (health == null) health = GetComponent<PlayerHealth>();
            if (viewCamera == null) viewCamera = GetComponentInChildren<Camera>();
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.Damaged += OnPlayerDamaged;
                health.Died += OnPlayerDied;
            }

            if (weapon != null)
            {
                weapon.Shoved += OnPlayerShoved;
                weapon.Fired += OnPlayerFired;
            }

            if (inputActions == null) return;

            InputActionMap map = inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
            if (map == null) return;

            surveillanceAction = map.FindAction("Surveillance", throwIfNotFound: false);
            if (surveillanceAction == null)
            {
                Debug.LogWarning("[Vigilancia] Falta la acción 'Surveillance' en el mapa Player.", this);
                return;
            }

            surveillanceAction.Enable();
            surveillanceAction.performed += OnSurveillancePerformed;
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.Damaged -= OnPlayerDamaged;
                health.Died -= OnPlayerDied;
            }

            if (weapon != null)
            {
                weapon.Shoved -= OnPlayerShoved;
                weapon.Fired -= OnPlayerFired;
            }

            if (surveillanceAction != null)
            {
                surveillanceAction.performed -= OnSurveillancePerformed;
                surveillanceAction.Disable();
            }
            surveillanceAction = null;

            if (isActive) Deactivate("se desactiva el componente");
        }

        private void OnDestroy()
        {
            ReleaseBuffers();

            if (feedCameraObject != null) Destroy(feedCameraObject);
            if (feedCanvas != null) Destroy(feedCanvas.gameObject);
            if (noiseTexture != null) Destroy(noiseTexture);
        }

        private void Update()
        {
            if (config == null) return;

            // El medidor de The Mother vive en el director: es único para el nivel.
            EnemyDirector director = EnemyDirector.Instance;
            if (director != null) director.TickMotherAttention(isActive, transform.position, Time.deltaTime);

            if (isActive)
            {
                TickActive(director);
                return;
            }

            UpdateBlock(director);
        }

        private void OnSurveillancePerformed(InputAction.CallbackContext context)
        {
            if (isActive) Deactivate("Q");
            else TryActivate();
        }

        // ------------------------------------------------------------- activación

        /// <summary>Intenta entrar en la vista. Devuelve false si no se puede (y lo registra).</summary>
        public bool TryActivate()
        {
            if (config == null) return false;

            if (health != null && health.IsDead)
            {
                if (logEvents) Debug.Log("[Vigilancia] No se puede usar estando muerto.");
                return false;
            }

            RoomVolume room = RoomVolume.FindContaining(transform.position);
            if (room == null)
            {
                if (logEvents) Debug.Log("[Vigilancia] No hay cámara: el jugador no está dentro de una sala.");
                return false;
            }

            if (!room.HasSecurityCamera)
            {
                if (logEvents) Debug.Log($"[Vigilancia] {room.RoomId} no tiene cámara de seguridad.");
                return false;
            }

            if (blocked)
            {
                if (logEvents) Debug.Log($"[Vigilancia] Bloqueada: los enemigos siguen en Caza ({room.RoomId}).");
                return false;
            }

            EnemyDirector director = EnemyDirector.Instance;
            EnemyState highest = director != null ? director.HighestStateIn(room, true) : EnemyState.Patrulla;

            if (highest >= EnemyState.Caza)
            {
                blocked = true;
                signal = highest;
                if (logEvents) Debug.Log($"[Vigilancia] Cortada: hay enemigos en {EnemyStateUtil.DisplayName(highest)} en {room.RoomId} o sus adyacentes.");
                return false;
            }

            Activate(room, highest);
            return true;
        }

        private void Activate(RoomVolume room, EnemyState highest)
        {
            currentRoom = room;
            signal = highest;
            isActive = true;

            ApplySignal(highest);
            BuildFeed();
            PlaceFeedCamera(room);

            if (feedCanvas != null) feedCanvas.gameObject.SetActive(true);
            if (viewCamera != null) viewCamera.enabled = false;
            if (feedCamera != null) feedCamera.enabled = true;

            if (logEvents)
            {
                Debug.Log($"[Vigilancia] Vista activa en {room.RoomId}: señal {EnemyStateUtil.DisplayName(highest)}, "
                    + $"ruido {noiseAmount:F2}, retraso {feedDelay:F2} s.");
            }
        }

        /// <summary>Corta la vista y devuelve el control normal al jugador.</summary>
        public void Deactivate(string reason)
        {
            if (!isActive) return;

            isActive = false;

            if (viewCamera != null) viewCamera.enabled = true;
            if (feedCamera != null) feedCamera.enabled = false;
            if (feedCanvas != null) feedCanvas.gameObject.SetActive(false);

            if (logEvents) Debug.Log($"[Vigilancia] Vista cortada ({reason}).");
        }

        // ---------------------------------------------------------------- en vivo

        private void TickActive(EnemyDirector director)
        {
            RoomVolume room = RoomVolume.FindContaining(transform.position);
            if (room != currentRoom)
            {
                Deactivate(room == null ? "el jugador sale de la sala" : $"el jugador pasa a {room.RoomId}");
                return;
            }

            EnemyState highest = director != null ? director.HighestStateIn(currentRoom, true) : EnemyState.Patrulla;
            signal = highest;

            if (highest >= EnemyState.Caza)
            {
                blocked = true;
                Deactivate($"un enemigo entra en {EnemyStateUtil.DisplayName(highest)}");
                return;
            }

            ApplySignal(highest);

            // Correr y apuntar la cancelan; disparar y empujar llegan por evento.
            if (aim != null && aim.IsAiming)
            {
                Deactivate("el jugador apunta");
                return;
            }

            if (motor != null && motor.WantsSprint)
            {
                Deactivate("el jugador corre");
                return;
            }

            SampleFeed();
            UpdateOverlay();
        }

        private void ApplySignal(EnemyState highest)
        {
            bool searching = highest >= EnemyState.Busqueda;

            feedDelay = searching ? config.SurveillanceDelaySearch : config.SurveillanceDelay;
            noiseAmount = searching ? config.SurveillanceNoiseSearch : config.SurveillanceNoise;
        }

        /// <summary>El bloqueo se levanta cuando ya no hay nadie persiguiendo en la sala actual.</summary>
        private void UpdateBlock(EnemyDirector director)
        {
            if (!blocked || director == null) return;

            RoomVolume room = RoomVolume.FindContaining(transform.position);
            EnemyState highest = room != null ? director.HighestStateIn(room, true) : EnemyState.Patrulla;

            if (highest >= EnemyState.Caza) return;

            blocked = false;
            if (logEvents) Debug.Log("[Vigilancia] Bloqueo levantado: ya no hay enemigos en Caza.");
        }

        // ------------------------------------------------------------- interrupciones

        private void OnPlayerDamaged(int hits)
        {
            if (isActive) Deactivate("el jugador recibe daño");
        }

        private void OnPlayerDied()
        {
            if (isActive) Deactivate("el jugador muere");
        }

        private void OnPlayerShoved()
        {
            if (isActive) Deactivate("el jugador empuja");
        }

        private void OnPlayerFired()
        {
            if (isActive) Deactivate("el jugador dispara");
        }

        // ------------------------------------------------------------------ feed

        private void BuildFeed()
        {
            if (feedCamera == null) CreateFeedCamera();
            if (feedBuffers == null) CreateBuffers();
            if (feedCanvas == null) CreateOverlay();

            feedCanvas.gameObject.SetActive(true);

            for (int i = 0; i < feedBuffers.Length; i++) feedBuffers[i].DiscardContents();

            writeIndex = 0;
            writtenSamples = 1;
            sampleTimer = 0f;
        }

        private void CreateFeedCamera()
        {
            feedCameraObject = new GameObject("CamaraVigilanciaFeed");
            feedCameraObject.transform.SetParent(null, false);

            feedCamera = feedCameraObject.AddComponent<Camera>();
            feedCamera.fieldOfView = config.SurveillanceFov;
            feedCamera.nearClipPlane = 0.05f;
            feedCamera.farClipPlane = 250f;
            feedCamera.clearFlags = CameraClearFlags.SolidColor;
            feedCamera.backgroundColor = Color.black;
            feedCamera.allowHDR = true;
            feedCamera.allowMSAA = false;
            feedCamera.depth = -10f;
            feedCamera.enabled = false;

            // Mismo culling que la cámara del jugador: el feed muestra lo mismo, desde arriba.
            if (viewCamera != null) feedCamera.cullingMask = viewCamera.cullingMask;
        }

        private void CreateBuffers()
        {
            int width = Mathf.Max(64, Mathf.RoundToInt(Screen.width * config.SurveillanceFeedScale));
            int height = Mathf.Max(64, Mathf.RoundToInt(Screen.height * config.SurveillanceFeedScale));

            float maxDelay = Mathf.Max(config.SurveillanceDelay, config.SurveillanceDelaySearch);
            int frames = Mathf.CeilToInt(maxDelay * Mathf.Max(1f, config.SurveillanceFeedFps)) + 2;

            feedBuffers = new RenderTexture[frames];

            for (int i = 0; i < frames; i++)
            {
                RenderTexture buffer = new RenderTexture(width, height, 24, RenderTextureFormat.Default);
                buffer.name = $"Vigilancia_Buffer_{i}";
                buffer.Create();
                feedBuffers[i] = buffer;
            }
        }

        /// <summary>
        /// Alinea la cámara isométrica con la sala: la aleja lo justo para que la sala entera
        /// quepa en el encuadre y la recentra en vertical (la perspectiva descentra el conjunto).
        /// La orientación sale de la cámara física de la sala si existe.
        /// </summary>
        private void PlaceFeedCamera(RoomVolume room)
        {
            Bounds bounds = ResolveBounds(room);
            Vector3 center = bounds.center;

            float yaw = room.SecurityCameraProp != null ? room.SecurityCameraProp.eulerAngles.y : config.SurveillanceYaw;
            Quaternion rotation = Quaternion.Euler(config.SurveillancePitch, yaw, 0f);

            if (feedCamera != null) feedCamera.fieldOfView = config.SurveillanceFov;

            float fov = feedCamera != null ? feedCamera.fieldOfView : config.SurveillanceFov;
            float tanY = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            float tanX = tanY * ResolveAspect();

            Quaternion inverse = Quaternion.Inverse(rotation);
            Vector3 forward = rotation * Vector3.forward;
            Vector3 up = rotation * Vector3.up;

            // Punto de partida holgado: la esfera que envuelve la sala cabe entera en el cono vertical.
            float radius = bounds.extents.magnitude;
            float start = radius / Mathf.Max(0.01f, Mathf.Sin(fov * 0.5f * Mathf.Deg2Rad));
            Vector3 position = center - forward * start;

            // Recentrado vertical: se desplaza la cámara sobre su eje "arriba" hasta que el
            // punto medio proyectado de la sala cae en el centro del encuadre.
            for (int pass = 0; pass < 3; pass++)
            {
                float min = float.MaxValue;
                float max = float.MinValue;

                for (int i = 0; i < 8; i++)
                {
                    Vector3 delta = Corner(bounds, i) - position;
                    float depth = Vector3.Dot(delta, forward);
                    if (depth <= 0.0001f) continue;

                    float viewportY = 0.5f + (Vector3.Dot(delta, up) / depth) / (2f * tanY);
                    if (viewportY < min) min = viewportY;
                    if (viewportY > max) max = viewportY;
                }

                if (min > max) break;
                position -= up * ((0.5f - (min + max) * 0.5f) * 2f * tanY * Mathf.Max(1f, start));
            }

            // Ajuste final: distancia exacta a la que la esquina más exigente toca el borde,
            // más el margen de configuración (encuadre y relleno).
            float fit = float.MinValue;

            for (int i = 0; i < 8; i++)
            {
                Vector3 delta = inverse * (Corner(bounds, i) - position);
                float needed = Mathf.Max(Mathf.Abs(delta.y) / tanY, Mathf.Abs(delta.x) / tanX) - delta.z;
                if (needed > fit) fit = needed;
            }

            float margin = Mathf.Max(0f, fit) * (config.SurveillanceFraming - 1f) + config.SurveillancePadding;
            position -= forward * (fit + margin);

            feedCameraObject.transform.SetPositionAndRotation(position, rotation);
        }

        /// <summary>Relación de aspecto del feed: la del búfer si ya existe, si no la de la cámara.</summary>
        private float ResolveAspect()
        {
            if (feedBuffers != null && feedBuffers.Length > 0 && feedBuffers[0] != null && feedBuffers[0].height > 0)
            {
                return (float)feedBuffers[0].width / feedBuffers[0].height;
            }

            return feedCamera != null && feedCamera.aspect > 0.01f ? feedCamera.aspect : 1f;
        }

        /// <summary>Esquinas de la caja envolvente, en el orden de los bits de <paramref name="index"/>.</summary>
        private static Vector3 Corner(Bounds bounds, int index)
        {
            Vector3 sign = new Vector3(
                (index & 1) == 0 ? -1f : 1f,
                (index & 2) == 0 ? -1f : 1f,
                (index & 4) == 0 ? -1f : 1f);

            return bounds.center + Vector3.Scale(bounds.extents, sign);
        }

        /// <summary>Caja envolvente de la sala: collider si lo hay, si no el renderer, si no el transform.</summary>
        private static Bounds ResolveBounds(RoomVolume room)
        {
            BoxCollider box = room.GetComponent<BoxCollider>();
            if (box != null) return box.bounds;

            Renderer mesh = room.GetComponent<Renderer>();
            if (mesh != null) return mesh.bounds;

            return new Bounds(room.transform.position, Vector3.one);
        }

        /// <summary>Escribe el fotograma actual en el búfer y muestra el que toca por el retraso.</summary>
        private void SampleFeed()
        {
            if (feedCamera == null || feedBuffers == null || feedBuffers.Length == 0) return;

            float interval = 1f / Mathf.Max(1f, config.SurveillanceFeedFps);

            sampleTimer += Time.unscaledDeltaTime;
            if (sampleTimer >= interval)
            {
                sampleTimer -= interval;
                writeIndex = (writeIndex + 1) % feedBuffers.Length;
                writtenSamples = Mathf.Min(writtenSamples + 1, feedBuffers.Length);
            }

            feedCamera.targetTexture = feedBuffers[writeIndex];

            int delayFrames = Mathf.RoundToInt(feedDelay * config.SurveillanceFeedFps);
            delayFrames = Mathf.Clamp(delayFrames, 0, Mathf.Max(0, writtenSamples - 1));

            int readIndex = ((writeIndex - delayFrames) % feedBuffers.Length + feedBuffers.Length) % feedBuffers.Length;

            if (feedImage != null) feedImage.texture = feedBuffers[readIndex];
        }

        private void ReleaseBuffers()
        {
            if (feedBuffers == null) return;

            for (int i = 0; i < feedBuffers.Length; i++)
            {
                if (feedBuffers[i] == null) continue;
                feedBuffers[i].Release();
                Destroy(feedBuffers[i]);
            }

            feedBuffers = null;
        }

        // ------------------------------------------------------------------- UI

        private void CreateOverlay()
        {
            GameObject canvasObject = new GameObject("VigilanciaUI", typeof(Canvas), typeof(CanvasScaler));
            canvasObject.transform.SetParent(null, false);

            feedCanvas = canvasObject.GetComponent<Canvas>();
            feedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            feedCanvas.sortingOrder = CanvasSortingOrder;

            feedImage = CreateRawImage("Feed", canvasObject.transform);
            feedImage.color = Color.white;

            noiseImage = CreateRawImage("Ruido", canvasObject.transform);
            noiseImage.texture = BuildNoiseTexture();
            noiseImage.color = new Color(1f, 1f, 1f, 0f);

            canvasObject.SetActive(false);
        }

        private RawImage CreateRawImage(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(RawImage));
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            RawImage image = go.GetComponent<RawImage>();
            image.raycastTarget = false;
            return image;
        }

        private Texture2D BuildNoiseTexture()
        {
            noiseTexture = new Texture2D(NoiseTextureSize, NoiseTextureSize, TextureFormat.RGBA32, false);
            noiseTexture.name = "Vigilancia_Ruido";
            noiseTexture.wrapMode = TextureWrapMode.Repeat;
            noiseTexture.filterMode = FilterMode.Point;

            Color32[] pixels = new Color32[NoiseTextureSize * NoiseTextureSize];
            for (int i = 0; i < pixels.Length; i++)
            {
                byte value = (byte)UnityEngine.Random.Range(0, 256);
                pixels[i] = new Color32(value, value, value, 255);
            }

            noiseTexture.SetPixels32(pixels);
            noiseTexture.Apply();
            return noiseTexture;
        }

        private void UpdateOverlay()
        {
            if (noiseImage == null) return;

            float flicker = 0.55f + 0.45f * Mathf.PerlinNoise(Time.unscaledTime * 12f, 0.37f);
            Color tint = noiseImage.color;
            tint.a = Mathf.Clamp01(noiseAmount * flicker);
            noiseImage.color = tint;

            // Desplazamiento aleatorio del grano: el ruido "se mueve" en cada fotograma.
            noiseImage.uvRect = new Rect(UnityEngine.Random.value, UnityEngine.Random.value, NoiseTiling, NoiseTiling);
        }

        // --------------------------------------------------------------- debug (F1)

        private void OnGUI()
        {
            if (!EnemyDirector.DebugEnabled) return;

            float attention = EnemyDirector.Instance != null ? EnemyDirector.Instance.MotherAttention01 : 0f;

            GUILayout.BeginArea(new Rect(12f, 12f, 340f, 96f), GUI.skin.box);
            GUILayout.Label($"[F1] The Mother: {attention * 100f:F0} %");
            GUILayout.Label($"Vista: {(isActive ? "ACTIVA" : (blocked ? "BLOQUEADA" : "lista"))}");
            GUILayout.Label($"Señal: {EnemyStateUtil.DisplayName(signal)} · ruido {noiseAmount:F2} · retraso {feedDelay:F2} s");
            GUILayout.EndArea();
        }
    }
}
