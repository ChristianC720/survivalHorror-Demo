using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Heartbeat.EditorTools
{
    /// <summary>Generates the Animator controllers from the imported clips. Idempotent: rebuilds the layers in place.</summary>
    public static class AnimatorBuilder
    {
        private const string LukasFbx = "Assets/_Project/Models/Characters/CH_Lukas_RH07.fbx";
        private const string LukasController = "Assets/_Project/Models/Characters/AC_Lukas.controller";

        [MenuItem("Heartbeat/Models/Build Lukas Animator")]
        public static void BuildLukas()
        {
            var clips = AssetDatabase.LoadAllAssetsAtPath(LukasFbx).OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview")).ToDictionary(c => c.name);
            AnimationClip C(string n) => clips["AN_Lukas_" + n];

            var ctrl = Fresh(LukasController);
            ctrl.AddParameter("Speed", AnimatorControllerParameterType.Float);
            ctrl.AddParameter("Dir", AnimatorControllerParameterType.Float);
            ctrl.AddParameter("Health", AnimatorControllerParameterType.Int);
            ctrl.AddParameter("Crouch", AnimatorControllerParameterType.Bool);
            ctrl.parameters = ctrl.parameters.Select(p => { if (p.name == "Dir") p.defaultFloat = 1f; return p; }).ToArray();

            var layer = ctrl.layers[0];
            layer.name = "Locomotion";
            var sm = layer.stateMachine;
            // timeScale makes the stride match the speed (D-151): PlayerBalance speed / the clip's natural ground speed
            // (planted foot speed of the full cycle: walk 1.03, run 3.05, crouch 0.45 m/s). Limps keep their hand-tuned values.
            var fine = Tree(ctrl, sm, "Fine", new[] { (C("Idle_Fine"), 0f, 1f), (C("Walk_Fine"), 1.5f, 1.45f), (C("Run"), 4f, 1.31f) });
            var injured = Tree(ctrl, sm, "Injured", new[] { (C("Idle_Injured"), 0f, 1f), (C("Walk_Injured"), 1.275f, 1.45f), (C("Run"), 3.4f, 1.11f) });
            var critical = Tree(ctrl, sm, "Critical", new[] { (C("Idle_Critical"), 0f, 1f), (C("Walk_Critical"), 1.05f, 1.7f), (C("Run"), 2.8f, 0.92f) });
            var crouch = Tree(ctrl, sm, "Crouch", new[] { (C("Crouch_Idle"), 0f, 1f), (C("Crouch_Walk"), 0.8f, 1.78f) });
            sm.defaultState = fine;

            foreach (var (state, health) in new[] { (fine, 0), (injured, 1), (critical, 2) })
            {
                var toCrouch = state.AddTransition(crouch);
                Quick(toCrouch, 0.2f);
                toCrouch.AddCondition(AnimatorConditionMode.If, 0, "Crouch");
                var fromCrouch = crouch.AddTransition(state);
                Quick(fromCrouch, 0.2f);
                fromCrouch.AddCondition(AnimatorConditionMode.IfNot, 0, "Crouch");
                fromCrouch.AddCondition(AnimatorConditionMode.Equals, health, "Health");
                foreach (var (other, oh) in new[] { (fine, 0), (injured, 1), (critical, 2) })
                {
                    if (oh == health) continue;
                    var t = state.AddTransition(other);
                    Quick(t, 0.3f);
                    t.AddCondition(AnimatorConditionMode.Equals, oh, "Health");
                    t.AddCondition(AnimatorConditionMode.IfNot, 0, "Crouch");
                }
            }

            var grip = Mask(ctrl, "M_RightFingers", AvatarMaskBodyPart.RightFingers);
            var upper = Mask(ctrl, "M_Upper", AvatarMaskBodyPart.Body, AvatarMaskBodyPart.Head, AvatarMaskBodyPart.LeftArm, AvatarMaskBodyPart.RightArm, AvatarMaskBodyPart.LeftFingers, AvatarMaskBodyPart.RightFingers);

            var gripSm = AddLayer(ctrl, "RightGrip", grip);
            gripSm.AddState("Grip").motion = C("Aim");

            var upperSm = AddLayer(ctrl, "Upper", upper);
            var upperEmpty = upperSm.AddState("Empty");
            upperSm.defaultState = upperEmpty;
            foreach (var n in new[] { "Aim", "Reload_Round", "Terminal_Hold", "Hack_Loop" })
                upperSm.AddState(n).motion = C(n);
            foreach (var n in new[] { "Fire", "Shove", "Use_Door", "Use_Panel", "PowerSwitch", "Pick_Shelf", "Heal", "Examine", "Hit" })
                upperSm.AddState(n).motion = C(n);

            var fullSm = AddLayer(ctrl, "Full", null);
            var fullEmpty = fullSm.AddState("Empty");
            fullSm.defaultState = fullEmpty;
            foreach (var n in new[] { "GroundStrike", "Finisher", "Pick_Floor", "Grab_Release" })
            {
                var s = fullSm.AddState(n);
                s.motion = C(n);
                var back = s.AddTransition(fullEmpty);
                back.hasExitTime = true;
                back.exitTime = 0.9f;
                back.duration = 0.15f;
            }
            var struggle = fullSm.AddState("Grab_Struggle");
            struggle.motion = C("Grab_Struggle");
            var strugleBack = struggle.AddTransition(fullEmpty);
            strugleBack.hasExitTime = true;
            strugleBack.exitTime = 1.5f;
            strugleBack.duration = 0.2f;
            foreach (var n in new[] { "Death_Hit", "Death_Grab", "Grab_Lifted" })
                fullSm.AddState(n).motion = C(n);

            EditorUtility.SetDirty(ctrl);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Heartbeat][Models] Built {LukasController}, {clips.Count} clips");
        }

        /// <summary>One state per brain activity; EnemyAnimator crossfades between them by name.</summary>
        public static void BuildEnemy(string enemy, (string state, string clip, bool rated)[] map)
        {
            string fbx = $"Assets/_Project/Models/Enemies/CH_{enemy}.fbx";
            var clips = AssetDatabase.LoadAllAssetsAtPath(fbx).OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview")).ToDictionary(c => c.name);
            var ctrl = Fresh($"Assets/_Project/Models/Enemies/AC_{enemy}.controller");
            ctrl.AddParameter("Rate", AnimatorControllerParameterType.Float);
            ctrl.parameters = ctrl.parameters.Select(p => { p.defaultFloat = 1f; return p; }).ToArray();
            var sm = ctrl.layers[0].stateMachine;
            foreach (var (state, clip, rated) in map)
            {
                var s = sm.AddState(state);
                s.motion = clips[$"AN_{enemy}_{clip}"];
                if (rated) { s.speedParameter = "Rate"; s.speedParameterActive = true; }
                if (state == "Idle") sm.defaultState = s;
            }
            EditorUtility.SetDirty(ctrl);
            AssetDatabase.SaveAssets();
        }

        public static AnimatorController Fresh(string path)
        {
            var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (ctrl == null) return AnimatorController.CreateAnimatorControllerAtPath(path);
            foreach (var sub in AssetDatabase.LoadAllAssetsAtPath(path))
                if (sub != ctrl) Object.DestroyImmediate(sub, true);
            ctrl.parameters = new AnimatorControllerParameter[0];
            ctrl.layers = new AnimatorControllerLayer[0];
            ctrl.AddLayer("Base Layer");
            return ctrl;
        }

        private static void Quick(AnimatorStateTransition t, float fade)
        {
            t.hasExitTime = false;
            t.duration = fade;
        }

        public static AnimatorState Tree(AnimatorController ctrl, AnimatorStateMachine sm, string name, (AnimationClip clip, float speed, float timeScale)[] entries)
        {
            var tree = new BlendTree { name = name, blendType = BlendTreeType.Simple1D, blendParameter = "Speed", useAutomaticThresholds = false, hideFlags = HideFlags.HideInHierarchy };
            AssetDatabase.AddObjectToAsset(tree, ctrl);
            tree.children = entries.Select(e => new ChildMotion { motion = e.clip, threshold = e.speed, timeScale = e.timeScale, directBlendParameter = "Speed" }).ToArray();
            var state = sm.AddState(name);
            state.motion = tree;
            state.speedParameter = "Dir";
            state.speedParameterActive = true;
            return state;
        }

        public static AvatarMask Mask(AnimatorController ctrl, string name, params AvatarMaskBodyPart[] on)
        {
            var mask = new AvatarMask { name = name, hideFlags = HideFlags.HideInHierarchy };
            for (int i = 0; i < (int)AvatarMaskBodyPart.LastBodyPart; i++)
                mask.SetHumanoidBodyPartActive((AvatarMaskBodyPart)i, on.Contains((AvatarMaskBodyPart)i));
            AssetDatabase.AddObjectToAsset(mask, ctrl);
            return mask;
        }

        public static AnimatorStateMachine AddLayer(AnimatorController ctrl, string name, AvatarMask mask)
        {
            var sm = new AnimatorStateMachine { name = name, hideFlags = HideFlags.HideInHierarchy };
            AssetDatabase.AddObjectToAsset(sm, ctrl);
            ctrl.AddLayer(new AnimatorControllerLayer { name = name, stateMachine = sm, avatarMask = mask, defaultWeight = 1f, blendingMode = AnimatorLayerBlendingMode.Override });
            return sm;
        }
    }
}
