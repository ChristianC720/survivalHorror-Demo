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
        private static readonly Vector3 TerminalRest = new Vector3(-0.355f, 0.99f, -0.045f);

        [SerializeField] private Animator animator;
        [SerializeField] private PlayerBalance balance;
        [SerializeField] private Transform weaponSocket;
        [SerializeField] private PlayerHealth health;
        [SerializeField] private TMP_FontAsset terminalFont;
        [Tooltip("The team's terminal model, worn on the left forearm. The screen text sits on its panel.")]
        [SerializeField] private GameObject terminalModel;
        [Tooltip("Intro: civilian clothes, nothing luminous, no terminal (GDD Personajes).")]
        [SerializeField] private bool civilian;
        [Tooltip("D-43: one more foreign piece per reconstruction, cumulative. Index 0 is the second reconstruction.")]
        [SerializeField] private Texture2D[] bodyVariants;

        private TextMeshPro screen;
        private int shownState = -1;

        private const int FirstSubjectCode = 7;   // R-H-07 still has his own body
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private SkinnedMeshRenderer suit;
        private MaterialPropertyBlock block;

        private void OnEnable() => GameEvents.Reconstructed += ApplyBody;
        private void OnDisable() => GameEvents.Reconstructed -= ApplyBody;

        private void Start()
        {
            if (!civilian && Services.TryGet<Cycle.CycleController>(out var cycle) && cycle.Data != null) ApplyBody(cycle.Data.subjectCode);
        }

        private void ApplyBody(int subjectCode)
        {
            if (civilian || suit == null || bodyVariants == null || bodyVariants.Length == 0) return;
            int index = subjectCode - FirstSubjectCode - 1;
            block ??= new MaterialPropertyBlock();
            suit.GetPropertyBlock(block);
            if (index >= 0) block.SetTexture(BaseMap, bodyVariants[Mathf.Min(index, bodyVariants.Length - 1)]);
            else block.Clear();
            suit.SetPropertyBlock(block);
        }

        private void Awake()
        {
            foreach (var r in animator.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                bool isCivil = r.name.EndsWith("_Civil");
                bool isHair = r.name.EndsWith("_Hair");   // the same hair in both outfits
                r.enabled = isHair || isCivil == civilian;
                if (!isCivil && !isHair) suit = r;
            }
            if (civilian) return;

            Transform hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            Transform forearm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            Transform chest = animator.GetBoneTransform(HumanBodyBones.Chest);

            if (weaponSocket != null && hand != null)
            {
                // The team's grip: three markers exported on the hand give origin, barrel and top of the gun.
                Transform grip = hand.Find("WeaponGrip"), toward = hand.Find("WeaponGrip_Barrel"), up = hand.Find("WeaponGrip_Up");
                weaponSocket.SetParent(hand, true);
                weaponSocket.SetPositionAndRotation(grip.position, Quaternion.LookRotation(toward.position - grip.position, up.position - grip.position));
            }

            Vector3 panel = animator.transform.TransformPoint(TerminalRest);
            AddGlow("Glow_Chest", chest, chest.position + animator.transform.up * 0.1f + animator.transform.forward * 0.2f);
            AddGlow("Glow_Terminal", forearm, panel);

            if (terminalModel != null)
            {
                var device = Instantiate(terminalModel, forearm, true);
                // The device's screen faces -Z as imported; turn it out of the arm with its long side along the forearm.
                device.transform.SetPositionAndRotation(panel, Quaternion.LookRotation(-animator.transform.right, animator.transform.up));
            }

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
            if (civilian) return;
            int s = (int)health.State;
            if (s == shownState) return;
            shownState = s;
            // Same diegetic labels as the terminal (D-86); not localized because the panel cannot be read at this size.
            screen.text = "R-H\n" + health.State.ToString().ToUpperInvariant();
        }
    }
}
