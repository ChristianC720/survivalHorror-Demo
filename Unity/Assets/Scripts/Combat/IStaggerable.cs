using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>Puede ser apartado por un empujón.</summary>
    public interface IStaggerable
    {
        /// <summary>Aparta al receptor en <paramref name="direction"/> con la fuerza indicada.</summary>
        void ApplyStagger(Vector3 direction, float strength);
    }
}
