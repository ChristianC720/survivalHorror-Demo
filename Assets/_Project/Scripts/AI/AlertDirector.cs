using System.Collections.Generic;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.AI
{
    /// <summary>
    /// Scene service of S-02: registry of enemies, distributes noises and The Mother's attention, computes the highest
    /// alert near Lukas (for surveillance M-03 and audio M-10) and raises hunt start/end (transit lockdown D-96).
    /// One per Floor scene; registers itself in Services.
    /// </summary>
    public sealed class AlertDirector : MonoBehaviour
    {
        [Tooltip("Enemies farther than this from Lukas do not count for the highest alert (M-03 scope).")]
        [SerializeField] private float alertScopeRadius = 25f;

        private readonly List<EnemyBrain> enemies = new List<EnemyBrain>();
        private Transform player;

        public AlertState HighestAlert { get; private set; } = AlertState.Patrol;
        /// <summary>Lukas is dead: enemies stop attacking until the reconstruction rebuilds the floor.</summary>
        public bool PlayerDead { get; private set; }
        public int HuntingCount { get; private set; }
        public IReadOnlyList<EnemyBrain> Enemies => enemies;

        private void Awake()
        {
            if (GetComponent<Recycle.RecycleSystem>() == null) gameObject.AddComponent<Recycle.RecycleSystem>();
        }

        private void OnEnable()
        {
            Services.Register(this);
            GameEvents.NoiseEmitted += OnNoise;
            GameEvents.AttentionFull += OnAttentionFull;
            GameEvents.PlayerDied += OnPlayerDied;
        }

        private void OnDisable()
        {
            Services.Unregister(this);
            GameEvents.NoiseEmitted -= OnNoise;
            GameEvents.AttentionFull -= OnAttentionFull;
            GameEvents.PlayerDied -= OnPlayerDied;
        }

        private void OnPlayerDied(DeathCause _) => PlayerDead = true;

        public void Register(EnemyBrain e) { if (!enemies.Contains(e)) enemies.Add(e); }
        public void Unregister(EnemyBrain e) => enemies.Remove(e);

        public Transform Player
        {
            get
            {
                if (player == null && Services.TryGet<Player.PlayerController>(out var pc)) player = pc.transform;
                return player;
            }
        }

        private void OnNoise(NoiseInfo noise)
        {
            foreach (var e in enemies)
                if (e.IsActive) e.HearNoise(noise);
        }

        private void OnAttentionFull(Vector3 playerPos)
        {
            foreach (var e in enemies)
                if (e.IsActive && Vector3.Distance(e.transform.position, playerPos) <= alertScopeRadius)
                    e.OnAttentionFull(playerPos);
        }

        private void LateUpdate()
        {
            var p = Player;
            var highest = AlertState.Patrol;
            int hunting = 0;
            foreach (var e in enemies)
            {
                if (!e.IsActive) continue;
                if (e.Alert == AlertState.Hunt) hunting++;
                if (p != null && Vector3.Distance(e.transform.position, p.position) > alertScopeRadius) continue;
                if (e.Alert > highest) highest = e.Alert;
            }

            if (hunting > 0 && HuntingCount == 0) { HbLog.Info("AI", "Hunt started"); GameEvents.RaiseHuntChanged(true); }
            if (hunting == 0 && HuntingCount > 0) { HbLog.Info("AI", "Hunt ended"); GameEvents.RaiseHuntChanged(false); }
            HuntingCount = hunting;

            if (highest != HighestAlert)
            {
                HighestAlert = highest;
                GameEvents.RaiseHighestAlertChanged(highest);
            }
        }
    }
}
