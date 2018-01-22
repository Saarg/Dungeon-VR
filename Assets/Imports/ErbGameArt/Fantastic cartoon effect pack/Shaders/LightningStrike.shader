Shader "ErbGameArt/Particles/AlphaBlended/LightningStrike" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0,0.503448,1,1)
        _Noise ("Noise", 2D) = "white" {}
        _Emissive ("Emissive", Float ) = 2
        _Alpha ("Alpha", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Emissive;
            uniform float _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 node_7952 = _Time + _TimeEditor;
                float2 node_6115 = (i.uv1+node_7952.g*float2(0,0.25));
                float4 node_3327 = tex2D(_Noise,TRANSFORM_TEX(node_6115, _Noise));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_228 = (node_3327.g*_MainTex_var.g);
                float node_8021 = (_MainTex_var.r*1.0);
                float3 emissive = (((saturate((node_228*node_228*16.0))+((_MainTex_var.r+0.1)*6.0)+node_8021)*_TintColor.rgb)*i.vertexColor.rgb*_Emissive);
                float3 finalColor = emissive;
                float2 node_5410 = (i.uv1+node_7952.g*float2(0,0.2));
                float4 node_3920 = tex2D(_Noise,TRANSFORM_TEX(node_5410, _Noise));
                return fixed4(finalColor,(i.vertexColor.a*(node_8021*saturate(pow(((_MainTex_var.r*(1.0 - _MainTex_var.g))+(_MainTex_var.g*node_3920.r)),i.uv1.g)))*_Alpha));
            }
            ENDCG
        }
    }
}
