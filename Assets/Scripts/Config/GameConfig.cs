using UnityEngine;

namespace Heartbeat.Config
{
    /// <summary>
    /// Cifras de un tipo de enemigo. Un solo prefab se ajusta en tiempo de ejecución
    /// con estos valores (tamaño, color, velocidades, sentidos, ataque y agarre).
    /// </summary>
    [System.Serializable]
    public struct EnemyProfile
    {
        public string displayName;
        public Color bodyColor;

        [Tooltip("Alto de la cápsula (m).")]
        public float capsuleHeight;

        [Tooltip("Radio de la cápsula (m).")]
        public float capsuleRadius;

        public float patrolSpeed;
        public float chaseSpeed;

        public float visionRange;
        public float hearingRange;

        [Tooltip("Altura de los ojos sobre la base (m).")]
        public float eyeHeight;

        [Tooltip("Adelanto de los ojos para no rozar la propia cápsula (m).")]
        public float eyeForwardOffset;

        [Tooltip("Alcance del golpe (m).")]
        public float attackRange;

        [Tooltip("Segundos entre golpes (s).")]
        public float attackInterval;

        [Tooltip("Golpes de salud que resta cada ataque.")]
        public Heartbeat.Player.EnemyHitType hitType;

        [Tooltip("Disparos que aguanta antes de morir.")]
        public int shotsToKill;

        [Tooltip("Si agarra en vez de golpear en el primer ataque de cada ciclo.")]
        public bool canGrab;

        [Tooltip("Si busca el flanco al perseguir en vez de ir de frente.")]
        public bool flanks;

        [Tooltip("Ángulo lateral del flanqueo (grados).")]
        public float flankAngle;

        [Tooltip("Separación lateral del punto de flanqueo (m).")]
        public float flankDistance;
    }

    /// <summary>
    /// Única fuente de valores numéricos del proyecto. Nada se hardcodea en los scripts.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Heartbeat/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Nivel — métricas de graybox (metros)")]
        [Tooltip("Alto de los muros grises.")]
        [SerializeField] private float wallHeight = 3f;

        [Tooltip("Grosor de los muros; el muro se centra en la línea de la sala.")]
        [SerializeField] private float wallThickness = 0.3f;

        [Tooltip("Ancho de todos los vanos de puerta (módulo de 2 m).")]
        [SerializeField] private float doorWidth = 2f;

        [Tooltip("Alto libre del vano; por encima se genera el dintel.")]
        [SerializeField] private float doorHeight = 2.2f;

        [Tooltip("Separación entre las cápsulas decorativas de la fila de cápsulas.")]
        [SerializeField] private float capsuleSpacing = 1.2f;

        [Header("Iluminación")]
        [Tooltip("Luz ambiente de la escena: muy baja, solo para que la oscuridad no sea ciega.")]
        [SerializeField] private Color ambientColor = new Color(0.08f, 0.08f, 0.09f, 1f);

        [Tooltip("Color de los focos tenues de emergencia.")]
        [SerializeField] private Color emergencyLightColor = new Color(1f, 0.62f, 0.24f, 1f);

        [SerializeField] private float emergencyLightIntensity = 4.6f;
        [SerializeField] private float emergencyLightRange = 16f;
        [SerializeField] private float emergencyLightAngle = 110f;

        [Tooltip("Altura del foco sobre el suelo de la sala.")]
        [SerializeField] private float emergencyLightHeight = 2.85f;

        [SerializeField] private bool emergencyLightShadows = true;

        [Header("Jugador — cápsula y movimiento")]
        [Tooltip("Alto de la cápsula de pie.")]
        [SerializeField] private float playerHeight = 1.78f;

        [SerializeField] private float playerRadius = 0.3f;

        [Tooltip("Alto de la cápsula agachado.")]
        [SerializeField] private float crouchHeight = 1.2f;

        [Tooltip("Velocidad agachado (m/s).")]
        [SerializeField] private float crouchSpeed = 0.8f;

        [Tooltip("Velocidad caminando (m/s).")]
        [SerializeField] private float walkSpeed = 1.5f;

        [Tooltip("Velocidad corriendo (m/s).")]
        [SerializeField] private float runSpeed = 4f;

        [SerializeField] private float gravity = -18f;

        [Header("Jugador — resistencia (0-100)")]
        [SerializeField] private float staminaMax = 100f;

        [Tooltip("Gasto por segundo al correr: 10 %/s.")]
        [SerializeField] private float staminaDrainPerSecond = 10f;

        [Tooltip("Segundos sin correr/empujar/rematar antes de regenerar.")]
        [SerializeField] private float staminaRegenDelay = 3f;

        [SerializeField] private float staminaRegenPerSecond = 15f;

        [Tooltip("Segundos de agotamiento al llegar a 0: no se puede correr ni empujar.")]
        [SerializeField] private float exhaustionDuration = 2f;

        [Header("Jugador — ruido emitido (radio en metros)")]
        [Tooltip("Agachado no emite ruido.")]
        [SerializeField] private float noiseRadiusCrouch = 0f;

        [Tooltip("Caminar emite un pulso bajo.")]
        [SerializeField] private float noiseRadiusWalk = 3f;

        [Tooltip("Correr emite un pulso alto.")]
        [SerializeField] private float noiseRadiusRun = 9f;

        [Tooltip("Cada cuántos segundos se emite el pulso mientras se mueve.")]
        [SerializeField] private float noisePulseInterval = 0.5f;

        [Header("Cámara OTS")]
        [Tooltip("FOV de cadera (reducido, visión cerrada).")]
        [SerializeField] private float cameraFov = 58f;

        [Tooltip("FOV al apuntar.")]
        [SerializeField] private float cameraAimFov = 44f;

        [Tooltip("Distancia de la cámara al hombro sin apuntar.")]
        [SerializeField] private float cameraDistance = 3.2f;

        [Tooltip("Distancia de la cámara al hombro apuntando.")]
        [SerializeField] private float cameraAimDistance = 1.35f;

        [Tooltip("Desplazamiento lateral al hombro sin apuntar.")]
        [SerializeField] private float cameraShoulder = 0.55f;

