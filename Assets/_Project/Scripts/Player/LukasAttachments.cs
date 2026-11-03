using Heartbeat.Core;
using TMPro;
using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>
    /// Puts what rides on Lukas's skeleton in place: the revolver in the right hand, the arm terminal screen on the left
    /// forearm, and the faint teal glow of the seams and terminal (D-152). The lights touch no gameplay system.
    /// </summary>
    public sealed class LukasAttachments : MonoBehaviour
    {
        // Rest-pose center of the terminal panel in model space, measured from the Blender mesh (BLENDER_PLAN.md).
        private static readonly Vector3 TerminalRest = new Vector3(-0.363f, 0.99f, -0.049f);

        [SerializeField] private Animator animator;
        [SerializeField] private PlayerBalance balance;
        [SerializeField] private Transform weaponSocket;
        [SerializeField] private PlayerHealth health;
        [SerializeField] private TMP_FontAsset terminalFont;

        private TextMeshPro screen;
        private int shownState = -1;

        private void Awake()
        {
            Transform hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            Transform fingers = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
            Transform forearm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            Transform chest = animator.GetBoneTransform(HumanBodyBones.Chest);

            if (weaponSocket != null && hand != null && fingers != null)
            {
                // Arms hang in the bind pose: the barrel follows the fingers, the sights face the way Lukas looks.
                Vector3 barrel = (fingers.position - hand.position).normalized;
                weaponSocket.SetParent(hand, true);
                weaponSocket.SetPositionAndRotation(hand.position + barrel * 0.07f, Quaternion.LookRotation(barrel, animator.transform.forward));
            }

            Vector3 panel = animator.transform.TransformPoint(TerminalRest);
            AddGlow("Glow_Chest", chest, chest.position + animator.transform.up * 0.1f + animator.transform.forward * 0.2f);
            AddGlow("Glow_Terminal", forearm, panel);

            var go = new GameObject("ArmTerminalScreen");
            go.transform.SetParent(forearm, true);
            // The panel faces out of the left arm, -X in the bind pose; TMP shows its face to a viewer looking along +Z.
            go.transform.SetPositionAndRotation(panel, Quaternion.LookRotation(animator.transform.right, animator.transform.up));
            screen = go.AddComponent<TextMeshPro>();
            if (terminalFont != null) screen.font = terminalFont;
            screen.rectTransform.sizeDelta = new Vector2(0.06f, 0.05f);
            screen.fontSize = 0.12f;
            screen.alignment = TextAlignmentOptions.Center;
            screen.textWrappingMode = TextWrappingModes.NoWrap;
            screen.color = new Color(0.431f, 0.624f, 0.627f);
        }

        private void AddGlow(string objectName, Transform bone, Vector3 at)
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(bone, true);
            go.transform.position = at;
            var l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = balance.glowRange;
            l.intensity = balance.glowIntensity;
            l.color = balance.glowColor;
            l.shadows = LightShadows.None;
            l.renderMode = LightRenderMode.ForcePixel;
        }

        private void Update()
        {
            int s = (int)health.State;
            if (s == shownState) return;
            shownState = s;
            // Same diegetic labels as the terminal (D-86); not localized because the panel cannot be read at this size.
            screen.text = "R-H\n" + health.State.ToString().ToUpperInvariant();
        }
    }
}
