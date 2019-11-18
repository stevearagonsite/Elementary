Shader "Created/Vignette"
{
	Properties
	{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		_Vignette ("Viñeta", 2D) = "black" {}
		_Color ("Color", Color) = (0,0,0,0)
		_Power ("float", float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _Vignette;
			float4 _Color;
			float _Power;

			fixed4 frag (v2f i) : SV_Target
			{
				
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 mask = tex2D(_Vignette, i.uv);
				return lerp(col, _Color, mask * _Power);
			}
			ENDCG
		}
	}
}
