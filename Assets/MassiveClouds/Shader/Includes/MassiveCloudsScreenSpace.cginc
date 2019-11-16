#ifndef MASSIVE_CLOUDS_SCREEN_SPACE_INCLUDED
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
#define MASSIVE_CLOUDS_SCREEN_SPACE_INCLUDED

#include "UnityCG.cginc"

#include "MassiveCloudsInput.cginc"

struct ScreenSpace
{
    float3           cameraPos;
    float3           worldPos;
    float            maxDist;
    float            isMaxPlane;
    float3           rayDir;
    float            depth;
};


#if defined(MASSIVE_CLOUDS_MATERIAL_ON)

float4 CalculateWorldPos(float4 uv, float depth)
{
#if defined(USING_STEREO_MATRICES)
    float4 posProjection = float4(2 * (2 * (uv.x - 0.5 * unity_StereoEyeIndex)) - 1, 2 * uv.y - 1, 1, 1);
    float3 view          = mul(unity_CameraInvProjection, posProjection) * _ProjectionParams.z;
    view = view * depth;
    view.z *= -1; // revert z on all platform
#else
    float4 posProjection = float4(2 * uv.x - 1, 2 * uv.y - 1, 1, 1);
    float3 view          = mul(unity_CameraInvProjection, posProjection) * _ProjectionParams.z;
    view = view * depth;
    view.z *= -1; // revert z on all platform
#endif
    return mul(unity_CameraToWorld, float4(view, 1));
}

#else

float4 CalculateWorldPos(float4 uv, float depth)
{
    float4 posProjection = float4(- 1 + 2 * uv.xy, 1, 1);
    float3 view          = mul(unity_CameraInvProjection, posProjection) * _ProjectionParams.z;
    view = view * depth;
    view.z *= -1; // revert z on all platform
    return mul(unity_CameraToWorld, float4(view, 1));
}

#endif

float CalculateDepth(float4 uv)
{
#if defined(USING_STEREO_MATRICES)
    float  depth          = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(uv, _CameraDepthTexture_ST))));
#else
    float  depth          = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, uv)));
#endif
   return depth;
}

float CalculateDepth(float4 uv, out float4 neighbors)
{
   neighbors.x = CalculateDepth(uv + float4(_CameraDepthTexture_TexelSize.x, 0, 0, 0));
   neighbors.y = CalculateDepth(uv + float4(-_CameraDepthTexture_TexelSize.x, 0, 0, 0));
   neighbors.z = CalculateDepth(uv + float4(0, _CameraDepthTexture_TexelSize.y, 0, 0));
   neighbors.w = CalculateDepth(uv + float4(0, -_CameraDepthTexture_TexelSize.y, 0, 0));
   return CalculateDepth(uv);
}

ScreenSpace CreateScreenSpace(float4 uv)
{
    ScreenSpace ss;
    float       depth          = CalculateDepth(uv);
    float3      cameraPos      = _WorldSpaceCameraPos;
    float3      world          = CalculateWorldPos(uv, depth);
    float3      rayDir         = normalize(world.xyz - cameraPos);
    float       isMaxPlane     = saturate(depth - 0.999) * 1000;
    float       maxDist        = length(world.xyz - cameraPos);
    
    ss.cameraPos  = cameraPos;
    ss.worldPos   = world;
    ss.isMaxPlane = isMaxPlane;
    ss.maxDist    = maxDist;
    ss.rayDir     = rayDir;
    ss.depth      = depth;

    return ss;
}

ScreenSpace CreateBlendedScreenSpace(float4 uv)
{
    ScreenSpace ss;
    float4      neighbors;
    float       depth          = CalculateDepth(uv, neighbors);
    depth = max(depth, max(neighbors.x, max(neighbors.y, max(neighbors.z, neighbors.w))));
    float3      cameraPos      = _WorldSpaceCameraPos;
    float3      world          = CalculateWorldPos(uv, depth);
    float3      rayDir         = normalize(world.xyz - cameraPos);
    float       isMaxPlane     = saturate(depth - 0.999) * 1000;
    float       maxDist        = length(world.xyz - cameraPos);
    
    ss.cameraPos  = cameraPos;
    ss.worldPos   = world;
    ss.isMaxPlane = isMaxPlane;
    ss.maxDist    = maxDist;
    ss.rayDir     = rayDir;

    return ss;
}

float3 Ramp(float3 screenCol, float factor)
{
    return screenCol +
        _RampStrength *
        tex2D( _RampTex, float2(1 - factor / _RampScale + _RampOffset * _RampScale, 0.5));
}

#endif