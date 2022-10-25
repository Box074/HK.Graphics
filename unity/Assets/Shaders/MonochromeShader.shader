Shader "HKGL/MonochromeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorWeight ("Weight", Vector) = (1,1,1)
        [Toggle] _ClipAlpha ("Clip", int) = 1
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "CanUseSpriteAtlas"="true" "PreviewType"="Plane" }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float3 _ColorWeight;
            fixed _ClipAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = col.rgb * _ColorWeight.rgb;
                col.rgb = (col.r + col.g + col.b) / (_ColorWeight.r + _ColorWeight.g + _ColorWeight.b);
                clip(col.a - 0.01 + (_ClipAlpha < 0.1 ? 1 : 0));
                return col;
            }
            ENDCG
        }
    }
}