using System.Linq;
using Heartbeat.Player;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>C-60: the civilian Lukas is a separate FBX that the player's skeleton can wear.</summary>
    public class LukasCivilianTests
    {
        private const string Dir = "Assets/_Project/Models/Characters/";

        [Test]
        public void CivilianBody_FitsThePlayerSkeleton()
        {
            var suit = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Dir + "CH_Lukas_RH07.fbx"));
            var civil = AssetDatabase.LoadAssetAtPath<GameObject>(Dir + "CH_Lukas_Civil.fbx").GetComponentInChildren<SkinnedMeshRenderer>();
            try
            {
                var body = suit.GetComponentInChildren<SkinnedMeshRenderer>();
                LukasAttachments.WearCivilian(body, civil, null);
                Assert.That(body.sharedMesh, Is.SameAs(civil.sharedMesh));
                Assert.That(body.bones.Length, Is.EqualTo(civil.bones.Length));
                Assert.That(body.bones.All(b => b != null && b.IsChildOf(suit.transform)), "every bone belongs to the player's own skeleton");
                Assert.That(body.bones.Select(b => b.name), Is.EqualTo(civil.bones.Select(b => b.name)));
            }
            finally { Object.DestroyImmediate(suit); }
        }

        [Test]
        public void PlayerPrefab_HasTheCivilianBodyWired()
        {
            var player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Characters/Player.prefab");
            var so = new SerializedObject(player.GetComponentInChildren<LukasAttachments>(true));
            Assert.That(so.FindProperty("civilianBody").objectReferenceValue, Is.Not.Null);
            Assert.That(so.FindProperty("civilianMaterial").objectReferenceValue, Is.Not.Null);
        }

        [Test]
        public void CivilianModel_IsAValidHumanoid()
        {
            var avatar = AssetDatabase.LoadAllAssetsAtPath(Dir + "CH_Lukas_Civil.fbx").OfType<Avatar>().First();
            Assert.That(avatar.isValid && avatar.isHuman);
        }
    }
}
