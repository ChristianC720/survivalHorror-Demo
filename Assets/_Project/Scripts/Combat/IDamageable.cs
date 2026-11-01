using UnityEngine;

namespace Heartbeat.Combat
{
    public readonly struct ShotHit
    {
        public readonly Vector3 Point;
        public readonly Vector3 Direction;
        public readonly bool WeakPoint;

        public ShotHit(Vector3 point, Vector3 direction, bool weakPoint)
        {
            Point = point;
            Direction = direction;
            WeakPoint = weakPoint;
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