        [Tooltip("Desplazamiento lateral al hombro apuntando.")]
        [SerializeField] private float cameraAimShoulder = 0.45f;

        [Tooltip("Radio de la esfera de colisión de la cámara.")]
        [SerializeField] private float cameraCollisionRadius = 0.2f;

        [Tooltip("Margen que se deja respecto a la pared golpeada.")]
        [SerializeField] private float cameraCollisionPadding = 0.06f;

        [SerializeField] private float cameraMinDistance = 0.35f;

        [Tooltip("Tiempo de acercado al chocar con una pared (rápido).")]
        [SerializeField] private float cameraPullInTime = 0.05f;

        [Tooltip("Tiempo de alejado al dejar de chocar.")]
        [SerializeField] private float cameraPullOutTime = 0.22f;

        [SerializeField] private float cameraFollowTime = 0.05f;

        [Tooltip("Velocidad de interpolación del FOV.")]
        [SerializeField] private float cameraFovSharpness = 18f;

        [SerializeField] private float cameraStartPitch = 6f;

        [Tooltip("Grados de giro por píxel de ratón.")]
        [SerializeField] private float lookSensitivity = 0.12f;

        [Tooltip("Límite de mirada hacia arriba (grados, negativo).")]
        [SerializeField] private float pitchMin = -60f;

        [Tooltip("Límite de mirada hacia abajo (grados, positivo).")]
        [SerializeField] private float pitchMax = 70f;

        [Header("Apuntar (clic derecho mantenido)")]
        [Tooltip("Multiplicador de velocidad al apuntar: 0.7 = 30 % más lento.")]
        [SerializeField] private float aimSpeedMultiplier = 0.7f;

        [Header("Retícula de punto")]
        [SerializeField] private float reticleDotSize = 3f;

        [Tooltip("Radio de la retícula cerrada (píxeles).")]
        [SerializeField] private float reticleClosedRadius = 4f;

        [Tooltip("Radio de la retícula abierta (píxeles).")]
        [SerializeField] private float reticleOpenRadius = 26f;

        [Tooltip("Segundos que tarda en cerrarse con el jugador quieto.")]
        [SerializeField] private float reticleCloseDuration = 1f;

        [Tooltip("Velocidad de apertura (1 = se abre en 1 s).")]
        [SerializeField] private float reticleOpenSpeed = 6f;

        [Tooltip("Apertura al caminar (0-1).")]
        [SerializeField] private float reticleMoveOpen = 0.45f;

        [Tooltip("Apertura al correr (0-1).")]
        [SerializeField] private float reticleRunOpen = 1f;

        [Tooltip("Apertura al recibir un golpe (0-1).")]
        [SerializeField] private float reticleHitOpen = 1f;

        [Tooltip("Velocidad de recuperación del golpe por segundo.")]
        [SerializeField] private float reticleHitRecovery = 1.4f;

        [Header("Linterna")]
        [Tooltip("Segundos de uso continuo con la batería llena.")]
        [SerializeField] private float flashlightDuration = 60f;

        [Tooltip("Segundos apagada antes de empezar a recargar.")]
        [SerializeField] private float flashlightRechargeDelay = 3f;

        [Tooltip("Segundos que tarda en recargar del todo.")]
        [SerializeField] private float flashlightRechargeDuration = 30f;

        [Tooltip("Carga mínima (0-1) para poder encenderla tras agotarse.")]
        [SerializeField] private float flashlightMinTurnOn = 0.25f;

        [Tooltip("Por debajo de esta carga (0-1) el haz parpadea.")]
        [SerializeField] private float flashlightFlickerThreshold = 0.1f;

        [Tooltip("Cono sin apuntar: corto y amplio.")]
        [SerializeField] private float flashlightHipSpotAngle = 70f;

        [SerializeField] private float flashlightHipRange = 13f;
        [SerializeField] private float flashlightHipIntensity = 4.5f;

        [Tooltip("Cono apuntando: estrecho y largo.")]
        [SerializeField] private float flashlightAimSpotAngle = 22f;

        [SerializeField] private float flashlightAimRange = 24f;
        [SerializeField] private float flashlightAimIntensity = 6.5f;

        [Tooltip("Rapidez de la transición entre ambos conos.")]
        [SerializeField] private float flashlightConeSharpness = 10f;

        [Tooltip("Cada cuántos segundos se comprueba qué enemigos ilumina.")]
        [SerializeField] private float flashlightScanInterval = 0.15f;

        [Tooltip("Distancia al frente del ancla donde nace el haz (evita que la cápsula del jugador proyecte sombra sobre el cono).")]
        [SerializeField] private float flashlightMuzzleOffset = 0.45f;

        [Header("Salud")]
        [Tooltip("Golpes que aguanta el jugador con la salud llena: 2 golpes pesados o 4 ligeros.")]
        [SerializeField] private int playerMaxHits = 4;

        [Tooltip("Ratio de golpes restantes (0-1) al que o por debajo del cual pasa a Herido.")]
        [SerializeField] private float playerHurtRatio = 0.66f;

        [Tooltip("Ratio de golpes restantes (0-1) al que o por debajo del cual pasa a Crítico.")]
        [SerializeField] private float playerCriticalRatio = 0.34f;

        [Tooltip("Multiplicador de velocidad en Herido.")]
        [SerializeField] private float hurtSpeedMultiplier = 0.85f;

        [Tooltip("Multiplicador de velocidad en Crítico.")]
        [SerializeField] private float criticalSpeedMultiplier = 0.7f;

        [Tooltip("Golpes que resta un ataque ligero enemigo.")]
        [SerializeField] private int hitLightHits = 1;

        [Tooltip("Golpes que resta un ataque pesado enemigo.")]
        [SerializeField] private int hitHeavyHits = 2;

        [Tooltip("Golpes que resta un ataque brutal enemigo.")]
        [SerializeField] private int hitBrutalHits = 3;

        [Tooltip("Segundos que tarda la cura en subir un estado.")]
        [SerializeField] private float healDuration = 1.5f;

