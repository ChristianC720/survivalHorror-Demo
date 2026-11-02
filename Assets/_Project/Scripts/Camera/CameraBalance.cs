using UnityEngine;

namespace Heartbeat.CameraSystem
{
    /// <summary>OTS camera and aim values (M-01). Initial values, subject to balance.</summary>
    [CreateAssetMenu(menuName = "Heartbeat/Balance/Camera", fileName = "CameraBalance")]
    public sealed class CameraBalance : ScriptableObject
    {
        [Header("Look")]
        [Tooltip("Mouse look, degrees per pixel of delta.")] public float mouseSensitivity = 0.12f;
        [Tooltip("Aiming mouse look, degrees per pixel (separate setting, D-104).")] public float aimMouseSensitivity = 0.08f;
        [Tooltip("Gamepad look, degrees per second at full stick.")] public float stickDegreesPerSecond = 160f;
        public bool invertY;
        public float minPitch = -55f;
        public float maxPitch = 65f;

        [Header("Aim assist (option, gamepad only, D-104)")]
        [Tooltip("Half-angle of the cone around the reticle that attracts it, degrees.")] public float aimAssistCone = 6f;
        [Tooltip("How fast the reticle is pulled toward the weak point, degrees per second.")] public float aimAssistPull = 30f;
        public float aimAssistRange = 20f;

        [Header("Exploration camera")]
        public Vector3 exploreShoulder = new Vector3(0.45f, 0.05f, 0f);
        public float exploreDistance = 2.4f;
        public float exploreFov = 55f;

        [Header("Aim camera")]
        public Vector3 aimShoulder = new Vector3(0.55f, 0.08f, 0f);
        public float aimDistance = 1.1f;
        public float aimFov = 38f;
        [Tooltip("Seconds to blend between explore and aim.")] public float aimBlendSeconds = 0.18f;

        [Header("Target")]
        [Tooltip("Height of the camera target above Lukas's feet.")] public float targetHeight = 1.55f;
        [Tooltip("Crouched target height.")] public float crouchTargetHeight = 1.05f;
    }
}
