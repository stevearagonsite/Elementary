// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/Lambert/Basic Tessellation"
{
	Properties
	{
		_AlbedoColor("Albedo Color", Color) = (1,1,1,0)
		_AlbedoTexture("Albedo Texture", 2D) = "white" {}
		_NormalTexture("Normal Texture", 2D) = "bump" {}
		_Normalvalue("Normal value", Range( 0 , 1)) = 0
		_DisplacementTexture("Displacement Texture", 2D) = "white" {}
		_Displacementvalue("Displacement value", Range( 0.01 , 1)) = 0
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 4
		_TessMin( "Tess Min Distance", Float ) = 2
		_TessMax( "Tess Max Distance", Float ) = 25
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		uniform sampler2D _NormalTexture;
		uniform float4 _NormalTexture_ST;
		uniform float _Normalvalue;
		uniform sampler2D _AlbedoTexture;
		uniform float4 _AlbedoTexture_ST;
		uniform float4 _AlbedoColor;
		uniform sampler2D _DisplacementTexture;
		uniform float4 _DisplacementTexture_ST;
		uniform float _Displacementvalue;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata v )
		{
			float2 uv_DisplacementTexture = v.texcoord * _DisplacementTexture_ST.xy + _DisplacementTexture_ST.zw;
			float3 temp_cast_0 = (( tex2Dlod( _DisplacementTexture, float4( uv_DisplacementTexture, 0, 0.0) ).r * _Displacementvalue )).xxx;
			v.vertex.xyz += temp_cast_0;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_NormalTexture = i.uv_texcoord * _NormalTexture_ST.xy + _NormalTexture_ST.zw;
			float4 lerpResult6 = lerp( float4(0.4901961,0.4901961,1,0) , tex2D( _NormalTexture, uv_NormalTexture ) , _Normalvalue);
			o.Normal = lerpResult6.rgb;
			float2 uv_AlbedoTexture = i.uv_texcoord * _AlbedoTexture_ST.xy + _AlbedoTexture_ST.zw;
			o.Albedo = ( tex2D( _AlbedoTexture, uv_AlbedoTexture ) * _AlbedoColor ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13701
7;29;1352;692;1061.344;373.6451;1.156201;True;False
Node;AmplifyShaderEditor.ColorNode;4;-961.535,-108.8322;Float;False;Constant;_Colornormal;Color normal;6;0;0.4901961,0.4901961,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-988.8243,54.33679;Float;True;Property;_NormalTexture;Normal Texture;2;0;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;5;-577.683,-59.31219;Float;False;Property;_AlbedoColor;Albedo Color;0;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;9;-929.6899,341.069;Float;True;Property;_DisplacementTexture;Displacement Texture;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;8;-875.512,544.2169;Float;False;Property;_Displacementvalue;Displacement value;5;0;0;0.01;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;-972.4732,242.1466;Float;False;Property;_Normalvalue;Normal value;3;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-657.577,-245.1167;Float;True;Property;_AlbedoTexture;Albedo Texture;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-346.3781,-64.082;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-537.6475,486.481;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;6;-534.3731,131.8598;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;6;Float;ASEMaterialInspector;0;0;Lambert;Created/Lambert/Basic Tessellation;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;True;0;4;2;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;6;0;0;0;False;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;3;0
WireConnection;7;1;5;0
WireConnection;10;0;9;1
WireConnection;10;1;8;0
WireConnection;6;0;4;0
WireConnection;6;1;1;0
WireConnection;6;2;2;0
WireConnection;0;0;7;0
WireConnection;0;1;6;0
WireConnection;0;11;10;0
ASEEND*/
//CHKSM=1A3D8E46D4B4274A58BF6DAF31F370CAFCB22ACB