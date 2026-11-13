using UnityEngine;

namespace Heartbeat.Combat
{
    public readonly struct ShotHit
    {
        public readonly Vector3 Point;
        public readonly Vector3 Direction;
        public readonly bool WeakPoint;
        /// <summary>Damage in normal-shot units: 1 for a body hit, more for a weak point (C-74).</summary>
        public readonly float Damage;

        public ShotHit(Vector3 point, Vector3 direction, bool weakPoint, float damage = 1f)
        {
            Point = point;
            Direction = direction;
            WeakPoint = weakPoint;
            Damage = damage;
        }
    }

    /// <summary>Anything a revolver round can hit: enemies, the boss's transformers, test dummies.</summary>
    public interface IDamageable
    {
        bool IsAlive { get; }
        void ReceiveShot(in ShotHit hit);
    }

    /// <summary>Shove / butt strike target (M-06).</summary>
    public interface IStaggerable
    {
        void Stagger(Vector3 from, float seconds);
    }
}
