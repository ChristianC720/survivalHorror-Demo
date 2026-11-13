using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>
    /// Hitbox on an enemy's weak point (D-74). A hit interrupts the current attack or grab and deals
    /// CombatBalance.weakPointDamageMultiplier times a normal hit (C-74). Forwards the shot to the IDamageable in its parents with WeakPoint = true.
    /// </summary>
    public sealed class WeakPoint : MonoBehaviour
    {
        public IDamageable Owner => GetComponentInParent<IDamageable>();

        /// <summary>
        /// The weak point a shot landed on: the hit collider's own, or one of the same enemy whose volume holds the hit
        /// point. Body hitboxes overlap the weak zone at the seams (an arm against the torso), and there the weak point wins.
        /// </summary>
        public static WeakPoint At(Collider hit, Vector3 point)
        {
            if (hit.TryGetComponent(out WeakPoint own)) return own;
            if (hit.GetComponentInParent<IDamageable>() is not Component owner) return null;
            foreach (var weak in owner.GetComponentsInChildren<WeakPoint>())
                if (weak.TryGetComponent(out Collider c) && c.enabled && (c.ClosestPoint(point) - point).sqrMagnitude < 1e-6f)
                    return weak;
            return null;
        }
    }
}
