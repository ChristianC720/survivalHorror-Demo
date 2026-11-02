using System.Collections;
using System.Collections.Generic;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Narrative
{
    /// <summary>
    /// Staged 3D shots of the Cinematics scene (loaded by the ending director). Builds its kit shells on start and shows
    /// a named shot through its own camera: e.g. "Assimilation" = Lukas, now a Mudo, standing far away on the floor that
    /// does not exist, the decoy for the next arrival (D-61). Every set is placed by hand in the scene.
    /// </summary>
    public sealed class CinematicStage : MonoBehaviour
    {
        [System.Serializable]
        public struct Shot
        {
            public string id;
            public Transform pose;
        }

        [SerializeField] private Camera shotCamera;
        [SerializeField] private Shot[] shots = new Shot[0];

        private void OnEnable() => Services.Register(this);
        private void OnDisable() => Services.Unregister(this);

        private void Start()
        {
            shotCamera.enabled = false;
            foreach (var shell in GetComponentsInChildren<Generation.RoomShell>(true))
            {
                var open = new List<Generation.SocketData>();
                foreach (var s in shell.GetComponentsInChildren<Generation.DoorSocket>(true))
                    open.Add(new Generation.SocketData { cell = s.LocalCell(shell.transform, shell.size), facing = s.LocalFacing(shell.transform) });
                shell.Build(open);
            }
        }

        /// <summary>Shows the shot for <paramref name="seconds"/> (unscaled), then switches the camera off again.</summary>
        public IEnumerator Play(string id, float seconds)
        {
            foreach (var s in shots)
            {
                if (s.id != id || s.pose == null) continue;
                shotCamera.transform.SetPositionAndRotation(s.pose.position, s.pose.rotation);
                shotCamera.enabled = true;
                HbLog.Info("Cinematic", $"Shot {id}");
                for (float t = 0f; t < seconds; t += Time.unscaledDeltaTime) yield return null;
                shotCamera.enabled = false;
                yield break;
            }
            HbLog.Warn("Cinematic", $"No shot '{id}'");
        }
    }
}
