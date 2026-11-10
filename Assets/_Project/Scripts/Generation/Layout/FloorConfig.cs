using System;
using Heartbeat.AI;
using UnityEngine;

namespace Heartbeat.Generation.Layout
{
    [Serializable]
    public struct IntRange
    {
        public int min, max;
        public IntRange(int min, int max) { this.min = min; this.max = max; }
        public int Pick(System.Random r) => r.Next(min, max + 1);
    }

    /// <summary>
    /// Composition and budget of one floor in one configuration (GDD 06 - Economía y Recursos, 02 - Niveles).
    /// Assets: Floors/FC_F1_Full, FC_F1_Showcase, … Values are initial, subject to balance.
    /// </summary>
    [CreateAssetMenu(menuName = "Heartbeat/Floor Config", fileName = "FC_")]
    public sealed class FloorConfig : ScriptableObject
    {
        [Range(1, 3)] public int floor = 1;
        [Header("Composition (D-33, D-34)")]
        public IntRange proceduralRooms = new IntRange(8, 12);
        public IntRange secondaryRooms = new IntRange(2, 3);
        [Tooltip("~1/3 of procedural rooms are empty of enemies (D-37).")] [Range(0f, 1f)] public float emptyFraction = 0.33f;
        [Tooltip("~1/3 of floors 1-2 start with the final room unpowered (D-72).")] [Range(0f, 1f)] public float unpoweredChance = 0.33f;
        [Tooltip("Chance to connect two facing open sockets (loops).")] [Range(0f, 1f)] public float loopChance = 0.35f;
        [Tooltip("Secondary key rooms are at least this many doors from the entry room (3 = two rooms in between).")]
        [Min(1)] public int minSecondaryDistance = 3;
        [Tooltip("The final key room is at least this many doors from the entry on the shortest route (D-100: 4 full, 3 showcase).")]
        [Min(1)] public int minFinalDistance = 4;
        [Tooltip("Rounds dropped next to the spawn point when the floor is built (outside the budget). Floor 1 only: Lukas wakes there every cycle.")]
        [Min(0)] public int startingAmmo = 0;
        [Tooltip("A tutorial exit opens by itself after this many seconds in the room, so nobody gets stuck (D-164).")]
        [Min(1f)] public float tutorialGateTimeout = 120f;

        [Header("Budget (D-17)")]
        public IntRange heals = new IntRange(2, 3);
        [Tooltip("Rounds; picked in blisters of blisterRounds.")] public IntRange ammo = new IntRange(12, 16);
        [Tooltip("Rounds in one ammo blister.")] [Min(1)] public int blisterRounds = 4;
        [Tooltip("Rounds in one ammo blister in Story mode; the number of blisters stays the same.")] [Min(1)] public int storyBlisterRounds = 6;
        public IntRange spikes = new IntRange(2, 3);
        public IntRange optionalKeys = new IntRange(1, 2);
        public IntRange enemies = new IntRange(6, 9);
        public EnemyType[] enemyTypes = { EnemyType.Alto, EnemyType.Rastrero };
        [Tooltip("Context documents and confiscated objects placed per attempt (D-73, D-79).")]
        public IntRange contextDocs = new IntRange(1, 2);
        public IntRange confiscated = new IntRange(1, 2);

        [Header("Tutorial (D-70)")]
        [Tooltip("Room ids used as tutorial rooms 2, 3, 4 on the first attempt of floor 1.")]
        public string[] tutorialRoomIds = new string[0];

        public FloorGenParams ToParams(int seed, bool firstAttempt, bool storyMode)
        {
            var p = new FloorGenParams
            {
                Floor = floor, Seed = seed, FirstAttempt = firstAttempt && floor == 1,
                Procedural = proceduralRooms, Secondary = secondaryRooms,
                EmptyFraction = emptyFraction, UnpoweredChance = floor < 3 ? unpoweredChance : 0f, LoopChance = loopChance, MinSecondaryDistance = minSecondaryDistance, MinFinalDistance = minFinalDistance,
                Heals = heals, Ammo = ammo, BlisterRounds = blisterRounds, StoryBlisterRounds = storyBlisterRounds, Spikes = spikes, OptionalKeys = optionalKeys, Enemies = enemies,
                EnemyTypes = enemyTypes, ContextDocs = contextDocs, Confiscated = confiscated,
                TutorialRoomIds = tutorialRoomIds,
            };
            if (storyMode) p.ApplyStoryMode();
            return p;
        }
    }

    /// <summary>Plain parameters for the pure generator (tests build these directly).</summary>
    public sealed class FloorGenParams
    {
        public int Floor = 1;
        public int Seed;
        public bool FirstAttempt;
        public IntRange Procedural = new IntRange(8, 12);
        public IntRange Secondary = new IntRange(2, 3);
        public float EmptyFraction = 0.33f;
        public float UnpoweredChance = 0.33f;
        public float LoopChance = 0.35f;
        public int MinSecondaryDistance = 3;
        public int MinFinalDistance = 4;
        public IntRange Heals = new IntRange(2, 3);
        public IntRange Ammo = new IntRange(12, 16);
        public int BlisterRounds = 4;
        public int StoryBlisterRounds = 6;
        public IntRange Spikes = new IntRange(2, 3);
        public IntRange OptionalKeys = new IntRange(1, 2);
        public IntRange Enemies = new IntRange(6, 9);
        public EnemyType[] EnemyTypes = { EnemyType.Alto, EnemyType.Rastrero };
        public IntRange ContextDocs = new IntRange(1, 2);
        public IntRange Confiscated = new IntRange(1, 2);
        public string[] TutorialRoomIds = new string[0];

        /// <summary>
        /// Story mode (D-103): +1 to each resource minimum (+1 blister for ammo); max rises if exceeded. Each blister then
        /// holds StoryBlisterRounds instead of BlisterRounds: same number of blisters, more rounds in each.
        /// </summary>
        public void ApplyStoryMode()
        {
            Heals = Raise(Heals, 1);
            Spikes = Raise(Spikes, 1);
            Ammo = Raise(Ammo, BlisterRounds);
            Ammo = new IntRange(Ammo.min / BlisterRounds * StoryBlisterRounds, Ammo.max / BlisterRounds * StoryBlisterRounds);
            BlisterRounds = StoryBlisterRounds;
        }

        private static IntRange Raise(IntRange r, int by) => new IntRange(r.min + by, Mathf.Max(r.max, r.min + by));
    }
}
