using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Heartbeat.AI;
using Heartbeat.Audio;
using Heartbeat.Core;
using Heartbeat.Generation;
using Heartbeat.Inventory;
using Heartbeat.Narrative;
using UnityEngine;

namespace Heartbeat.Boss
{
    /// <summary>
    /// The Mother encounter (M-19, S-10) in the PA central (D-59). Drives <see cref="MotherFight"/>: phase 1 hack the 3
    /// nodes while the collector arm patrols; phase 2 hostile cameras release assimilated from the archive capsules
    /// under cryo fog; phase 3 shoot the 3 exposed transformers. Then the common victory sequence (fall, names with the
    /// 6 Emilia documents, Awakening) and the final decision (D-51). No recycling in this room (D-28). Logs
    /// "[Heartbeat][Boss] …". Lives on the ROOM_Mother prefab root; parts are found among its children.
    /// </summary>
    public sealed class MotherDirector : MonoBehaviour
    {
        [SerializeField] private BossBalance balance;
        [SerializeField] private GameObject altoPrefab;
        [SerializeField] private GameObject rastreroPrefab;

        private MotherFight fight;
        private BossNode[] nodes;
        private HostileCamera[] cameras;
        private BossTransformer[] transformers;
        private DecisionPoint[] decisions;
        private CollectorArm arm;
        private Transform[] capsules;
        private readonly List<EnemyBrain> released = new List<EnemyBrain>();
        private AudioSource music;
        private bool decided;

        public MotherFight Fight => fight;
        public BossBalance Balance => balance;

        private void OnEnable()
        {
            Services.Register(this);
            GameEvents.EnemyKilled += OnEnemyKilled;
        }

        private void OnDisable()
        {
            Services.Unregister(this);
            GameEvents.EnemyKilled -= OnEnemyKilled;
            if (Services.TryGet(out Recycle.RecycleSystem recycle)) recycle.Suspended = false;
            RenderSettings.fog = false;
            StopMusic();
        }

        private void Start()
        {
            nodes = GetComponentsInChildren<BossNode>(true);
            cameras = GetComponentsInChildren<HostileCamera>(true);
            transformers = GetComponentsInChildren<BossTransformer>(true);
            decisions = GetComponentsInChildren<DecisionPoint>(true);
            arm = GetComponentInChildren<CollectorArm>(true);
            capsules = GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith("Capsule_")).ToArray();
            if (Services.TryGet(out Recycle.RecycleSystem recycle)) recycle.Suspended = true;   // only machine here (D-28)

            fight = new MotherFight(balance.releasesTotal, balance.maxActive);
            fight.PhaseChanged += Enter;
            foreach (var d in decisions) d.SetActive(false);
            HbLog.Info("Boss", $"The Mother: {nodes.Length} nodes, {cameras.Length} cameras, {transformers.Length} transformers, {capsules.Length} capsules");
            if (arm != null) arm.SetActive(false);
            StartCoroutine(BeginWhenPlaying());
        }

        /// <summary>The room is live behind the floor-transition fade; the fight starts once Lukas can see it.</summary>
        private IEnumerator BeginWhenPlaying()
        {
            var states = Services.Get<GameStateMachine>();
            while (states.Current != GameStateId.Playing) yield return null;
            yield return new WaitForSeconds(2f);
            Enter(MotherPhase.Nodes);
        }

        // ---------- Phases ----------

