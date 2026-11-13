using Heartbeat.Core;
using Heartbeat.Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartbeat.Combat
{
    /// <summary>
    /// Revolver visual effects (C-71): a muzzle flash on <see cref="GameEvents.ShotFired"/> and, where a shot lands on a
    /// surface without <see cref="IDamageable"/>, sparks, dust and a bullet mark that fades out. Enemies, alive or dead,
    /// never get this impact. A live enemy gets a dark blood splatter instead (C-72). All of them run through one <see cref="VfxPool"/>.
    /// </summary>
    public sealed class ShotEffects : MonoBehaviour
    {
        private const int FlashCap = 2;              // shots are 0.8 s apart and a flash lasts under 0.1 s
        private const float OwnShotDistance = 0.05f; // during the Intro to Floor load two players listen: only the shooter reacts

        [SerializeField] private CombatBalance balance;
        [SerializeField] private Transform muzzle;
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private GameObject impactPrefab;
        [SerializeField] private GameObject splatterPrefab;

        private GameObject root;
        private VfxPool pool;

        /// <summary>Rule 3 of C-71: only a hit on something that cannot take a shot shows the impact.</summary>
        public static bool ShowsImpact(Collider hit) => hit != null && hit.GetComponentInParent<IDamageable>() == null;

        /// <summary>C-72: only enemies bleed and get the hit cue; the boss transformers and the debug dummy do not.</summary>
        public static bool IsEnemy(IDamageable target) => target is AI.EnemyHealth;

        private void Awake()
        {
            root = new GameObject("VFX_Shots");
            if (gameObject.scene.IsValid()) SceneManager.MoveGameObjectToScene(root, gameObject.scene);
            pool = new VfxPool(root.transform);
        }

        private void OnEnable()
        {
            GameEvents.ShotFired += OnShot;
            GameEvents.ShotImpact += OnImpact;
            GameEvents.ShotHitEnemy += OnHitEnemy;
            GameEvents.FloorBuilt += OnFloorBuilt;
        }

        private void OnDisable()
        {
            GameEvents.ShotFired -= OnShot;
            GameEvents.ShotImpact -= OnImpact;
            GameEvents.ShotHitEnemy -= OnHitEnemy;
            GameEvents.FloorBuilt -= OnFloorBuilt;
        }

        private void OnDestroy()
        {
            if (root != null) Destroy(root);
        }

        private bool IsMine(Vector3 origin) => muzzle != null && (origin - muzzle.position).sqrMagnitude < OwnShotDistance * OwnShotDistance;

        private void OnShot(Vector3 origin)
        {
            if (!IsMine(origin)) return;
            pool.Spawn(muzzleFlashPrefab, FlashCap, muzzle.position, muzzle.rotation, balance.muzzleFlashSeconds, muzzle);
        }

        private void OnImpact(Vector3 origin, Vector3 point, Vector3 normal)
        {
            if (!IsMine(origin)) return;
            pool.Spawn(impactPrefab, balance.bulletMarkMax, point, Quaternion.LookRotation(normal), balance.bulletMarkSeconds);
        }

        private void OnHitEnemy(Vector3 origin, Vector3 point, Vector3 normal)
        {
            if (!IsMine(origin)) return;
            pool.Spawn(splatterPrefab, balance.splatterMax, point, Quaternion.LookRotation(normal), balance.splatterSeconds);
        }

        private void OnFloorBuilt(int floor, int seed) => pool.Clear();
    }
}
