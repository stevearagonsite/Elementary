// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/VFX/Line Animation"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_AlphaAnimation("AlphaAnimation", 2D) = "white" {}
		_Color("Color", Color) = (0,1,0.1724138,0)
		_Speed("Speed", Range( -2 , 2)) = -1
		_Tilling("Tilling", Range( 0 , 2)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IsEmissive" = "true"  }
		Cull Back
		Blend One One
		BlendOp Add
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.5
		#pragma surface surf StandardCustomLighting keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform fixed4 _Color;
		uniform sampler2D _AlphaAnimation;
		uniform fixed _Tilling;
		uniform fixed _Speed;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			fixed2 temp_cast_0 = (_Tilling).xx;
			float mulTime14 = _Time.y * _Speed;
			fixed2 temp_cast_1 = (mulTime14).xx;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + temp_cast_1;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			fixed2 temp_cast_1 = (i.texcoord_0.x).xx;
			fixed4 tex2DNode1 = tex2D( _AlphaAnimation, temp_cast_1 );
			c.rgb = _Color.rgb;
			c.a = tex2DNode1.r;
			clip( tex2DNode1.r - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13701
7;29;1352;692;1391.368;334.2458;1.552754;True;False
Node;AmplifyShaderEditor.RangedFloatNode;21;-1441.3,290.2001;Float;False;Property;_Speed;Speed;3;0;-1;-2;2;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;14;-1131.4,296;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;74;-1070.301,204.2996;Float;False;Property;_Tilling;Tilling;4;0;1;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;73;-738.5992,250.4998;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;82;-451.6055,5.599182;Float;False;Property;_Color;Color;2;0;0,1,0.1724138,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-416,225.5;Float;True;Property;_AlphaAnimation;AlphaAnimation;1;0;Assets/Art/Shaders/Texture/TrailAnimation.psd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2,-1;Fixed;False;True;3;Fixed;ASEMaterialInspector;0;0;CustomLighting;Created/VFX/Line Animation;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;False;0;True;Transparent;Overlay;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;False;4;One;One;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;21;0
WireConnection;73;0;74;0
WireConnection;73;1;14;0
WireConnection;1;1;73;1
WireConnection;0;2;82;0
WireConnection;0;9;1;0
WireConnection;0;10;1;0
ASEEND*/
//CHKSM=A3BEDAD31116115AD5F7E9B46F28122398F85179