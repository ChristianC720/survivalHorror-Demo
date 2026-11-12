using System;
using System.Collections;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Save;
using UnityEngine;

namespace Heartbeat.Narrative
{
    /// <summary>
    /// D-91: a narrative cinematic (full reconstruction, Assimilation, victory sequence, Reliefs, Ambiguous and the hook)
    /// can be skipped by holding Skip ~1 s, only once it has been seen. The seen list is global (Options data), so
    /// New game never clears it. Needs the Cinematic input context.
    /// </summary>
    public static class SkippableCinematic
    {
        public const float HoldSeconds = 1f;

        /// <summary>Playtests (C-65): counts as holding Skip for the whole hold; cleared once a cinematic is skipped.</summary>
        public static bool DebugSkip;

        /// <summary>Runs <paramref name="body"/> on <paramref name="host"/>; a skip stops it and calls <paramref name="onSkipped"/>.</summary>
        public static IEnumerator Run(MonoBehaviour host, string id, IEnumerator body, Action onSkipped = null)
        {
            var saves = Services.Get<SaveService>();
            bool canSkip = saves.LoadGlobal().seenCinematics.Contains(id);
            bool done = false;
            var routine = host.StartCoroutine(Until(body, () => done = true));
            var skip = Services.Get<InputRouter>().Actions.Cinematic.Skip;
            float held = 0f;
            while (!done)
            {
                held = canSkip && (skip.IsPressed() || DebugSkip) ? held + Time.unscaledDeltaTime : 0f;
                if (held >= HoldSeconds)
                {
                    host.StopCoroutine(routine);
                    DebugSkip = false;
                    HbLog.Info("Cinematic", $"{id} skipped (held {HoldSeconds:0.#} s)");
                    onSkipped?.Invoke();
                    break;
                }
                yield return null;
            }
            var g = saves.LoadGlobal();
            if (!g.seenCinematics.Contains(id)) { g.seenCinematics.Add(id); saves.SaveGlobal(g); }
        }

        private static IEnumerator Until(IEnumerator body, Action end)
        {
            yield return body;
            end();
        }
    }
}
