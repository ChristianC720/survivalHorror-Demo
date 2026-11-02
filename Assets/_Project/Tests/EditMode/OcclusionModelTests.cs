using Heartbeat.Core;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public sealed class OcclusionModelTests
    {
        [Test] public void Clear_line_is_full_volume() => Assert.AreEqual(1f, OcclusionModel.Factor(0, 0, 0.3f, 0.5f), 1e-5f);
        [Test] public void One_wall_cuts_seventy_percent() => Assert.AreEqual(0.3f, OcclusionModel.Factor(1, 0, 0.3f, 0.5f), 1e-5f);
        [Test] public void One_closed_door_cuts_half() => Assert.AreEqual(0.5f, OcclusionModel.Factor(0, 1, 0.3f, 0.5f), 1e-5f);
        [Test] public void Walls_and_doors_stack() => Assert.AreEqual(0.3f * 0.3f * 0.5f, OcclusionModel.Factor(2, 1, 0.3f, 0.5f), 1e-5f);
        [Test] public void Negative_counts_are_ignored() => Assert.AreEqual(1f, OcclusionModel.Factor(-1, -2, 0.3f, 0.5f), 1e-5f);
    }
}