        [Tooltip("Curas disponibles al empezar.")]
        [SerializeField] private int playerStartHeals = 2;

        [Tooltip("Sacudida de cámara al recibir daño: desplazamiento máximo en metros.")]
        [SerializeField] private float damageShakeMagnitude = 0.12f;

        [Tooltip("Sacudida de cámara al recibir daño: giro máximo en grados.")]
        [SerializeField] private float damageShakeAngle = 1.5f;

        [Tooltip("Duración de la sacudida de cámara en segundos.")]
        [SerializeField] private float damageShakeDuration = 0.3f;

        [Header("Inventario")]
        [Tooltip("Número de casillas del inventario.")]
        [SerializeField] private int inventorySlots = 6;

        [Tooltip("Segundos que tarda usar o descartar un objeto.")]
        [SerializeField] private float itemActionDuration = 1.5f;

        [Tooltip("Distancia máxima al jugador para poder recoger con E.")]
        [SerializeField] private float pickupDistance = 1.5f;

        [Tooltip("Balas por casilla de munición.")]
        [SerializeField] private int ammoStackSize = 12;

        [Tooltip("Balas que da cada recogida de munición.")]
        [SerializeField] private int ammoPickupAmount = 3;

        [Tooltip("Balas en el inventario al empezar (el revólver arranca descargado).")]
        [SerializeField] private int startAmmoBullets = 3;

        [Tooltip("Distancia al frente del jugador donde caen los objetos descartados.")]
        [SerializeField] private float itemDropDistance = 0.7f;

        [Tooltip("Altura sobre el suelo a la que reposa un objeto descartado.")]
        [SerializeField] private float itemDropHeight = 0.2f;

        [Tooltip("Alcance para usar la llave sobre una puerta.")]
        [SerializeField] private float keyUseRange = 2.5f;

        [Header("Revólver")]
        [Tooltip("Balas que caben en el tambor.")]
        [SerializeField] private int revolverDrumSize = 6;

        [Tooltip("Segundos mínimos entre dos disparos.")]
        [SerializeField] private float revolverFireInterval = 0.8f;

        [Tooltip("Segundos que tarda la recarga en meter cada bala (cancelable por golpe).")]
        [SerializeField] private float revolverReloadPerBullet = 0.7f;

        [Tooltip("Golpes que resta un disparo en el cuerpo.")]
        [SerializeField] private int revolverDamage = 1;

        [Tooltip("Alcance del disparo en metros.")]
        [SerializeField] private float revolverRange = 25f;

        [Tooltip("Radio de ruido que oye la IA al disparar.")]
        [SerializeField] private float revolverShotNoiseRadius = 20f;

        [Tooltip("Sacudida de cámara por disparo: giro máximo en grados.")]
        [SerializeField] private float revolverRecoilAngle = 0.5f;

        [Tooltip("Duración de la sacudida de cámara por disparo.")]
        [SerializeField] private float revolverRecoilDuration = 0.12f;

        [Header("Empujón")]
        [Tooltip("Fracción de la resistencia máxima (0-1) que cuesta empujar.")]
        [SerializeField] private float shoveStaminaCost01 = 0.35f;

        [Tooltip("Alcance del empujón desde el jugador.")]
        [SerializeField] private float shoveRange = 2.2f;

        [Tooltip("Radio del barrido del empujón.")]
        [SerializeField] private float shoveRadius = 0.9f;

        [Tooltip("Fuerza con la que el empujón aparta al enemigo.")]
        [SerializeField] private float shoveStrength = 3f;

        [Tooltip("Segundos antes de poder volver a empujar.")]
        [SerializeField] private float shoveCooldown = 0.4f;

        [Tooltip("Segundos de jadeo bloqueado cuando no hay resistencia para empujar.")]
        [SerializeField] private float shoveWheezeLock = 0.6f;

        [Header("IA de enemigos")]
        [Tooltip("Velocidad de patrulla y búsqueda (m/s).")]
        [SerializeField] private float enemyWalkSpeed = 1.4f;

        [Tooltip("Velocidad de caza (m/s).")]
        [SerializeField] private float enemyChaseSpeed = 3.1f;

        [Tooltip("Velocidad de giro del cuerpo en grados por segundo.")]
        [SerializeField] private float enemyTurnSpeed = 220f;

        [Tooltip("Aceleración del NavMeshAgent (m/s²).")]
        [SerializeField] private float enemyAcceleration = 12f;

        [Tooltip("Distancia a la que el agente considera que ha llegado (m).")]
        [SerializeField] private float enemyStoppingDistance = 0.4f;

        [Tooltip("Alcance máximo de visión (m).")]
        [SerializeField] private float enemyVisionRange = 12f;

        [Tooltip("Ángulo total del cono de visión (grados).")]
        [SerializeField] private float enemyVisionAngle = 100f;

        [Tooltip("Altura de los ojos sobre la base del enemigo (m).")]
        [SerializeField] private float enemyEyeHeight = 1.6f;

        [Tooltip("Adelanto del origen del rayo de visión para no chocar con su propia cápsula (m).")]
        [SerializeField] private float enemyEyeForwardOffset = 0.6f;

        [Tooltip("Altura del punto del jugador al que mira la IA (m).")]
        [SerializeField] private float enemyVisionTargetHeight = 1.2f;

        [Tooltip("Subida de alerta por segundo con el jugador a la vista y cerca (0-1).")]
        [SerializeField] private float enemyVisionAlertPerSecond = 0.9f;

        [Tooltip("Factor de subida de alerta con el jugador en el borde del alcance (0-1).")]
        [SerializeField] private float enemyVisionEdgeFactor = 0.35f;

        [Tooltip("Multiplicador de subida de alerta mientras la linterna lo ilumina.")]
        [SerializeField] private float enemyFlashlightAlertMultiplier = 2f;

        [Tooltip("Segundos que dura la marca de 'iluminado' entre escaneos de la linterna.")]
        [SerializeField] private float enemyFlashlightMemory = 0.3f;

        [Tooltip("Alerta que aporta un pulso de ruido oído.")]
        [SerializeField] private float enemyHearingAlert = 0.5f;

