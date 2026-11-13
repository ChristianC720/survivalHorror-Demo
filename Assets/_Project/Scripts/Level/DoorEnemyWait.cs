namespace Heartbeat.Level
{
    /// <summary>
    /// C-68: a search or hunt enemy at a closed, unlocked door stands there this long before it opens the door.
    /// The wait restarts whenever no such enemy is at the door (it calmed down, left, or the door changed state).
    /// </summary>
    public sealed class DoorEnemyWait
    {
        private const float Epsilon = 1e-4f;   // float sums of frame times land a hair under the delay

        public float Elapsed { get; private set; }

        /// <summary>True on the frame the wait is over and the enemy opens the door.</summary>
        public bool Tick(float dt, bool enemyWaiting, float delay)
        {
            if (!enemyWaiting) { Elapsed = 0f; return false; }
            Elapsed += dt;
            if (Elapsed < delay - Epsilon) return false;
            Elapsed = 0f;
            return true;
        }

        public void Reset() => Elapsed = 0f;
    }
}
