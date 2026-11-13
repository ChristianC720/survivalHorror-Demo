using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>
    /// Where a revolver shot lands (C-70). The camera ray picks the aim point, the bullet leaves Lukas's chest. When the
    /// shot would not land on a hitbox, a light magnet bends it to the nearest live enemy hitbox within
    /// <c>bulletMagnetRadius</c> of the ray and <c>bulletMagnetMaxDegrees</c> of its direction, never through a wall.
    /// Static and free of services so EditMode tests can drive it.
    /// </summary>
    public static class BulletMagnet
    {
        private const float Inset = 0.02f;   // the sphere touches the silhouette edge: aim a little inside so the chest ray cannot graze past
        private const float ConvergeDistance = 2f;   // with no backstop the bullet joins the crosshair line this far in front of Lukas (C-75)

        /// <param name="camOrigin">Camera ray origin, already moved up to Lukas.</param>
        /// <param name="dir">Shot direction after spread.</param>
        /// <param name="hit">The collider the bullet reaches from the chest; only valid when this returns true.</param>
        /// <param name="shotDir">Direction the bullet travels from the chest.</param>
        public static bool Resolve(Vector3 camOrigin, Vector3 dir, Vector3 chest, float range, float magnetRadius,
            float magnetMaxDegrees, int shotMask, int hitboxLayer, out RaycastHit hit, out Vector3 shotDir)
        {
            shotDir = dir;
            bool aimed = Physics.Raycast(camOrigin, dir, out var aimHit, range, shotMask, QueryTriggerInteraction.Ignore);
            // No backstop: aiming the chest at a point at range crossed the crosshair line only there, 0.5 m left of it at
            // 10 m (C-75). Join the line close to Lukas; past that point the camera ray already found nothing to hit.
            Vector3 aimPoint = aimed ? aimHit.point : camOrigin + dir * Mathf.Min(ConvergeDistance, range);
            bool landed = ChestRay(chest, aimPoint, shotMask, out hit, ref shotDir);
            // Point blank (a grab, a body against Lukas): the chest starts inside the hitbox the camera sees, and a ray
            // never hits the collider it starts in. The camera's hit stands.
            if (!landed && aimed && aimHit.collider.gameObject.layer == hitboxLayer) { hit = aimHit; landed = true; }
            if (landed && hit.collider.gameObject.layer == hitboxLayer) return true;   // rule 5: a straight hit never changes

            // Search only up to the wall the camera sees: an enemy behind it is not on screen.
            float reach = aimed ? aimHit.distance + magnetRadius : range;
            if (magnetRadius > 0f && TryMagnet(camOrigin, dir, chest, reach, magnetRadius, magnetMaxDegrees, shotMask, hitboxLayer,
                    out var magnetHit, out var magnetDir))
            {
                hit = magnetHit;
                shotDir = magnetDir;
                return true;
            }
            return landed;
        }

        private static bool ChestRay(Vector3 chest, Vector3 point, int shotMask, out RaycastHit hit, ref Vector3 shotDir)
        {
            hit = default;
            Vector3 to = point - chest;
            float dist = to.magnitude;
            if (dist <= 0.01f) return false;
            shotDir = to / dist;
            return Physics.Raycast(chest, shotDir, out hit, dist + 0.05f, shotMask, QueryTriggerInteraction.Ignore);
        }

        private static bool TryMagnet(Vector3 camOrigin, Vector3 dir, Vector3 chest, float reach, float radius, float maxDegrees,
            int shotMask, int hitboxLayer, out RaycastHit hit, out Vector3 shotDir)
        {
            hit = default;
            shotDir = dir;
            float best = float.MaxValue;
            Collider target = null;
            IDamageable owner = null;
            Vector3 targetPoint = default;
            foreach (var h in Physics.SphereCastAll(camOrigin, radius, dir, reach, 1 << hitboxLayer, QueryTriggerInteraction.Ignore))
            {
                if (h.distance <= 0f) continue;   // started overlapping: point is (0,0,0)
                var damageable = h.collider.GetComponentInParent<IDamageable>();
                if (damageable == null || !damageable.IsAlive) continue;
                Vector3 v = h.point - camOrigin;
                float along = Vector3.Dot(v, dir);
                if (along <= 0f) continue;
                float perp = (v - dir * along).magnitude;
                if (perp > radius + 0.001f || Mathf.Atan2(perp, along) * Mathf.Rad2Deg > maxDegrees || perp >= best) continue;
                best = perp;
                target = h.collider;
                owner = damageable;
                targetPoint = h.point + (h.collider.bounds.center - h.point).normalized * Inset;
            }
            if (target == null) return false;
            // Rule 3: the bullet must reach that enemy from the chest, or the shot keeps its own path.
            return ChestRay(chest, targetPoint, shotMask, out hit, ref shotDir)
                && hit.collider.gameObject.layer == hitboxLayer && hit.collider.GetComponentInParent<IDamageable>() == owner;
        }
    }
}