        [Tooltip("Multiplicador del radio de ruido que oye la IA.")]
        [SerializeField] private float enemyHearingMultiplier = 1f;

        [Tooltip("Bajada de alerta por segundo sin estímulos.")]
        [SerializeField] private float enemyAlertDecayPerSecond = 0.25f;

        [Tooltip("Alerta mínima para pasar a Sospecha.")]
        [SerializeField] private float enemySuspicionThreshold = 0.3f;

        [Tooltip("Alerta mínima para pasar a Búsqueda.")]
        [SerializeField] private float enemySearchThreshold = 0.6f;

        [Tooltip("Alerta mínima para pasar a Caza.")]
        [SerializeField] private float enemyChaseThreshold = 0.95f;

        [Tooltip("Segundos que duda en Sospecha antes de pasar a Búsqueda.")]
        [SerializeField] private float enemySuspicionDuration = 1.5f;

        [Tooltip("Segundos máximos de Búsqueda antes de volver a Patrulla.")]
        [SerializeField] private float enemySearchDuration = 8f;

        [Tooltip("Segundos sin ver al jugador en Caza antes de pasar a Búsqueda.")]
        [SerializeField] private float enemyLoseSightDuration = 3f;

        [Tooltip("Segundos de persecución tras cruzar una puerta.")]
        [SerializeField] private float enemyChaseAcrossDoorDuration = 4f;

        [Tooltip("Distancia de ataque (m).")]
        [SerializeField] private float enemyAttackRange = 2.5f;

        [Tooltip("Multiplicador de la distancia de ataque al soltar al jugador.")]
        [SerializeField] private float enemyAttackReleaseFactor = 1.25f;

        [Tooltip("Segundos entre ataques.")]
        [SerializeField] private float enemyAttackInterval = 1.6f;

        [Tooltip("Segundos de espera en cada punto de patrulla.")]
        [SerializeField] private float enemyPatrolWait = 2f;

        [Tooltip("Intentos de muestreo del NavMesh al elegir un punto de patrulla.")]
        [SerializeField] private int enemyPatrolSampleAttempts = 12;

        [Tooltip("Radio de búsqueda alrededor del punto muestreado (m).")]
        [SerializeField] private float enemyPatrolSampleRadius = 2f;

        [Tooltip("Radio de las vueltas durante la Búsqueda (m).")]
        [SerializeField] private float enemySearchRadius = 5f;

        [Tooltip("Distancia máxima para recolocar al enemigo sobre el NavMesh al arrancar (m).")]
        [SerializeField] private float enemyNavSnapDistance = 2f;

        [Tooltip("Máximo de enemigos que pueden estar a la vez en una misma sala.")]
        [SerializeField] private int enemyMaxPerRoom = 2;

        [Tooltip("Cada cuántos segundos se comprueba el cupo de la sala (s).")]
        [SerializeField] private float enemyRoomCheckInterval = 0.5f;

        [Tooltip("Sala prohibida para la IA; debe coincidir con la zona segura.")]
        [SerializeField] private string enemySafeRoomId = "FilaCapsulas";

        [Header("IA de enemigos — tipos")]
        [Tooltip("Alto: cápsula de 2.5 m, patrulla 1.2, caza 3.0, visión 15 m, oído 6 m, 4 disparos, 2 golpes, brazo largo, agarra.")]
        [SerializeField] private EnemyProfile enemyProfileAlto = new EnemyProfile
        {
            displayName = "Alto",
            bodyColor = new Color(0.55f, 0.15f, 0.18f, 1f),
            capsuleHeight = 2.5f,
            capsuleRadius = 0.42f,
            patrolSpeed = 1.2f,
            chaseSpeed = 3f,
            visionRange = 15f,
            hearingRange = 6f,
            eyeHeight = 2.15f,
            eyeForwardOffset = 0.55f,
            attackRange = 2.8f,
            attackInterval = 1.6f,
            hitType = Heartbeat.Player.EnemyHitType.Heavy,
            shotsToKill = 4,
            canGrab = true,
            flanks = false,
            flankAngle = 0f,
            flankDistance = 0f
        };

        [Tooltip("Rastrero: cápsula baja y aplastada, patrulla 1.8, caza 4.2, visión y oído 8 m, 2 disparos, 4 golpes, flanquea, no agarra.")]
        [SerializeField] private EnemyProfile enemyProfileRastrero = new EnemyProfile
        {
            displayName = "Rastrero",
            bodyColor = new Color(0.16f, 0.42f, 0.5f, 1f),
            capsuleHeight = 1.05f,
            capsuleRadius = 0.5f,
            patrolSpeed = 1.8f,
            chaseSpeed = 4.2f,
            visionRange = 8f,
            hearingRange = 8f,
            eyeHeight = 0.7f,
            eyeForwardOffset = 0.45f,
            attackRange = 1.5f,
            attackInterval = 1.6f,
            hitType = Heartbeat.Player.EnemyHitType.Light,
            shotsToKill = 2,
            canGrab = false,
            flanks = true,
            flankAngle = 55f,
            flankDistance = 3.5f
        };

        [Tooltip("Segundos que el punzón aturde al enemigo que agarra (s).")]
        [SerializeField] private float enemyGrabStunDuration = 2f;

        [Header("IA de enemigos — beeps de estado")]
        [Tooltip("Volumen de los beeps.")]
        [SerializeField] private float enemyBeepVolume = 0.35f;

        [Tooltip("Distancia máxima audible de los beeps (m).")]
        [SerializeField] private float enemyBeepMaxDistance = 25f;

        [Tooltip("Duración de cada beep (s).")]
        [SerializeField] private float enemyBeepDuration = 0.12f;

        [Tooltip("Ventana de arbitraje: si varios enemigos pitan a la vez, suena el estado más alto (s).")]
        [SerializeField] private float enemyBeepPriorityWindow = 0.15f;

        [Tooltip("Segundos entre beeps en Patrulla.")]
        [SerializeField] private float enemyBeepIntervalPatrol = 3.5f;

        [Tooltip("Segundos entre beeps en Sospecha.")]
        [SerializeField] private float enemyBeepIntervalSuspicion = 2.5f;

