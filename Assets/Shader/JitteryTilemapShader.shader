Shader "Custom/JitteryTilemap" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _JitterSpeed ("Jitter Speed", Range(0, 10)) = 1
        _JitterAmount ("Jitter Amount", Range(0, 0.1)) = 0.05
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200
        
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float _JitterSpeed;
            float _JitterAmount;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 offset = float2(
                    sin(_Time.y * _JitterSpeed + v.vertex.x),
                    cos(_Time.y * _JitterSpeed + v.vertex.y)
                ) * _JitterAmount;
                o.uv = v.uv + offset;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
