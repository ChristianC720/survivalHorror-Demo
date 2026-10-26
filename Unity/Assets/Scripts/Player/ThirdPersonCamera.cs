using Heartbeat.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>
    /// Cámara en tercera persona sobre el hombro (OTS) con FOV reducido.
    /// El ratón mira, el pivote se desplaza al hombro y una esfera de colisión
    /// acorta la distancia cuando hay una pared detrás. Al apuntar se acerca al
    /// hombro y estrecha el FOV; ambas transiciones se interpolan con rapidez.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private Transform target;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private PlayerAim aim;
        [SerializeField] private InputActionAsset inputActions;

        [Header("Cursor")]
        [SerializeField] private bool lockCursorOnStart = true;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private const string ActionMapName = "Player";

        private InputAction lookAction;
        private readonly RaycastHit[] hitBuffer = new RaycastHit[8];

        private float yaw;
        private float pitch;
        private float currentDistance;
        private float distanceVelocity;
        private float currentFov;
        private Vector3 smoothedPivot;
        private Vector3 pivotVelocity;
        private bool ready;
        private bool wasObstructed;

        private float shakeMagnitude;
        private float shakeAngle;
        private float shakeDuration;
        private float shakeTimeLeft;

        public float CurrentDistance => currentDistance;
        public float CurrentFov => currentFov;
        public float Yaw => yaw;
        public float Pitch => pitch;
        public bool IsAiming => aim != null && aim.IsAiming;

        /// <summary>La pared ha acortado la distancia respecto a la deseada.</summary>
        public bool IsObstructed { get; private set; }

        /// <summary>Hay una sacudida activa (daño recibido).</summary>
        public bool IsShaking => shakeTimeLeft > 0f;

        /// <summary>Falso mientras una UI tiene el cursor (inventario, hackeo): no se reengancha con clic izquierdo.</summary>
        public bool CursorCaptureEnabled { get; set; } = true;

        private void Awake()
        {
            if (viewCamera == null) viewCamera = GetComponent<Camera>();
            if (aim == null) aim = GetComponentInParent<PlayerAim>();

            if (target == null && transform.parent != null)
            {
                target = transform.parent.Find("CameraTarget");
            }
        }

        private void OnEnable()
        {
            if (inputActions == null) return;

            InputActionMap map = inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
            if (map == null) return;

            lookAction = map.FindAction("Look", throwIfNotFound: false);
            lookAction?.Enable();
        }

        private void OnDisable()
        {
            lookAction?.Disable();
            lookAction = null;
        }

        private void Start()
        {
            if (config == null || target == null || viewCamera == null)
            {
                Debug.LogError("[Camara] Faltan referencias (GameConfig, CameraTarget o Camera).", this);
                return;
            }

            yaw = target.eulerAngles.y;
            pitch = config.CameraStartPitch;
            currentDistance = config.CameraDistance;
            currentFov = config.CameraFov;
            smoothedPivot = target.position;

            viewCamera.fieldOfView = currentFov;

            ApplyTransform();
            if (lockCursorOnStart) SetCursorLocked(true);
            ready = true;
        }

        private void Update()
        {
            if (!lockCursorOnStart || !CursorCaptureEnabled) return;

            if (Cursor.lockState != CursorLockMode.Locked)
            {
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) SetCursorLocked(true);
                return;
            }

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) SetCursorLocked(false);
        }

        private void LateUpdate()
        {
            if (!ready) return;

            float deltaTime = Time.deltaTime;

            ReadLook();

            bool aiming = aim != null && aim.IsAiming;
            float desiredDistance = aiming ? config.CameraAimDistance : config.CameraDistance;
            float shoulder = aiming ? config.CameraAimShoulder : config.CameraShoulder;

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 forward = rotation * Vector3.forward;
            Vector3 right = rotation * Vector3.right;

            Vector3 pivot = target.position + right * shoulder;
            Vector3 camPivot = pivot;
            if (config.CameraFollowTime > 0f)
            {
                smoothedPivot = Vector3.SmoothDamp(smoothedPivot, pivot, ref pivotVelocity, config.CameraFollowTime);
                camPivot = smoothedPivot;
            }

            float allowed = ComputeAllowedDistance(camPivot, -forward, desiredDistance);
            IsObstructed = allowed < desiredDistance - 0.01f;

            float smoothTime = allowed < currentDistance ? config.CameraPullInTime : config.CameraPullOutTime;
            currentDistance = Mathf.SmoothDamp(currentDistance, allowed, ref distanceVelocity, smoothTime);

            float targetFov = aiming ? config.CameraAimFov : config.CameraFov;
            float fovBlend = 1f - Mathf.Exp(-config.CameraFovSharpness * deltaTime);
            currentFov = Mathf.Lerp(currentFov, targetFov, fovBlend);

            transform.SetPositionAndRotation(camPivot - forward * currentDistance, rotation);
            viewCamera.fieldOfView = currentFov;
            ApplyShake(deltaTime);

            if (logEvents && IsObstructed != wasObstructed)
            {
                wasObstructed = IsObstructed;
                Debug.Log(IsObstructed
                    ? $"[Camara] Pared: distancia acortada a {currentDistance:F2} m."
                    : "[Camara] Sin obstáculo: distancia recuperada.");
            }
        }

        private void ReadLook()
        {
            if (lookAction == null || config == null) return;
            if (Cursor.lockState != CursorLockMode.Locked) return;

            Vector2 look = lookAction.ReadValue<Vector2>();
            if (look.sqrMagnitude <= 0f) return;

            yaw += look.x * config.LookSensitivity;
            pitch -= look.y * config.LookSensitivity;
            pitch = Mathf.Clamp(pitch, config.PitchMin, config.PitchMax);
        }

        private float ComputeAllowedDistance(Vector3 pivot, Vector3 direction, float desiredDistance)
        {
            int count = Physics.SphereCastNonAlloc(
                pivot, config.CameraCollisionRadius, direction, hitBuffer, desiredDistance,
                ~0, QueryTriggerInteraction.Ignore);

            float allowed = desiredDistance;
            for (int i = 0; i < count; i++)
            {
                Collider collider = hitBuffer[i].collider;
                if (collider == null) continue;
                if (target != null && collider.transform.root == target.root) continue;
                if (hitBuffer[i].distance <= 0f) continue;

                allowed = Mathf.Min(allowed, hitBuffer[i].distance - config.CameraCollisionPadding);
            }

            return Mathf.Clamp(allowed, config.CameraMinDistance, desiredDistance);
        }

        private void ApplyTransform()
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.SetPositionAndRotation(target.position - rotation * Vector3.forward * currentDistance, rotation);
            viewCamera.fieldOfView = currentFov;
        }

        /// <summary>Sacude la cámara: desplazamiento en metros, giro en grados y duración en segundos.</summary>
        public void Shake(float magnitude, float angle, float duration)
        {
            if (duration <= 0f) return;
            if (magnitude <= 0f && angle <= 0f) return;

            shakeMagnitude = Mathf.Max(shakeMagnitude, magnitude);
            shakeAngle = Mathf.Max(shakeAngle, angle);
            shakeDuration = Mathf.Max(duration, 0.01f);
            shakeTimeLeft = shakeDuration;
        }

        private void ApplyShake(float deltaTime)
        {
            if (shakeTimeLeft <= 0f) return;

            shakeTimeLeft = Mathf.Max(0f, shakeTimeLeft - deltaTime);

            float falloff = shakeDuration > 0f ? shakeTimeLeft / shakeDuration : 0f;
            falloff *= falloff;

            float t = Time.time * 26f;
            float noiseX = (Mathf.PerlinNoise(t, 0.37f) - 0.5f) * 2f;
            float noiseY = (Mathf.PerlinNoise(0.71f, t) - 0.5f) * 2f;
            float noiseZ = (Mathf.PerlinNoise(t * 0.83f, 5.19f) - 0.5f) * 2f;

            transform.position += (transform.right * noiseX + transform.up * noiseY) * (shakeMagnitude * falloff);
            transform.rotation *= Quaternion.Euler(
                noiseY * shakeAngle * falloff,
                noiseX * shakeAngle * falloff,
                noiseZ * shakeAngle * falloff * 0.5f);

            if (shakeTimeLeft > 0f) return;

            shakeMagnitude = 0f;
            shakeAngle = 0f;
        }

        public void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