        [Tooltip("Segundos entre beeps en Búsqueda.")]
        [SerializeField] private float enemyBeepIntervalSearch = 1.6f;

        [Tooltip("Segundos entre beeps en Caza.")]
        [SerializeField] private float enemyBeepIntervalChase = 1f;

        [Tooltip("Segundos entre beeps en Atacar.")]
        [SerializeField] private float enemyBeepIntervalAttack = 0.6f;

        [Tooltip("Tono (Hz) del beep en Patrulla.")]
        [SerializeField] private int enemyBeepTonePatrol = 440;

        [Tooltip("Tono (Hz) del beep en Sospecha.")]
        [SerializeField] private int enemyBeepToneSuspicion = 560;

        [Tooltip("Tono (Hz) del beep en Búsqueda.")]
        [SerializeField] private int enemyBeepToneSearch = 700;

        [Tooltip("Tono (Hz) del beep en Caza.")]
        [SerializeField] private int enemyBeepToneChase = 900;

        [Tooltip("Tono (Hz) del beep en Atacar.")]
        [SerializeField] private int enemyBeepToneAttack = 1150;

        [Header("IA de enemigos — debug (F1)")]
        [Tooltip("Color de la cápsula en Patrulla.")]
        [SerializeField] private Color enemyDebugPatrolColor = new Color(0.25f, 1f, 0.45f);

        [Tooltip("Color de la cápsula en Sospecha.")]
        [SerializeField] private Color enemyDebugSuspicionColor = new Color(1f, 0.92f, 0.25f);

        [Tooltip("Color de la cápsula en Búsqueda.")]
        [SerializeField] private Color enemyDebugSearchColor = new Color(1f, 0.55f, 0.15f);

        [Tooltip("Color de la cápsula en Caza.")]
        [SerializeField] private Color enemyDebugChaseColor = new Color(1f, 0.2f, 0.15f);

        [Tooltip("Color de la cápsula en Atacar.")]
        [SerializeField] private Color enemyDebugAttackColor = new Color(0.85f, 0.2f, 1f);

        [Header("Vista de vigilancia (Q)")]
        [Tooltip("FOV de la cámara isométrica de la sala.")]
        [SerializeField] private float surveillanceFov = 50f;

        [Tooltip("Inclinación fija de la vista isométrica (grados hacia abajo).")]
        [SerializeField] private float surveillancePitch = 52f;

        [Tooltip("Giro fijo para salas sin cámara física asignada (grados).")]
        [SerializeField] private float surveillanceYaw = 45f;

        [Tooltip("Distancia de la cámara = lado mayor de la sala * este factor.")]
        [SerializeField] private float surveillanceFraming = 1.05f;

        [Tooltip("Margen extra de encuadre sobre la sala (m).")]
        [SerializeField] private float surveillancePadding = 1.2f;

        [Tooltip("Multiplicador de velocidad del jugador mientras usa la vista.")]
        [SerializeField] private float surveillanceSpeedMultiplier = 0.35f;

        [Tooltip("Escala de la textura del feed respecto a la pantalla (0-1).")]
        [SerializeField] private float surveillanceFeedScale = 0.5f;

        [Tooltip("Fotogramas por segundo que se guardan en el búfer de retardo.")]
        [SerializeField] private float surveillanceFeedFps = 20f;

        [Tooltip("Retardo de la imagen en segundos con la señal limpia (s).")]
        [SerializeField] private float surveillanceDelay = 0.3f;

        [Tooltip("Retardo de la imagen con enemigos en Búsqueda (s).")]
        [SerializeField] private float surveillanceDelaySearch = 0.65f;

        [Tooltip("Ruido de la imagen (0-1) con la señal limpia.")]
        [SerializeField] private float surveillanceNoise = 0.05f;

        [Tooltip("Ruido de la imagen (0-1) con enemigos en Búsqueda.")]
        [SerializeField] private float surveillanceNoiseSearch = 0.16f;

        [Tooltip("Segundos de uso continuado para llenar la atención de The Mother.")]
        [SerializeField] private float motherAttentionFillDuration = 5f;

        [Tooltip("Segundos sin usar la vista para vaciar la atención de The Mother.")]
        [SerializeField] private float motherAttentionDrainDuration = 15f;

        [Header("Reciclaje de enemigos")]
        [Tooltip("Segundos que el cuerpo queda en el suelo antes de empezar a avisar (s).")]
        [SerializeField] private float corpseLifetime = 40f;

        [Tooltip("Segundos que tiembla y suena el aviso antes de desaparecer (s).")]
        [SerializeField] private float corpseWarningDuration = 3f;

        [Tooltip("Segundos entre avisos sonoros durante el temblor (s).")]
        [SerializeField] private float corpseWarningInterval = 0.75f;

        [Tooltip("Tono (Hz) del aviso de reciclaje.")]
        [SerializeField] private int corpseWarningTone = 220;

        [Tooltip("Volumen del aviso de reciclaje (0-1).")]
        [SerializeField] private float corpseWarningVolume = 0.45f;

        [Tooltip("Amplitud del temblor del cuerpo durante el aviso (m).")]
        [SerializeField] private float corpseShakeAmplitude = 0.04f;

        [Tooltip("Velocidad extra del enemigo reciclado (1.25 = 25 % más rápido).")]
        [SerializeField] private float recycledSpeedMultiplier = 1.25f;

        [Tooltip("Disparos que aguanta un Alto reciclado.")]
        [SerializeField] private int recycledShotsToKill = 2;

        [Tooltip("Disparos que aguanta un Rastrero reciclado.")]
        [SerializeField] private int recycledShotsToKillRastrero = 1;

        [Header("Remate")]
        [Tooltip("Distancia máxima al cuerpo para poder rematarlo (m).")]
        [SerializeField] private float finisherRange = 1.5f;

        [Tooltip("Radio en viewport (0-1) alrededor del centro de pantalla para apuntar a un cuerpo.")]
        [SerializeField] private float finisherAimRadius = 0.35f;

