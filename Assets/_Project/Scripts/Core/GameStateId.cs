namespace Heartbeat.Core
{
    /// <summary>Main states (GDD 02 - Diseño/07 - Reglas y Estados del Juego, "Estados principales").</summary>
    public enum GameStateId
    {
        Boot,
        BrightnessCalibration,
        Title,
        MainMenu,
        Intro,                // Cinemática de introducción (playable parts + full reconstruction cinematic)
        Loading,              // Carga y Generación
        Playing,
        Paused,
        FloorTransition,
        Death,
        Reconstruction,       // Reconstrucción corta
        Victory,
        FinalDecision,
        EndingBasicRelief,    // Relevo básico
        EndingFullRelief,     // Relevo completo
        EndingAmbiguous,      // Ambiguo
        EndingAssimilation,   // Final: Asimilación (M-18)
        Credits,
        Quit,
    }

    /// <summary>Sub-states of Playing. Inventory and terminal are overlays, not sub-states.</summary>
    public enum PlaySubState
    {
        Exploration,
        Surveillance,
        Alert,     // Alerta/Combate: at least one enemy hunting
        Hacking,
    }
}
