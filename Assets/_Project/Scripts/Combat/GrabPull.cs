using Heartbeat.AI;
using Heartbeat.Core;
using Heartbeat.Player;
using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>
    /// C-64: while an enemy's grab windup closes its hands, Lukas slides to the enemy's anchor distance and turns to face it,
    /// so the hands land on his shoulders instead of stretching to where he stood. He stays held until the hands let go,
    /// the enemy is staggered (spike, weak point) or dies, or Lukas dies. The damage itself stays in PlayerDamageReceiver.
    /// </summary>
    public sealed class GrabPull : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private PlayerHealth health;

        private EnemyBrain grabber;
        private float startDistance;

        public bool IsHeld => grabber != null;

        private void OnEnable() => GameEvents.GrabStarted += OnGrabStarted;

        private void OnDisable()
        {
            GameEvents.GrabStarted -= OnGrabStarted;
            Release("disabled");
        }

        private void OnGrabStarted(MonoBehaviour source)
        {
            if (grabber != null || health.Model.IsDead || !(source is EnemyBrain brain)) return;   // one grab at a time
            grabber = brain;
            startDistance = Flat(transform.position - brain.transform.position).magnitude;
            player.Grabbed = true;
            player.StandUp();
            HbLog.Info("Combat", $"Grabbed by {brain.name}: pulled from {startDistance:0.00} m to {brain.Definition.grabAnchorDistance:0.00} m");
        }

        private void Update()
        {
            if (grabber == null) return;
            if (!grabber.isActiveAndEnabled || !grabber.IsGrabbing || health.Model.IsDead) { Release("let go"); return; }

            Vector3 from = grabber.transform.position;
            Vector3 dir = Flat(transform.position - from);
            dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : grabber.transform.forward;
            float d = Distance(startDistance, grabber.Definition.grabAnchorDistance, grabber.GrabProgress);
            player.Pull(Flat(from + dir * d - transform.position));
            if (!player.IsAiming) transform.rotation = Quaternion.LookRotation(-dir);
        }

        private void Release(string why)
        {
            if (grabber == null) return;
            HbLog.Info("Combat", $"Grab released ({why})");
            grabber = null;
            player.Grabbed = false;
        }

        /// <summary>Lukas's distance to the grabber: eases from where the grab started to the anchor as the hands close.</summary>
        public static float Distance(float start, float anchor, float progress) => Mathf.Lerp(start, anchor, Mathf.SmoothStep(0f, 1f, progress));

        private static Vector3 Flat(Vector3 v) { v.y = 0f; return v; }
    }
}
