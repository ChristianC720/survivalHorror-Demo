using Heartbeat.Save;

namespace Heartbeat.Cycle
{
    /// <summary>
    /// Cost of quitting from the pause menu (D-31, D-96, Reglas y Estados rule 4). Pause freezes the situation, so the
    /// cost depends on the state when pausing. "Alert" means hunt only: quitting in Suspicion or Search is calm.
    /// A crash costs nothing (the last autosave is used), so it never goes through here.
    /// </summary>
    public static class ExitRules
    {
        public static ExitCost Evaluate(bool hunting, bool inSavedFinalRoom) =>
            hunting ? ExitCost.Hunt : inSavedFinalRoom ? ExitCost.None : ExitCost.Calm;
    }
}