        [Tooltip("Fracción de la resistencia máxima que cuesta el pisotón (0-1).")]
        [SerializeField] private float finisherStompStaminaCost01 = 0.25f;

        [Tooltip("Segundos que tarda el pisotón (s).")]
        [SerializeField] private float finisherStompDuration = 2f;

        [Tooltip("Radio de ruido del pisotón al impactar (m).")]
        [SerializeField] private float finisherStompNoiseRadius = 8f;

        [Tooltip("Segundos que tarda el remate con punzón (s).")]
        [SerializeField] private float finisherSpikeDuration = 1.5f;

        [Tooltip("Segundos de bloqueo cuando falta resistencia para el pisotón (s).")]
        [SerializeField] private float finisherWheezeLock = 0.6f;

        [Header("Hackeo de paneles")]
        [Tooltip("Alcance al panel o a la hoja de la puerta para hackear (m).")]
        [SerializeField] private float hackRange = 2f;

        [Tooltip("Radio en viewport (0-1) alrededor del centro de pantalla para apuntar a un panel.")]
        [SerializeField] private float hackAimRadius = 0.3f;

        [Tooltip("Flechas mínimas para sellar una puerta.")]
        [SerializeField] private int hackSelladoMin = 2;

        [Tooltip("Flechas máximas para sellar una puerta.")]
        [SerializeField] private int hackSelladoMax = 3;

        [Tooltip("Flechas mínimas de una puerta estándar.")]
        [SerializeField] private int hackPuertaMin = 4;

        [Tooltip("Flechas máximas de una puerta estándar.")]
        [SerializeField] private int hackPuertaMax = 5;

        [Tooltip("Flechas mínimas de la escalera de la sala final.")]
        [SerializeField] private int hackEscaleraMin = 7;

        [Tooltip("Flechas máximas de la escalera de la sala final.")]
        [SerializeField] private int hackEscaleraMax = 8;

        [Tooltip("Segundos que se resalta el fallo y la secuencia vuelve a empezar (s).")]
        [SerializeField] private float hackErrorFlashDuration = 0.45f;

        [Tooltip("Segundos de golpes de un enemigo para romper una puerta cerrada (s).")]
        [SerializeField] private float sealBreakDuration = 30f;

        [Tooltip("Distancia máxima de un enemigo al vano para contar como golpe (m).")]
        [SerializeField] private float sealBreakRange = 2.2f;

        [Tooltip("Ángulo de apertura de la hoja de una puerta (grados).")]
        [SerializeField] private float doorOpenAngle = 95f;

        [Tooltip("Velocidad de giro de la hoja de una puerta (grados por segundo).")]
        [SerializeField] private float doorOpenSpeed = 220f;

        [Header("Muerte y reaparición")]
        [Tooltip("Segundos que tarda la pantalla en irse a negro al morir (s).")]
        [SerializeField] private float deathFadeInDuration = 0.35f;

        [Tooltip("Segundos que se mantiene el placeholder de la cinemática de reconstrucción; Espacio lo salta (s).")]
        [SerializeField] private float reconstructionDuration = 2.5f;

        [Tooltip("Segundos que tarda el negro en retirarse al reaparecer en FilaCapsulas (s).")]
        [SerializeField] private float respawnFadeOutDuration = 0.8f;

        [Tooltip("Número del primer código de sujeto; cada muerte suma uno (R-H-07, R-H-08...).")]
        [SerializeField] private int subjectCodeStart = 7;

        [Header("Colores de objeto")]
        [SerializeField] private Color itemHealColor = new Color(0.3f, 1f, 0.45f);
        [SerializeField] private Color itemAmmoColor = new Color(1f, 0.85f, 0.2f);
        [SerializeField] private Color itemSpikeColor = new Color(0.45f, 0.65f, 1f);
        [SerializeField] private Color itemKeyColor = new Color(1f, 0.75f, 0.15f);

        public float WallHeight => wallHeight;
        public float WallThickness => wallThickness;
        public float DoorWidth => doorWidth;
        public float DoorHeight => doorHeight;
        public float CapsuleSpacing => capsuleSpacing;

        public Color AmbientColor => ambientColor;
        public Color EmergencyLightColor => emergencyLightColor;
        public float EmergencyLightIntensity => emergencyLightIntensity;
        public float EmergencyLightRange => emergencyLightRange;
        public float EmergencyLightAngle => emergencyLightAngle;
        public float EmergencyLightHeight => emergencyLightHeight;
        public bool EmergencyLightShadows => emergencyLightShadows;

        public float PlayerHeight => playerHeight;
        public float PlayerRadius => playerRadius;
        public float CrouchHeight => crouchHeight;
        public float CrouchSpeed => crouchSpeed;
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float Gravity => gravity;

        public float StaminaMax => staminaMax;
        public float StaminaDrainPerSecond => staminaDrainPerSecond;
        public float StaminaRegenDelay => staminaRegenDelay;
        public float StaminaRegenPerSecond => staminaRegenPerSecond;
        public float ExhaustionDuration => exhaustionDuration;

        public float NoiseRadiusCrouch => noiseRadiusCrouch;
        public float NoiseRadiusWalk => noiseRadiusWalk;
        public float NoiseRadiusRun => noiseRadiusRun;
        public float NoisePulseInterval => noisePulseInterval;

        public float CameraFov => cameraFov;
        public float CameraAimFov => cameraAimFov;
        public float CameraDistance => cameraDistance;
        public float CameraAimDistance => cameraAimDistance;
        public float CameraShoulder => cameraShoulder;
        public float CameraAimShoulder => cameraAimShoulder;
        public float CameraCollisionRadius => cameraCollisionRadius;
        public float CameraCollisionPadding => cameraCollisionPadding;
        public float CameraMinDistance => cameraMinDistance;
        public float CameraPullInTime => cameraPullInTime;
        public float CameraPullOutTime => cameraPullOutTime;
        public float CameraFollowTime => cameraFollowTime;
        public float CameraFovSharpness => cameraFovSharpness;
        public float CameraStartPitch => cameraStartPitch;
        public float LookSensitivity => lookSensitivity;
        public float PitchMin => pitchMin;
        public float PitchMax => pitchMax;

