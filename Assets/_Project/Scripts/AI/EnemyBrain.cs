using Heartbeat.Combat;
using Heartbeat.Core;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Heartbeat.AI
{
    /// <summary>
    /// One assimilated enemy (S-02). Perceives by sight (cone + line of sight against Environment, so cover blocks it),
    /// light (x2) and noise; escalates Patrol → Suspicion → Search → Hunt (<see cref="AlertModel"/>); chases, flanks
    /// (Rastrero), attacks and grabs (Alto/Mudo) through GameEvents.PlayerAttacked. No on-screen perception (M-10):
    /// state changes raise EnemyAlertChanged for the audio cues.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(EnemyHealth))]
    public sealed class EnemyBrain : MonoBehaviour, IStaggerable, IGrabber
    {
        [SerializeField] private EnemyDefinition definition;
        [Tooltip("Eye height for sight checks.")]
        [SerializeField] private float eyeHeight = 1.6f;
        [Tooltip("Visual root (PH_ mesh) tilted when the body falls.")]
        [SerializeField] private Transform visual;

        private NavMeshAgent agent;
        private EnemyHealth health;
        private AlertModel alert;
        private AlertDirector director;
        private Vector3 home;
        private Vector3 lastKnown;
        private float waitTimer;
        private float staggerTimer;
        private float windupTimer = -1f;
        private float cooldownTimer;
        private float flankSide = 1f;
        private int envMask;

        public EnemyDefinition Definition => definition;
        public AlertState Alert => alert != null ? alert.State : AlertState.Patrol;
        public float AlertLevel => alert != null ? alert.Level : 0f;
        public bool IsActive => isActiveAndEnabled && health.IsAlive;
        public bool IsDead => !health.IsAlive;
        public bool IsStaggered => staggerTimer > 0f;
        public bool IsWindingUp => windupTimer >= 0f;
        /// <summary>Set by the flashlight while its beam is on this enemy (M-12).</summary>
        public bool IsLit { get; set; }
        public bool SeesPlayer { get; private set; }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyHealth>();
            alert = new AlertModel(definition.alert);
            alert.StateChanged += OnAlertChanged;
            health.Shot += OnShot;
            health.Died += OnDied;
            envMask = LayerMask.GetMask("Environment");
            flankSide = Random.value < 0.5f ? -1f : 1f;
        }

        private void OnEnable()
        {
            home = transform.position;
            if (Services.TryGet(out director)) director.Register(this);
        }

        private void Start()
        {
            if (director == null && Services.TryGet(out director)) director.Register(this);
            agent.speed = definition.patrolSpeed;
        }

        private void OnDisable()
        {
            if (director != null) director.Unregister(this);
        }

        private Transform Player => director != null ? director.Player : null;

        private void Update()
        {
            if (IsDead) return;
            float dt = Time.deltaTime;
            cooldownTimer = Mathf.Max(0f, cooldownTimer - dt);

            var p = Player;
            float proximity = 0f;
            SeesPlayer = p != null && CanSee(p, out proximity);
            if (SeesPlayer) lastKnown = p.position;
            alert.Tick(dt, SeesPlayer, proximity, IsLit && definition.reactsToLight);

            if (staggerTimer > 0f)
            {
                staggerTimer -= dt;
                agent.isStopped = true;
                return;
            }

            if (windupTimer >= 0f) { TickWindup(dt, p); return; }

            switch (alert.State)
            {
                case AlertState.Patrol: TickPatrol(dt); break;
                case AlertState.Suspicion: TickSuspicion(); break;
                case AlertState.Search: TickSearch(dt); break;
                case AlertState.Hunt: TickHunt(p); break;
            }
        }

        // ---------- Perception ----------

        private bool CanSee(Transform p, out float proximity01)
        {
            proximity01 = 0f;
            if (definition.sightRange <= 0f) return false;   // La Boca is blind
            Vector3 eye = transform.position + Vector3.up * eyeHeight;
            var pc = p.GetComponent<PlayerController>();
            float targetHeight = pc != null && pc.IsCrouched ? 0.8f : 1.3f;
            Vector3 target = p.position + Vector3.up * targetHeight;
            Vector3 to = target - eye;
            float dist = to.magnitude;
            if (dist > definition.sightRange) return false;
            if (Vector3.Angle(transform.forward, new Vector3(to.x, 0f, to.z)) > definition.sightAngle * 0.5f && dist > 1.5f) return false;
            if (Physics.Raycast(eye, to / dist, dist, envMask, QueryTriggerInteraction.Ignore)) return false;  // cover blocks sight (M-09)
            proximity01 = 1f - dist / definition.sightRange;
            return true;
        }

        public void HearNoise(NoiseInfo noise)
        {
            if (IsDead) return;
            float d = Vector3.Distance(transform.position, noise.Position);
            if (d > noise.Radius || d > Mathf.Max(definition.hearingRange, noise.Source == NoiseSource.Shot ? noise.Radius : 0f)) return;
            lastKnown = noise.Position;
            alert.Hear(noise.Source);
        }

        public void OnAttentionFull(Vector3 playerPos)
        {
            if (IsDead) return;
            if (alert.State < AlertState.Search) lastKnown = playerPos;
            alert.OnAttentionFull();
        }

        // ---------- Behaviour ----------

        private void TickPatrol(float dt)
        {
            agent.speed = Speed(definition.patrolSpeed);
            agent.isStopped = false;
            if (agent.pathPending || agent.remainingDistance > 0.4f) return;
            waitTimer -= dt;
            if (waitTimer > 0f) return;
            waitTimer = Random.Range(1f, 3f);
            Vector3 random = home + Random.insideUnitSphere * definition.patrolRadius;
            if (NavMesh.SamplePosition(random, out var hit, 2f, NavMesh.AllAreas)) agent.SetDestination(hit.position);
        }

        private void TickSuspicion()
        {
            agent.isStopped = true;
            FaceTowards(lastKnown, 120f);
        }

        private void TickSearch(float dt)
        {
            agent.isStopped = false;
            agent.speed = Speed(definition.patrolSpeed * 1.4f);
            if (Vector3.Distance(agent.destination, lastKnown) > 0.5f) agent.SetDestination(lastKnown);
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                transform.Rotate(0f, 60f * dt, 0f);   // look around
            }
        }

        private void TickHunt(Transform p)
        {
            if (p == null) return;
            agent.isStopped = false;
            agent.speed = Speed(definition.huntSpeed);
            float dist = Vector3.Distance(transform.position, p.position);
            Vector3 chaseTarget = SeesPlayer ? p.position : lastKnown;

            if (definition.flanks && dist > definition.attackRange * 2f)
            {
                Vector3 side = Vector3.Cross(Vector3.up, (p.position - transform.position).normalized) * flankSide;
                Vector3 flank = p.position + side * definition.flankOffset;
                if (NavMesh.SamplePosition(flank, out var hit, 2f, NavMesh.AllAreas)) chaseTarget = hit.position;
            }

            agent.SetDestination(chaseTarget);

            if (dist <= definition.attackRange && cooldownTimer <= 0f && SeesPlayer && !(director != null && director.PlayerDead))
            {
                windupTimer = definition.attackWindup;
                agent.isStopped = true;
                HbLog.Info("AI", $"{name} attack windup");
            }
        }

        private void TickWindup(float dt, Transform p)
        {
            if (p != null) FaceTowards(p.position, 360f);
            windupTimer -= dt;
            if (windupTimer > 0f) return;
            windupTimer = -1f;
            cooldownTimer = definition.attackCooldown;
            agent.isStopped = false;
            if (p == null || (director != null && director.PlayerDead)) return;

            float dist = Vector3.Distance(transform.position, p.position);
            if (dist > definition.attackRange * 1.2f)
            {
                HbLog.Info("AI", $"{name} attack missed (out of reach)");
                return;
            }
            bool grab = definition.canGrab && dist <= definition.grabRange && Random.value < definition.grabChance;
            HbLog.Info("AI", $"{name} {(grab ? "grabs" : "hits")} Lukas");
            GameEvents.RaisePlayerAttacked(new AttackInfo(definition.hitsToKillLukas, grab, transform.position, this));
        }

        private float Speed(float baseSpeed) => health.IsRecycled ? baseSpeed * definition.recycledSpeedMultiplier : baseSpeed;

        private void FaceTowards(Vector3 point, float degPerSec)
        {
            Vector3 dir = point - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) return;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), degPerSec * Time.deltaTime);
        }

        // ---------- Damage / stagger ----------

        private void OnShot(ShotHit hit)
        {
            if (health.IsAlive)
            {
                lastKnown = hit.Point - hit.Direction * 10f;
                alert.ForceHunt();
                if (hit.WeakPoint) Stagger(hit.Point, definition.weakPointStagger);   // interrupts attack or grab (D-74)
            }
        }

        public void Stagger(Vector3 from, float seconds)
        {
            if (IsDead) return;
            staggerTimer = Mathf.Max(staggerTimer, seconds);
            windupTimer = -1f;
            agent.isStopped = true;
            if (agent.isOnNavMesh)
            {
                Vector3 push = (transform.position - from); push.y = 0f;
                agent.Move(push.normalized * 0.5f);
            }
            HbLog.Info("AI", $"{name} staggered {seconds:0.0}s");
        }

        /// <summary>The discharge spike fired during this enemy's grab (M-11).</summary>
        public void OnGrabCountered(float stunSeconds) => Stagger(Player != null ? Player.position : transform.position, stunSeconds);

        private void OnDied()
        {
            agent.isStopped = true;
            agent.enabled = false;
            windupTimer = -1f;
            if (visual != null) visual.localRotation = Quaternion.Euler(-90f, 0f, 0f);   // placeholder fall
            gameObject.layer = LayerMask.NameToLayer("Corpse");
            foreach (var c in GetComponents<Collider>()) c.isTrigger = true;   // the body does not block movement
            if (alert.State != AlertState.Patrol) OnAlertChanged(alert.State, AlertState.Patrol);
            GameEvents.RaiseEnemyKilled(this, transform.position);
        }

        /// <summary>The lab raised this body (M-20): stand up, collide again, back on the NavMesh, searching.</summary>
        public void OnRecycled()
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            foreach (var c in GetComponents<Collider>()) c.isTrigger = false;
            if (visual != null) visual.localRotation = Quaternion.identity;
            agent.enabled = true;
            if (agent.isOnNavMesh) agent.Warp(transform.position);
            agent.isStopped = false;
            staggerTimer = 0f;
            windupTimer = -1f;
            alert.Reset();
            alert.Hear(NoiseSource.Other);   // rises disoriented: suspicion
            if (director == null) Services.TryGet(out director);
            director?.Register(this);
        }

        private void OnAlertChanged(AlertState from, AlertState to)
        {
            HbLog.Info("AI", $"{name} {from}->{to}");
            float d = Player != null ? Vector3.Distance(transform.position, Player.position) : 0f;
            GameEvents.RaiseEnemyAlertChanged(this, to, d);
        }

        /// <summary>Debug / tests: force an alert state.</summary>
        public void DebugForceHunt() => alert.ForceHunt();

        /// <summary>Debug / tests: move the agent to a point (and face yaw degrees).</summary>
        public void DebugWarp(float x, float y, float z, float yaw)
        {
            if (agent.enabled) agent.Warp(new Vector3(x, y, z)); else transform.position = new Vector3(x, y, z);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            home = transform.position;
        }

        /// <summary>Debug / tests: land an attack or grab right now, bypassing range and chance.</summary>
        public void DebugAttack(bool grab)
        {
            HbLog.Info("AI", $"{name} debug {(grab ? "grab" : "hit")}");
            GameEvents.RaisePlayerAttacked(new AttackInfo(definition.hitsToKillLukas, grab, transform.position, this));
        }

        private void OnDrawGizmosSelected()
        {
            if (definition == null) return;
            Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, definition.hearingRange);
            Gizmos.color = new Color(1f, 1f, 0.3f, 0.4f);
            Vector3 eye = transform.position + Vector3.up * eyeHeight;
            Quaternion l = Quaternion.Euler(0f, -definition.sightAngle * 0.5f, 0f), r = Quaternion.Euler(0f, definition.sightAngle * 0.5f, 0f);
            Gizmos.DrawLine(eye, eye + l * transform.forward * definition.sightRange);
            Gizmos.DrawLine(eye, eye + r * transform.forward * definition.sightRange);
        }
    }

    /// <summary>Something that can grab Lukas and be countered by the discharge spike (Alto, Mudo, collector arm).</summary>
    public interface IGrabber
    {
        void OnGrabCountered(float stunSeconds);
    }
}
