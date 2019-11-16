#ifndef MASSIVE_CLOUDS_HEIGHT_FOG_INCLUDED
#define MASSIVE_CLOUDS_HEIGHT_FOG_INCLUDED

#include "Includes/MassiveCloudsScreenSpace.cginc"
#include "Includes/MassiveCloudsFog.cginc"

sampler2D _MainTex;
half4     _MainTex_ST;
float4    _MainTex_TexelSize;

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
    ScreenSpace ss = CreateScreenSpace(i.uv);

    half4 screenCol = tex2Dproj(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
    #if defined(_HEIGHTFOG_ON)
    HeightFogFragment(screenCol, ss);
    #endif
    screenCol.a = 0;
    return screenCol;
}

#endif