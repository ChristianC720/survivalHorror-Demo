using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>Recibe daño de un disparo o de un remate.</summary>
    public interface IDamageable
    {
        /// <summary>Aplica el daño indicado en el punto de impacto.</summary>
        void ApplyDamage(int amount, Vector3 point);
    }
}
