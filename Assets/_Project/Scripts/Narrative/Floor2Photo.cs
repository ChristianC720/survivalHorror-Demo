using System.Collections;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Narrative
{
    /// <summary>
    /// The floor-2 finding (D-56, D-67): the first time Lukas reaches floor 2's final room (after any hunt, like the
    /// autosave) a short, unskippable shot shows the family photo from the house, Emilia's face blurred, impossible
    /// and never explained. It brings obstinacy, not hope; the counter reset itself lives in the cycle (D-35).
    /// Placed by hand in KEY_F2_Final; <see cref="Cycle.CycleController"/> plays it once per slot.
    /// </summary>
    public sealed class Floor2Photo : MonoBehaviour
    {
        [SerializeField] private Camera shotCamera;
        [SerializeField] private float seconds = 5f;

        private void OnEnable() => Services.Register(this);
        private void OnDisable() => Services.Unregister(this);

        private void Awake()
        {
            if (shotCamera != null) shotCamera.enabled = false;
        }

        public IEnumerator Play()
        {
            var loader = Services.Get<SceneLoader>();
            Services.TryGet(out Player.PlayerController player);
            if (player != null) player.MovementLocked = true;
            HbLog.Info("Narrative", "Floor 2 finding: the family photo (D-56)");
            yield return loader.Fade(0f, 1f);
            shotCamera.enabled = true;
            yield return loader.Fade(1f, 0f);
            for (float t = 0f; t < 1.2f; t += Time.deltaTime) yield return null;
            Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.PHOTO_FLOOR2");
            for (float t = 0f; t < seconds; t += Time.deltaTime) yield return null;
            yield return loader.Fade(0f, 1f);
            shotCamera.enabled = false;
            if (player != null) player.MovementLocked = false;
            yield return loader.Fade(1f, 0f);
        }
    }
}
