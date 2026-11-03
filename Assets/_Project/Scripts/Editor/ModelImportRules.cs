using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Import rules for every Blender export under Assets/_Project/Models (GDD Personajes y Animacion, D-151):
    /// point-filter textures without mipmaps, no imported materials, 30 fps clips named from their Blender action.
    /// </summary>
    public sealed class ModelImportRules : AssetPostprocessor
    {
        private const string Root = "Assets/_Project/Models/";
        private static readonly string[] Looping = { "Idle", "Walk", "Run", "Crawl", "Patrol", "Hunt", "Wander", "Hack", "Still", "Aim", "Reload", "Hold", "Struggle", "Lifted", "Flank" };

        private static readonly (string human, string bone)[] Extra =
        {
            ("Chest", "Chest"),
            ("Left Thumb Proximal", "LeftThumbProximal"), ("Left Thumb Distal", "LeftThumbDistal"),
            ("Left Index Proximal", "LeftIndexProximal"), ("Left Index Distal", "LeftIndexDistal"),
            ("Left Middle Proximal", "LeftFingers"),
            ("Right Thumb Proximal", "RightThumbProximal"), ("Right Thumb Distal", "RightThumbDistal"),
            ("Right Index Proximal", "RightIndexProximal"), ("Right Index Distal", "RightIndexDistal"),
            ("Right Middle Proximal", "RightFingers"),
        };

        /// <summary>
        /// Makes an FBX on the shared skeleton a Humanoid: adds the bones Unity's auto-mapping skips and stores a T-pose
        /// (the Blender rest pose has the arms down, which would skew every retargeted clip). Idempotent.
        /// </summary>
        public static string ConfigureHumanoid(string path)
        {
            var m = (ModelImporter)AssetImporter.GetAtPath(path);
            m.animationType = ModelImporterAnimationType.Human;
            m.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            m.SaveAndReimport();

            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var go = Object.Instantiate(asset);
            try
            {
                var all = go.GetComponentsInChildren<Transform>(true);
                Transform Bone(string n) => all.FirstOrDefault(t => t.name == n);
                void Aim(string bone, string tip, Vector3 dir)
                {
                    Transform b = Bone(bone), t = Bone(tip);
                    if (b == null || t == null) return;
                    b.rotation = Quaternion.FromToRotation(t.position - b.position, dir) * b.rotation;
                }
                foreach (var (side, x) in new[] { ("Left", -1f), ("Right", 1f) })
                {
                    Aim(side + "UpperArm", side + "LowerArm", new Vector3(x, 0, 0));
                    Aim(side + "LowerArm", side + "Hand", new Vector3(x, 0, 0));
                    Aim(side + "UpperLeg", side + "LowerLeg", Vector3.down);
                    Aim(side + "LowerLeg", side + "Foot", Vector3.down);
                }

                var d = m.humanDescription;
                var human = d.human.Where(h => Extra.All(e => e.human != h.humanName)).ToList();
                foreach (var (hn, bn) in Extra)
                    if (Bone(bn) != null)
                        human.Add(new HumanBone { humanName = hn, boneName = bn, limit = new HumanLimit { useDefaultValues = true } });
                d.human = human.ToArray();
                d.skeleton = all.Select(t => new SkeletonBone { name = t == go.transform ? asset.name : t.name, position = t.localPosition, rotation = t.localRotation, scale = t.localScale }).ToArray();
                m.humanDescription = d;
                m.SaveAndReimport();
            }
            finally { Object.DestroyImmediate(go); }

            var avatar = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Avatar>().FirstOrDefault();
            return $"{path}: avatar valid={avatar != null && avatar.isValid} human={avatar != null && avatar.isHuman}";
        }

        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(Root)) return;
            var t = (TextureImporter)assetImporter;
            t.filterMode = FilterMode.Point;
            t.mipmapEnabled = false;
            t.textureCompression = TextureImporterCompression.Uncompressed;
            t.maxTextureSize = assetPath.Contains("T_Palette") ? 2048 : assetPath.Contains("TheMother") || assetPath.Contains("/Enemies/") || assetPath.Contains("/Statics/") ? 1024 : assetPath.Contains("/Characters/") || assetPath.Contains("/Items/") ? 512 : 256;
        }

        private void OnPreprocessModel()
        {
            if (!assetPath.StartsWith(Root)) return;
            var m = (ModelImporter)assetImporter;
            m.materialImportMode = ModelImporterMaterialImportMode.None;
            m.importCameras = false;
            m.importLights = false;
            m.importBlendShapes = true;
            m.isReadable = false;
            m.importNormals = ModelImporterNormals.Import;
            m.resampleCurves = false;
        }

        private void OnPreprocessAnimation()
        {
            if (!assetPath.StartsWith(Root)) return;
            var m = (ModelImporter)assetImporter;
            // Always from the file: a clip list saved in the .meta would hide clips added in a later export.
            var clips = m.defaultClipAnimations;
            foreach (var c in clips)
            {
                int bar = c.name.LastIndexOf('|');
                if (bar >= 0) c.name = c.name.Substring(bar + 1);
                c.loopTime = Looping.Any(c.name.Contains);
                // In-place clips (D-151): the CharacterController moves the body; hips stay on the body's own axis.
                c.lockRootRotation = true;
                c.lockRootHeightY = true;
                c.lockRootPositionXZ = true;
                c.keepOriginalOrientation = true;
                c.keepOriginalPositionY = true;
                c.keepOriginalPositionXZ = true;
            }
            m.clipAnimations = clips;
        }
    }
}
