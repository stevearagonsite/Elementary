// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/Lambert/Basic"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_AlbedoColor("Albedo Color", Color) = (1,1,1,0)
		_AlbedoTexture("Albedo Texture", 2D) = "white" {}
		_NormalTexture("Normal Texture", 2D) = "bump" {}
		_Normalvalue("Normal value", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalTexture;
		uniform float4 _NormalTexture_ST;
		uniform float _Normalvalue;
		uniform sampler2D _AlbedoTexture;
		uniform float4 _AlbedoTexture_ST;
		uniform float4 _AlbedoColor;

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
Version=13101
7;29;1352;692;1971.922;493.0823;2.056201;True;True
Node;AmplifyShaderEditor.RangedFloatNode;2;-972.4732,242.1466;Float;False;Property;_Normalvalue;Normal value;3;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-657.577,-245.1167;Float;True;Property;_AlbedoTexture;Albedo Texture;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;4;-961.535,-108.8322;Float;False;Constant;_Colornormal;Color normal;6;0;0.4901961,0.4901961,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-988.8243,54.33679;Float;True;Property;_NormalTexture;Normal Texture;2;0;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;5;-577.683,-59.31219;Float;False;Property;_AlbedoColor;Albedo Color;0;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;6;-534.3731,131.8598;Float;False;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-346.3781,-64.082;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;6;Float;ASEMaterialInspector;0;0;Lambert;Created/Lambert/Basic;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;2;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;4;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;4;0
WireConnection;6;1;1;0
WireConnection;6;2;2;0
WireConnection;7;0;3;0
WireConnection;7;1;5;0
WireConnection;0;0;7;0
WireConnection;0;1;6;0
ASEEND*/
//CHKSM=BDC1581C5D6BFF82FC6F5C237EDE05D37D313611