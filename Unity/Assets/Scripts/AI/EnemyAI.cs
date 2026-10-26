using System;
using Heartbeat.Config;
using Heartbeat.Items;
using Heartbeat.Level;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Heartbeat.AI
{
    /// <summary>
    /// IA base del enemigo: máquina de estados Patrulla → Sospecha → Búsqueda → Caza → Atacar
    /// sobre NavMeshAgent. Ve con cono + raycast (no atraviesa obstáculos), oye los pulsos de
    /// ruido del jugador, duplica la subida de alerta si la linterna lo ilumina, pita distinto
    /// en cada estado, nunca entra en la zona segura, respeta el cupo de 2 por sala y persigue
    /// brevemente al cruzar una puerta. El tipo (Alto o Rastrero) fija tamaño, color,
    /// velocidades, sentidos, golpe, flanqueo y agarre desde <see cref="GameConfig"/>.
    /// El debug F1 (desactivado por defecto) colorea la cápsula.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(IlluminatedTarget))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private NavMeshAgent agent;
        [Tooltip("Malla de la cápsula; el debug F1 la colorea según el estado.")]
        [SerializeField] private Renderer bodyRenderer;
        [Tooltip("Ancla desde la que suenan los beeps de estado.")]
        [SerializeField] private Transform beepAnchor;

        [Tooltip("Tipo de enemigo: fija tamaño, color, velocidades, sentidos, golpe y agarre desde GameConfig.")]
        [SerializeField] private EnemyType enemyType = EnemyType.Alto;

        [Header("Depuración")]
        [Tooltip("Registra transiciones y percepciones en consola.")]
        [SerializeField] private bool logEvents = true;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private PlayerHealth playerHealth;
        private PlayerInventory playerInventory;
        private PlayerNoiseEmitter noiseEmitter;
        private Flashlight flashlight;
        private NavMeshPath navPath;
        private MaterialPropertyBlock propertyBlock;
        private CapsuleCollider bodyCollider;
        private EnemyProfile profile;
        private Color baseColor = Color.white;

        private EnemyState state = EnemyState.Patrulla;
        private RoomVolume currentRoom;
        private RoomVolume homeRoom;
        private Vector3 lastKnownPosition;
        private Vector3 lastSafePosition;

        private float alert01;
        private float patrolWaitTimer;
        private float suspicionTimer;
        private float searchTimer;
        private float searchWaitTimer;
        private float loseSightTimer;
        private float crossDoorTimer;
        private float attackTimer;
        private float beepTimer;
        private float litUntil;
        private float roomCheckTimer;
        private float stunTimer;
        private float flankSign = 1f;

        private bool seesPlayer;
        private bool hasDestination;
        private bool debugColorApplied;
        private bool litLogged;
        private bool grabNextAttack;
        private bool motherAttention;

        /// <summary>Tipo de enemigo (Alto o Rastrero).</summary>
        public EnemyType Type => enemyType;

        /// <summary>Perfil de cifras que está usando este enemigo.</summary>
        public EnemyProfile Profile => profile;

        /// <summary>Aturdido por un punzón: no se mueve ni ataca.</summary>
        public bool IsStunned => stunTimer > 0f;

        /// <summary>Estado actual de la máquina.</summary>
        public EnemyState State => state;

        /// <summary>Alerta 0-1 que alimenta las transiciones.</summary>
        public float Alert01 => alert01;

        /// <summary>Sala en la que está ahora mismo el enemigo.</summary>
        public RoomVolume CurrentRoom => currentRoom;

        /// <summary>Último punto donde vio u oyó al jugador.</summary>
        public Vector3 LastKnownPosition => lastKnownPosition;

        /// <summary>El jugador está dentro del cono de visión y sin obstáculos de por medio.</summary>
        public bool SeesPlayer => seesPlayer;

        /// <summary>Cambio de estado (anterior, nuevo).</summary>
        public event Action<EnemyState, EnemyState> StateChanged;

        /// <summary>El enemigo ataca; acto seguido aplica el golpe o el agarre al jugador.</summary>
        public event Action<EnemyAI> Attacked;

        private void Awake()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (bodyRenderer == null) bodyRenderer = GetComponentInChildren<Renderer>();
            if (beepAnchor == null) beepAnchor = transform;
            if (bodyCollider == null) bodyCollider = GetComponent<CapsuleCollider>();

            navPath = new NavMeshPath();
            propertyBlock = new MaterialPropertyBlock();

            if (bodyRenderer != null && bodyRenderer.sharedMaterial != null)
            {
                baseColor = bodyRenderer.sharedMaterial.color;
            }
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("[IA] Falta GameConfig.", this);
                enabled = false;
                return;
            }

            profile = config.GetEnemyProfile(enemyType);
            ApplyProfile();
            ConfigureAgent();
            FindPlayer();
            Subscribe();

            EnemyDirector director = EnemyDirector.Instance;
            if (director != null) director.Register(this);

            lastSafePosition = transform.position;
            homeRoom = RoomVolume.FindContaining(transform.position + Vector3.up * 0.5f);
            SnapToNavMesh();

            EnterState(EnemyState.Patrulla, "inicio");
        }

        private void OnDisable()
        {
            Unsubscribe();

            if (EnemyDirector.Instance != null) EnemyDirector.Instance.Unregister(this);
        }

        private void Update()
        {
            if (config == null) return;

            if (playerHealth == null)
            {
                FindPlayer();
                Subscribe();
                if (playerHealth == null) return;
            }

            float deltaTime = Time.deltaTime;

            UpdateRoom();

            // Aturdido por el punzón: ni se mueve ni ataca.
            if (stunTimer > 0f)
            {
                stunTimer -= deltaTime;
                if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
                if (stunTimer <= 0f && logEvents) Debug.Log($"[IA] {name}: se recupera del aturdimiento.");
                UpdateDebugColor();
                return;
            }

            UpdatePerception(deltaTime);
            UpdateState(deltaTime);
            UpdateBeep(deltaTime);
            UpdateDebugColor();
        }

        // ------------------------------------------------------------ configuración

        private void ConfigureAgent()
        {
            agent.speed = profile.patrolSpeed;
            agent.angularSpeed = config.EnemyTurnSpeed;
            agent.acceleration = config.EnemyAcceleration;
            agent.stoppingDistance = config.EnemyStoppingDistance;
            agent.autoBraking = true;
            agent.updateRotation = true;
        }

        /// <summary>Ajusta el cuerpo al tipo: tamaño de cápsula, colisión, agente y color.</summary>
        private void ApplyProfile()
        {
            float height = Mathf.Max(0.2f, profile.capsuleHeight);
            float radius = Mathf.Max(0.1f, profile.capsuleRadius);

            if (bodyCollider != null)
            {
                bodyCollider.radius = radius;
                bodyCollider.height = Mathf.Max(height, radius * 2f);
                bodyCollider.center = new Vector3(0f, height * 0.5f, 0f);
            }

            if (agent != null)
            {
                agent.radius = radius;
                agent.height = height;
                agent.baseOffset = 0f;
            }

            // La malla de cápsula de Unity mide 2 m de alto y 1 m de diámetro.
            if (bodyRenderer != null)
            {
                bodyRenderer.transform.localScale = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
            }

            baseColor = profile.bodyColor;
            grabNextAttack = profile.canGrab;

            // El color del tipo entra ya en el primer frame, sin esperar al debug F1.
            if (bodyRenderer != null)
            {
                propertyBlock.SetColor(BaseColorId, baseColor);
                bodyRenderer.SetPropertyBlock(propertyBlock);
                debugColorApplied = false;
            }
        }

        private void FindPlayer()
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
            if (playerHealth == null) return;

            noiseEmitter = playerHealth.GetComponent<PlayerNoiseEmitter>();
            playerInventory = playerHealth.GetComponent<PlayerInventory>();
            flashlight = FindAnyObjectByType<Flashlight>();
        }

        private void Subscribe()
        {
            if (config == null) return;
            if (playerHealth == null) FindPlayer();

            if (noiseEmitter != null)
            {
                noiseEmitter.Pulsed -= OnNoisePulsed;
                noiseEmitter.Pulsed += OnNoisePulsed;
            }

            if (flashlight != null)
            {
                flashlight.EnemyIlluminated -= OnEnemyIlluminated;
                flashlight.EnemyIlluminated += OnEnemyIlluminated;
            }
        }

        private void Unsubscribe()
        {
            if (noiseEmitter != null) noiseEmitter.Pulsed -= OnNoisePulsed;
            if (flashlight != null) flashlight.EnemyIlluminated -= OnEnemyIlluminated;
        }

        private void SnapToNavMesh()
        {
            if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, config.EnemyNavSnapDistance, NavMesh.AllAreas))
            {
                Debug.LogWarning($"[IA] {name}: no hay NavMesh bajo el enemigo; revisa el horneado.", this);
                return;
            }

            if ((hit.position - transform.position).sqrMagnitude > 0.0001f) agent.Warp(hit.position);
        }

        // -------------------------------------------------------------------- salas

        private void UpdateRoom()
        {
            RoomVolume room = RoomVolume.FindContaining(transform.position + Vector3.up * 0.5f);
            if (room == null) room = RoomVolume.FindContaining(transform.position);

            bool changed = room != currentRoom;
            currentRoom = room;

            if (room != null && room.IsSafeZone)
            {
                EscapeSafeZone(room);
                return;
            }

            if (room != null) lastSafePosition = transform.position;

            if (changed && state == EnemyState.Caza)
            {
                crossDoorTimer = config.EnemyChaseAcrossDoorDuration;
                if (logEvents)
                {
                    Debug.Log($"[IA] {name}: cruza una puerta hacia {(room != null ? room.RoomId : "pasillo")}; persecución breve de {config.EnemyChaseAcrossDoorDuration:F1} s.");
                }
            }
        }

        private void EscapeSafeZone(RoomVolume room)
        {
            if (logEvents) Debug.LogWarning($"[IA] {name}: no entra en {room.RoomId} (zona segura); retrocede.");

            alert01 = 0f;
            hasDestination = false;

            if (NavMesh.SamplePosition(lastSafePosition, out NavMeshHit hit, config.EnemyNavSnapDistance, NavMesh.AllAreas))
            {
                if ((hit.position - transform.position).sqrMagnitude > 0.0001f) agent.Warp(hit.position);
            }

            EnterState(EnemyState.Patrulla, "zona segura prohibida");
        }

        // --------------------------------------------------------------- percepción

        private void UpdatePerception(float deltaTime)
        {
            seesPlayer = CanSeePlayer(out Vector3 seenPoint);
            bool lit = Time.time < litUntil;

            if (seesPlayer)
            {
                lastKnownPosition = seenPoint;
                loseSightTimer = 0f;

                float distance = Vector3.Distance(transform.position, playerHealth.transform.position);
                float proximity = Mathf.Clamp01(1f - distance / Mathf.Max(0.01f, profile.visionRange));
                float factor = Mathf.Lerp(config.EnemyVisionEdgeFactor, 1f, proximity);
                float gain = config.EnemyVisionAlertPerSecond * factor;
                if (lit) gain *= config.EnemyFlashlightAlertMultiplier;

                alert01 = Mathf.Clamp01(alert01 + gain * deltaTime);
            }
            else
            {
                alert01 = Mathf.Clamp01(alert01 - config.EnemyAlertDecayPerSecond * deltaTime);
            }
        }

        private bool CanSeePlayer(out Vector3 point)
        {
            point = Vector3.zero;
            if (playerHealth == null) return false;

            // La zona segura es invisible para la IA: nunca entra y no persigue dentro.
            RoomVolume playerRoom = RoomVolume.FindContaining(playerHealth.transform.position);
            if (playerRoom != null && playerRoom.IsSafeZone) return false;

            point = playerHealth.transform.position + Vector3.up * config.EnemyVisionTargetHeight;

            Vector3 eye = EyePosition();
            Vector3 toTarget = point - eye;

            if (toTarget.sqrMagnitude > profile.visionRange * profile.visionRange) return false;
            if (Vector3.Angle(transform.forward, toTarget) > config.EnemyVisionAngle * 0.5f) return false;

            return HasLineOfSight(eye, point);
        }

        private Vector3 EyePosition()
        {
            return transform.position + Vector3.up * profile.eyeHeight + transform.forward * profile.eyeForwardOffset;
        }

        private bool HasLineOfSight(Vector3 origin, Vector3 point)
        {
            if (!Physics.Linecast(origin, point, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore)) return true;

            return hit.collider != null && hit.collider.transform.root == playerHealth.transform.root;
        }

        private void OnNoisePulsed(NoisePulse pulse)
        {
            if (config == null) return;

            // Un pulso emitido desde la zona segura no se oye: la IA no entra ahí.
            if (EnemyDirector.Instance != null && EnemyDirector.Instance.IsSafeZoneAt(pulse.Position)) return;

            // El alcance del oído lo pone el tipo; el pulso pone cuánto viaja el ruido.
            float reach = Mathf.Min(pulse.Radius, profile.hearingRange) * config.EnemyHearingMultiplier;
            float distance = Vector3.Distance(transform.position, pulse.Position);
            if (distance > reach) return;

            ReceiveNoise(pulse.Position, config.EnemyHearingAlert);

            if (logEvents)
            {
                Debug.Log($"[IA] {name} ({EnemyTypeUtil.DisplayName(enemyType)}): oye {pulse.Level} a {distance:F1} m (oído {reach:F1}); alerta {alert01:F2}.");
            }
        }

        /// <summary>Estímulo de oído (también sirve para pruebas y para el ruido de disparos).</summary>
        public void ReceiveNoise(Vector3 position, float alertAmount)
        {
            lastKnownPosition = position;
            alert01 = Mathf.Max(alert01, Mathf.Clamp01(alertAmount));

            if (state == EnemyState.Patrulla) EnterState(EnemyState.Sospecha, "ruido");
            else if (state == EnemyState.Sospecha && alert01 >= config.EnemySearchThreshold) EnterState(EnemyState.Busqueda, "ruido insistente");
        }

        private void OnEnemyIlluminated(IlluminatedTarget target)
        {
            if (target == null || target.transform.root != transform.root) return;

            litUntil = Time.time + config.EnemyFlashlightMemory;

            if (!litLogged && logEvents)
            {
                litLogged = true;
                Debug.Log($"[IA] {name}: linterna encima; la alerta sube x{config.EnemyFlashlightAlertMultiplier:F1}.");
            }
        }

        /// <summary>Marca de iluminación directa (pruebas o linterna externa).</summary>
        public void ReceiveIllumination()
        {
            if (config == null) return;
            litUntil = Time.time + config.EnemyFlashlightMemory;
            litLogged = false;
        }

        // ------------------------------------------------------- máquina de estados

        private void UpdateState(float deltaTime)
        {
            switch (state)
            {
                case EnemyState.Sospecha: TickSuspicion(deltaTime); break;
                case EnemyState.Busqueda: TickSearch(deltaTime); break;
                case EnemyState.Caza: TickChase(deltaTime); break;
                case EnemyState.Atacar: TickAttack(deltaTime); break;
                default: TickPatrol(deltaTime); break;
            }
        }

        private void TickPatrol(float deltaTime)
        {
            agent.isStopped = false;
            agent.speed = profile.patrolSpeed;

            if (alert01 >= config.EnemyChaseThreshold)
            {
                EnterState(EnemyState.Caza, "alerta al máximo");
                return;
            }

            if (alert01 >= config.EnemySuspicionThreshold)
            {
                EnterState(EnemyState.Sospecha, $"alerta {alert01:F2}");
                return;
            }

            roomCheckTimer -= deltaTime;
            if (roomCheckTimer <= 0f)
            {
                roomCheckTimer = config.EnemyRoomCheckInterval;

                if (IsRoomOvercrowded())
                {
                    SendHome();
                    return;
                }
            }

            bool arrived = !agent.pathPending && agent.remainingDistance <= config.EnemyStoppingDistance + 0.1f;
            if (hasDestination && !arrived) return;

            patrolWaitTimer -= deltaTime;
            if (patrolWaitTimer > 0f) return;

            patrolWaitTimer = config.EnemyPatrolWait;
            ChoosePatrolDestination();
        }

        private void TickSuspicion(float deltaTime)
        {
            agent.isStopped = true;
            FaceTowards(lastKnownPosition, deltaTime);

            if (alert01 >= config.EnemyChaseThreshold)
            {
                EnterState(EnemyState.Caza, "alerta al máximo");
                return;
            }

            if (alert01 >= config.EnemySearchThreshold)
            {
                EnterState(EnemyState.Busqueda, "pasa a buscar");
                return;
            }

            suspicionTimer -= deltaTime;
            if (suspicionTimer > 0f) return;

            if (alert01 >= config.EnemySuspicionThreshold || motherAttention)
            {
                EnterState(EnemyState.Busqueda, motherAttention ? "atención de The Mother" : "duda agotada");
                return;
            }

            EnterState(EnemyState.Patrulla, "sin rastro");
        }

        private void TickSearch(float deltaTime)
        {
            agent.speed = profile.patrolSpeed;

            if (alert01 >= config.EnemyChaseThreshold)
            {
                EnterState(EnemyState.Caza, "alerta al máximo");
                return;
            }

            searchTimer -= deltaTime;
            if (searchTimer <= 0f)
            {
                EnterState(EnemyState.Patrulla, "búsqueda agotada");
                return;
            }

            if (searchWaitTimer > 0f)
            {
                searchWaitTimer -= deltaTime;
                agent.isStopped = true;
                FaceTowards(lastKnownPosition, deltaTime);
                return;
            }

            agent.isStopped = false;

            bool arrived = !agent.pathPending && agent.remainingDistance <= config.EnemyStoppingDistance + 0.1f;
            if (hasDestination && !arrived) return;

            searchWaitTimer = config.EnemyPatrolWait;
            ChooseSearchDestination();
        }

        private void TickChase(float deltaTime)
        {
            agent.isStopped = false;
            agent.speed = profile.chaseSpeed;

            if (!seesPlayer)
            {
                loseSightTimer += deltaTime;
                if (loseSightTimer >= config.EnemyLoseSightDuration)
                {
                    EnterState(EnemyState.Busqueda, "pierde de vista");
                    return;
                }
            }
            else
            {
                loseSightTimer = 0f;
                lastKnownPosition = playerHealth.transform.position;
            }

            if (crossDoorTimer > 0f)
            {
                crossDoorTimer -= deltaTime;
                if (crossDoorTimer <= 0f)
                {
                    alert01 = Mathf.Min(alert01, config.EnemySuspicionThreshold);
                    EnterState(EnemyState.Patrulla, "persecución breve tras cruzar puerta");
                    return;
                }
            }

            float distance = Vector3.Distance(transform.position, playerHealth.transform.position);
            if (seesPlayer && distance <= profile.attackRange)
            {
                EnterState(EnemyState.Atacar, "al alcance");
                return;
            }

            if (seesPlayer)
            {
                RoomVolume playerRoom = RoomVolume.FindContaining(playerHealth.transform.position);
                EnemyDirector director = EnemyDirector.Instance;

                if (director != null && !director.CanEnterRoom(playerRoom, this))
                {
                    // La sala del jugador está llena: se queda buscando en el borde.
                    lastKnownPosition = transform.position;
                    EnterState(EnemyState.Busqueda, "sala llena");
                    return;
                }
            }

            if (TrySetDestination(profile.flanks && seesPlayer ? FlankPoint() : lastKnownPosition)) hasDestination = true;
        }

        private void TickAttack(float deltaTime)
        {
            agent.isStopped = true;
            agent.speed = profile.patrolSpeed;
            FaceTowards(playerHealth.transform.position, deltaTime);

            float distance = Vector3.Distance(transform.position, playerHealth.transform.position);
            if (!seesPlayer || distance > profile.attackRange * config.EnemyAttackReleaseFactor)
            {
                EnterState(EnemyState.Caza, "fuera de alcance");
                return;
            }

            attackTimer -= deltaTime;
            if (attackTimer > 0f) return;

            attackTimer = profile.attackInterval;
            Attacked?.Invoke(this);
            PerformAttack(distance);
        }

        /// <summary>Resuelve el ataque: agarre (si toca y el tipo puede) o golpe que resta salud.</summary>
        private void PerformAttack(float distance)
        {
            if (playerHealth == null || playerHealth.IsDead) return;

            if (profile.canGrab && grabNextAttack)
            {
                grabNextAttack = false;
                Grab();
                return;
            }

            if (profile.canGrab) grabNextAttack = true;

            playerHealth.ApplyHit(profile.hitType);
            if (logEvents)
            {
                Debug.Log($"[IA] {name} ({EnemyTypeUtil.DisplayName(enemyType)}): golpea {profile.hitType} a {distance:F1} m.");
            }
        }

        /// <summary>Agarre: con punzón se consume solo y el enemigo queda aturdido; sin punzón el jugador baja un estado.</summary>
        private void Grab()
        {
            if (playerInventory != null && playerInventory.ConsumeOne(ItemType.Spike))
            {
                Stun(config.EnemyGrabStunDuration);
                if (logEvents)
                {
                    Debug.Log($"[IA] {name}: agarra; se consume 1 punzón y queda aturdido {config.EnemyGrabStunDuration:F1} s.");
                }
                return;
            }

            playerHealth.DropOneState();
            if (logEvents) Debug.Log($"[IA] {name}: agarra sin punzón; el jugador baja un estado de salud.");
        }

        /// <summary>Aturde al enemigo (punzón): deja de moverse y de atacar.</summary>
        public void Stun(float seconds)
        {
            if (config == null || seconds <= 0f) return;

            stunTimer = Mathf.Max(stunTimer, seconds);
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
        }

        /// <summary>
        /// Devuelve el enemigo a la vida en otro punto del nivel: algo más rápido, sin alerta y
        /// patrullando desde su nueva sala. Lo llama el cadáver al reciclarse; la vida la pone
        /// <see cref="EnemyHealth.ResetForRecycle"/>.
        /// </summary>
        public void BecomeRecycled(Vector3 position, float speedMultiplier)
        {
            if (config == null) return;

            float multiplier = Mathf.Max(0.01f, speedMultiplier);
            profile.patrolSpeed *= multiplier;
            profile.chaseSpeed *= multiplier;

            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (bodyCollider == null) bodyCollider = GetComponent<CapsuleCollider>();

            if (bodyCollider != null) bodyCollider.enabled = true;

            // El agente quedó apagado al morir: se reactiva y se reubica en el punto elegido.
            if (agent != null)
            {
                agent.enabled = true;
                agent.speed = profile.patrolSpeed;
                agent.isStopped = false;

                if (!agent.Warp(position))
                {
                    Debug.LogWarning($"[IA] {name}: el reciclaje no encontró NavMesh en el punto elegido.", this);
                }
            }

            alert01 = 0f;
            motherAttention = false;
            stunTimer = 0f;
            loseSightTimer = 0f;
            crossDoorTimer = 0f;
            patrolWaitTimer = 0f;
            roomCheckTimer = 0f;
            grabNextAttack = profile.canGrab;
            hasDestination = false;
            seesPlayer = false;
            litUntil = 0f;
            litLogged = false;

            lastKnownPosition = position;
            lastSafePosition = transform.position;
            currentRoom = RoomVolume.FindContaining(transform.position + Vector3.up * 0.5f);
            homeRoom = currentRoom;

            // El OnDisable de la muerte cortó suscripciones y registro: se rehacen aquí.
            FindPlayer();
            Subscribe();

            enabled = true;

            EnemyDirector director = EnemyDirector.Instance;
            if (director != null) director.Register(this);

            // EnterState sale sin hacer nada si el estado ya es Patrulla: se arranca a mano.
            state = EnemyState.Patrulla;
            beepTimer = 0f;

            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                ChoosePatrolDestination();
            }

            if (logEvents)
            {
                Debug.Log($"[IA] {name}: reciclado en {(currentRoom != null ? currentRoom.RoomId : "el nivel")}"
                    + $" a {multiplier:F2}× velocidad (patrulla {profile.patrolSpeed:F1} m/s).");
            }
        }

        /// <summary>
        /// The Mother le llama la atención: sospecha en la posición indicada y, agotada la
        /// duda, pasa a Buscar exactamente ahí. Sólo afecta a los que están en Patrulla.
        /// </summary>
        public void AlertByMother(Vector3 position)
        {
            if (config == null) return;

            lastKnownPosition = position;
            motherAttention = true;
            alert01 = Mathf.Max(alert01, config.EnemySuspicionThreshold);

            if (state == EnemyState.Patrulla) EnterState(EnemyState.Sospecha, "atención de The Mother");
        }

        /// <summary>Punto de flanqueo: se acerca al jugador desplazado de frente, no en línea recta.</summary>
        private Vector3 FlankPoint()
        {
            Vector3 fromPlayer = transform.position - playerHealth.transform.position;
            fromPlayer.y = 0f;
            if (fromPlayer.sqrMagnitude < 0.01f) return lastKnownPosition;

            Vector3 rotated = Quaternion.Euler(0f, profile.flankAngle * flankSign, 0f) * fromPlayer.normalized;
            Vector3 point = playerHealth.transform.position + rotated * profile.flankDistance;
            point.y = transform.position.y;
            return point;
        }

        private void EnterState(EnemyState next, string reason)
        {
            if (state == next) return;

            EnemyState previous = state;
            state = next;
            beepTimer = 0f;

            // La atención de The Mother sólo dura la duda que abre; después manda la FSM.
            if (next != EnemyState.Sospecha) motherAttention = false;

            switch (next)
            {
                case EnemyState.Sospecha:
                    suspicionTimer = config.EnemySuspicionDuration;
                    agent.isStopped = true;
                    break;

                case EnemyState.Busqueda:
                    searchTimer = config.EnemySearchDuration;
                    searchWaitTimer = 0f;
                    agent.isStopped = false;
                    hasDestination = TrySetDestination(lastKnownPosition);
                    break;

                case EnemyState.Caza:
                    loseSightTimer = 0f;
                    crossDoorTimer = 0f;
                    flankSign = UnityEngine.Random.value < 0.5f ? -1f : 1f;
                    agent.isStopped = false;
                    break;

                case EnemyState.Atacar:
                    attackTimer = profile.attackInterval;
                    agent.isStopped = true;
                    break;

                default:
                    patrolWaitTimer = 0f;
                    agent.isStopped = false;
                    ChoosePatrolDestination();
                    break;
            }

            if (logEvents)
            {
                Debug.Log($"[IA] {name}: {EnemyStateUtil.DisplayName(previous)} → {EnemyStateUtil.DisplayName(next)} ({reason}).");
            }

            StateChanged?.Invoke(previous, next);
        }

        // ------------------------------------------------------------------ destinos

        private void ChoosePatrolDestination()
        {
            hasDestination = false;

            EnemyDirector director = EnemyDirector.Instance;

            // Primero su sala de origen: así los enemigos no se acumulan en los pasillos.
            if (homeRoom != null && !homeRoom.IsSafeZone && (director == null || director.CanEnterRoom(homeRoom, this)))
            {
                if (TrySamplePointInRoom(homeRoom, out Vector3 homePoint))
                {
                    hasDestination = true;
                    return;
                }
            }

            var rooms = RoomVolume.All;
            if (rooms.Count == 0) return;

            int start = UnityEngine.Random.Range(0, rooms.Count);

            for (int i = 0; i < rooms.Count; i++)
            {
                RoomVolume room = rooms[(start + i) % rooms.Count];
                if (room == null || room.IsSafeZone) continue;

                if (director != null && !director.CanEnterRoom(room, this)) continue;

                if (!TrySamplePointInRoom(room, out Vector3 point)) continue;

                if (TrySetDestination(point)) hasDestination = true;
                return;
            }
        }

        private bool IsRoomOvercrowded()
        {
            EnemyDirector director = EnemyDirector.Instance;
            if (director == null || currentRoom == null) return false;

            return director.EnemiesInRoom(currentRoom) > config.EnemyMaxPerRoom;
        }

        /// <summary>Si la sala supera el cupo, vuelve a su sala de origen (o a otra con sitio).</summary>
        private void SendHome()
        {
            EnemyDirector director = EnemyDirector.Instance;

            RoomVolume target = homeRoom;
            if (target == null || target.IsSafeZone || (director != null && !director.CanEnterRoom(target, this)))
            {
                target = null;

                var rooms = RoomVolume.All;
                for (int i = 0; i < rooms.Count; i++)
                {
                    RoomVolume room = rooms[i];
                    if (room == null || room.IsSafeZone) continue;
                    if (director != null && !director.CanEnterRoom(room, this)) continue;

                    target = room;
                    break;
                }
            }

            if (target == null) return;
            if (!TrySamplePointInRoom(target, out Vector3 point)) return;

            hasDestination = true;

            if (logEvents)
            {
                Debug.Log($"[IA] {name}: {(currentRoom != null ? currentRoom.RoomId : "sala")} supera {config.EnemyMaxPerRoom} enemigos; se marcha a {target.RoomId}.");
            }
        }

        private void ChooseSearchDestination()
        {
            hasDestination = false;
            if (config == null) return;

            for (int i = 0; i < config.EnemyPatrolSampleAttempts; i++)
            {
                Vector2 offset = UnityEngine.Random.insideUnitCircle * config.EnemySearchRadius;
                Vector3 candidate = lastKnownPosition + new Vector3(offset.x, 0f, offset.y);

                if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, config.EnemyPatrolSampleRadius, NavMesh.AllAreas)) continue;

                RoomVolume room = RoomVolume.FindContaining(hit.position);
                if (room != null && room.IsSafeZone) continue;

                EnemyDirector director = EnemyDirector.Instance;
                if (room != null && director != null && !director.CanEnterRoom(room, this)) continue;

                if (TrySetDestination(hit.position))
                {
                    hasDestination = true;
                    return;
                }
            }
        }

        private bool TrySamplePointInRoom(RoomVolume room, out Vector3 point)
        {
            point = Vector3.zero;

            BoxCollider box = room.GetComponent<BoxCollider>();
            if (box == null) return false;

            for (int i = 0; i < config.EnemyPatrolSampleAttempts; i++)
            {
                Vector3 local = new Vector3(
                    UnityEngine.Random.Range(-box.size.x * 0.4f, box.size.x * 0.4f),
                    0f,
                    UnityEngine.Random.Range(-box.size.z * 0.4f, box.size.z * 0.4f));

                Vector3 candidate = room.transform.TransformPoint(box.center + local);
                if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, config.EnemyPatrolSampleRadius, NavMesh.AllAreas)) continue;

                RoomVolume hitRoom = RoomVolume.FindContaining(hit.position);
                if (hitRoom != null && hitRoom.IsSafeZone) continue;

                if (!TrySetDestination(hit.position)) continue;

                point = hit.position;
                return true;
            }

            return false;
        }

        private bool TrySetDestination(Vector3 point)
        {
            if (agent == null || !agent.isOnNavMesh) return false;

            if (!NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, navPath)) return false;
            if (navPath.status != NavMeshPathStatus.PathComplete) return false;

            if (agent.isStopped) agent.isStopped = false;

            return agent.SetDestination(point);
        }

        private void FaceTowards(Vector3 position, float deltaTime)
        {
            Vector3 flat = position - transform.position;
            flat.y = 0f;
            if (flat.sqrMagnitude < 0.0001f) return;

            Quaternion target = Quaternion.LookRotation(flat, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, config.EnemyTurnSpeed * deltaTime);
        }

        // -------------------------------------------------------------------- beeps

        private void UpdateBeep(float deltaTime)
        {
            beepTimer -= deltaTime;
            if (beepTimer > 0f) return;

            beepTimer = config.GetEnemyBeepInterval(state);

            EnemyDirector director = EnemyDirector.Instance;
            if (director == null) return;

            Vector3 position = beepAnchor != null ? beepAnchor.position : transform.position;
            director.RequestBeep(state, position, name);
        }

        // ------------------------------------------------------------------- debug

        private void UpdateDebugColor()
        {
            if (bodyRenderer == null) return;

            if (!EnemyDirector.DebugEnabled)
            {
                if (!debugColorApplied) return;

                propertyBlock.SetColor(BaseColorId, baseColor);
                bodyRenderer.SetPropertyBlock(propertyBlock);
                debugColorApplied = false;
                return;
            }

            Color color = config != null ? config.GetEnemyStateColor(state) : Color.white;

            propertyBlock.SetColor(BaseColorId, color);
            bodyRenderer.SetPropertyBlock(propertyBlock);
            debugColorApplied = true;
        }
    }
}
