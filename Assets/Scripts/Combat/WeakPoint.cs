using System;
using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>
    /// Punto débil del enemigo. Un disparo aquí interrumpe su ataque o su agarre, pero no
    /// resta menos disparos para matarlo: el daño sigue siendo el del cuerpo.
    /// La IA se suscribe a <see cref="Hit"/> para cortar lo que estuviera haciendo.
    /// </summary>
    public class WeakPoint : MonoBehaviour
    {
        [Header("Depuración")]
        [SerializeField] private bool logHits = true;

        /// <summary>El punto débil recibió un disparo.</summary>
        public event Action<WeakPoint> Hit;

        /// <summary>El arma avisa del impacto en el punto débil.</summary>
        public void NotifyHit()
        {
            if (logHits) Debug.Log($"[PuntoDébil] Impacto en {name}.", this);
            Hit?.Invoke(this);
        }
    }
}
