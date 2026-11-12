namespace Heartbeat.Interaction
{
    public enum HoldResult { None, Tap, Completed, Cancelled }

    /// <summary>
    /// One press of Interact on a target that also has a hold action (a door: tap opens, hold seals, M-14).
    /// Released before <c>tapSeconds</c> = tap; held for <c>holdSeconds</c> = completed; released in between = cancelled,
    /// and neither action runs. Plain C# so it is EditMode-testable.
    /// </summary>
    public sealed class HoldInteraction
    {
        private float tapSeconds, holdSeconds, elapsed;

        public bool Active { get; private set; }
        public float Progress => Active && holdSeconds > 0f ? System.Math.Min(1f, elapsed / holdSeconds) : 0f;

        public void Begin(float tap, float hold)
        {
            tapSeconds = tap;
            holdSeconds = hold;
            elapsed = 0f;
            Active = true;
        }

        public void Cancel() => Active = false;

        /// <summary>Call once per frame while <see cref="Active"/>.</summary>
        public HoldResult Tick(float deltaTime, bool pressed)
        {
            if (!Active) return HoldResult.None;
            if (!pressed)
            {
                Active = false;
                return elapsed < tapSeconds ? HoldResult.Tap : HoldResult.Cancelled;
            }
            elapsed += deltaTime;
            if (elapsed < holdSeconds) return HoldResult.None;
            Active = false;
            return HoldResult.Completed;
        }
    }
}
