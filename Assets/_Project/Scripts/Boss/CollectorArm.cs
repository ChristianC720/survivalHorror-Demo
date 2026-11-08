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
    /// <summary>
    /// The collector arm on its crane (D-65, D-98): runs along the rails at ~2.5 m/s (Lukas running outpaces it),
    /// warns ~1 s with light and hydraulic sound, then grabs within ~3 m with the M-11 rules (spike stuns it 2 s).
    /// Phase 1 it patrols the nodes and only chases Lukas when he is close.
    /// </summary>
    public sealed class CollectorArm : MonoBehaviour, IGrabber
    {
        [SerializeField] private BossBalance balance;
        [SerializeField] private Light warnLight;
        [Tooltip("Rail rectangle in room-local XZ (the crane cannot leave it).")]
        [SerializeField] private Vector2 railMin = new Vector2(2f, 2f), railMax = new Vector2(18f, 18f);
        [SerializeField] private int hitsToKill = 2;

        private const float GrabTolerance = 1.15f;
        private bool active;
        private float windup = -1f, cooldown, stun;
        private int patrolIndex;

        private LineRenderer outerRing, closingRing;
        private const int RingPoints = 48;
        private const float RingLift = 0.05f, RingWidth = 0.07f;

        private LineRenderer MakeRing(string ringName)
        {
            var ring = new GameObject(ringName).AddComponent<LineRenderer>();
            ring.transform.SetParent(transform, false);
            var material = new Material(Resources.Load<Shader>("HB_InteractOutline"));
            material.SetFloat("_Width", 0f);
            material.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            material.SetFloat("_StencilComp", (float)UnityEngine.Rendering.CompareFunction.Always);   // no rim logic on a flat marker
            material.SetColor("_Color", UI.UiKit.Red);
            ring.sharedMaterial = material;
            ring.loop = true;
            ring.useWorldSpace = true;
            ring.positionCount = RingPoints;
            ring.widthMultiplier = RingWidth;
            ring.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            ring.enabled = false;
            return ring;
        }

        private void DrawRing(LineRenderer ring, float radius, float floorY)
        {
            ring.enabled = true;
            for (int i = 0; i < RingPoints; i++)
            {
                float a = i * Mathf.PI * 2f / RingPoints;
                ring.SetPosition(i, new Vector3(transform.position.x + Mathf.Cos(a) * radius, floorY + RingLift, transform.position.z + Mathf.Sin(a) * radius));
            }
        }

        private void HideRings()
        {
            if (outerRing != null) outerRing.enabled = false;
            if (closingRing != null) closingRing.enabled = false;
        }

        public void SetActive(bool on)
        {
            active = on;
            windup = -1f;
            HideRings();
            if (warnLight != null) warnLight.enabled = false;
        }

        public void OnGrabCountered(float stunSeconds)
        {
            stun = balance.armStunSeconds;
            HbLog.Info("Boss", $"Collector arm stunned {stun:0.#} s (discharge spike)");
        }

        private void Update()
        {
            if (!active || !Services.TryGet(out PlayerController p) || !Services.TryGet(out MotherDirector director)) return;
            float dt = Time.deltaTime;
            cooldown -= dt;
            if (stun > 0f) { stun -= dt; windup = -1f; HideRings(); return; }

            Vector3 player = p.transform.position;
            float dist = Flat(player - transform.position).magnitude;

            if (windup >= 0f)
            {
                windup -= dt;
                if (windup > 0f)
                {
                    // The closing ring reaches the edge when the grab lands: step outside the outer ring to dodge it.
                    float reach = balance.armReach * GrabTolerance;
                    DrawRing(outerRing, reach, player.y);
                    DrawRing(closingRing, reach * (1f - windup / balance.armWarning), player.y);
                    return;
                }
                windup = -1f;
                HideRings();
                cooldown = balance.armCooldown;
                if (warnLight != null) warnLight.enabled = false;
                if (dist <= balance.armReach * GrabTolerance)
                {
                    HbLog.Info("Boss", "Collector arm grabs Lukas");
                    GameEvents.RaisePlayerAttacked(new AttackInfo(hitsToKill, true, transform.position, this, neverKills: true));   // D-163
                }
                else HbLog.Info("Boss", "Collector arm missed");
                return;
            }

            if (dist <= balance.armReach && cooldown <= 0f)
            {
                windup = balance.armWarning;
                if (outerRing == null) { outerRing = MakeRing("WarnRing"); closingRing = MakeRing("ClosingRing"); }
                if (warnLight != null) warnLight.enabled = true;
                Sfx.Play("CUE_Arm_Hydraulic", transform.position);
                HbLog.Info("Boss", "Collector arm winds up");
                return;
            }

            // Phase 1: patrol the unhacked nodes unless Lukas is close; later phases: chase him.
            Vector3 target = player;
            if (director.Fight.Phase == MotherPhase.Nodes && dist > balance.armChaseRadius)
            {
                var nodes = director.GetComponentsInChildren<BossNode>();
                if (nodes.Length > 0)
                {
                    var n = nodes[patrolIndex % nodes.Length];
                    if (Flat(n.transform.position - transform.position).magnitude < 0.5f) patrolIndex++;
                    target = n.transform.position;
                }
            }
            Vector3 next = Vector3.MoveTowards(transform.position, new Vector3(target.x, transform.position.y, target.z), balance.armSpeed * dt);
            Vector3 local = director.transform.InverseTransformPoint(next);
            local.x = Mathf.Clamp(local.x, railMin.x, railMax.x);
            local.z = Mathf.Clamp(local.z, railMin.y, railMax.y);
            transform.position = director.transform.TransformPoint(local);
        }

        private static Vector3 Flat(Vector3 v) { v.y = 0f; return v; }
    }
}
