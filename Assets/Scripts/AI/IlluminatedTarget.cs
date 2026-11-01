using UnityEngine;

namespace Heartbeat.AI
{
    /// <summary>
    /// Marca un objeto como detectable por la linterna del jugador.
    /// La IA se suscribe a <see cref="Heartbeat.Player.Flashlight.EnemyIlluminated"/>.
    /// </summary>
    public class IlluminatedTarget : MonoBehaviour
    {
        [Tooltip("Sólo los enemigos disparan el evento de la IA.")]
        [SerializeField] private bool isEnemy = true;

        public bool IsEnemy => isEnemy;
    }
}