        public float AimSpeedMultiplier => aimSpeedMultiplier;

        public float ReticleDotSize => reticleDotSize;
        public float ReticleClosedRadius => reticleClosedRadius;
        public float ReticleOpenRadius => reticleOpenRadius;
        public float ReticleCloseDuration => reticleCloseDuration;
        public float ReticleOpenSpeed => reticleOpenSpeed;
        public float ReticleMoveOpen => reticleMoveOpen;
        public float ReticleRunOpen => reticleRunOpen;
        public float ReticleHitOpen => reticleHitOpen;
        public float ReticleHitRecovery => reticleHitRecovery;

        public float FlashlightDuration => flashlightDuration;
        public float FlashlightRechargeDelay => flashlightRechargeDelay;
        public float FlashlightRechargeDuration => flashlightRechargeDuration;
        public float FlashlightMinTurnOn => flashlightMinTurnOn;
        public float FlashlightFlickerThreshold => flashlightFlickerThreshold;
        public float FlashlightHipSpotAngle => flashlightHipSpotAngle;
        public float FlashlightHipRange => flashlightHipRange;
        public float FlashlightHipIntensity => flashlightHipIntensity;
        public float FlashlightAimSpotAngle => flashlightAimSpotAngle;
        public float FlashlightAimRange => flashlightAimRange;
        public float FlashlightAimIntensity => flashlightAimIntensity;
        public float FlashlightConeSharpness => flashlightConeSharpness;
        public float FlashlightScanInterval => flashlightScanInterval;
        public float FlashlightMuzzleOffset => flashlightMuzzleOffset;

        public int PlayerMaxHits => playerMaxHits;
        public float PlayerHurtRatio => playerHurtRatio;
        public float PlayerCriticalRatio => playerCriticalRatio;
        public float HurtSpeedMultiplier => hurtSpeedMultiplier;
        public float CriticalSpeedMultiplier => criticalSpeedMultiplier;
        public int HitLightHits => hitLightHits;
        public int HitHeavyHits => hitHeavyHits;
        public int HitBrutalHits => hitBrutalHits;
        public float HealDuration => healDuration;
        public int PlayerStartHeals => playerStartHeals;
        public float DamageShakeMagnitude => damageShakeMagnitude;
        public float DamageShakeAngle => damageShakeAngle;
        public float DamageShakeDuration => damageShakeDuration;

        public int InventorySlots => inventorySlots;
        public float ItemActionDuration => itemActionDuration;
        public float PickupDistance => pickupDistance;
        public int AmmoStackSize => ammoStackSize;
        public int AmmoPickupAmount => ammoPickupAmount;
        public int StartAmmoBullets => startAmmoBullets;
        public float ItemDropDistance => itemDropDistance;

        public float ItemDropHeight => itemDropHeight;
        public float KeyUseRange => keyUseRange;

        public int RevolverDrumSize => revolverDrumSize;
        public float RevolverFireInterval => revolverFireInterval;
        public float RevolverReloadPerBullet => revolverReloadPerBullet;
        public int RevolverDamage => revolverDamage;
        public float RevolverRange => revolverRange;
        public float RevolverShotNoiseRadius => revolverShotNoiseRadius;
        public float RevolverRecoilAngle => revolverRecoilAngle;
        public float RevolverRecoilDuration => revolverRecoilDuration;

        public float ShoveStaminaCost01 => shoveStaminaCost01;
        public float ShoveRange => shoveRange;
        public float ShoveRadius => shoveRadius;
        public float ShoveStrength => shoveStrength;
        public float ShoveCooldown => shoveCooldown;
        public float ShoveWheezeLock => shoveWheezeLock;

        public float EnemyWalkSpeed => enemyWalkSpeed;
        public float EnemyChaseSpeed => enemyChaseSpeed;
        public float EnemyTurnSpeed => enemyTurnSpeed;
        public float EnemyAcceleration => enemyAcceleration;
        public float EnemyStoppingDistance => enemyStoppingDistance;

        public float EnemyVisionRange => enemyVisionRange;
        public float EnemyVisionAngle => enemyVisionAngle;
        public float EnemyEyeHeight => enemyEyeHeight;
        public float EnemyEyeForwardOffset => enemyEyeForwardOffset;
        public float EnemyVisionTargetHeight => enemyVisionTargetHeight;
        public float EnemyVisionAlertPerSecond => enemyVisionAlertPerSecond;
        public float EnemyVisionEdgeFactor => enemyVisionEdgeFactor;
        public float EnemyFlashlightAlertMultiplier => enemyFlashlightAlertMultiplier;
        public float EnemyFlashlightMemory => enemyFlashlightMemory;
        public float EnemyHearingAlert => enemyHearingAlert;
        public float EnemyHearingMultiplier => enemyHearingMultiplier;
        public float EnemyAlertDecayPerSecond => enemyAlertDecayPerSecond;

        public float EnemySuspicionThreshold => enemySuspicionThreshold;
        public float EnemySearchThreshold => enemySearchThreshold;
        public float EnemyChaseThreshold => enemyChaseThreshold;
        public float EnemySuspicionDuration => enemySuspicionDuration;
        public float EnemySearchDuration => enemySearchDuration;
        public float EnemyLoseSightDuration => enemyLoseSightDuration;
        public float EnemyChaseAcrossDoorDuration => enemyChaseAcrossDoorDuration;

        public float EnemyAttackRange => enemyAttackRange;
        public float EnemyAttackReleaseFactor => enemyAttackReleaseFactor;
        public float EnemyAttackInterval => enemyAttackInterval;

        public float EnemyPatrolWait => enemyPatrolWait;
        public int EnemyPatrolSampleAttempts => enemyPatrolSampleAttempts;
        public float EnemyPatrolSampleRadius => enemyPatrolSampleRadius;
        public float EnemySearchRadius => enemySearchRadius;
        public float EnemyNavSnapDistance => enemyNavSnapDistance;
        public int EnemyMaxPerRoom => enemyMaxPerRoom;
        public float EnemyRoomCheckInterval => enemyRoomCheckInterval;
        public string EnemySafeRoomId => enemySafeRoomId;

