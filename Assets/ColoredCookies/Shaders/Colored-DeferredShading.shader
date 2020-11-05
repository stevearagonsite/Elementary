// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/Colored-DeferredShading" {
Properties {
    _LightTexture0 ("", any) = "" {}
    _LightTextureB0 ("", 2D) = "" {}
    _ShadowMapTexture ("", any) = "" {}
    _SrcBlend ("", Float) = 1
    _DstBlend ("", Float) = 1
}
SubShader {

// Pass 1: Lighting pass
//  LDR case - Lighting encoded into a subtractive ARGB8 buffer
//  HDR case - Lighting additively blended into floating point buffer
Pass {
    ZWrite Off
    Blend [_SrcBlend] [_DstBlend]

CGPROGRAM
#pragma target 3.0
#pragma vertex vert_deferred
#pragma fragment frag
#pragma multi_compile_lightpass
#pragma multi_compile ___ UNITY_HDR_ON

#pragma exclude_renderers nomrt

#include "UnityCG.cginc"
#include "UnityDeferredLibrary.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardBRDF.cginc"

sampler2D _CameraGBufferTexture0;
sampler2D _CameraGBufferTexture1;
sampler2D _CameraGBufferTexture2;

// --------------------------------------------------------
// Common lighting data calculation (direction, attenuation, ...)
void UnityDeferredCalculateLightParamsColoredCookie(
	unity_v2f_deferred i,
	out float3 outWorldPos,
	out float2 outUV,
	out half3 outLightDir,
	out float3 outAtten,
	out float outFadeDist)
{
	i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
	float2 uv = i.uv.xy / i.uv.w;

	// read depth and reconstruct world position
	float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
	depth = Linear01Depth(depth);
	float4 vpos = float4(i.ray * depth, 1);
	float3 wpos = mul(unity_CameraToWorld, vpos).xyz;

	float fadeDist = UnityComputeShadowFadeDistance(wpos, vpos.z);

	// spot light case
#if defined (SPOT)
	float3 tolight = _LightPos.xyz - wpos;
	half3 lightDir = normalize(tolight);

	float4 uvCookie = mul(unity_WorldToLight, float4(wpos, 1));
	// negative bias because http://aras-p.info/blog/2010/01/07/screenspace-vs-mip-mapping/
	float3 atten = tex2Dbias(_LightTexture0, float4(uvCookie.xy / uvCookie.w, 0, -8)).rgb;
	atten *= uvCookie.w < 0;
	float att = dot(tolight, tolight) * _LightPos.w;
	atten *= tex2D(_LightTextureB0, att.rr).r;

	atten *= UnityDeferredComputeShadow(wpos, fadeDist, uv);

	// directional light case
#elif defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
	half3 lightDir = -_LightDir.xyz;
	float3 atten = 1.0;

	atten *= UnityDeferredComputeShadow(wpos, fadeDist, uv);

#if defined (DIRECTIONAL_COOKIE)
	atten *= tex2Dbias(_LightTexture0, float4(mul(unity_WorldToLight, half4(wpos, 1)).xy, 0, -8)).rgb;
#endif //DIRECTIONAL_COOKIE

	// point light case
#elif defined (POINT) || defined (POINT_COOKIE)
	float3 tolight = wpos - _LightPos.xyz;
	half3 lightDir = -normalize(tolight);

	float att = dot(tolight, tolight) * _LightPos.w;
	float3 atten = tex2D(_LightTextureB0, att.rr).r;

	atten *= UnityDeferredComputeShadow(tolight, fadeDist, uv);

#if defined (POINT_COOKIE)
	atten *= texCUBEbias(_LightTexture0, float4(mul(unity_WorldToLight, half4(wpos, 1)).xyz, -8)).rgb;
#endif //POINT_COOKIE
#else
	half3 lightDir = 0;
	float atten = 0;
#endif

	outWorldPos = wpos;
	outUV = uv;
	outLightDir = lightDir;
	outAtten = atten;
	outFadeDist = fadeDist;
}

half4 CalculateLight (unity_v2f_deferred i)
{
    float3 wpos;
    float2 uv;
	float3 atten;
	float fadeDist;
    UnityLight light;
    UNITY_INITIALIZE_OUTPUT(UnityLight, light);
	UnityDeferredCalculateLightParamsColoredCookie(i, wpos, uv, light.dir, atten, fadeDist);

    light.color = _LightColor.rgb * atten;

    // unpack Gbuffer
    half4 gbuffer0 = tex2D (_CameraGBufferTexture0, uv);
    half4 gbuffer1 = tex2D (_CameraGBufferTexture1, uv);
    half4 gbuffer2 = tex2D (_CameraGBufferTexture2, uv);
    UnityStandardData data = UnityStandardDataFromGbuffer(gbuffer0, gbuffer1, gbuffer2);

    float3 eyeVec = normalize(wpos-_WorldSpaceCameraPos);
    half oneMinusReflectivity = 1 - SpecularStrength(data.specularColor.rgb);

    UnityIndirect ind;
    UNITY_INITIALIZE_OUTPUT(UnityIndirect, ind);
    ind.diffuse = 0;
    ind.specular = 0;

    half4 res = UNITY_BRDF_PBS (data.diffuseColor, data.specularColor, oneMinusReflectivity, data.smoothness, data.normalWorld, -eyeVec, light, ind);

    return res;
}

#ifdef UNITY_HDR_ON
half4
#else
fixed4
#endif
frag (unity_v2f_deferred i) : SV_Target
{
    half4 c = CalculateLight(i);
    #ifdef UNITY_HDR_ON
    return c;
    #else
    return exp2(-c);
    #endif
}

ENDCG
}


// Pass 2: Final decode pass.
// Used only with HDR off, to decode the logarithmic buffer into the main RT
Pass {
    ZTest Always Cull Off ZWrite Off
    Stencil {
        ref [_StencilNonBackground]
        readmask [_StencilNonBackground]
        // Normally just comp would be sufficient, but there's a bug and only front face stencil state is set (case 583207)
        compback equal
        compfront equal
    }

CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers nomrt

#include "UnityCG.cginc"

sampler2D _LightBuffer;
struct v2f {
    float4 vertex : SV_POSITION;
    float2 texcoord : TEXCOORD0;
};

v2f vert (float4 vertex : POSITION, float2 texcoord : TEXCOORD0)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(vertex);
    o.texcoord = texcoord.xy;
#ifdef UNITY_SINGLE_PASS_STEREO
    o.texcoord = TransformStereoScreenSpaceTex(o.texcoord, 1.0f);
#endif
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    return -log2(tex2D(_LightBuffer, i.texcoord));
}
ENDCG
}

}
Fallback Off
}
