using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Compilation;

namespace Heartbeat.Tests
{
    /// <summary>The asset gallery and its developer options must never reach an executable.</summary>
    public class SandboxIsolationTests
    {
        [Test]
        public void NoSandboxSceneIsInTheBuild()
        {
            Assert.That(EditorBuildSettings.scenes.Where(s => s.path.Contains("/Sandbox_")).Select(s => s.path), Is.Empty);
        }

        [Test]
        public void SandboxCodeIsNotCompiledForPlayers()
        {
            var player = CompilationPipeline.GetAssemblies(AssembliesType.Player).Select(a => a.name).ToArray();
            Assert.That(player, Does.Contain("Heartbeat.Runtime"), "the query must list player assemblies");
            Assert.That(player, Does.Not.Contain("Heartbeat.Sandbox"));
        }

        [Test]
        public void GameCodeDoesNotReferenceTheSandbox()
        {
            var runtime = CompilationPipeline.GetAssemblies(AssembliesType.Player).First(a => a.name == "Heartbeat.Runtime");
            Assert.That(runtime.assemblyReferences.Select(a => a.name), Does.Not.Contain("Heartbeat.Sandbox"));
        }
    }
}
