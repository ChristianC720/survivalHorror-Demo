using UnityEngine;

namespace Heartbeat.Render
{
    /// <summary>
    /// Player brightness (D-106): a perceptual gain applied in the pixel/dither pass. 1 = neutral. The calibration
    /// screen previews it on UI with <see cref="Perceived"/>, which uses the same formula as the shader.
    /// </summary>
    public static class Brightness
    {
        public const float Min = 0.5f, Max = 1.5f;
        private static readonly int Id = Shader.PropertyToID("_HbBrightness");

        public static void Apply(float value) => Shader.SetGlobalFloat(Id, Mathf.Clamp(value, Min, Max) - 1f);

        /// <summary>Linear colour level after the brightness gain (shader: perceptual space × value).</summary>
        public static float Perceived(float linearLevel, float value) =>
            Mathf.Pow(Mathf.Clamp01(Mathf.Pow(linearLevel, 1f / 2.2f) * value), 2.2f);
    }
}
