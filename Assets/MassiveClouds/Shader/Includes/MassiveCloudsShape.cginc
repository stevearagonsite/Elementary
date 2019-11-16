#ifndef MASSIVE_CLOUDS_SHAPE_INCLUDED
#define MASSIVE_CLOUDS_SHAPE_INCLUDED

#include "MassiveCloudsScreenSpace.cginc"
#include "MassiveCloudsInput.cginc"

struct HorizontalRegion
{
    float height;
    float thickness;
    float2 softness;
};

struct Ray
{
    float from;
    float to;
    float max;
    float length;
};

inline Ray CalculateHorizontalRayRange(
          ScreenSpace      ss,
          HorizontalRegion region)
{
    const float3 up = float3(0, 1, 0);
    float maxDist = min(_MaxDistance, max(ss.maxDist, ss.isMaxPlane * _MaxDistance));

    float  dbottom          = (region.height                      - (1 - _RelativeHeight) * ss.cameraPos.y);
    float  dtop             = ((region.height  + region.thickness) - (1 - _RelativeHeight) * ss.cameraPos.y);
    float  horizontalFactor = dot(ss.rayDir, up);
    float  bottomDist       = max(0, dbottom / horizontalFactor);
    float  topDist          = max(0, dtop / horizontalFactor);
    
    float  fromDist         = min(bottomDist, topDist);
    float  toDist           = max(bottomDist, topDist);

    Ray ray;
#ifdef MASSIVE_CLOUDS_MATERIAL_ON
    ray.from   = min(maxDist, fromDist);
    ray.to     = min(maxDist, toDist);
#else
    if (maxDist > fromDist && toDist > fromDist)
//    if (toDist > fromDist)
    {
        ray.from   = min(maxDist, fromDist);
        ray.to     = min(maxDist, toDist);
    }
    else
    {
        ray.from   = fromDist;
        ray.to     = toDist;
    }
#endif
    ray.max    = maxDist;

    ray.length = max(0, ray.to - ray.from);
    ray.length = min(_MaxDistance / 4, ray.length);
    return ray;
}

inline Ray CalculateSphericalRayRange(ScreenSpace ss)
{
    Ray ray;
    ray.from   = _FromDistance;
//    ray.to     = lerp(min(_MaxDistance, ss.maxDist), _MaxDistance, ss.isMaxPlane);
    ray.to     = min(min(_MaxDistance, ss.maxDist), _FromDistance + _Thickness);
    ray.length = _Thickness;
    return ray;
}

HorizontalRegion CreateRegion()
{
    HorizontalRegion horizontalRegion;
    horizontalRegion.height = _FromHeight;
    horizontalRegion.thickness = _Thickness;
    horizontalRegion.softness = float2(_HorizontalSoftnessBottom, _HorizontalSoftnessTop);
    return horizontalRegion;
}


#endif