using Heartbeat.Core;
using Heartbeat.Narrative;
using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>
    /// Floor scene entry point. Builds the requested floor on Start. The death cycle and save system (E-08) set
    /// <see cref="Pending"/> before loading the Floor scene; pressing Play in the Floor scene directly uses the
    /// inspector values (seed 0 = random) — the seed is always logged so bugs can be reproduced.
    /// </summary>
    public sealed class FloorSession : MonoBehaviour
    {
        public struct Request
        {
            public int Floor, Seed;
            public bool FirstAttempt, StoryMode, Showcase, StartInFinalRoom;
        }

        /// <summary>Set by the cycle / save system before loading this scene.</summary>
        public static Request? Pending;

        [SerializeField] private FloorBuilder builder;
        [SerializeField, Range(1, 3)] private int floor = 1;
        [SerializeField] private int seed;
        [SerializeField] private bool firstAttempt;
        [SerializeField] private bool storyMode;
        [SerializeField] private bool showcase;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => Pending = null;

        private void Start()
        {
            var r = Pending ?? new Request
            {
                Floor = floor,
                Seed = seed != 0 ? seed : Random.Range(1, int.MaxValue / 2),
                FirstAttempt = firstAttempt,
                StoryMode = storyMode,
                Showcase = showcase,
            };
            Pending = null;
            Services.TryGet<RegistryModel>(out var registry);
            builder.Build(r.Floor, r.Seed, r.FirstAttempt, r.StoryMode, r.Showcase, registry != null ? new System.Collections.Generic.HashSet<string>(registry.Order) : null, r.StartInFinalRoom);
        }

        /// <summary>Debug console / tests: rebuild with a given seed.</summary>
        public void DebugRebuild(int newSeed, int newFloor)
        {
            builder.Build(newFloor, newSeed, false, storyMode, showcase);
        }
    }
}
