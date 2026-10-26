using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>
    /// Estado de muerte de la sesión: muertes acumuladas y código de sujeto. Vive en
    /// estáticos porque la reaparición recarga la escena tal cual está montada, de modo
    /// que enemigos, objetos, llave, puertas y sellados vuelven a su estado inicial sin
    /// generación procedural; los contadores tienen que sobrevivir a esa recarga.
    /// Los números salen de <see cref="Heartbeat.Config.GameConfig"/>.
    /// </summary>
    public static class PlayerRespawn
    {
        /// <summary>Muertes acumuladas en la sesión (incluye "Reiniciar" del menú de pausa).</summary>
        public static int Deaths { get; private set; }

        /// <summary>La escena que está arrancando viene de una muerte: hay que presentarla en negro.</summary>
        public static bool IsRespawning { get; private set; }

        /// <summary>Suma una muerte y marca la siguiente escena como reaparición.</summary>
        public static void RegisterDeath()
        {
            Deaths++;
            IsRespawning = true;
        }

        /// <summary>La escena nueva ya se está presentando: el jugador vuelve a jugar.</summary>
        public static void ConsumeRespawn()
        {
            IsRespawning = false;
        }

        /// <summary>Sesión nueva: contadores a cero.</summary>
        public static void ResetSession()
        {
            Deaths = 0;
            IsRespawning = false;
        }

        /// <summary>Código de sujeto del cuerpo actual: R-H-07, R-H-08...</summary>
        public static string SubjectCode(int startNumber)
        {
            return $"R-H-{Mathf.Max(0, startNumber + Deaths):D2}";
        }
    }
}
