namespace Heartbeat.AI
{
    /// <summary>Tipo de enemigo; cada uno toma sus cifras del perfil correspondiente de <c>GameConfig</c>.</summary>
    public enum EnemyType
    {
        /// <summary>Alto: cápsula de 2.5 m, visión larga, golpe de alcance largo y agarre.</summary>
        Alto = 0,

        /// <summary>Rastrero: cápsula baja y aplastada, rápido, flanquea y no agarra.</summary>
        Rastrero = 1
    }

    public static class EnemyTypeUtil
    {
        public static string DisplayName(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Rastrero: return "Rastrero";
                default: return "Alto";
            }
        }
    }
}
