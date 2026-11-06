using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Heartbeat.Render
{
    /// <summary>
    /// Heartbeat render look (D-81, GDD 05 - Arte y Audio/01): renders the camera colour into a ~540-line target
    /// with ordered dithering and colour quantization, then upscales it with point filtering.
    /// Screen-space overlay UI is drawn after this, so text stays sharp.
    /// </summary>
    public sealed class PixelationFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public sealed class Settings
        {
            [Tooltip("Internal render height in pixels (GDD: ~540p).")]
            [Range(180, 1080)] public int targetHeight = 540;
            [Tooltip("Colour levels per channel after quantization.")]
            [Range(2, 256)] public int colorSteps = 32;
            [Tooltip("Strength of the 4x4 Bayer dither.")]
            [Range(0f, 1f)] public float ditherStrength = 0.6f;
            public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingPostProcessing;
            [Tooltip("Apply in Scene view too (off keeps editing comfortable).")]
            public bool applyInSceneView;
        }

        [SerializeField] private Settings settings = new Settings();
        [SerializeField] private Shader shader;

        private Material material;
        private PixelationPass pass;
        private static int scaledForHeight;

        /// <summary>Runtime override used by surveillance / degradation effects (null = use the asset settings).</summary>
        public static int? HeightOverride;

        /// <summary>Lines the look keeps; <see cref="VideoSettings"/> renders the scene at this height instead of throwing pixels away.</summary>
        public static int TargetHeight { get; private set; } = 540;

        public override void Create()
        {
            TargetHeight = settings.targetHeight;
            if (shader == null) shader = Shader.Find("Hidden/Heartbeat/PixelDither");
            if (shader != null && material == null) material = CoreUtils.CreateEngineMaterial(shader);
            pass = new PixelationPass { renderPassEvent = settings.passEvent };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (material == null) return;
            var camType = renderingData.cameraData.cameraType;
            if (camType == CameraType.Preview || camType == CameraType.Reflection) return;
            if (camType == CameraType.SceneView && !settings.applyInSceneView) return;
            if (camType == CameraType.Game && Application.isPlaying && Screen.height != scaledForHeight)
            {
                scaledForHeight = Screen.height;   // resolution option, window resize, Game view size
                VideoSettings.ApplyRenderScale(scaledForHeight);
            }

            pass.Setup(material, settings);
            renderer.EnqueuePass(pass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
            material = null;
        }

        private sealed class PixelationPass : ScriptableRenderPass
        {
            private static readonly int ColorStepsId = Shader.PropertyToID("_ColorSteps");
            private static readonly int DitherStrengthId = Shader.PropertyToID("_DitherStrength");

            private Material material;
            private Settings settings;

            public PixelationPass()
            {
                requiresIntermediateTexture = true;
            }

            public void Setup(Material mat, Settings s)
            {
                material = mat;
                settings = s;
                renderPassEvent = s.passEvent;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resources = frameData.Get<UniversalResourceData>();
                if (resources.isActiveTargetBackBuffer) return;

                TextureHandle source = resources.activeColorTexture;
                TextureDesc desc = renderGraph.GetTextureDesc(source);

                int height = Mathf.Clamp(HeightOverride ?? settings.targetHeight, 16, desc.height);
                int width = Mathf.Max(16, Mathf.RoundToInt(desc.width * (height / (float)desc.height)));

                desc.name = "_HeartbeatLowRes";
                desc.width = width;
                desc.height = height;
                desc.clearBuffer = false;
                desc.filterMode = FilterMode.Point;
                desc.depthBufferBits = DepthBits.None;
                desc.msaaSamples = MSAASamples.None;
                TextureHandle lowRes = renderGraph.CreateTexture(desc);

                material.SetFloat(ColorStepsId, settings.colorSteps);
                material.SetFloat(DitherStrengthId, settings.ditherStrength);

                renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(source, lowRes, material, 0),
                    "Heartbeat Pixel Downsample");
                renderGraph.AddBlitPass(lowRes, source, Vector2.one, Vector2.zero,
                    filterMode: RenderGraphUtils.BlitFilterMode.ClampNearest, passName: "Heartbeat Pixel Upscale");
            }
        }
    }
}
