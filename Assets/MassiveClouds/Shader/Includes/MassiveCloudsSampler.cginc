#ifndef MASSIVE_CLOUDS_SAMPLER_INCLUDED
#define MASSIVE_CLOUDS_SAMPLER_INCLUDED

#include "MassiveCloudsInput.cginc"
#include "MassiveCloudsShape.cginc"

inline void PrepareSampler()
{
    _TimeFactor = _Time.y / 3600;
    _TexOffset = _ScrollVelocity.xyz * 1000 * _TimeFactor;
}

inline float ClipHorizontalDensity(
    float3 pos,
    float  density)
{
    HorizontalRegion horizontalRegion = CreateRegion();

#if defined(_HORIZONTAL_ON)
    float softnessLengthTop    = horizontalRegion.thickness * (0.01 + horizontalRegion.softness[1]);
    float softnessLengthBottom = horizontalRegion.thickness / 2 * (0.01 + horizontalRegion.softness[0]);

    float top                  = horizontalRegion.height + horizontalRegion.thickness;
    float bottom               = horizontalRegion.height;

    float topRate              = saturate((top - pos.y)     / softnessLengthTop);
    float bottomRate           = saturate((pos.y -  bottom) / softnessLengthBottom);

    float sculptFactorTop    = 1 - topRate;
    float sculptFactorBottom = 1 - bottomRate;
    float sculptFactor       = pow(pow(density, 1.5), 2 * _HorizontalSoftnessFigure)
                             * max(sculptFactorTop, pow(sculptFactorBottom, 2))
                             ;
    float fadeTop    = saturate(topRate * 10);
    float fadeBottom = saturate(bottomRate * 10);

    return saturate(fadeTop * fadeBottom * density - pow(sculptFactor, 1));
#else

    return density;

#endif
}

inline float SampleDensity(float3 pos, float scale, float phase)
{
    float bias = 10000;
    float3 texScale  = _VolumeTex_ST.xyx / bias / scale;
    float3 texOffset = (_TexOffset + _ScrollOffset) * phase * texScale;
    float3 texPos    =  pos * texScale + texOffset;

    return tex3Dlod(_VolumeTex, float4(texPos.xyz, 0)).a;
}

float SampleDetailedDensity(float3 pos)
{
    float  base         = SampleDensity(pos, _Scale, 1);
    float  baseInv      = 1 - base;
    float  dist         = length(pos - _WorldSpaceCameraPos);
    float  distFactor   = dist / 60000;
    float  sculpture    = _Sculpture * (1 + 2 * exp(-dist * 0.001));

    if (_Octave > 1)
    {
        float o = _Octave;
        float octave = SampleDensity(pos, _Scale / o, 1 + _Phase);
        float hardSculpture = baseInv * saturate(2 * sculpture - 1);
        float a = lerp(base, octave, base * baseInv * saturate(2 * sculpture));
        base = a * ( (1 - base * hardSculpture) + base * octave * hardSculpture);
        if (dist < _DetailDistance / 2)
        {
            float o2 = 4 * _Octave;
            float octave2 = SampleDensity(pos, _Scale / o2, 1 + _Phase);
            base = base + base * (1 - pow(dist / _DetailDistance * 2, 0.8)) * sculpture * (1 - a) * (0.5 - octave2);
        }
        else if (dist < _DetailDistance)
        {
            float o2 = 2 * _Octave;
            float octave2 = SampleDensity(pos, _Scale / o2, 1 + _Phase);
            base = base + base * (1 - pow(dist / _DetailDistance, 0.8)) * sculpture * (1 - a) * (0.5 - octave2);
        }
    }

    // Softness
    float softnessnFactor = 1 - 0.9 * saturate(_Softness);
    float density = pow(base, softnessnFactor) * softnessnFactor;
    float densityFactor = 0.9 * _Softness;
    float densityScale = ClipHorizontalDensity(pos, _Density);
    density = saturate(density - (1 - densityFactor) * (1 - densityScale));
    density = pow(density, 0.1 + densityFactor);
    base = density * softnessnFactor;
    base = ClipHorizontalDensity(pos, base);
    return base;
}
#endif