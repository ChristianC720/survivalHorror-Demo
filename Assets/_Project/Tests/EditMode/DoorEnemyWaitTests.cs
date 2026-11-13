using Heartbeat.Core;
using Heartbeat.Level;
using NUnit.Framework;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>C-68: a search/hunt enemy at a closed, unlocked door waits 5 s before it opens it.</summary>
    public sealed class DoorEnemyWaitTests
    {
        private const float Delay = 5f;

        [Test]
        public void Balance_DefaultDelay_IsFiveSeconds()
        {
            var b = ScriptableObject.CreateInstance<SystemsBalance>();
            Assert.AreEqual(Delay, b.enemyDoorOpenDelaySeconds);
            Object.DestroyImmediate(b);
        }

        [Test]
        public void StillClosedAt4_9Seconds_OpensAt5()
        {
            var w = new DoorEnemyWait();
            Assert.IsFalse(w.Tick(4.9f, true, Delay));
            Assert.IsTrue(w.Tick(0.1f, true, Delay));
        }

        [Test]
        public void FrameByFrame_OpensOnlyAtFiveSeconds()
        {
            var w = new DoorEnemyWait();
            const float dt = 1f / 60f;
            int frames = 0;
            while (!w.Tick(dt, true, Delay)) frames++;
            Assert.AreEqual(5f, (frames + 1) * dt, dt);
        }

        [Test]
        public void EnemyBackToPatrol_CancelsTheWait()
        {
            var w = new DoorEnemyWait();
            Assert.IsFalse(w.Tick(4f, true, Delay));
            Assert.IsFalse(w.Tick(0.1f, false, Delay), "back to patrol: no enemy waits, the door stays shut");
            Assert.AreEqual(0f, w.Elapsed);
            Assert.IsFalse(w.Tick(4.9f, true, Delay), "a new wait starts from zero");
            Assert.IsTrue(w.Tick(0.1f, true, Delay));
        }

        [Test]
        public void Reset_ByLukasOpeningOrSealing_StartsOver()
        {
            var w = new DoorEnemyWait();
            w.Tick(3f, true, Delay);
            w.Reset();
            Assert.IsFalse(w.Tick(4.9f, true, Delay));
        }
    }
}
