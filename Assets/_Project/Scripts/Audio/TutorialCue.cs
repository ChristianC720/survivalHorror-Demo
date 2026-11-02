using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Audio
{
    /// <summary>
    /// A one-shot sound the tutorial plays when Lukas gets close (D-70: at the end of the long tunnel a Rastrero is heard
    /// behind a door). Placed by hand in the room prefab; <see cref="Generation.FloorBuilder"/> keeps it only in the
    /// fixed rooms of the first attempt.
    /// </summary>
    public sealed class TutorialCue : MonoBehaviour
    {
        [SerializeField] private string cue = "CUE_Rastrero_Behind";
        [Tooltip("Plays once when Lukas is this close (metres, flat distance).")]
        [SerializeField] private float radius = 4f;

        private Transform player;

        private void Update()
        {
            if (player == null)
            {
                if (!Services.TryGet(out Player.PlayerController pc)) return;
                player = pc.transform;
            }
            Vector3 d = player.position - transform.position;
            d.y = 0f;
            if (d.sqrMagnitude > radius * radius) return;
            Sfx.Play(cue, transform.position);
            HbLog.Info("Tutorial", $"{cue} at {name}");
            enabled = false;
        }
    }
}
