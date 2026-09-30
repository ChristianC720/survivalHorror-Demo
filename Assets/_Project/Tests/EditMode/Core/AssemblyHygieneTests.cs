using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Compilation;

namespace Project.Tests.EditMode.Core
{
    /// <summary>
    /// Guards the assembly-definition boundaries established by Spec 01.
    /// </summary>
    public sealed class AssemblyHygieneTests
    {
        private const string RuntimeAssembly = "Project.Runtime";
        private const string EditorAssembly = "Project.Editor";
        private const string EditModeTestsAssembly = "Project.Tests.EditMode";
        private const string PlayModeTestsAssembly = "Project.Tests.PlayMode";
        private const string InputSystemTestFramework = "Unity.InputSystem.TestFramework";

        [Test]
        public void RuntimeAssembly_IsCompiledForPlayer()
        {
            Assert.That(FindAssembly(AssembliesType.PlayerWithoutTestAssemblies, RuntimeAssembly), Is.Not.Null);
        }

        [Test]
        public void RuntimeAssembly_PlayerBuild_DoesNotReferenceUnityEditor()
        {
            var runtime = FindAssembly(AssembliesType.PlayerWithoutTestAssemblies, RuntimeAssembly);
            Assert.That(runtime, Is.Not.Null);

            var editorReferences = runtime.compiledAssemblyReferences
                .Select(System.IO.Path.GetFileNameWithoutExtension)
                .Concat(runtime.assemblyReferences.Select(reference => reference.name))
                .Where(name => name.StartsWith("UnityEditor", StringComparison.Ordinal)
                               || name == EditorAssembly)
                .ToArray();

            Assert.That(editorReferences, Is.Empty,
                $"{RuntimeAssembly} must not reference editor assemblies: {string.Join(", ", editorReferences)}");
        }

        [Test]
        public void EditorAssembly_IsEditorOnly()
        {
            Assert.That(FindAssembly(AssembliesType.PlayerWithoutTestAssemblies, EditorAssembly), Is.Null,
                $"{EditorAssembly} must not be compiled into player builds.");

            var editor = FindAssembly(AssembliesType.Editor, EditorAssembly);
            Assert.That(editor, Is.Not.Null);
            Assert.That(editor.flags.HasFlag(AssemblyFlags.EditorAssembly), Is.True);
        }

        [TestCase(EditModeTestsAssembly)]
        [TestCase(PlayModeTestsAssembly)]
        public void TestAssembly_ReferencesRuntime(string testAssemblyName)
        {
            var testAssembly = FindAssembly(AssembliesType.Editor, testAssemblyName);
            Assert.That(testAssembly, Is.Not.Null, $"{testAssemblyName} was not found.");

            Assert.That(testAssembly.assemblyReferences.Any(reference => reference.name == RuntimeAssembly), Is.True,
                $"{testAssemblyName} must reference {RuntimeAssembly}.");
        }

        [TestCase(RuntimeAssembly)]
        [TestCase(EditorAssembly)]
        [TestCase(EditModeTestsAssembly)]
        public void InputSystemTestFramework_IsReferencedOnlyByPlayModeTests(string assemblyName)
        {
            var assembly = FindAssembly(AssembliesType.Editor, assemblyName);
            Assert.That(assembly, Is.Not.Null, $"{assemblyName} was not found.");

            Assert.That(assembly.assemblyReferences.Any(reference => reference.name == InputSystemTestFramework), Is.False,
                $"{assemblyName} must not reference {InputSystemTestFramework}.");
        }

        [Test]
        public void PlayModeTests_ReferenceInputSystemTestFramework()
        {
            var assembly = FindAssembly(AssembliesType.Editor, PlayModeTestsAssembly);

            Assert.That(assembly.assemblyReferences.Any(reference => reference.name == InputSystemTestFramework), Is.True);
        }

        private static Assembly FindAssembly(AssembliesType type, string name)
        {
            return CompilationPipeline.GetAssemblies(type).FirstOrDefault(assembly => assembly.name == name);
        }
    }
}
