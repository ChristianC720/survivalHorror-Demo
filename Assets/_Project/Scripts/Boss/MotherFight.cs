using System;

namespace Heartbeat.Boss
{
    public enum MotherPhase { Nodes = 1, Cameras = 2, Transformers = 3, Defeated = 4 }

    /// <summary>
    /// Pure phase rules of The Mother (M-19). Phase 1: hack the 3 nodes. Phase 2: 4 assimilated released by camera
    /// locks (max 2 active); it ends when all 4 are dead and Lukas has reached the core; reaching the core releases
    /// the rest so dodging every camera never blocks the phase (D-115, D-117). Phase 3: destroy the 3 transformers.
    /// </summary>
    public sealed class MotherFight
    {
        public const int Nodes = 3, Transformers = 3;

        private readonly int releasesTotal, maxActive;

        public MotherPhase Phase { get; private set; } = MotherPhase.Nodes;
        public int NodesHacked { get; private set; }
        public int Released { get; private set; }
        public int Killed { get; private set; }
        public int Active => Released - Killed;
        public bool AtCore { get; private set; }
        public int TransformersDown { get; private set; }

        public event Action<MotherPhase> PhaseChanged;

        public MotherFight(int releasesTotal = 4, int maxActive = 2)
        {
            this.releasesTotal = releasesTotal;
            this.maxActive = maxActive;
        }

        public void NodeHacked()
        {
            if (Phase != MotherPhase.Nodes) return;
            NodesHacked++;
            if (NodesHacked >= Nodes) SetPhase(MotherPhase.Cameras);
        }

        /// <summary>A camera locked on Lukas: may one assimilated leave its capsule now?</summary>
        public bool TryRelease()
        {
            if (Phase != MotherPhase.Cameras || Released >= releasesTotal || Active >= maxActive) return false;
            Released++;
            return true;
        }

        /// <summary>How many to release right now after Lukas reached the core (still respecting max active).</summary>
        public int PendingAtCore => Phase == MotherPhase.Cameras && AtCore ? Math.Min(releasesTotal - Released, maxActive - Active) : 0;

        public void AssimilatedKilled()
        {
            if (Phase != MotherPhase.Cameras) return;
            Killed++;
            CheckCameras();
        }

        public void ReachedCore()
        {
            if (Phase != MotherPhase.Cameras) return;
            AtCore = true;
            CheckCameras();
        }

        public void TransformerDestroyed()
        {
            if (Phase != MotherPhase.Transformers) return;
            TransformersDown++;
            if (TransformersDown >= Transformers) SetPhase(MotherPhase.Defeated);
        }

        private void CheckCameras()
        {
            if (AtCore && Killed >= releasesTotal) SetPhase(MotherPhase.Transformers);
        }

        private void SetPhase(MotherPhase p)
        {
            Phase = p;
            PhaseChanged?.Invoke(p);
        }
    }
}
