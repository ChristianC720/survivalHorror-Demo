// Teal rim for nearby interactables and flat world markers. Loaded from Resources so builds always include it.
// The first pass masks the object's own pixels in the stencil; the second grows the silhouette and draws only outside them,
// so a door recessed in its frame still shows a rim.
Shader "Heartbeat/InteractOutline"
{
    Properties
    {
        _Color ("Color", Color) = (0.31, 0.88, 0.8, 1)
        _Width ("Width (fraction of screen height)", Float) = 0.006
        _CenterOS ("Mesh centre (object space)", Vector) = (0, 0, 0, 0)
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("Depth test", Float) = 8
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil test", Float) = 6
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent+50" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float _Width;
            float4 _CenterOS;
        CBUFFER_END

        float4 VertMask(float4 positionOS : POSITION) : SV_POSITION
        {
            return TransformObjectToHClip(positionOS.xyz);
        }

        float4 VertRim(float4 positionOS : POSITION) : SV_POSITION
        {
            float4 clip = TransformObjectToHClip(positionOS.xyz);
            float4 centre = TransformObjectToHClip(_CenterOS.xyz);
            // Sign, not direction: a long box grows the same on every side and quads (posters) get a rim too.
            float2 offset = sign(clip.xy / clip.w - centre.xy / centre.w) * _Width;
            offset.x *= _ScreenParams.y / _ScreenParams.x;
            clip.xy += offset * clip.w;
            return clip;
        }

        float4 Frag() : SV_Target { return _Color; }
        ENDHLSL

        Pass
        {
            Name "Mask"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Off
            ZWrite Off
            ZTest Always
            ColorMask 0
            Stencil { Ref 77 Comp Always Pass Replace }
            HLSLPROGRAM
            #pragma vertex VertMask
            #pragma fragment Frag
            ENDHLSL
        }
        Pass
        {
            Name "Rim"
            Tags { "LightMode" = "UniversalForward" }
            Cull Off
            ZWrite Off
            ZTest [_ZTest]
            Stencil { Ref 77 Comp [_StencilComp] }
            HLSLPROGRAM
            #pragma vertex VertRim
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
