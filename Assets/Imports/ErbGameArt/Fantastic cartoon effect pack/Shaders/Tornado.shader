Shader "Tornado" {
    Properties {
        [HDR]_TintColor ("color", Color) = (1,0.6,0,1)
        _MainTex ("MainTex", 2D) = "black" {}
        _FresnelStrench ("FresnelStrench", Float ) = 1
        [MaterialToggle] _Fresnel ("Fresnel", Float ) = 0
        _EmmisionStrench ("EmmisionStrench", Range(0, 8)) = 2
        _FresnelOutline ("FresnelOutline", Float ) = 1
        _U_Speed ("U_Speed", Float ) = -0.2
        _V_Speed ("V_Speed", Float ) = -0.5
        _FrenselColor ("FrenselColor", Color) = (1,1,1,1)
        _Numberofwawes ("Number of wawes", Float ) = 3
        _Sizeofwawes ("Size of wawes", Float ) = -0.07
        _DirectionSpeedofwawes ("Direction&Speed of wawes", Float ) = -0.5
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_Reel("Reel vector", Vector) = (1, 0.1, 5, 0)
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _FresnelStrench;
            uniform fixed _Fresnel;
            uniform float _EmmisionStrench;
            uniform float _FresnelOutline;
            uniform float _U_Speed;
            uniform float _V_Speed;
            uniform float4 _FrenselColor;
            uniform float _Numberofwawes;
            uniform float _Sizeofwawes;
            uniform float _DirectionSpeedofwawes;
			half _Cutoff;
			float4 _Reel;
			
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord0 : TEXCOORD0;
				fixed4 color : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
				fixed4 color : COLOR;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
				float height : TEXCOORD3;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 timer = _Time + _TimeEditor;
                float h = o.uv0.g;
				o.color = v.color * _TintColor;
                v.vertex.xyz += ((sin((_Numberofwawes*(mul(unity_ObjectToWorld, v.vertex).g.r+(timer.g*_DirectionSpeedofwawes))*6.28318530718))*h.r)*v.normal*_Sizeofwawes);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				float3 worldpos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float4 pivot = mul(unity_ObjectToWorld, float4(0,0,0,1));
				float height = o.uv0.g * _Reel.y;
				v.vertex.x += sin(_Time.y*_Reel.z + worldpos.y * _Reel.x) * height;
				v.vertex.z += sin(_Time.y*_Reel.z + worldpos.y * _Reel.x + 3.1415/2) * height;
				o.height = height;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 node_4620 = _Time + _TimeEditor;
                float2 node_5411 = ((i.uv0+(node_4620.g*_U_Speed)*float2(1,0))+(node_4620.g*_V_Speed)*float2(0,1));
                float4 node_9700 = tex2D(_MainTex,TRANSFORM_TEX(node_5411, _MainTex));
				clip((dot(node_9700.rgb,float3(0.3,0.59,0.11))*i.uv0.b) - 0.5);
                float3 node_4951 = (_TintColor.rgb*i.color*node_9700.rgb*_EmmisionStrench);
                float3 node_2916 = (pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelStrench)*_FrenselColor.rgb);
                float3 emissive = lerp( node_4951, (node_4951+saturate((node_2916*node_2916*_FresnelOutline))), _Fresnel );
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
				clip(node_9700.r - _Cutoff);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
