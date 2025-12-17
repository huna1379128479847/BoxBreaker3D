Shader "Hidden/URP/Fullscreen/Glitch"
{
    Properties
    {
        _strength ("Glitch Intensity", Range(0,1)) = 0.1
        _BlockScale("Block Scale", Range(1,50)) = 10
        _NoiseSpeed("Noise Speed", Range(1,10)) = 10
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        ZWrite Off
        Cull Off
        ZTest Always
        Blend Off

        Pass
        {
            Name "Blit"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // Vert / Attributes / Varyings が入ってる（フルスクリーン用）
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // Full Screen Pass / Fetch Color Buffer が供給してくれる入力
            // Use the header-provided sampling macros. Declare the sampler
            // object only (do not redeclare the texture) so we can use
            // SAMPLE_TEXTURE2D_X which expects both texture and sampler.
            SAMPLER(sampler_BlitTexture);

            float _strength;
            float _BlockScale;
            float _NoiseSpeed;

            float random01(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float blockNoise01(float2 p)
            {
                return random01(floor(p));
            }

            float signedNoise(float2 p)
            {
                return -1.0 + 2.0 * blockNoise01(p);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;

                // 元のロジックを「uv.y をブロック化」する形に整理
                float noise = blockNoise01(float2(uv.y * _BlockScale, 0.0));
                noise += random01(float2(uv.x, 0.0)) * 0.3;

                float rv = signedNoise(float2(uv.y, _Time.y * _NoiseSpeed));

                float2 gv = uv;
                gv.x += rv
                        * sin(_strength) * 0.5 // * sin(sin(_strength) * 0.5)
                        * -sin(noise) * 0.2 //* sin(-sin(noise) * 0.2)
                        * frac(_Time.y); // * frac(_Time.y);

                // 色収差は別に分ける予定
                // _ScreenSize が来ない環境でも _ScreenParams は基本ある
                // float2 invSize = rcp(_ScreenParams.xy);
                // float2 offR = float2(2.0, 0.0) * invSize;  2px
                // float2 offB = float2(3.0, 0.0) * invSize;  3px

                // float4 col;
                // col.r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, gv + offR).r;
                // col.g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, gv).g;
                // col.b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, gv - offB).b;
                // col.a = 1.0;

                float4 col;
                col.r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, gv).r;
                col.g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, gv).g;
                col.b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, gv).b;
                col.a = 1.0;

                return col;
            }
            ENDHLSL
        }
    }
}
