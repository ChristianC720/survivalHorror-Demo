using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Audio
{
    /// <summary>
    /// Footstep cues by surface (M-10) for Lukas and enemies. Measures the horizontal distance its transform travels
    /// (no coupling to the movement code) and requests CUE_Step_Metal / _Water / _Debris every stride. The surface is
    /// read from the PhysicsMaterial name under the feet ("Water", "Debris"; anything else is metal).
    /// Put it on the character's parent prefab, never on the PH mesh.
    /// </summary>
    public sealed class FootstepEmitter : MonoBehaviour
    {
        [SerializeField] private AudioBalance balance;

        private Vector3 last;
        private float travelled;

        private void OnEnable() => last = transform.position;

        private void Update()
        {
            if (balance == null || Time.deltaTime <= 0f) return;
            Vector3 p = transform.position;
            Vector3 delta = p - last;
            delta.y = 0f;
            last = p;
            float speed = delta.magnitude / Time.deltaTime;
            if (speed < balance.minStepSpeed) { travelled = 0f; return; }
            travelled += delta.magnitude;
            if (travelled < balance.strideLength) return;
            travelled = 0f;
            Sfx.Play(StepCue(p), p);
        }

        private static string StepCue(Vector3 p)
        {
            if (!Physics.Raycast(p + Vector3.up * 0.3f, Vector3.down, out var hit, 1f, ~0, QueryTriggerInteraction.Ignore)
                || hit.collider.sharedMaterial == null) return "CUE_Step_Metal";
            string m = hit.collider.sharedMaterial.name;
            return m.Contains("Water") ? "CUE_Step_Water" : m.Contains("Debris") ? "CUE_Step_Debris" : "CUE_Step_Metal";
        }
    }
}
