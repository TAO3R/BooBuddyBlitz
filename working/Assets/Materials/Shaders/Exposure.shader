Shader "Custom/Exposure"
{
    Properties
    {
        _Exposure("Exposure", Float) = 1.0
    }
    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        float _Exposure;
        ENDHLSL

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "Exposure"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag (Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgba;
                return color * _Exposure;
            }
            
            ENDHLSL
        }
    }
}
