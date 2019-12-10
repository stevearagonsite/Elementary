Shader "VertExmotion/Jellyfish" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
	_RimColor ("Rim Color", Color) = (0.2,0.2,0.2,0.0)
    _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	_Emission("Emission", Range(0.0,1.0)) = .5
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 300
	Blend SrcAlpha OneMinusSrcAlpha
    Cull off

CGPROGRAM
//#pragma target 3.0
#pragma surface surf BlinnPhong vertex:vert addshadow alpha:fade
#include "../../Shaders/VertExmotion.cginc"
void vert (inout appdata_full v) {VertExmotion( v );}

sampler2D _MainTex;
fixed4 _Color;
half _Shininess;
float _Emission;

struct Input {
	float2 uv_MainTex;
	float3 viewDir;
	float3 worldRefl;
};
	float4 _RimColor;
     float _RimPower;
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	half rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.worldRefl)) * _RimColor.a;
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess + _SpecColor;	
	o.Emission = _Emission*_RimColor.rgb * pow (rim, _RimPower) * o.Specular;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
