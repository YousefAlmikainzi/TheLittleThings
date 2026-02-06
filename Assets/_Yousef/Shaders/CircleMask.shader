Shader "Custom/FullOpaqueDarkness2D_StandardVF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DarknessColor ("Darkness Color", Color) = (0,0,0,1)
        _LightPosition ("Light Position", Vector) = (0,0,0,0)
        _LightRadius ("Light Radius", Float) = 2.0
        _EdgeSoftness ("Edge Softness", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _DarknessColor;
            float4 _LightPosition;
            float _LightRadius;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fragPos = i.worldPos.xy;
                float2 lightPos = _LightPosition.xy;

                float dist = distance(fragPos, lightPos);
                float mask = dist < _LightRadius ? 0.0 : 1.0;

                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = _DarknessColor.rgb;
                col.a = _DarknessColor.a * mask;

                return col;
            }
            ENDCG
        }
    }
}
