using System.Collections.Generic;
using Heartbeat.AI;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Recycle
{
    /// <summary>
    /// The body of a dead enemy (S-09, M-20). Every unfinished body can be raised once by the lab; finishing it
    /// (ground strike or discharge spike) burns its last trace of identity. Recycled enemies and boss-room bodies
    /// cannot be recycled, so the finisher prompt never targets them. The timer/rise lives in RecycleSystem (E-08).
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class Corpse : MonoBehaviour
    {
        [Tooltip("Bodies in the boss room are never recycled (M-20).")]
        [SerializeField] private bool neverRecycle;

        private static readonly List<Corpse> all = new();

        /// <summary>Every enabled enemy body holder, alive or dead. The finisher filters with <see cref="CanBeRecycled"/>.</summary>
        public static IReadOnlyList<Corpse> All => all;

        private EnemyHealth health;

        private void OnEnable() => all.Add(this);
        private void OnDisable() => all.Remove(this);

        public bool IsDown => health != null && !health.IsAlive;
        public bool IsFinished { get; private set; }
        public float DiedAt { get; private set; } = -1f;

        /// <summary>True when the lab could still raise this body: down, not finished, not already recycled.</summary>
        public bool CanBeRecycled => IsDown && !IsFinished && !neverRecycle && !health.IsRecycled;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
            health.Died += () => DiedAt = Time.time;
        }

        public void Finish(string how)
        {
            if (!CanBeRecycled) return;
            IsFinished = true;
            HbLog.Info("Recycle", $"{name} finished by {how}; it will not be recycled");
        }

        /// <summary>
        /// C-76: the part of the lying body Lukas has most in front among those within <paramref name="range"/> of
        /// <paramref name="from"/>: head, torso, feet or a limb, not only the root where the enemy died. Parts are the
        /// vertices of the skinned mesh in its current pose. The shot hitboxes do not serve: they are disabled on death
        /// and wrap every clip but death, up to 0.2 m off the lying body. Without a skinned mesh the root colliders stand
        /// in. <paramref name="facing"/> is the dot of <paramref name="forward"/> with the flat direction to the part.
        /// </summary>
        public bool TryFacingPoint(Vector3 from, Vector3 forward, float range, out Vector3 point, out float facing)
        {
            point = from;
            facing = float.NegativeInfinity;
            skins ??= GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skins.Length == 0)
            {
                if (!RootClosestPoint(from, range, out point)) return false;
                facing = Facing(from, forward, point);
                return true;
            }
            if (points == null) Bake();
            // the pose may still be settling: 0.5 m of slack before skipping a body by its last reach
            if ((from - transform.position).sqrMagnitude > Sqr(range + reach + 0.5f)) return false;
            if (Time.time - bakedAt >= RebakeSeconds) Bake();

            float rangeSqr = range * range;
            Matrix4x4 toWorld = transform.localToWorldMatrix;
            foreach (var v in points)
            {
                Vector3 p = toWorld.MultiplyPoint3x4(v);
                if ((p - from).sqrMagnitude > rangeSqr) continue;
                float f = Facing(from, forward, p);
                if (f > facing) { facing = f; point = p; }
            }
            return facing > float.NegativeInfinity;
        }

        /// <summary>Dot of the flat forward with the flat direction to <paramref name="p"/>; 1 when Lukas stands on it.</summary>
        public static float Facing(Vector3 from, Vector3 forward, Vector3 p)
        {
            Vector3 to = p - from; to.y = 0f;
            forward.y = 0f;
            return to.sqrMagnitude < 0.01f ? 1f : Vector3.Dot(forward.normalized, to.normalized);
        }

        private const float RebakeSeconds = 0.25f;   // the death clip moves the body for about 2 s, then it lies still
        private SkinnedMeshRenderer[] skins;
        private Mesh baked;
        private readonly List<Vector3> bakedVertices = new();
        private List<Vector3> points;   // root-local, so the convulsion before rising moves them too
        private float bakedAt;
        private float reach;

        private void Bake()
        {
            baked ??= new Mesh { name = "CorpseBake" };
            points ??= new List<Vector3>();
            points.Clear();
            reach = 0f;
            foreach (var skin in skins)
            {
                if (skin == null) continue;
                skin.BakeMesh(baked, true);
                baked.GetVertices(bakedVertices);
                Vector3 s = skin.transform.lossyScale;
                Matrix4x4 toRoot = transform.worldToLocalMatrix * skin.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1f / s.x, 1f / s.y, 1f / s.z));
                foreach (var v in bakedVertices)
                {
                    Vector3 q = toRoot.MultiplyPoint3x4(v);
                    points.Add(q);
                    reach = Mathf.Max(reach, transform.TransformVector(q).magnitude);
                }
            }
            bakedAt = Time.time;
        }

        private bool RootClosestPoint(Vector3 p, float range, out Vector3 point)
        {
            point = p;
            float bestSqr = float.PositiveInfinity;
            foreach (var c in GetComponents<Collider>())
            {
                Vector3 q = c.ClosestPoint(p);
                float sqr = (q - p).sqrMagnitude;
                if (sqr < bestSqr) { bestSqr = sqr; point = q; }
            }
            return bestSqr <= range * range;
        }

        private static float Sqr(float x) => x * x;

        private void OnDestroy()
        {
            if (baked != null) Destroy(baked);
        }

        /// <summary>Debug / tests: pretend the body died <paramref name="seconds"/> ago.</summary>
        public void DebugAge(float seconds) => DiedAt = Time.time - seconds;

        /// <summary>Called when the lab raises the body (E-08) so it becomes a living, recycled enemy again.</summary>
        public void OnRaised()
        {
            SetConvulsing(false);
            DiedAt = -1f;
        }

        // ---------- Convulsion before rising (M-20 warning) ----------

        private Quaternion restRotation;
        private bool convulsing;

        public void SetConvulsing(bool on)
        {
            if (on == convulsing) return;
            if (on) restRotation = transform.rotation;
            else transform.rotation = restRotation;
            convulsing = on;
        }

        private void LateUpdate()
        {
            if (convulsing) transform.rotation = restRotation * Quaternion.Euler(Random.Range(-4f, 4f), Random.Range(-6f, 6f), Random.Range(-4f, 4f));
        }
    }
}
