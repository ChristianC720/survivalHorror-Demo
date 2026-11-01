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
    /// <summary>Phase 3 weak point (D-80): a high-voltage transformer, 2 shots each, armed only once the core opens.</summary>
    public sealed class BossTransformer : MonoBehaviour, IDamageable
    {
        [SerializeField] private BossBalance balance;
        [SerializeField] private Renderer glow;
        [SerializeField] private Material deadMaterial;

        private int hits;
        private bool armed, destroyed;

        public bool IsAlive => armed && !destroyed;

        public void SetArmed(bool on)
        {
            armed = on && !destroyed;
            if (glow != null) glow.enabled = armed;
        }

        public void ReceiveShot(in ShotHit hit)
        {
            if (!IsAlive) return;
            hits++;
            Sfx.Play(Sfx.SpikeDischarge, transform.position);   // electric crack (placeholder cue)
            HbLog.Info("Boss", $"{name} hit ({hits}/{balance.transformerShots})");
            if (hits >= balance.transformerShots) Destroy_();
        }

        private void Destroy_()
        {
            destroyed = true;
            armed = false;
            if (glow != null)
            {
                if (deadMaterial != null) glow.sharedMaterial = deadMaterial;
                glow.enabled = true;
            }
            Services.Get<MotherDirector>().TransformerDestroyed(this);
        }

        public void DebugDestroy() { if (IsAlive) Destroy_(); }
    }
}
