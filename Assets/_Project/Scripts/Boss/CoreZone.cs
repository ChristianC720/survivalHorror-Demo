using Heartbeat.AI;
using Heartbeat.Audio;
using Heartbeat.Combat;
using Heartbeat.Core;
using Heartbeat.Interaction;
using Heartbeat.Level;
using Heartbeat.Player;
using UnityEngine;

namespace Heartbeat.Boss
{
    /// <summary>Trigger around The Mother's core (phase 2 ends when Lukas reaches it, D-115).</summary>
    public sealed class CoreZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerController>() != null && Services.TryGet(out MotherDirector d)) d.ReachedCore();
        }
    }
}