        /// <summary>Perfil de cifras del tipo de enemigo pedido.</summary>
        public EnemyProfile GetEnemyProfile(Heartbeat.AI.EnemyType type)
        {
            return type == Heartbeat.AI.EnemyType.Rastrero ? enemyProfileRastrero : enemyProfileAlto;
        }

        /// <summary>Segundos que aturde el punzón al enemigo que agarra.</summary>
        public float EnemyGrabStunDuration => enemyGrabStunDuration;

        public float EnemyBeepVolume => enemyBeepVolume;
        public float EnemyBeepMaxDistance => enemyBeepMaxDistance;
        public float EnemyBeepDuration => enemyBeepDuration;
        public float EnemyBeepPriorityWindow => enemyBeepPriorityWindow;

        public float GetEnemyBeepInterval(Heartbeat.AI.EnemyState state)
        {
            switch (state)
            {
                case Heartbeat.AI.EnemyState.Sospecha: return enemyBeepIntervalSuspicion;
                case Heartbeat.AI.EnemyState.Busqueda: return enemyBeepIntervalSearch;
                case Heartbeat.AI.EnemyState.Caza: return enemyBeepIntervalChase;
                case Heartbeat.AI.EnemyState.Atacar: return enemyBeepIntervalAttack;
                default: return enemyBeepIntervalPatrol;
            }
        }

        public int GetEnemyBeepTone(Heartbeat.AI.EnemyState state)
        {
            switch (state)
            {
                case Heartbeat.AI.EnemyState.Sospecha: return enemyBeepToneSuspicion;
                case Heartbeat.AI.EnemyState.Busqueda: return enemyBeepToneSearch;
                case Heartbeat.AI.EnemyState.Caza: return enemyBeepToneChase;
                case Heartbeat.AI.EnemyState.Atacar: return enemyBeepToneAttack;
                default: return enemyBeepTonePatrol;
            }
        }

        public Color GetEnemyStateColor(Heartbeat.AI.EnemyState state)
        {
            switch (state)
            {
                case Heartbeat.AI.EnemyState.Sospecha: return enemyDebugSuspicionColor;
                case Heartbeat.AI.EnemyState.Busqueda: return enemyDebugSearchColor;
                case Heartbeat.AI.EnemyState.Caza: return enemyDebugChaseColor;
                case Heartbeat.AI.EnemyState.Atacar: return enemyDebugAttackColor;
                default: return enemyDebugPatrolColor;
            }
        }

        public float SurveillanceFov => surveillanceFov;
        public float SurveillancePitch => surveillancePitch;
        public float SurveillanceYaw => surveillanceYaw;
        public float SurveillanceFraming => surveillanceFraming;
        public float SurveillancePadding => surveillancePadding;
        public float SurveillanceSpeedMultiplier => surveillanceSpeedMultiplier;
        public float SurveillanceFeedScale => surveillanceFeedScale;
        public float SurveillanceFeedFps => surveillanceFeedFps;
        public float SurveillanceDelay => surveillanceDelay;
        public float SurveillanceDelaySearch => surveillanceDelaySearch;
        public float SurveillanceNoise => surveillanceNoise;
        public float SurveillanceNoiseSearch => surveillanceNoiseSearch;

        public float MotherAttentionFillDuration => motherAttentionFillDuration;
        public float MotherAttentionDrainDuration => motherAttentionDrainDuration;

        public float CorpseLifetime => corpseLifetime;
        public float CorpseWarningDuration => corpseWarningDuration;
        public float CorpseWarningInterval => corpseWarningInterval;
        public int CorpseWarningTone => corpseWarningTone;
        public float CorpseWarningVolume => corpseWarningVolume;
        public float CorpseShakeAmplitude => corpseShakeAmplitude;
        public float RecycledSpeedMultiplier => recycledSpeedMultiplier;

        /// <summary>Disparos que aguanta un enemigo reciclado según su tipo.</summary>
        public int GetRecycledShotsToKill(Heartbeat.AI.EnemyType type)
        {
            return type == Heartbeat.AI.EnemyType.Rastrero
                ? Mathf.Max(1, recycledShotsToKillRastrero)
                : Mathf.Max(1, recycledShotsToKill);
        }

        public float FinisherRange => finisherRange;
        public float FinisherAimRadius => finisherAimRadius;
        public float FinisherStompStaminaCost01 => finisherStompStaminaCost01;
        public float FinisherStompDuration => finisherStompDuration;
        public float FinisherStompNoiseRadius => finisherStompNoiseRadius;
        public float FinisherSpikeDuration => finisherSpikeDuration;
        public float FinisherWheezeLock => finisherWheezeLock;

        public float HackRange => hackRange;
        public float HackAimRadius => hackAimRadius;
        public int HackSelladoMin => hackSelladoMin;
        public int HackSelladoMax => hackSelladoMax;
        public int HackPuertaMin => hackPuertaMin;
        public int HackPuertaMax => hackPuertaMax;
        public int HackEscaleraMin => hackEscaleraMin;
        public int HackEscaleraMax => hackEscaleraMax;
        public float HackErrorFlashDuration => hackErrorFlashDuration;
        public float SealBreakDuration => sealBreakDuration;
        public float SealBreakRange => sealBreakRange;
        public float DoorOpenAngle => doorOpenAngle;
        public float DoorOpenSpeed => doorOpenSpeed;

        public float DeathFadeInDuration => deathFadeInDuration;
        public float ReconstructionDuration => reconstructionDuration;
        public float RespawnFadeOutDuration => respawnFadeOutDuration;
        public int SubjectCodeStart => subjectCodeStart;

        public Color GetItemColor(Heartbeat.Items.ItemType type)
        {
            switch (type)
            {
                case Heartbeat.Items.ItemType.Heal: return itemHealColor;
                case Heartbeat.Items.ItemType.Ammo: return itemAmmoColor;
                case Heartbeat.Items.ItemType.Spike: return itemSpikeColor;
                case Heartbeat.Items.ItemType.Key: return itemKeyColor;
                default: return Color.white;
            }
        }
    }
}
