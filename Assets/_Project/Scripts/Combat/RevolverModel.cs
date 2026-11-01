using System;

namespace Heartbeat.Combat
{
    public enum FireResult { Fired, DryFire, Cooldown, Reloading }

    /// <summary>
    /// Kessler revolver (M-04, D-74): 6-round drum, ~0.8 s between shots, round-by-round reload (~0.7 s each)
    /// that can be cancelled keeping the rounds already loaded; spread closes in ~1 s standing still.
    /// Pure logic; rounds come from the inventory through a callback.
    /// </summary>
    public sealed class RevolverModel
    {
        private readonly int drumSize;
        private readonly float shotInterval;
        private readonly float reloadPerRound;
        private readonly float spreadCloseSeconds;
        private readonly float spreadPerShot;

        private float cooldown;
        private float reloadTimer;

        public int Loaded { get; private set; }
        public bool IsReloading { get; private set; }
        /// <summary>0 = fully closed, 1 = maximum spread.</summary>
        public float Spread { get; private set; }

        public event Action<int> LoadedChanged;

        public RevolverModel(int drumSize, float shotInterval, float reloadPerRound, float spreadCloseSeconds, float spreadPerShot, int loaded = 0)
        {
            this.drumSize = drumSize;
            this.shotInterval = shotInterval;
            this.reloadPerRound = reloadPerRound;
            this.spreadCloseSeconds = spreadCloseSeconds;
            this.spreadPerShot = spreadPerShot;
            Loaded = loaded;
        }

        public bool IsFull => Loaded >= drumSize;

        public FireResult TryFire()
        {
            if (IsReloading) return FireResult.Reloading;
            if (cooldown > 0f) return FireResult.Cooldown;
            cooldown = shotInterval;
            if (Loaded <= 0) return FireResult.DryFire;
            Loaded--;
            Spread = Math.Min(1f, Spread + spreadPerShot);
            LoadedChanged?.Invoke(Loaded);
            return FireResult.Fired;
        }

        /// <summary>Starts loading round by round. Returns false if the drum is full or there is no ammo.</summary>
        public bool BeginReload(bool hasAmmo)
        {
            if (IsReloading || IsFull || !hasAmmo) return false;
            IsReloading = true;
            reloadTimer = reloadPerRound;
            return true;
        }

        public void CancelReload() => IsReloading = false;

        /// <param name="takeRound">Removes one round from the inventory; false when there is none left.</param>
        /// <param name="unsteady">Moving, running or just hit: the spread opens.</param>
        public void Tick(float dt, Func<bool> takeRound, bool unsteady)
        {
            if (cooldown > 0f) cooldown = Math.Max(0f, cooldown - dt);

            if (unsteady) Spread = 1f;
            else Spread = Math.Max(0f, Spread - dt / spreadCloseSeconds);

            if (!IsReloading) return;
            reloadTimer -= dt;
            while (IsReloading && reloadTimer <= 0f)
            {
                if (IsFull || !takeRound()) { IsReloading = false; break; }
                Loaded++;
                LoadedChanged?.Invoke(Loaded);
                if (IsFull) { IsReloading = false; break; }
                reloadTimer += reloadPerRound;
            }
        }

        /// <summary>Restores the drum when the inventory is carried to the next floor or loaded from an autosave.</summary>
        public void SetLoaded(int rounds)
        {
            Loaded = Math.Max(0, Math.Min(drumSize, rounds));
            IsReloading = false;
            LoadedChanged?.Invoke(Loaded);
        }

        /// <summary>Reconstruction: The Mother returns it unloaded (D-74).</summary>
        public void Unload()
        {
            Loaded = 0;
            IsReloading = false;
            LoadedChanged?.Invoke(0);
        }
    }
}
