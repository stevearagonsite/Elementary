// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/Forest/Trees basic"
{
	Properties
	{
		_AlbedoColor("AlbedoColor", Color) = (0,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
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
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _AlbedoColor;
		uniform sampler2D _TreesVertex;
		uniform float4 _TreesVertex_ST;
		uniform float3 DisplacementVertex;
		uniform float _ScaleVertex;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 uv_TreesVertex = v.texcoord * _TreesVertex_ST.xy + _TreesVertex_ST.zw;
			v.vertex.xyz += ( float3( 0,0,0 ) * tex2Dlod( _TreesVertex, float4( uv_TreesVertex, 0, 0.0) ).g * ( DisplacementVertex * _ScaleVertex ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
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
7;29;1352;692;1842.754;945.1707;1.75201;True;False
Node;AmplifyShaderEditor.RangedFloatNode;38;-694.5293,624.9489;Float;False;Property;_ScaleVertex;ScaleVertex;13;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;36;-727.2523,429.2129;Float;False;Global;DisplacementVertex;DisplacementVertex;5;0;0,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-411.5056,-555.8207;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-472.5291,450.949;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;18;-740.249,138.1063;Float;True;Property;_TreesVertex;TreesVertex;12;0;Assets/Art/Shaders/Texture/LinearGradient.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;4;-331.6116,-370.0163;Float;False;Property;_AlbedoColor;AlbedoColor;0;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;7;-400.4861,-17.52583;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;None;True;0;False;white;Auto;False;Instance;3;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-245.94,280.5117;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-100.3067,-374.7861;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;73.32418,-131.3848;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Created/Forest/Trees basic;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;0;False;0;0;Custom;0.5;True;True;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;10;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;14;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;37;0;36;0
WireConnection;37;1;38;0
WireConnection;12;1;18;2
WireConnection;12;2;37;0
WireConnection;5;0;3;0
WireConnection;5;1;4;0
WireConnection;0;0;5;0
WireConnection;0;10;7;4
WireConnection;0;11;12;0
ASEEND*/
//CHKSM=C35C0FC5D9997761DB63D64C2AB69F7C6A1D1BCF