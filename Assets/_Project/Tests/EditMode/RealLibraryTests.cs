using System.Linq;
using Heartbeat.Generation;
using Heartbeat.Generation.Layout;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>Runs the generator on the real hand-built room library and floor configs (1000 seeds each).</summary>
    public class RealLibraryTests
    {
        private static RoomLibrary Lib => AssetDatabase.LoadAssetAtPath<RoomLibrary>("Assets/_Project/ScriptableObjects/Rooms/RoomLibrary.asset");
        private static FloorConfig Cfg(string n) => AssetDatabase.LoadAssetAtPath<FloorConfig>($"Assets/_Project/ScriptableObjects/Floors/{n}.asset");

        [TestCase("FC_F1_Full", false)]
        [TestCase("FC_F1_Full", true)]
        [TestCase("FC_F1_Showcase", false)]
        [TestCase("FC_F1_Showcase", true)]
        public void Floor1_RealRooms_1000Seeds(string config, bool firstAttempt)
        {
            var lib = Lib;
            var cfg = Cfg(config);
            Assume.That(lib != null && cfg != null, "room library / floor config assets missing");
            var templates = lib.rooms.Where(r => r.floor == 1).Select(RoomTemplate.From).ToList();
            int worst = 0;
            for (int seed = 0; seed < 1000; seed++)
            {
                var p = cfg.ToParams(seed * 977, firstAttempt, storyMode: false);
                var layout = LayoutGenerator.Generate(p, templates);
                Assert.IsNotNull(layout, $"seed {p.Seed}");
                Assert.IsTrue(LayoutValidator.Validate(layout, p, out var errors), string.Join("; ", errors));
                worst = Mathf.Max(worst, layout.Attempts);
            }
            Debug.Log($"[Heartbeat][Gen] {config} first={firstAttempt}: 1000 seeds OK, worst case {worst} attempts");
        }
    }
}
