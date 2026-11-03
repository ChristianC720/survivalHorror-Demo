using Heartbeat.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Heartbeat.AI
{
    /// <summary>
    /// Plays an enemy's clips from what its brain is doing (D-151). Listens only; the AI never depends on it. Adds the
    /// broken rhythm of the assimilated with bone noise (GDD Personajes y Animacion), stronger once recycled.
    /// </summary>
    [RequireComponent(typeof(EnemyBrain))]
    public sealed class EnemyAnimator : MonoBehaviour
    {
        private const float Fade = 0.15f;
        private const float GrabHoldSeconds = 0.9f;
        private const float ScreamSeconds = 2f;

        [SerializeField] private Animator animator;
        [Tooltip("Weak points follow the head bone.")]
        [SerializeField] private Transform[] headAttachments;
        [SerializeField] private float noiseDegrees = 3f;
        [SerializeField] private float recycledNoiseDegrees = 9f;

        private static readonly int RateId = Animator.StringToHash("Rate");

        private EnemyBrain brain;
        private EnemyHealth health;
        private NavMeshAgent agent;
        private Transform spine, head;
        private string current;
        private float attackLength = 1f, overrideTimer;
        private string overrideState;
        private float seed;
        private int shownBody = -1;   // -1 until the first frame picks a mesh

        private void Awake()
        {
            brain = GetComponent<EnemyBrain>();
            health = GetComponent<EnemyHealth>();
            agent = GetComponent<NavMeshAgent>();
            seed = Random.value * 100f;
            foreach (var t in animator.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "Chest") spine = t;
                else if (t.name == "Head") head = t;
            }
            if (head != null && headAttachments != null)
                foreach (var a in headAttachments)
                {
                    if (a == null) continue;
                    a.SetParent(head, false);
                    a.localPosition = new Vector3(0f, 0.12f, 0f);
                }
            foreach (var c in animator.runtimeAnimatorController.animationClips)
                if (c.name.EndsWith("_Attack")) attackLength = c.length;
            brain.Screamed += OnScreamed;
        }

        private void OnEnable() => GameEvents.PlayerAttacked += OnPlayerAttacked;
        private void OnDisable() => GameEvents.PlayerAttacked -= OnPlayerAttacked;
        private void OnDestroy() { if (brain != null) brain.Screamed -= OnScreamed; }

        private void OnScreamed() { overrideState = "Scream"; overrideTimer = ScreamSeconds; }

        private void OnPlayerAttacked(AttackInfo attack)
        {
            if (attack.IsGrab && ReferenceEquals(attack.Source, brain)) { overrideState = "Grab"; overrideTimer = GrabHoldSeconds; }
        }

        private void Update()
        {
            ShowBody(health.IsRecycled);
            overrideTimer -= Time.deltaTime;
            string state = Wanted(out float rate);
            animator.SetFloat(RateId, rate);
            if (state == current) return;
            animator.CrossFadeInFixedTime(state, Fade, 0);
            current = state;
        }

        private void ShowBody(bool recycled)
        {
            int body = recycled ? 1 : 0;
            if (body == shownBody) return;
            shownBody = body;
            foreach (var r in animator.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                r.enabled = r.name.EndsWith("_Recycled") == recycled;
        }

        private string Wanted(out float rate)
        {
            rate = 1f;
            if (brain.IsDead) return "Death";
            if (brain.IsStaggered) return "Stunned";
            if (overrideTimer > 0f) return overrideState;
            if (brain.IsWindingUp)
            {
                // The swing lands when the windup ends: the blow sits about 40% into the clip.
                rate = attackLength * 0.4f / Mathf.Max(brain.Definition.attackWindup, 0.05f);
                return "Attack";
            }
            if (brain.IsDormant) return "Idle";
            float speed = agent != null && agent.enabled ? agent.velocity.magnitude : 0f;
            if (speed < 0.1f) return "Idle";
            bool hunting = brain.Alert == AlertState.Hunt;
            rate = Mathf.Clamp(speed / Mathf.Max(hunting ? brain.Definition.huntSpeed : brain.Definition.patrolSpeed, 0.1f), 0.4f, 2f);
            return hunting ? "Hunt" : "Move";
        }

        private void LateUpdate()
        {
            if (brain.IsDead || brain.IsDormant) return;
            float amp = health.IsRecycled ? recycledNoiseDegrees : noiseDegrees;
            float t = Time.time * 7f + seed;
            if (spine != null) spine.Rotate(Noise(t) * amp * 0.5f, Space.Self);
            if (head != null) head.Rotate(Noise(t * 1.7f + 40f) * amp, Space.Self);
        }

        private static Vector3 Noise(float t) => new Vector3(Mathf.PerlinNoise(t, 0f) - 0.5f, Mathf.PerlinNoise(0f, t) - 0.5f, Mathf.PerlinNoise(t, t) - 0.5f) * 2f;
    }
}
