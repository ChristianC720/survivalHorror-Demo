// Pulsing amber status lamp on large interactables. Loaded from Resources so builds always include it.
Shader "Heartbeat/StatusLamp"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.62, 0.12, 1)
        _Speed ("Pulse speed", Float) = 4
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        Pass
        {
            Name "Lamp"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Speed;
            CBUFFER_END

            float4 Vert(float4 positionOS : POSITION) : SV_POSITION { return TransformObjectToHClip(positionOS.xyz); }
            float4 Frag() : SV_Target { return _Color * (0.55 + 0.45 * sin(_Time.y * _Speed)); }
            ENDHLSL
        }
    }
}
