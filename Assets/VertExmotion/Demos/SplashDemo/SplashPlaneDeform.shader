Shader "VertExmotion/SplashPlaneDeform" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SplashTex("Splash", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard vertex:vert  fullforwardshadows  addshadow 

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0
			
			//Add VertExmotion cginc
			#include "Assets/VertExmotion/Shaders/VertExmotion.cginc" 

		sampler2D _MainTex;
		sampler2D _SplashTex;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float3 planePoint;
		float3 planeNormal;
		float splashAmplitude;
		float splashMinDist;
		float splashDeformation;

		float RND(float2 seed) {
			seed = round(seed * 65536) / 65536;
			return abs(frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453));
		}

		float4 Splash( float4 pos, float3 normal, float2 uv )
		{
			//float minDist = .3;

			pos = mul(unity_ObjectToWorld, pos);
			float3 n = mul(unity_ObjectToWorld, float4(normal,0));
			float d = dot(normalize(pos - planePoint), normalize(planeNormal));
			
			float dProj = dot((pos.xyz - planePoint.xyz), normalize(planeNormal.xyz));
			float3 proj = dProj * normalize(planeNormal.xyz);

			float dist = length(proj - dProj);

			float spashFactor = tex2Dlod(_SplashTex, float4(uv,0,0));
			//float spashFactor = RND(uv);
			
			float3 splash = float3(0, 0, 0);
			if (d > 0 && dist < splashMinDist)
			{
				splash = normal * (1 - abs(dot(n, planeNormal))) * splashAmplitude * (1 - dist / splashMinDist) * lerp(1, spashFactor, splashDeformation);
			}

			if (d < 0)
			{				
				pos.xyz -= proj;
				splash = normal * (1 - abs(dot(n, planeNormal))) * splashAmplitude * lerp(1, spashFactor, splashDeformation);
			}

			splash.xyz -= normalize(planeNormal.xyz) * dot(splash.xyz, normalize(planeNormal.xyz)) - normalize(planeNormal.xyz)* .01;
			pos.xyz += splash.xyz;

			return mul(unity_WorldToObject, pos);
		}
			
		void vert (inout appdata_full v) 
		{	
			VertExmotion(v);//VertExmotion function for jelly animation
			v.vertex = Splash(v.vertex, v.normal, v.texcoord);
			
		}


		

		struct Input {
			float2 uv_MainTex;
		};

		

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
