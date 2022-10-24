Shader "HKGL/TextureMaskMakerShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _P0 ("P0", Vector) = (0, 0, 0, 0)
        _P1 ("P1", Vector) = (0, 0, 0, 0)
        _P2 ("P2", Vector) = (0, 0, 0, 0)
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
            float2 _P0;
            float2 _P1;
            float2 _P2;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                float2 uv = i.uv;

                float signOfTrig = (_P1.x - _P0.x)*(_P2.y - _P0.y) - (_P1.y - _P0.y)*(_P2.x - _P0.x);
                float signOfAB = (_P1.x - _P0.x)*(uv.y - _P0.y) - (_P1.y - _P0.y)*(uv.x - _P0.x);
                float signOfCA = (_P0.x - _P2.x)*(uv.y - _P2.y) - (_P0.y - _P2.y)*(uv.x - _P2.x);
                float signOfBC = (_P2.x - _P1.x)*(uv.y - _P2.y) - (_P2.y - _P1.y)*(uv.x - _P2.x);
                fixed4 col = fixed4(0,0,0,0);
                col.r = (signOfAB * signOfTrig > 0) ? 
                    ((signOfCA * signOfTrig > 0) ? ((signOfBC * signOfTrig > 0) ? 1 : 0) : 0) : 0;
                clip(col.r < 0.1);
                return col;
            }
            ENDCG
        }
    }
}