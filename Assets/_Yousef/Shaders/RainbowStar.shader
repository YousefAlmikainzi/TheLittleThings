Shader "Custom/StarDiagonalCycleOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}

        _ColorA ("Color A", Color) = (1,1,0,1)
        _ColorB ("Color B", Color) = (1,0,0,1)
        _ColorC ("Color C", Color) = (0,0,1,1)

        _Speed ("Cycle Speed", Float) = 0.5

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _ColorA;
            float4 _ColorB;
            float4 _ColorC;
            float4 _OutlineColor;

            float _Speed;
            float _OutlineThickness;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float alpha = tex.a;

                float2 px = _OutlineThickness / _ScreenParams.xy;

                float aR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(px.x, 0)).a;
                float aL = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - float2(px.x, 0)).a;
                float aU = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(0, px.y)).a;
                float aD = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - float2(0, px.y)).a;

                if (alpha > 0.01 && (aR < 0.01 || aL < 0.01 || aU < 0.01 || aD < 0.01))
                    return _OutlineColor;

                float diag = frac(i.uv.x + i.uv.y + _Time.y * _Speed);
                float band = diag * 3.0;

                float4 col;
                if (band < 1.0)
                    col = lerp(_ColorA, _ColorB, band);
                else if (band < 2.0)
                    col = lerp(_ColorB, _ColorC, band - 1.0);
                else
                    col = lerp(_ColorC, _ColorA, band - 2.0);

                return float4(col.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
