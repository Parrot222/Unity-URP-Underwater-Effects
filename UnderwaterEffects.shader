Shader "Paro222/UnderwaterEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            half remap(half x, half t1, half t2, half s1, half s2)
            {
                return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
            }

            sampler2D _MainTex;
            sampler2D _NormalMap;
            float4 _normalUV;
            float4 _MainTex_ST;
            fixed4 _color;
            float _dis;
            float _alpha;
            float _refraction;
            sampler2D_float _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed3 normalmap = UnpackNormal(tex2D(_NormalMap, i.uv * _normalUV.xy + _normalUV.zw * _Time.y));

                float depth_tex = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv + normalmap * _refraction * 0.01));
                float depth = saturate(smoothstep(0, _dis * 0.01, Linear01Depth(depth_tex)) + _alpha);
                
                fixed4 col = tex2D(_MainTex, i.uv + normalmap * _refraction * 0.01);

                return lerp(col, _color, depth);
            }
            ENDCG
        }
    }
}
