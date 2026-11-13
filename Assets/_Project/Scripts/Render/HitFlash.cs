using Heartbeat.AI;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Render
{
    /// <summary>
    /// A shot enemy flashes (C-72): on <see cref="EnemyHealth.Shot"/> its mesh renderers swap to a flat flash material for
    /// <see cref="SystemsBalance.hitFlashSeconds"/>, then get their own materials back. With Reduce flashes the flash
    /// is a dim grey instead of white (D-106).
    /// </summary>
    public sealed class HitFlash : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly Color Bright = Color.white, Soft = new(0.35f, 0.35f, 0.35f, 1f);

        [SerializeField] private Material flashMaterial;

        private EnemyHealth health;
        private Renderer[] renderers;
        private Material[][] originals;
        private MaterialPropertyBlock block;
        private float remaining;

        public bool Flashing => originals != null;

        private void Awake() => health = GetComponent<EnemyHealth>();
        private void OnEnable() { if (health != null) health.Shot += OnShot; }

        private void OnDisable()
        {
            if (health != null) health.Shot -= OnShot;
            Stop();
        }

        private void OnShot(Combat.ShotHit _) => Play(SystemsBalance.Current.hitFlashSeconds, Save.AccessOptions.ReduceFlashes);

        /// <summary>Starts the flash, or restarts its timer if it is already on (the original materials are kept once).</summary>
        public void Play(float seconds, bool reduced = false)
        {
            remaining = seconds;
            if (Flashing || flashMaterial == null) return;
            renderers = GetComponentsInChildren<Renderer>();
            originals = new Material[renderers.Length][];
            block ??= new MaterialPropertyBlock();
            block.SetColor(BaseColor, reduced ? Soft : Bright);
            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                if (r is not SkinnedMeshRenderer && r is not MeshRenderer) continue;   // particles, lines: leave them be
                if (r.HasPropertyBlock()) continue;   // someone else drives it (an outline hull): Stop would clear their block
                originals[i] = r.sharedMaterials;
                var flash = new Material[originals[i].Length];
                for (int m = 0; m < flash.Length; m++) flash[m] = flashMaterial;
                r.sharedMaterials = flash;
                r.SetPropertyBlock(block);
            }
        }

        /// <summary>Gives every renderer its own materials back.</summary>
        public void Stop()
        {
            if (!Flashing) return;
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null || originals[i] == null) continue;
                renderers[i].sharedMaterials = originals[i];
                renderers[i].SetPropertyBlock(null);
            }
            originals = null;
        }

        private void Update()
        {
            if (!Flashing) return;
            remaining -= Time.deltaTime;
            if (remaining <= 0f) Stop();
        }
    }
}
