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
            [Tooltip("Actors shown only during this shot (several shots share one set).")] public GameObject actors;
            [Tooltip("Slow camera drift, metres per second.")] public Vector3 drift;
        }

        [SerializeField] private Camera shotCamera;
        [SerializeField] private Shot[] shots = new Shot[0];
        [Header("Short reconstruction (D-39, D-43)")]
        [Tooltip("Lukas's suit in the capsule shot: shows the foreign piece of the current reconstruction.")]
        [SerializeField] private SkinnedMeshRenderer capsuleSuit;
        [SerializeField] private Texture2D[] bodyVariants = new Texture2D[0];

        /// <summary>Dresses the capsule actor for this subject code (one more foreign piece per reconstruction).</summary>
        public void SetSubject(int subjectCode)
        {
            if (capsuleSuit != null) Player.LukasAttachments.ApplyVariant(capsuleSuit, new MaterialPropertyBlock(), bodyVariants, subjectCode);
        }

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

        /// <summary>A skipped cinematic (D-91): camera off and every shot's actors hidden.</summary>
        public void StopAll()
        {
            shotCamera.enabled = false;
            foreach (var s in shots) if (s.actors != null) s.actors.SetActive(false);
        }

        /// <summary>Shows the shot for <paramref name="seconds"/> (unscaled), then switches the camera off again.</summary>
        public IEnumerator Play(string id, float seconds)
        {
            foreach (var s in shots)
            {
                if (s.id != id || s.pose == null) continue;
                shotCamera.transform.SetPositionAndRotation(s.pose.position, s.pose.rotation);
                shotCamera.enabled = true;
                if (s.actors != null) s.actors.SetActive(true);
                HbLog.Info("Cinematic", $"Shot {id}");
                for (float t = 0f; t < seconds; t += Time.unscaledDeltaTime)
                {
                    shotCamera.transform.position += s.drift * Time.unscaledDeltaTime;
                    yield return null;
                }
                shotCamera.enabled = false;
                if (s.actors != null) s.actors.SetActive(false);
                yield break;
            }
            HbLog.Warn("Cinematic", $"No shot '{id}'");
        }
    }
}
