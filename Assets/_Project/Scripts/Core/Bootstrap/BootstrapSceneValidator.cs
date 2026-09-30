using System;
using System.Collections.Generic;
using System.IO;

namespace Project.Core.Bootstrap
{
    /// <summary>
    /// Validates the bootstrap entry-flow configuration against the build scene list.
    /// Pure logic with no Unity dependencies so it can be tested in EditMode.
    /// </summary>
    public sealed class BootstrapSceneValidator
    {
        /// <summary>Build index the bootstrap scene must occupy.</summary>
        public const int RequiredBootstrapBuildIndex = 0;

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> when the configuration cannot produce a valid entry flow.
        /// </summary>
        /// <param name="nextSceneName">Scene name (without path or extension) loaded after bootstrap.</param>
        /// <param name="buildScenePaths">Enabled build scene paths, ordered by build index.</param>
        /// <param name="bootstrapBuildIndex">Build index of the scene that contains the bootstrap.</param>
        public void Validate(string nextSceneName, IReadOnlyList<string> buildScenePaths, int bootstrapBuildIndex)
        {
            if (buildScenePaths == null)
            {
                throw new ArgumentNullException(nameof(buildScenePaths));
            }

            if (bootstrapBuildIndex != RequiredBootstrapBuildIndex)
            {
                throw new InvalidOperationException(
                    $"Bootstrap scene must be at build index {RequiredBootstrapBuildIndex}, but is at {bootstrapBuildIndex}.");
            }

            if (string.IsNullOrWhiteSpace(nextSceneName))
            {
                throw new InvalidOperationException("Bootstrap next scene name is not configured.");
            }

            var bootstrapSceneName = bootstrapBuildIndex < buildScenePaths.Count
                ? Path.GetFileNameWithoutExtension(buildScenePaths[bootstrapBuildIndex])
                : null;

            if (string.Equals(nextSceneName, bootstrapSceneName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Bootstrap next scene '{nextSceneName}' must not be the bootstrap scene itself.");
            }

            for (var i = 0; i < buildScenePaths.Count; i++)
            {
                if (string.Equals(Path.GetFileNameWithoutExtension(buildScenePaths[i]), nextSceneName, StringComparison.Ordinal))
                {
                    return;
                }
            }

            throw new InvalidOperationException(
                $"Bootstrap next scene '{nextSceneName}' does not exist or is not in the build scene list.");
        }
    }
}
