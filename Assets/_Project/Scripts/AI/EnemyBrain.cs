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
        private float screamTimer;
        private float burstClock;
        private bool asleep;
        private int envMask;
        // C-64: the next attack is a grab (rolled per attack at grabChance); it closes to grabRange before winding up.
        private bool grabIntent, grabWindup;
        private float holdTimer;

        public event System.Action Screamed;

        public EnemyDefinition Definition => definition;
        public AlertState Alert => alert != null ? alert.State : AlertState.Patrol;
        public float AlertLevel => alert != null ? alert.Level : 0f;
        public bool IsActive => isActiveAndEnabled && health.IsAlive;
        public bool IsDead => !health.IsAlive;
        public bool IsStaggered => staggerTimer > 0f;
        public bool IsWindingUp => windupTimer >= 0f;
        /// <summary>C-64: from the grab windup until the hands let go. Lukas is pulled and held while this is true.</summary>
        public bool IsGrabbing => !IsDead && !IsStaggered && ((grabWindup && windupTimer >= 0f) || holdTimer > 0f);
        /// <summary>0 at the start of the grab windup, 1 when the hands close.</summary>
        public float GrabProgress => grabWindup && windupTimer >= 0f ? 1f - windupTimer / Mathf.Max(definition.attackWindup, 0.05f) : 1f;
        /// <summary>Set by the flashlight while its beam is on this enemy (M-12).</summary>
        public bool IsLit { get; set; }
        public bool SeesPlayer { get; private set; }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyHealth>();
            var tuning = definition.alert;
            if (Cycle.CycleController.StoryMode) tuning.sightGainPerSecond *= SystemsBalance.Current.storyDetectionMultiplier;   // D-103: slower detection
            alert = new AlertModel(tuning);
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
            asleep = definition.dormant;
            grabIntent = RollGrab();
        }

        private bool RollGrab() => definition.canGrab && Random.value < definition.grabChance;

        public bool IsDormant => asleep || heldUntilShot;

        /// <summary>
        /// Tutorial enemy (user 2026-11-09): stands still and never attacks, sees or hears until Lukas shoots it the first time.
        /// </summary>
        public void HoldUntilShot()
        {
            heldUntilShot = true;
            HbLog.Info("AI", $"{name} holds still until shot (tutorial)");
        }

        private bool heldUntilShot;

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
            screamTimer = Mathf.Max(0f, screamTimer - dt);

            var p = Player;
            if (heldUntilShot) { agent.isStopped = true; return; }
            if (asleep)
            {
                // El Mudo: still like a corpse until Lukas comes within ~3 m or lights it (D-60).
                agent.isStopped = true;
                if (p != null && (IsLit || Vector3.Distance(transform.position, p.position) <= definition.activationRange))
                {
                    asleep = false;
                    lastKnown = p.position;
                    HbLog.Info("AI", $"{name} activates ({(IsLit ? "lit" : "proximity")})");
                    alert.ForceHunt();
                }
                return;
            }
            float proximity = 0f;
            SeesPlayer = p != null && CanSee(p, out proximity);
            if (SeesPlayer) lastKnown = p.position;
            alert.Tick(dt, SeesPlayer, proximity, IsLit && definition.reactsToLight);
            // D-173: a beam on an enemy that cannot see Lukas (back turned, behind cover) still makes it suspicious, towards him.
            if (IsLit && definition.reactsToLight && !SeesPlayer && p != null && alert.State < AlertState.Search) { lastKnown = p.position; alert.Hear(NoiseSource.Other); }

            if (staggerTimer > 0f)
            {
                staggerTimer -= dt;
                agent.isStopped = true;
                return;
            }

            if (holdTimer > 0f)
            {
                holdTimer -= dt;
                agent.isStopped = true;
                if (p != null) FaceTowards(p.position, 360f);
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
            if (asleep || heldUntilShot) return;   // El Mudo only wakes by proximity or light; the tutorial enemy only by a shot
            if (definition.screams && noise.Source == NoiseSource.Scream) return;   // La Boca ignores screams (her own too)
            float d = Vector3.Distance(transform.position, noise.Position);
            float radius = noise.Radius * definition.hearingMultiplier;
            float reach = Mathf.Min(radius, Mathf.Max(definition.hearingRange, (noise.Source == NoiseSource.Shot || noise.Source == NoiseSource.Scream) ? radius : 0f));   // shots and screams carry
            if (d > reach) return;
            // Walls (−70%) and closed doors (−50%) shrink what reaches us (S-02/S-11).
            if (Services.TryGet(out Audio.AudioService audio) && d > reach * audio.OcclusionBetween(noise.Position + Vector3.up, transform.position + Vector3.up * eyeHeight)) return;
            lastKnown = noise.Position;
            bool blind = definition.sightRange <= 0f;
            if (blind && alert.State >= AlertState.Search && noise.Source != NoiseSource.Scream && d <= definition.hearingRange * definition.blindHuntFraction)
                alert.ForceHunt();   // blind: a close sound is as good as a sighting
            else alert.Hear(noise.Source);
        }

        public void OnAttentionFull(Vector3 playerPos, bool immediate = false)
        {
            if (IsDead || heldUntilShot) return;
            if (alert.State < AlertState.Search) lastKnown = playerPos;
            if (immediate) alert.Hear(NoiseSource.Machinery);   // boss release: straight to the point the camera saw
            else alert.OnAttentionFull(SystemsBalance.Current.attentionSuspicionSeconds);
        }

        // ---------- Behaviour ----------

        private void TickPatrol(float dt)
        {
            agent.stoppingDistance = 0.2f;
            agent.speed = Speed(definition.patrolSpeed);
            agent.isStopped = false;
            if (agent.pathPending || agent.remainingDistance > 0.4f) return;
            waitTimer -= dt;
            if (waitTimer > 0f) return;
            waitTimer = Random.Range(1f, 3f);
            // Closed doors are carved walls: never pick a point behind one. Left behind a closed door? This is home now.
            if (!Reachable(home)) home = transform.position;
            Vector3 random = home + Random.insideUnitSphere * definition.patrolRadius;
            if (NavMesh.SamplePosition(random, out var hit, 2f, NavMesh.AllAreas) && Reachable(hit.position)) agent.SetDestination(hit.position);
        }

        private NavMeshPath scratchPath;
        private bool Reachable(Vector3 point)
        {
            scratchPath ??= new NavMeshPath();
            if (!NavMesh.SamplePosition(point, out var onMesh, 1.5f, agent.areaMask)) return false;
            return NavMesh.CalculatePath(transform.position, onMesh.position, agent.areaMask, scratchPath)
                   && scratchPath.status == NavMeshPathStatus.PathComplete;
        }

        // ---------- Doors (M-14): search/hunt walk up to the closed door on the way and open it (or pound a seal) ----------

        private Level.DoorLeaf viaDoor;
        private float routeCheck;

        /// <summary>The goal itself, or the near side of the closed door that stands between us and it.</summary>
        private Vector3 Route(Vector3 goal)
        {
            if (viaDoor != null && !viaDoor.EnemyCanReach) viaDoor = null;   // it opened
            routeCheck -= Time.deltaTime;
            if (viaDoor == null && routeCheck <= 0f)
            {
                routeCheck = 0.5f;
                if (!Reachable(goal)) viaDoor = DoorTowards(goal);
            }
            if (viaDoor == null) return goal;
            var t = viaDoor.transform;
            float side = t.InverseTransformPoint(transform.position).z >= 0f ? 1f : -1f;
            return t.TransformPoint(new Vector3(0f, 0f, side));
        }

        private Level.DoorLeaf DoorTowards(Vector3 goal)
        {
            if (!Services.TryGet(out Generation.FloorBuilder builder)) return null;
            float direct = Vector3.Distance(transform.position, goal);
            Level.DoorLeaf best = null;
            float bestCost = direct * 1.6f + 4f;   // only doors roughly on the way
            foreach (var d in builder.Doors)
            {
                if (d == null || !d.EnemyCanReach) continue;
                Vector3 p = d.transform.position;
                float cost = Vector3.Distance(transform.position, p) + Vector3.Distance(p, goal);
                if (cost < bestCost) { bestCost = cost; best = d; }
            }
            return best;
        }

        private void TickSuspicion()
        {
            agent.isStopped = true;
            FaceTowards(lastKnown, 120f);
        }

        private void TickSearch(float dt)
        {
            agent.stoppingDistance = 0.2f;
            agent.isStopped = false;
            agent.speed = Speed(definition.patrolSpeed * 1.4f);
            Vector3 dest = Route(lastKnown);
            if (Vector3.Distance(agent.destination, dest) > 0.5f) agent.SetDestination(dest);
            if (viaDoor == null && !agent.pathPending && agent.remainingDistance < 0.5f)
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

            // Never overlap Lukas, seen or not: his CharacterController depenetrates out of our collider on every Move,
            // and near a wall that shoved him through it and out of the level (the "cyan screen").
            float keep = agent.radius + PlayerRadius(p) + 0.2f;
            agent.stoppingDistance = Mathf.Max(SeesPlayer ? definition.grabRange * 0.8f : 0.2f, keep);
            Vector3 dest = Route(chaseTarget);
            if (viaDoor != null) agent.stoppingDistance = 0.3f;
            agent.SetDestination(dest);

            // Bursts (El Mudo): sprint, freeze, sprint.
            if (definition.burstSeconds > 0f)
            {
                burstClock += Time.deltaTime;
                float cycle = definition.burstSeconds + definition.burstPauseSeconds;
                agent.isStopped = Mathf.Repeat(burstClock, cycle) > definition.burstSeconds;
            }
            if (dist < keep) agent.isStopped = true;   // avoidance from other agents can still squeeze us in

            bool blind = definition.sightRange <= 0f;   // La Boca strikes at what she hears
            bool grab = grabIntent && GrabPathClear(p);
            float reach = grab ? definition.grabRange : definition.attackRange;
            if (dist <= reach && cooldownTimer <= 0f && (SeesPlayer || blind) && !(director != null && director.PlayerDead))
                StartWindup(grab);
        }

        // C-64: no grab across a low wall or a counter: Lukas would be pulled into it while the arms reach over.
        private bool GrabPathClear(Transform p)
        {
            Vector3 waist = Vector3.up * 0.9f;
            return !Physics.Linecast(transform.position + waist, p.position + waist, envMask, QueryTriggerInteraction.Ignore);
        }

        private void StartWindup(bool grab)
        {
            windupTimer = definition.attackWindup;
            grabWindup = grab;
            agent.isStopped = true;
            HbLog.Info("AI", $"{name} {(grab ? "grab" : "attack")} windup");
            if (grab) GameEvents.RaiseGrabStarted(this);   // Lukas is pulled to the hands while the clip closes them (C-64)
        }

        private void TickWindup(float dt, Transform p)
        {
            if (p != null) FaceTowards(p.position, 360f);
            windupTimer -= dt;
            if (windupTimer > 0f) return;
            windupTimer = -1f;
            cooldownTimer = definition.attackCooldown;
            agent.isStopped = false;
            bool grabbing = grabWindup;
            grabWindup = false;
            grabIntent = RollGrab();
            if (p == null || (director != null && director.PlayerDead)) return;

            float dist = Vector3.Distance(transform.position, p.position);
            if (dist > definition.attackRange * 1.2f)
            {
                HbLog.Info("AI", $"{name} attack missed (out of reach)");
                return;
            }
            // El Mudo's ambush: if the grab does not land, it hits (Enemigos y NPC).
            bool grab = grabbing && dist <= definition.grabRange * 1.2f;
            if (grab) { holdTimer = definition.grabHoldSeconds; cooldownTimer += holdTimer; }   // the cooldown runs after the hands let go
            HbLog.Info("AI", $"{name} {(grab ? "grabs" : "hits")} Lukas");
            GameEvents.RaisePlayerAttacked(new AttackInfo(definition.hitsToKillLukas, grab, transform.position, this));
        }

        private float Speed(float baseSpeed) => health.IsRecycled ? baseSpeed * definition.recycledSpeedMultiplier : baseSpeed;

        private float playerRadius = -1f;
        private float PlayerRadius(Transform p)
        {
            if (playerRadius < 0f) playerRadius = p.TryGetComponent(out CharacterController cc) ? cc.radius : 0.35f;
            return playerRadius;
        }

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
                asleep = false;   // a bullet wakes a dormant Mudo at any distance
                if (heldUntilShot) { heldUntilShot = false; HbLog.Info("AI", $"{name} shot: the tutorial hold ends"); }
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
            grabWindup = false;
            holdTimer = 0f;   // a weak-point hit or the spike frees Lukas (D-74, M-11)
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
            gameObject.layer = LayerMask.NameToLayer("Corpse");
            foreach (var c in GetComponents<Collider>()) c.isTrigger = true;   // the body does not block movement
            SetChildColliders(false);   // nor do its weak points: a solid head sphere on the floor soft-locked corridors
            if (alert.State != AlertState.Patrol) OnAlertChanged(alert.State, AlertState.Patrol);
            GameEvents.RaiseEnemyKilled(this, transform.position);
        }

        private void SetChildColliders(bool on)
        {
            foreach (var c in GetComponentsInChildren<Collider>(true))
                if (c.gameObject != gameObject) c.enabled = on;
        }

        /// <summary>The lab raised this body (M-20): stand up, collide again, back on the NavMesh, searching.</summary>
        public void OnRecycled()
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            foreach (var c in GetComponents<Collider>()) c.isTrigger = false;
            SetChildColliders(true);
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
            if (definition.screams && to >= AlertState.Search && to > from && screamTimer <= 0f && !IsDead) Scream();
            if (definition.dormant && to == AlertState.Patrol && !IsDead) asleep = true;   // back to stillness where it stands
        }

        /// <summary>La Boca's own scream (D-64): not the heartbeat; nearby enemies search towards her.</summary>
        private void Scream()
        {
            screamTimer = definition.screamCooldown;
            HbLog.Info("AI", $"{name} SCREAMS");
            Screamed?.Invoke();
            Sfx.Play("CUE_Boca_Scream", transform.position);
            GameEvents.RaiseNoiseEmitted(new NoiseInfo(transform.position, definition.screamRadius, NoiseSource.Scream));
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

        /// <summary>Debug / tests: start a grab windup right now (Lukas is pulled in), bypassing range and chance.</summary>
        public void DebugStartGrab()
        {
            if (!definition.canGrab || IsDead) return;
            StartWindup(Player == null || GrabPathClear(Player));   // across cover it is a plain attack, as in the hunt
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