        private void Enter(MotherPhase p)
        {
            HbLog.Info("Boss", $"Phase {(int)p} ({p})");
            GameEvents.RaiseBossPhaseChanged((int)p);
            string subject = Services.TryGet(out Cycle.CycleController c) && c.Data != null ? c.Data.SubjectLabel : "R-H-07";
            foreach (var n in nodes) n.SetActive(p == MotherPhase.Nodes);
            foreach (var cam in cameras) cam.SetActive(p == MotherPhase.Cameras);
            foreach (var t in transformers) t.SetArmed(p == MotherPhase.Transformers);
            if (arm != null) arm.SetActive(p != MotherPhase.Defeated);
            RenderSettings.fog = p == MotherPhase.Cameras;   // cryo fog: less visibility, never hides M-10 signals
            RenderSettings.fogColor = new Color(0.55f, 0.6f, 0.65f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = balance.fogDensity;

            switch (p)
            {
                case MotherPhase.Nodes:
                    Subtitles.Say(Subtitles.Mother, "SUB.MOTHER.PHASE1", subject);
                    PlayMusic("CUE_Music_Mother_P1");
                    break;
                case MotherPhase.Cameras:
                    Subtitles.Say(Subtitles.Mother, "SUB.MOTHER.PHASE2");
                    PlayMusic("CUE_Music_Mother_P2");
                    DropAmmo("Ammo2_", balance.phase2Ammo);
                    break;
                case MotherPhase.Transformers:
                    Subtitles.Say(Subtitles.Mother, "SUB.MOTHER.PHASE3", subject);
                    PlayMusic("CUE_Music_Mother_P3");
                    DropAmmo("Ammo3_", balance.phase3Ammo);
                    break;
                case MotherPhase.Defeated:
                    StopMusic();
                    GameEvents.RaiseBossDefeated();
                    StartCoroutine(Victory());
                    break;
            }
        }

        public void NodeHacked(BossNode node)
        {
            HbLog.Info("Boss", $"Node {node.name} hacked ({fight.NodesHacked + 1}/{MotherFight.Nodes})");
            fight.NodeHacked();
        }

        /// <summary>A hostile camera locked on Lukas (D-28): release one assimilated towards that point.</summary>
        public void CameraLocked(HostileCamera cam, Vector3 point)
        {
            HbLog.Info("Boss", $"{cam.name} locked on Lukas");
            if (fight.TryRelease()) Release(point);
        }

        public void ReachedCore()
        {
            if (fight.Phase != MotherPhase.Cameras || fight.AtCore) return;
            HbLog.Info("Boss", "Lukas reached the core");
            fight.ReachedCore();
            ReleasePending();
        }

        public void TransformerDestroyed(BossTransformer t)
        {
            HbLog.Info("Boss", $"{t.name} destroyed ({fight.TransformersDown + 1}/{MotherFight.Transformers})");
            fight.TransformerDestroyed();
        }

        private void ReleasePending()
        {
            for (int n = fight.PendingAtCore; n > 0; n--)
                if (fight.TryRelease()) Release(transform.position + new Vector3(10f, 0f, 10f));
        }

        private void Release(Vector3 towards)
        {
            // 2 Altos and 2 Rastreros, alternating (D-65, D-115).
            var prefab = fight.Released % 2 == 1 ? altoPrefab : rastreroPrefab;
            var capsule = capsules.Length > 0 ? capsules[(fight.Released - 1) % capsules.Length] : transform;
            var go = Instantiate(prefab, capsule.position + capsule.forward * 1.2f, capsule.rotation, transform);
            go.name = $"Assimilated_{fight.Released}";
            var brain = go.GetComponent<EnemyBrain>();
            released.Add(brain);
            brain.OnAttentionFull(towards);   // heads to the point the camera saw
            Sfx.Play("CUE_Capsule_Reconstruction", capsule.position);
            HbLog.Info("Boss", $"Released {go.name} ({prefab.name}) from {capsule.name}: {fight.Active} active, {fight.Released}/{balance.releasesTotal}");
        }

        private void OnEnemyKilled(MonoBehaviour enemy, Vector3 _)
        {
            if (!(enemy is EnemyBrain b) || !released.Contains(b)) return;
            released.Remove(b);
            fight.AssimilatedKilled();
            HbLog.Info("Boss", $"Assimilated down: {fight.Killed}/{balance.releasesTotal}");
            ReleasePending();
        }

        private void DropAmmo(string prefix, int rounds)
        {
            if (!Services.TryGet(out FloorBuilder builder)) return;
            var points = GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith(prefix)).ToArray();
            if (points.Length == 0) return;
            int per = Mathf.CeilToInt(rounds / (float)points.Length);
            int left = rounds;
            foreach (var p in points)
            {
                int n = Mathf.Min(per, left);
                if (n <= 0) break;
                builder.DropItem(ItemKind.Ammo, n, p.position);
                left -= n;
            }
            HbLog.Info("Boss", $"{rounds} rounds placed at {points.Length} {prefix.TrimEnd('_')} points");
        }

        // ---------- Victory and decision (D-51, D-55, D-61) ----------

        private IEnumerator Victory()
        {
            var states = Services.Get<GameStateMachine>();
            states.TryTransition(GameStateId.Victory);
            HbLog.Info("Boss", "The Mother falls");
            Subtitles.Say(Subtitles.Mother, "SUB.MOTHER.FALL");
            yield return new WaitForSeconds(5f);

            bool allEmilia = Services.Get<RegistryModel>().HasAllEmilia;
            if (allEmilia)
            {
                // The documents complete; Lukas recovers Emilia's name, then his own (D-55).
                Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.NAME_EMILIA");
                yield return new WaitForSeconds(3f);
                Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.NAME_OWN");
                yield return new WaitForSeconds(3.5f);
            }
            // Awakening: the lab heartbeat quickens (LabHeartbeat listens to BossDefeated).
            yield return new WaitForSeconds(2f);

            if (!allEmilia)
            {
                HbLog.Info("Boss", "No choice without the 6 Emilia documents: Lukas examines the central terminal");
                yield return new WaitForSeconds(balance.autoDecisionSeconds);
                End("BasicRelief", GameStateId.EndingBasicRelief);
                yield break;
            }

            states.TryTransition(GameStateId.FinalDecision);
            foreach (var d in decisions) d.SetActive(true);
            HbLog.Info("Boss", "Final decision: central terminal or exit (E)");
            // Pressure without death (D-61): speakers and heartbeat rise, the room shakes and the lights fail.
            var lights = GetComponentsInChildren<Light>(true);
            while (!decided)
            {
                foreach (var l in lights) l.enabled = Random.value > 0.25f;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.4f));
            }
        }

        public void Decide(DecisionPoint.Kind kind)
        {
            if (decided) return;
            decided = true;
            foreach (var d in decisions) d.SetActive(false);
            HbLog.Info("Boss", $"Decision: {kind}");
            if (kind == DecisionPoint.Kind.CentralTerminal) End("FullRelief", GameStateId.EndingFullRelief);
            else End("Ambiguous", GameStateId.EndingAmbiguous);
        }

        private static void End(string id, GameStateId state)
        {
            Services.Get<GameStateMachine>().TryTransition(state);
            GameEvents.RaiseEndingTriggered(id);
        }

        // ---------- Music ----------

        private void PlayMusic(string cue)
        {
            StopMusic();
            if (Services.TryGet(out AudioService audio) && audio.TryGetCue(cue, out var c)) music = audio.StartLoop(c);
        }

        private void StopMusic()
        {
            if (music != null && Services.TryGet(out AudioService audio)) audio.Stop(music);
            music = null;
        }

        /// <summary>Debug / playtests: jump to a phase's end condition.</summary>
        public void DebugHackAllNodes() { foreach (var n in nodes) if (n.IsActive) n.OnHackSucceeded(); }
        public void DebugDestroyTransformers() { foreach (var t in transformers) t.DebugDestroy(); }
        public void DebugKillReleased() { foreach (var b in released.ToArray()) b.GetComponent<EnemyHealth>()?.DebugKill(); }
    }
}
