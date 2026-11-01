using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>
    /// Hitbox on an enemy's weak point (D-74). A hit interrupts the current attack or grab but does not reduce
    /// the shots needed. Forwards the shot to the IDamageable in its parents with WeakPoint = true.
    /// </summary>
    public sealed class WeakPoint : MonoBehaviour
    {
        public IDamageable Owner => GetComponentInParent<IDamageable>();
    }
}
