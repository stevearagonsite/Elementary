#ifndef MASSIVE_CLOUDS_INCLUDED
#define MASSIVE_CLOUDS_INCLUDED

#include "Includes/MassiveCloudsLight.cginc"
#include "Includes/MassiveCloudsSampler.cginc"
#include "Includes/MassiveCloudsPostProcess.cginc"

#include "MassiveCloudsRaymarch.cginc"

sampler2D _MainTex;
half4     _MainTex_ST;
float4    _MainTex_TexelSize;

sampler2D _ScreenTexture;
half4     _ScreenTexture_ST;
sampler2D _CloudTexture;


struct appdata
{
    float4 vertex : POSITION;
    float4 uv : TEXCOORD0;
};

struct v2f
{
    float4 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

v2f MassiveCloudsVert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0) {
            o.uv.y = 1 - o.uv.y;
        }
    #endif
    return o;
}

fixed4 MassiveCloudsFragment(v2f i) : SV_Target
{
    PrepareSampler();
    PrepareLighting();

    ScreenSpace ss = CreateScreenSpace(i.uv);

#ifdef _HORIZONTAL_ON
    HorizontalRegion region = CreateRegion();
    Ray ray = CalculateHorizontalRayRange(ss, region);
#else
    Ray ray = CalculateSphericalRayRange(ss);
#endif

    half4 screenCol = tex2Dproj(_ScreenTexture, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
    half4 texCol = tex2Dproj(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));

    #if defined(_SHADOW_ON) && defined(_HORIZONTAL_ON)
    ScreenSpaceShadow(screenCol, ss);
    #endif

    #if defined(_HEIGHTFOG_ON)
    HeightFogFragment(screenCol, ss);
    #endif

#if !defined(_RENDERER_AUTHENTIC)
    // Optimize
    if (ray.from >= _MaxDistance) return screenCol;
    if (ray.to <= 0) return screenCol;
    float iterationScale = 1 + saturate(-0.5 + _Optimize * (ray.from/1000));
    float iteration = (_Iteration / iterationScale);
#else
    float iteration = _Iteration;
#endif

    // RelativeHeight
    float3 fixedCameraPos = ss.cameraPos;
    fixedCameraPos.y *= (1 - _RelativeHeight);

#if defined(_RENDERER_SURFACE)
    fixed4 col = WorldSpaceSurfaceRaymarch(screenCol,
                                    fixedCameraPos,
                                    ss.rayDir,
                                    ray,
                                    iteration);
#elif defined(_RENDERER_AUTHENTIC)
    fixed4 col = WorldSpaceAuthenticRaymarch(screenCol,
                                    fixedCameraPos,
                                    ss.rayDir,
                                    ray,
                                    iteration,
                                    ss);
#else
    fixed4 col = WorldSpaceRaymarch(screenCol,
                                    fixedCameraPos,
                                    ss.rayDir,
                                    ray,
                                    iteration);
#endif

    col = PostProcess(col);
    float finalA = max(texCol.a, col.a);
    col = lerp(float4(texCol.rgb, finalA), float4(col.rgb, finalA), col.a);

    return col;
}
#endif