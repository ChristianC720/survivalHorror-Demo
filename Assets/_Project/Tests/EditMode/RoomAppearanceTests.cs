using System.Linq;
using Heartbeat.Generation;
using Heartbeat.Generation.Layout;
using NUnit.Framework;
using UnityEditor;

namespace Heartbeat.Tests
{
    /// <summary>C-46: the rooms that hold the Blender forklift, gantry crane and the two F2 capsules come up in real generated floors.</summary>
    public class RoomAppearanceTests
    {
        private const int Seeds = 200;

        [TestCase(1, "ROOM_F1_Storage_B"), TestCase(1, "ROOM_F1_Vertical_A"), TestCase(2, "ROOM_F2_Open_B"), TestCase(2, "ROOM_F2_Storage_B")]
        public void RoomAppearsWithinSeeds(int floor, string roomId)
        {
            var library = AssetDatabase.LoadAssetAtPath<RoomLibrary>("Assets/_Project/ScriptableObjects/Rooms/RoomLibrary.asset");
            var cfg = AssetDatabase.LoadAssetAtPath<FloorConfig>($"Assets/_Project/ScriptableObjects/Floors/FC_F{floor}_Full.asset");
            var templates = library.rooms.Where(r => r != null && r.floor == floor).Select(RoomTemplate.From).ToList();
            int hits = 0;
            for (int seed = 1; seed <= Seeds; seed++)
                if (LayoutGenerator.Generate(cfg.ToParams(seed, false, false), templates).Rooms.Any(r => r.Template.Id == roomId)) hits++;
            Assert.That(hits, Is.GreaterThan(0), $"{roomId} never appears in {Seeds} floor-{floor} layouts");
            UnityEngine.Debug.Log($"[Heartbeat][Test] {roomId}: {hits}/{Seeds} floor-{floor} layouts");
        }
    }
}
