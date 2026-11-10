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
        // Terminal panel on the outer side of the left forearm, measured on the first mesh (BLENDER_PLAN.md): 53% of the way
        // from elbow to wrist, 4.6 cm out from the bone. Read from the bones, so any rest pose of the rig works.
        private const float TerminalAlong = 0.53f, TerminalOut = 0.046f;

        /// <summary>Bind-pose panel centre, the direction out of the arm and the direction up the forearm. Shared with the gallery.</summary>
        public static Vector3 TerminalPanel(Transform root, Transform forearm, Transform hand, out Vector3 outward, out Vector3 along)
        {
            along = (forearm.position - hand.position).normalized;
            outward = Vector3.ProjectOnPlane(-root.right, along).normalized;
            return Vector3.Lerp(forearm.position, hand.position, TerminalAlong) + outward * TerminalOut;
        }

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
            block ??= new MaterialPropertyBlock();
            ApplyVariant(suit, block, bodyVariants, subjectCode);
        }

        /// <summary>D-43: the body texture for a subject code (R-H-07 keeps his own body); shared with the capsule shot.</summary>
        public static void ApplyVariant(SkinnedMeshRenderer suit, MaterialPropertyBlock block, Texture2D[] variants, int subjectCode)
        {
            int index = subjectCode - FirstSubjectCode - 1;
            suit.GetPropertyBlock(block);
            if (index >= 0 && variants.Length > 0) block.SetTexture(BaseMap, variants[Mathf.Min(index, variants.Length - 1)]);
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

            Vector3 panel = TerminalPanel(animator.transform, forearm, animator.GetBoneTransform(HumanBodyBones.LeftHand), out Vector3 outward, out Vector3 along);
            AddGlow("Glow_Chest", chest, chest.position + animator.transform.up * 0.1f + animator.transform.forward * 0.2f);
            AddGlow("Glow_Terminal", forearm, panel);

            if (terminalModel != null)
            {
                var device = Instantiate(terminalModel, forearm, true);
                // The device's screen faces -Z as imported; turn it out of the arm with its long side along the forearm.
                device.transform.SetPositionAndRotation(panel, Quaternion.LookRotation(outward, along));
            }

            var go = new GameObject("ArmTerminalScreen");
            go.transform.SetParent(forearm, true);
            // The panel faces out of the left arm; TMP shows its face to a viewer looking along +Z.
            go.transform.SetPositionAndRotation(panel, Quaternion.LookRotation(-outward, along));
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
