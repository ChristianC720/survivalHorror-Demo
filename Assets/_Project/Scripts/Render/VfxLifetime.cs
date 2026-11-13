using UnityEngine;

namespace Heartbeat.Render
{
    /// <summary>
    /// Lifetime of a pooled effect (C-71, see <see cref="VfxPool"/>). On restart it replays its particle systems; plain
    /// renderers (a bullet mark) fade out over the lifetime, then the whole effect hides itself.
    /// </summary>
    public sealed class VfxLifetime : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private ParticleSystem[] particles;
        private Renderer[] fading;
        private Color[] baseColors;
        private MaterialPropertyBlock block;
        private float age, lifetime;

        /// <summary>Opacity of a fading renderer: holds near full early on, gone at the end.</summary>
        public static float Alpha(float age, float lifetime)
        {
            if (lifetime <= 0f) return 0f;
            float t = Mathf.Clamp01(age / lifetime);
            return 1f - t * t;
        }

        public void Restart(float seconds)
        {
            Cache();
            age = 0f;
            lifetime = seconds;
            gameObject.SetActive(true);
            foreach (var ps in particles)
            {
                ps.Clear(false);
                ps.Play(false);
            }
            ApplyAlpha(1f);
        }

        private void Update()
        {
            age += Time.deltaTime;
            if (age >= lifetime)
            {
                gameObject.SetActive(false);
                return;
            }
            ApplyAlpha(Alpha(age, lifetime));
        }

        private void Cache()
        {
            if (particles != null) return;
            particles = GetComponentsInChildren<ParticleSystem>(true);
            var all = GetComponentsInChildren<Renderer>(true);
            int n = 0;
            foreach (var r in all) if (!(r is ParticleSystemRenderer)) n++;
            fading = new Renderer[n];
            baseColors = new Color[n];
            n = 0;
            foreach (var r in all)
            {
                if (r is ParticleSystemRenderer) continue;
                fading[n] = r;
                var mat = r.sharedMaterial;
                baseColors[n++] = mat != null && mat.HasProperty(BaseColor) ? mat.GetColor(BaseColor) : Color.white;
            }
            block = new MaterialPropertyBlock();
        }

        private void ApplyAlpha(float alpha)
        {
            for (int i = 0; i < fading.Length; i++)
            {
                var c = baseColors[i];
                c.a *= alpha;
                fading[i].GetPropertyBlock(block);
                block.SetColor(BaseColor, c);
                fading[i].SetPropertyBlock(block);
            }
        }
    }
}
