namespace Heartbeat.Level
{
    /// <summary>Something a completed hack activates: a door panel, an elevator, a boss node, the intro hatch.</summary>
    public interface IHackTarget
    {
        HackDifficulty Difficulty { get; }
        void OnHackSucceeded();
    }
}
