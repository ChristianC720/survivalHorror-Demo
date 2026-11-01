namespace Heartbeat.AI
{
    /// <summary>Estados de la máquina de la IA enemiga.</summary>
    public enum EnemyState
    {
        Patrulla = 0,
        Sospecha = 1,
        Busqueda = 2,
        Caza = 3,
        Atacar = 4
    }

    public static class EnemyStateUtil
    {
        public static string DisplayName(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Sospecha: return "Sospecha";
                case EnemyState.Busqueda: return "Búsqueda";
                case EnemyState.Caza: return "Caza";
                case EnemyState.Atacar: return "Atacar";
                default: return "Patrulla";
            }
        }

        /// <summary>Prioridad de audio: el estado más alto gana cuando varios enemigos pitan a la vez.</summary>
        public static int Priority(EnemyState state)
        {
            return (int)state;
        }
    }
}
