using System;
using Heartbeat.Combat;
using Heartbeat.Config;
using Heartbeat.Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>
    /// Revólver de 6 balas y empujón, ambos con el clic izquierdo.
    /// Apuntando (clic derecho) el clic dispara: 0.8 s entre disparos, ruido de 20 m,
    /// chasquido en el vacío y nunca recarga solo. Sin apuntar, el clic empuja gastando
    /// el 35 % de la resistencia; sin resistencia suficiente sólo hay jadeo.
    /// R recarga bala a bala desde las casillas de munición; un golpe cancela la recarga
    /// conservando las balas ya cargadas.
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    public class PlayerWeapon : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private PlayerAim aim;
        [SerializeField] private PlayerHealth health;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerStamina stamina;
        [SerializeField] private PlayerNoiseEmitter noiseEmitter;
        [SerializeField] private ThirdPersonCamera cameraRig;
        [Tooltip("Remate de cuerpos: sin apuntar, el clic remata en vez de empujar.")]
        [SerializeField] private PlayerFinisher finisher;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private const string ActionMapName = "Player";

        /// <summary>Holgura de distancia para que un punto débil solapado con el cuerpo cuente.</summary>
        private const float WeakPointTolerance = 0.05f;

        private InputAction attackAction;
        private InputAction reloadAction;
        private readonly RaycastHit[] hitBuffer = new RaycastHit[8];
        private readonly Collider[] shoveBuffer = new Collider[16];

        private int drum;
        private float nextFireTime;
        private float nextShoveTime;
        private bool isReloading;
        private float reloadTimer;

        /// <summary>Balas cargadas en el tambor.</summary>
        public int DrumBullets => drum;

        /// <summary>Capacidad del tambor.</summary>
        public int DrumSize => config != null ? config.RevolverDrumSize : 0;

        public bool IsDrumEmpty => drum <= 0;
        public bool IsReloading => isReloading;

        /// <summary>0-1 mientras recarga la bala en curso.</summary>
        public float ReloadProgress01
        {
            get
            {
                if (!isReloading || config == null || config.RevolverReloadPerBullet <= 0f) return 0f;
                return Mathf.Clamp01(reloadTimer / config.RevolverReloadPerBullet);
            }
        }

        /// <summary>Cambió el número de balas del tambor.</summary>
        public event Action<int> DrumChanged;

        /// <summary>Se disparó un tiro.</summary>
        public event Action Fired;

        /// <summary>Empezó o terminó una recarga.</summary>
        public event Action<bool> ReloadChanged;

        /// <summary>Se dio un empujón (cancela la vista de vigilancia).</summary>
        public event Action Shoved;

        private void Awake()
        {
            if (aim == null) aim = GetComponent<PlayerAim>();
            if (health == null) health = GetComponent<PlayerHealth>();
            if (inventory == null) inventory = GetComponent<PlayerInventory>();
            if (stamina == null) stamina = GetComponent<PlayerStamina>();
            if (noiseEmitter == null) noiseEmitter = GetComponent<PlayerNoiseEmitter>();
            if (finisher == null) finisher = GetComponent<PlayerFinisher>();
            if (cameraRig == null) cameraRig = GetComponentInChildren<ThirdPersonCamera>();
            if (viewCamera == null) viewCamera = GetComponentInChildren<Camera>();

            drum = 0;
        }

        private void OnEnable()
        {
            if (health != null) health.Damaged += OnDamaged;

            if (inputActions == null) return;

            InputActionMap map = inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
            if (map == null) return;

            attackAction = map.FindAction("Attack", throwIfNotFound: false);
            reloadAction = map.FindAction("Reload", throwIfNotFound: false);

            if (attackAction == null)
            {
                Debug.LogWarning("[Revólver] Falta la acción 'Attack' en el mapa Player.", this);
            }
            else
            {
                attackAction.Enable();
                attackAction.performed += OnAttackPerformed;
            }

            if (reloadAction == null)
            {
                Debug.LogWarning("[Revólver] Falta la acción 'Reload' en el mapa Player.", this);
            }
            else
            {
                reloadAction.Enable();
                reloadAction.performed += OnReloadPerformed;
            }
        }

        private void OnDisable()
        {
            if (health != null) health.Damaged -= OnDamaged;

            if (attackAction != null)
            {
                attackAction.performed -= OnAttackPerformed;
                attackAction.Disable();
            }

            if (reloadAction != null)
            {
                reloadAction.performed -= OnReloadPerformed;
                reloadAction.Disable();
            }

            attackAction = null;
            reloadAction = null;
        }

        private void Update()
        {
            if (!isReloading || config == null) return;

            reloadTimer += Time.deltaTime;
            if (reloadTimer < config.RevolverReloadPerBullet) return;

            reloadTimer = 0f;

            if (!LoadOneBullet())
            {
                StopReload("recarga terminada");
                return;
            }

            if (drum >= DrumSize) StopReload("tambor lleno");
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (aim != null && aim.IsAiming)
            {
                TryFire();
                return;
            }

            // Sin apuntar y con un cuerpo delante, el clic es remate y no empujón.
            if (finisher != null && finisher.HasTarget)
            {
                finisher.TryStomp();
                return;
            }

            TryShove();
        }

        private void OnReloadPerformed(InputAction.CallbackContext context)
        {
            TryReload();
        }

        private void OnDamaged(int hits)
        {
            CancelReload();
        }

        /// <summary>Dispara si el tambor tiene bala y ha pasado el intervalo.</summary>
        public bool TryFire()
        {
            if (config == null) return false;
            if (health != null && health.IsDead) return false;
            if (isReloading) return false;
            if (Time.time < nextFireTime) return false;

            if (drum <= 0)
            {
                // Chasquido: no hay bala y el revólver no recarga solo.
                nextFireTime = Time.time + config.RevolverFireInterval;
                if (logEvents) Debug.Log("[Revólver] Chasquido: tambor vacío.");
                return false;
            }

            drum--;
            nextFireTime = Time.time + config.RevolverFireInterval;
            DrumChanged?.Invoke(drum);

            ShootRay();
            Recoil();

            if (noiseEmitter != null) noiseEmitter.EmitPulse(config.RevolverShotNoiseRadius);

            if (logEvents) Debug.Log($"[Revólver] Disparo: tambor {drum}/{DrumSize}.");
            Fired?.Invoke();
            return true;
        }

        /// <summary>Empuja lo que haya delante gastando resistencia; si falta, jadea.</summary>
        public bool TryShove()
        {
            if (config == null) return false;
            if (health != null && health.IsDead) return false;
            if (Time.time < nextShoveTime) return false;

            float cost = stamina != null ? stamina.Max * config.ShoveStaminaCost01 : 0f;

            if (stamina != null && !stamina.TrySpend(cost))
            {
                nextShoveTime = Time.time + config.ShoveWheezeLock;
                if (logEvents) Debug.Log($"[Empujón] Jadeo: sin resistencia (necesita {cost:F0}, hay {stamina.Current:F0}).");
                return false;
            }

            if (stamina != null) stamina.ReportExertion();
            nextShoveTime = Time.time + config.ShoveCooldown;

            Vector3 forward = FlatForward();
            Vector3 center = transform.position + Vector3.up + forward * (config.ShoveRange * 0.5f);
            float radius = config.ShoveRange * 0.5f;

            int count = Physics.OverlapSphereNonAlloc(center, radius, shoveBuffer, ~0, QueryTriggerInteraction.Ignore);
            int pushed = 0;

            for (int i = 0; i < count; i++)
            {
                Collider collider = shoveBuffer[i];
                if (collider == null) continue;
                if (collider.transform.root == transform.root) continue;

                IStaggerable target = collider.GetComponentInParent<IStaggerable>();
                if (target == null) continue;

                Vector3 toTarget = collider.transform.position - transform.position;
                toTarget.y = 0f;
                if (toTarget.sqrMagnitude > config.ShoveRange * config.ShoveRange) continue;
                if (Vector3.Dot(forward, toTarget.normalized) < 0.2f) continue;

                target.ApplyStagger(toTarget.normalized, config.ShoveStrength);
                pushed++;
            }

            if (logEvents) Debug.Log($"[Empujón] Empujados {pushed} ({cost:F0} de resistencia).");
            Shoved?.Invoke();
            return true;
        }

        /// <summary>Empieza la recarga bala a bala desde las casillas de munición.</summary>
        public bool TryReload()
        {
            if (config == null) return false;
            if (health != null && health.IsDead) return false;
            if (isReloading) return false;

            if (drum >= DrumSize)
            {
                if (logEvents) Debug.Log("[Revólver] El tambor ya está lleno.");
                return false;
            }

            if (inventory == null || inventory.CountOf(ItemType.Ammo) <= 0)
            {
                if (logEvents) Debug.Log("[Revólver] Sin munición en el inventario.");
                return false;
            }

            isReloading = true;
            reloadTimer = 0f;
            if (logEvents) Debug.Log($"[Revólver] Recargando ({drum}/{DrumSize})...");
            ReloadChanged?.Invoke(true);
            return true;
        }

        /// <summary>Corta la recarga conservando las balas ya cargadas.</summary>
        public void CancelReload()
        {
            if (!isReloading) return;

            isReloading = false;
            reloadTimer = 0f;
            if (logEvents) Debug.Log($"[Revólver] Recarga cancelada: se conservan {drum} balas.");
            ReloadChanged?.Invoke(false);
        }

        /// <summary>Fija el tambor (depuración y pruebas).</summary>
        public void SetDrum(int bullets)
        {
            drum = Mathf.Clamp(bullets, 0, DrumSize);
            DrumChanged?.Invoke(drum);
        }

        private bool LoadOneBullet()
        {
            if (drum >= DrumSize) return false;
            if (inventory == null) return false;
            if (!inventory.ConsumeOne(ItemType.Ammo)) return false;

            drum++;
            DrumChanged?.Invoke(drum);
            if (logEvents) Debug.Log($"[Revólver] Bala cargada ({drum}/{DrumSize}).");
            return true;
        }

        private void StopReload(string reason)
        {
            isReloading = false;
            reloadTimer = 0f;
            if (logEvents) Debug.Log($"[Revólver] Recarga: {reason} ({drum}/{DrumSize}).");
            ReloadChanged?.Invoke(false);
        }

        private void ShootRay()
        {
            Transform reference = viewCamera != null ? viewCamera.transform : transform;
            Ray ray = new Ray(reference.position, reference.forward);

            int count = Physics.RaycastNonAlloc(ray, hitBuffer, config.RevolverRange, ~0, QueryTriggerInteraction.Collide);

            RaycastHit best = default;
            float bestDistance = float.MaxValue;
            RaycastHit weakHit = default;
            float weakDistance = float.MaxValue;

            // El primer impacto sólido que no sea el propio jugador; los triggers (recogibles,
            // volúmenes de sala) no detienen la bala.
            for (int i = 0; i < count; i++)
            {
                Collider collider = hitBuffer[i].collider;
                if (collider == null) continue;
                if (collider.transform.root == transform.root) continue;
                if (collider.isTrigger) continue;

                if (hitBuffer[i].distance < weakDistance && collider.GetComponentInParent<WeakPoint>() != null)
                {
                    weakDistance = hitBuffer[i].distance;
                    weakHit = hitBuffer[i];
                }

                if (hitBuffer[i].distance >= bestDistance) continue;

                bestDistance = hitBuffer[i].distance;
                best = hitBuffer[i];
            }

            if (best.collider == null && weakHit.collider == null)
            {
                if (logEvents) Debug.Log("[Revólver] Disparo al aire.");
                return;
            }

            // En los enemigos el colisionador del punto débil puede solaparse con el del cuerpo:
            // si el impacto cae casi a la misma distancia, gana el punto débil.
            bool hitWeakPoint = weakHit.collider != null && weakDistance <= bestDistance + WeakPointTolerance;
            RaycastHit impact = hitWeakPoint ? weakHit : best;

            IDamageable damageable = impact.collider.GetComponentInParent<IDamageable>();
            damageable?.ApplyDamage(config.RevolverDamage, impact.point);

            // Punto débil: interrumpe ataque o agarre, pero no resta menos disparos para matar.
            if (hitWeakPoint) impact.collider.GetComponentInParent<WeakPoint>()?.NotifyHit();

            if (logEvents)
            {
                Debug.Log($"[Revólver] Impacto en {impact.collider.name} a {impact.distance:F1} m"
                    + (hitWeakPoint ? " (punto débil)." : "."));
            }
        }

        private void Recoil()
        {
            if (cameraRig != null)
            {
                cameraRig.Shake(0f, config.RevolverRecoilAngle, config.RevolverRecoilDuration);
            }

            if (aim != null) aim.NotifyHit();
        }

        private Vector3 FlatForward()
        {
            Transform reference = viewCamera != null ? viewCamera.transform : transform;
            Vector3 forward = reference.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f) forward = transform.forward;

            return forward.normalized;
        }
    }
}
