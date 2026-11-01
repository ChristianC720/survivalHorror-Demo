namespace Heartbeat.Level
{
    /// <summary>Something a completed hack activates: a door (open or seal), an elevator, a boss node, the intro hatch.</summary>
    public interface IHackTarget
    {
        HackDifficulty Difficulty { get; }
        void OnHackSucceeded();
    }
}
