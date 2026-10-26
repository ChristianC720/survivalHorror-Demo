using System;
using System.Collections.Generic;
using Heartbeat.AI;
using Heartbeat.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>
    /// Linterna del jugador: Spot Light que apunta hacia donde mira la cámara.
    /// Tecla F para encender/apagar. Sin apuntar usa un cono corto y amplio; apuntando,
    /// uno estrecho y largo. Tiene ~60 s de carga, se recarga tras ~3 s apagada y al
    /// llegar a 0 parpadea y se apaga hasta recuperar un mínimo.
    /// Expone <see cref="EnemyIlluminated"/> para la IA.
    /// </summary>
    public class Flashlight : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private Light spotLight;
        [SerializeField] private PlayerAim aim;
        [Tooltip("Transform desde el que se orienta el haz (la cámara).")]
        [SerializeField] private Transform aimSource;
        [Tooltip("Ancla de origen del haz (CameraTarget).")]
        [SerializeField] private Transform beamAnchor;
        [SerializeField] private InputActionAsset inputActions;

        [Header("Estado inicial")]
        [SerializeField] private bool startsOn;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private const string ActionMapName = "Player";

        private InputAction toggleAction;
        private readonly Collider[] overlapBuffer = new Collider[32];
        private readonly HashSet<IlluminatedTarget> litTargets = new HashSet<IlluminatedTarget>();
        private readonly List<IlluminatedTarget> toRemove = new List<IlluminatedTarget>();

        private float charge;
        private float timeSinceOff;
        private float baseIntensity;
        private float nextScanTime;
        private bool isOn;
        private bool depleted;

        /// <summary>Encendida.</summary>
        public bool IsOn => isOn;

        /// <summary>Se quedó sin carga: no enciende hasta recuperar el mínimo.</summary>
        public bool IsDepleted => depleted;

        /// <summary>Segundos de carga restantes.</summary>
        public float Charge => charge;

        /// <summary>Carga 0-1.</summary>
        public float Charge01 => config != null && config.FlashlightDuration > 0f
            ? Mathf.Clamp01(charge / config.FlashlightDuration)
            : 0f;

        /// <summary>Objetivos iluminados ahora mismo.</summary>
        public IReadOnlyCollection<IlluminatedTarget> LitTargets => litTargets;

        /// <summary>Evento para la IA: un enemigo acaba de quedar iluminado.</summary>
        public event Action<IlluminatedTarget> EnemyIlluminated;

        private void Awake()
        {
            if (spotLight == null) spotLight = GetComponent<Light>();
            if (aim == null) aim = GetComponentInParent<PlayerAim>();
            if (aimSource == null && transform.parent != null)
            {
                Transform cameraTransform = transform.parent.Find("Camera");
                if (cameraTransform != null) aimSource = cameraTransform;
            }

            if (config != null)
            {
                charge = config.FlashlightDuration;
                baseIntensity = config.FlashlightHipIntensity;
            }

            isOn = false;
            ApplyLightState();
        }

        private void OnEnable()
        {
            if (inputActions == null) return;

            InputActionMap map = inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
            if (map == null) return;

            toggleAction = map.FindAction("Flashlight", throwIfNotFound: false);
            if (toggleAction == null)
            {
                Debug.LogWarning("[Linterna] Falta la acción 'Flashlight' en el mapa Player.", this);
                return;
            }

            toggleAction.Enable();
            toggleAction.performed += OnTogglePerformed;
        }

        private void OnDisable()
        {
            if (toggleAction != null)
            {
                toggleAction.performed -= OnTogglePerformed;
                toggleAction.Disable();
            }
            toggleAction = null;
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("[Linterna] Falta GameConfig.", this);
                return;
            }

            if (spotLight == null)
            {
                Debug.LogError("[Linterna] Falta el Spot Light.", this);
                return;
            }

            spotLight.type = LightType.Spot;
            spotLight.spotAngle = config.FlashlightHipSpotAngle;
            spotLight.range = config.FlashlightHipRange;
            spotLight.intensity = config.FlashlightHipIntensity;
            baseIntensity = config.FlashlightHipIntensity;

            if (startsOn) SetOn(true);
            else ApplyLightState();
        }

        private void OnTogglePerformed(InputAction.CallbackContext context)
        {
            Toggle();
        }

        public void Toggle()
        {
            SetOn(!isOn);
        }

        /// <summary>Ajusta la carga en segundos (recargas, baterías, pruebas).</summary>
        public void SetCharge(float seconds)
        {
            float max = config != null ? config.FlashlightDuration : 60f;
            charge = Mathf.Clamp(seconds, 0f, max);

            if (charge <= 0f)
            {
                if (isOn) Deplete();
                else depleted = true;
                return;
            }

            if (depleted && Charge01 >= (config != null ? config.FlashlightMinTurnOn : 0.25f)) depleted = false;
        }

        /// <summary>Enciende o apaga. Encender falla si está agotada o sin carga.</summary>
        public bool SetOn(bool value)
        {
            if (!value)
            {
                if (isOn)
                {
                    isOn = false;
                    timeSinceOff = 0f;
                    ApplyLightState();
                    if (logEvents) Debug.Log($"[Linterna] Apagada (carga {Charge01 * 100f:F0} %).");
                }
                return true;
            }

            if (isOn) return true;

            if (depleted || charge <= 0f)
            {
                if (logEvents) Debug.Log($"[Linterna] Sin carga suficiente ({Charge01 * 100f:F0} %).");
                return false;
            }

            isOn = true;
            timeSinceOff = 0f;
            ApplyLightState();
            if (logEvents) Debug.Log($"[Linterna] Encendida (carga {Charge01 * 100f:F0} %).");
            return true;
        }

        private void Update()
        {
            if (config == null || spotLight == null) return;

            float deltaTime = Time.deltaTime;

            UpdateCharge(deltaTime);
            UpdateCone(deltaTime);
            UpdateFacing();
            ApplyLightState();
            DetectTargets();
        }

        private void UpdateCharge(float deltaTime)
        {
            if (isOn)
            {
                timeSinceOff = 0f;
                charge = Mathf.Max(0f, charge - deltaTime);

                if (charge <= 0f) Deplete();
                return;
            }

            timeSinceOff += deltaTime;
            if (timeSinceOff < config.FlashlightRechargeDelay) return;

            float rate = config.FlashlightDuration / Mathf.Max(0.01f, config.FlashlightRechargeDuration);
            charge = Mathf.Min(config.FlashlightDuration, charge + rate * deltaTime);

            if (depleted && Charge01 >= config.FlashlightMinTurnOn)
            {
                depleted = false;
                if (logEvents) Debug.Log($"[Linterna] Carga mínima recuperada ({Charge01 * 100f:F0} %).");
            }
        }

        private void Deplete()
        {
            isOn = false;
            depleted = true;
            timeSinceOff = 0f;
            ApplyLightState();
            if (logEvents) Debug.Log("[Linterna] Sin carga: parpadeo y apagado hasta recuperar el mínimo.");
        }

        private void UpdateCone(float deltaTime)
        {
            bool aiming = aim != null && aim.IsAiming;

            float targetAngle = aiming ? config.FlashlightAimSpotAngle : config.FlashlightHipSpotAngle;
            float targetRange = aiming ? config.FlashlightAimRange : config.FlashlightHipRange;
            float targetIntensity = aiming ? config.FlashlightAimIntensity : config.FlashlightHipIntensity;

            float blend = 1f - Mathf.Exp(-config.FlashlightConeSharpness * deltaTime);
            spotLight.spotAngle = Mathf.Lerp(spotLight.spotAngle, targetAngle, blend);
            spotLight.range = Mathf.Lerp(spotLight.range, targetRange, blend);
            baseIntensity = Mathf.Lerp(baseIntensity, targetIntensity, blend);
        }

        private void UpdateFacing()
        {
            if (aimSource == null || spotLight == null) return;

            Vector3 forward = aimSource.forward;
            Vector3 anchor = beamAnchor != null ? beamAnchor.position : transform.position;

            spotLight.transform.position = anchor + forward * config.FlashlightMuzzleOffset;
            spotLight.transform.rotation = Quaternion.LookRotation(forward, aimSource.up);
        }

        private void ApplyLightState()
        {
            if (spotLight == null) return;

            bool flickering = isOn && config != null && Charge01 < config.FlashlightFlickerThreshold;
            float flicker = flickering ? (UnityEngine.Random.value < 0.5f ? 0.15f : 1f) : 1f;

            spotLight.enabled = isOn;
            spotLight.intensity = isOn ? baseIntensity * flicker : 0f;
        }

        private void DetectTargets()
        {
            if (Time.time < nextScanTime) return;
            nextScanTime = Time.time + config.FlashlightScanInterval;

            if (!isOn)
            {
                ClearLitTargets();
                return;
            }

            Vector3 origin = spotLight.transform.position;
            Vector3 forward = spotLight.transform.forward;
            float range = spotLight.range;
            float halfAngleCos = Mathf.Cos(spotLight.spotAngle * 0.5f * Mathf.Deg2Rad);

            int count = Physics.OverlapSphereNonAlloc(
                origin, range, overlapBuffer, ~0, QueryTriggerInteraction.Collide);

            for (int i = 0; i < count; i++)
            {
                Collider collider = overlapBuffer[i];
                if (collider == null) continue;
                if (collider.transform.root == transform.root) continue;

                IlluminatedTarget target = collider.GetComponentInParent<IlluminatedTarget>();
                if (target == null || !target.IsEnemy) continue;
                if (litTargets.Contains(target)) continue;

                Vector3 point = collider.bounds.center;
                Vector3 toTarget = point - origin;
                if (toTarget.sqrMagnitude > range * range) continue;
                if (Vector3.Dot(forward, toTarget.normalized) < halfAngleCos) continue;
                if (!HasLineOfSight(origin, point, target)) continue;

                litTargets.Add(target);
                if (logEvents) Debug.Log($"[Linterna] Enemigo iluminado: {target.name} a {toTarget.magnitude:F1} m.");
                EnemyIlluminated?.Invoke(target);
            }

            // Salidas: el objetivo ya no está en el cono o ya no hay luz.
            toRemove.Clear();
            foreach (IlluminatedTarget target in litTargets)
            {
                if (target == null || !IsStillLit(origin, forward, range, halfAngleCos, target)) toRemove.Add(target);
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                litTargets.Remove(toRemove[i]);
            }
        }

        private bool IsStillLit(Vector3 origin, Vector3 forward, float range, float halfAngleCos, IlluminatedTarget target)
        {
            Vector3 point = target.transform.position;
            Vector3 toTarget = point - origin;
            if (toTarget.sqrMagnitude > range * range) return false;
            if (Vector3.Dot(forward, toTarget.normalized) < halfAngleCos) return false;
            return HasLineOfSight(origin, point, target);
        }

        private bool HasLineOfSight(Vector3 origin, Vector3 point, IlluminatedTarget target)
        {
            if (!Physics.Linecast(origin, point, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore)) return true;
            if (hit.collider == null) return true;
            return hit.collider.transform.root == target.transform.root;
        }

        private void ClearLitTargets()
        {
            if (litTargets.Count == 0) return;
            litTargets.Clear();
        }
    }
}
