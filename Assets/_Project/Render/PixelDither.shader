// Low-resolution pass of the Heartbeat look (D-81): ordered 4x4 Bayer dithering + per-channel colour quantization.
// Runs while blitting the camera colour into the ~540p target; the upscale back is a point (nearest) blit.
Shader "Hidden/Heartbeat/PixelDither"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off ZTest Always Cull Off Blend Off

        Pass
        {
            Name "PixelDither"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _ColorSteps;      // levels per channel, e.g. 32
            float _DitherStrength;  // 0..1

            static const float Bayer4[16] =
            {
                 0.0,  8.0,  2.0, 10.0,
                12.0,  4.0, 14.0,  6.0,
                 3.0, 11.0,  1.0,  9.0,
                15.0,  7.0, 13.0,  5.0
            };

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                half4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                uint2 p = (uint2)input.positionCS.xy % 4;
                float threshold = Bayer4[p.y * 4 + p.x] / 16.0 - 0.5;

                // Quantize in perceptual space so the steps are even to the eye.
                float3 c = pow(saturate(col.rgb), 1.0 / 2.2);
                float steps = max(_ColorSteps, 2.0);
                c = floor(c * steps + 0.5 + threshold * _DitherStrength) / steps;
                col.rgb = pow(saturate(c), 2.2);
                return col;
            }
            ENDHLSL
        }
    }
}
