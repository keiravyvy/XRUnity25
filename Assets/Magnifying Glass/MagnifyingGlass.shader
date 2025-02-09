Shader "Custom/MagnifyingGlass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0, 5)) = 1.0
        _ChromaticAberration ("Chromatic Aberration", Range(0, 1)) = 0.1
        _GlassReflection ("Glass Reflection", Range(0, 1)) = 0.1
        _EdgeBlur ("Edge Blur", Range(0, 1)) = 0.1
        _EdgeBlurPower ("Edge Blur Power", Range(1, 5)) = 2.0
        _GlassColor ("Glass Color", Color) = (1, 1, 1, 0.1)
        _BlurTexture ("Blur Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD1;
                float3 normal : NORMAL;
                float4 screenPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _BlurTexture;
            float _Distortion;
            float _ChromaticAberration;
            float _GlassReflection;
            float _EdgeBlur;
            float _EdgeBlurPower;
            float4 _GlassColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            
            float4 blur(sampler2D tex, float2 uv, float blur)
            {
                float4 col = 0;
                float2 offset = float2(blur * 0.001, 0);
                
                // 9-tap gaussian blur
                col += tex2D(tex, uv - offset * 4) * 0.05;
                col += tex2D(tex, uv - offset * 3) * 0.09;
                col += tex2D(tex, uv - offset * 2) * 0.12;
                col += tex2D(tex, uv - offset) * 0.15;
                col += tex2D(tex, uv) * 0.16;
                col += tex2D(tex, uv + offset) * 0.15;
                col += tex2D(tex, uv + offset * 2) * 0.12;
                col += tex2D(tex, uv + offset * 3) * 0.09;
                col += tex2D(tex, uv + offset * 4) * 0.05;
                
                return col;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Convert UV to centered coordinates
                float2 center = i.uv - 0.5;
                float dist = length(center);
                
                // Apply barrel distortion
                float2 distortedUV = center * (1.0 + _Distortion * dist * dist);
                distortedUV += 0.5;
                
                // Calculate edge fade
                float edgeFade = 1 - pow(smoothstep(0.3, 0.5, dist), _EdgeBlurPower);
                
                // Sample with chromatic aberration and blur
                fixed4 col;
                float blurAmount = _EdgeBlur * (1 - edgeFade);
                
                col.r = blur(_MainTex, distortedUV + _ChromaticAberration * dist, blurAmount).r;
                col.g = blur(_MainTex, distortedUV, blurAmount).g;
                col.b = blur(_MainTex, distortedUV - _ChromaticAberration * dist, blurAmount).b;
                col.a = 1;
                
                // Add glass reflection
                float fresnel = pow(1 - saturate(dot(i.normal, i.viewDir)), 5) * _GlassReflection;
                col.rgb += fresnel;
                
                // Apply edge fade
                col.a *= edgeFade;
                
                // Blend with glass color
                col = lerp(_GlassColor, col, col.a);
                
                return col;
            }
            ENDCG
        }
    }
}