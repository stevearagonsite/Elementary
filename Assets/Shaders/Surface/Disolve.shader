// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Disolve"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoColor("AlbedoColor", Color) = (0,0,0,0)
		_Noise("Noise", 2D) = "white" {}
		_DisolveAmount("DisolveAmount", Range( 0 , 1)) = 0.3556422
		_BorderThickness("BorderThickness", Float) = 0.03
		_BorderColor("BorderColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _AlbedoColor;
		uniform sampler2D _Noise;
		uniform float4 _Noise_ST;
		uniform float _BorderThickness;
		uniform float _DisolveAmount;
		uniform float4 _BorderColor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode26 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( tex2DNode26 * _AlbedoColor ).rgb;
			float2 uv_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			float4 tex2DNode1 = tex2D( _Noise, uv_Noise );
			float4 temp_cast_1 = (( _BorderThickness + _DisolveAmount )).xxxx;
			o.Emission = ( step( tex2DNode1 , temp_cast_1 ) * _BorderColor ).rgb;
			o.Alpha = 1;
			clip( tex2DNode1.r - _DisolveAmount );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
1927;58;1666;964;1325.066;361.6908;1.416896;True;False
Node;AmplifyShaderEditor.RangedFloatNode;2;-618.6216,559.9999;Float;False;Property;_DisolveAmount;DisolveAmount;3;0;Create;True;0;0;False;0;0.3556422;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-546.3576,479.2371;Float;False;Property;_BorderThickness;BorderThickness;4;0;Create;True;0;0;False;0;0.03;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-635.6234,219.9452;Float;True;Property;_Noise;Noise;2;0;Create;True;0;0;False;0;93ab99799ba9ff64b8b6b5973bb717a7;93ab99799ba9ff64b8b6b5973bb717a7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-312.5704,483.4879;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;22;-160.9625,578.4203;Float;False;Property;_BorderColor;BorderColor;5;0;Create;True;0;0;False;0;0,0,0,0;0.8823529,0.8823529,0.8823529,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;20;-160.9623,333.2967;Float;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;26;-264.3961,-222.1263;Float;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;27;-177.3803,-18.802;Float;False;Property;_AlbedoColor;AlbedoColor;1;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;118.7509,-74.06094;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;166.3406,431.0626;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;406.6495,14.16896;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Disolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;2;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;18;0
WireConnection;19;1;2;0
WireConnection;20;0;1;0
WireConnection;20;1;19;0
WireConnection;28;0;26;0
WireConnection;28;1;27;0
WireConnection;21;0;20;0
WireConnection;21;1;22;0
WireConnection;0;0;28;0
WireConnection;0;2;21;0
WireConnection;0;10;1;0
ASEEND*/
//CHKSM=71A97587A1594EE81853A13E5D9C19A31482C209