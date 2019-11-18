// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/Forest/Trees Tessellation"
{
	Properties
	{
		_AlbedoColor("AlbedoColor", Color) = (0,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_NormalValue("NormalValue", Range( 0 , 1)) = 0
		_DispacementTexture("Dispacement Texture", 2D) = "white" {}
		_DisplacementValue("DisplacementValue", Range( 0.01 , 1)) = 0
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 10
		_TessMin( "Tess Min Distance", Float ) = 10
		_TessMax( "Tess Max Distance", Float ) = 25
		_TreesVertex("TreesVertex", 2D) = "white" {}
		_ScaleVertex("ScaleVertex", Float) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
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

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalValue;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _AlbedoColor;
		uniform sampler2D _DispacementTexture;
		uniform float4 _DispacementTexture_ST;
		uniform float _DisplacementValue;
		uniform sampler2D _TreesVertex;
		uniform float4 _TreesVertex_ST;
		uniform float3 DisplacementVertex;
		uniform float _ScaleVertex;
		uniform float _Cutoff = 0.5;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata v )
		{
			float2 uv_DispacementTexture = v.texcoord * _DispacementTexture_ST.xy + _DispacementTexture_ST.zw;
			float2 uv_TreesVertex = v.texcoord * _TreesVertex_ST.xy + _TreesVertex_ST.zw;
			float4 tex2DNode18 = tex2Dlod( _TreesVertex, float4( uv_TreesVertex, 0, 0.0) );
			v.vertex.xyz += ( ( ( tex2Dlod( _DispacementTexture, float4( uv_DispacementTexture, 0, 0.0) ).r * _DisplacementValue ) + tex2DNode18.r ) * tex2DNode18.g * ( DisplacementVertex * _ScaleVertex ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float4 lerpResult41 = lerp( float4(0.4901961,0.4901961,1,0) , tex2D( _Normal, uv_Normal ) , _NormalValue);
			o.Normal = lerpResult41.rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = ( tex2D( _Albedo, uv_Albedo ) * _AlbedoColor ).rgb;
			o.Alpha = 1;
			clip( tex2D( _Albedo, uv_Albedo ).a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13701
7;29;1352;692;1349.301;215.3139;1.349083;True;False
Node;AmplifyShaderEditor.SamplerNode;46;-1276.456,39.06754;Float;True;Property;_DispacementTexture;Dispacement Texture;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;44;-1226.354,247.6507;Float;False;Property;_DisplacementValue;DisplacementValue;5;0;0;0.01;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;18;-1062.001,367.9373;Float;True;Property;_TreesVertex;TreesVertex;12;0;Assets/Art/Shaders/Texture/LinearGradient.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-884.4128,184.4795;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;36;-1056.895,561.1936;Float;False;Global;DisplacementVertex;DisplacementVertex;5;0;0,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;38;-1024.172,756.9296;Float;False;Property;_ScaleVertex;ScaleVertex;13;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-516.6867,-561.4555;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-669.129,273.055;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;39;-820.6447,-425.171;Float;False;Constant;_ColorNormal;Color Normal;6;0;0.4901961,0.4901961,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-847.934,-262.002;Float;True;Property;_Normal;Normal;2;0;None;True;0;True;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;40;-832.7391,-74.1922;Float;False;Property;_NormalValue;NormalValue;3;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-802.1717,582.9297;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ColorNode;4;-436.7927,-375.651;Float;False;Property;_AlbedoColor;AlbedoColor;0;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;41;-393.4828,-184.479;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-422.4939,336.737;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;7;-495.3109,-4.456442;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;None;True;0;False;white;Auto;False;Instance;3;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-205.4878,-380.4208;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;186.0181,-135.1413;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Created/Forest/Trees Tessellation;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;0;False;0;0;Custom;0.5;True;True;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;True;0;10;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;14;-1;-1;6;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;45;0;46;1
WireConnection;45;1;44;0
WireConnection;47;0;45;0
WireConnection;47;1;18;1
WireConnection;37;0;36;0
WireConnection;37;1;38;0
WireConnection;41;0;39;0
WireConnection;41;1;1;0
WireConnection;41;2;40;0
WireConnection;12;0;47;0
WireConnection;12;1;18;2
WireConnection;12;2;37;0
WireConnection;5;0;3;0
WireConnection;5;1;4;0
WireConnection;0;0;5;0
WireConnection;0;1;41;0
WireConnection;0;10;7;4
WireConnection;0;11;12;0
ASEEND*/
//CHKSM=40FAE1C8E496446AC0DA1F08C8CA53E00E79B7CA