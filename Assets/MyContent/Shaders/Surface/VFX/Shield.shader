// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/VFX/Shield"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (0.9930021,0.4926471,1,0)
		_Normal("Normal", 2D) = "white" {}
		_Tiling("Tiling", Float) = 1
		_Alpha("Alpha", 2D) = "white" {}
		_Opacity("Opacity", Range( 0 , 2)) = 0.7
		_Power("Power", Range( 0 , 10)) = 0.5
		_BloomForce("BloomForce", Range( 0 , 0.5)) = 2.35
		_BloomColor("BloomColor", Color) = (0.9448277,1,0,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 texcoord_0;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Normal;
		uniform float _Tiling;
		uniform sampler2D _Albedo;
		uniform float4 _Color;
		uniform float _BloomForce;
		uniform float4 _BloomColor;
		uniform float _Power;
		uniform sampler2D _Alpha;
		uniform float _Opacity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (_Tiling).xx;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = tex2D( _Normal, i.texcoord_0 ).xyz;
			float4 temp_output_7_0 = ( tex2D( _Albedo, i.texcoord_0 ) * _Color );
			float4 temp_output_35_0 = ( _BloomForce * _BloomColor );
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelFinalVal16 = (0.0 + 1.0*pow( 1.0 - dot( ase_worldNormal, worldViewDir ) , ( _Power + -1.0 )));
			float4 temp_output_40_0 = smoothstep( temp_output_7_0 , ( temp_output_35_0 * fresnelFinalVal16 ) , _Color );
			o.Emission = ( temp_output_7_0 + temp_output_40_0 ).xyz;
			float fresnelFinalVal64 = (0.0 + 1.0*pow( 1.0 - dot( ase_worldNormal, worldViewDir ) , 5.0));
			o.Alpha = ( ( temp_output_40_0 + ( ( ( tex2D( _Alpha, i.texcoord_0 ) * _Color.a * Luminance(( temp_output_35_0 * fresnelFinalVal64 ).rgb) ) + -temp_output_40_0 ) / 2.0 ) ) * _Opacity ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
7;29;1352;692;343.155;-287.4798;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;57;-1820.202,665.801;Float;False;1102.002;628.5005;Bloom;10;54;21;38;27;20;55;35;16;28;44;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;38;-1587.601,791.0016;Float;False;Property;_BloomColor;BloomColor;8;0;0.9448277,1,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;81;-1276.849,1345.482;Float;False;685.5002;303;SmoothAlpha;3;64;65;69;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1607.202,715.801;Float;False;Property;_BloomForce;BloomForce;7;0;2.35;0;0.5;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;-1603.502,983.3007;Float;False;Constant;_Scale;Scale;6;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;54;-1651.901,1179.302;Float;False;Constant;_Const;Const;9;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;21;-1786.202,1092.6;Float;False;Property;_Power;Power;6;0;0.5;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;64;-1226.849,1419.082;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1244.901,770.5018;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;55;-1446.901,1095.902;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;-2061.799,-97.39999;Float;False;Property;_Tiling;Tiling;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1026.149,1395.482;Float;True;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TFHCGrayscale;69;-801.3487,1396.482;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;16;-1284.801,940.1998;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1758.099,-116.2;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;78;-439.655,1346.78;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1016.103,874.6016;Float;True;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;8;-1282.5,56.49998;Float;False;Property;_Color;Color;1;0;0.9930021,0.4926471,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;4;-1305.6,-145.4;Float;True;Property;_Albedo;Albedo;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Image;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;44;-788.8,780.201;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;75;-522.9526,688.3807;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;52;-1487.601,228.2999;Float;False;632.9999;370.0001;Alpha;2;12;15;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-929.6995,-140.9;Float;True;2;2;0;FLOAT4;0.0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;12;-1437.601,278.2999;Float;True;Property;_Alpha;Alpha;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Image;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;95;-460.5999,245.4004;Float;False;621.999;325.0999;Comment;4;46;45;48;47;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SmoothstepOpNode;40;-689.502,54.30064;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;76;-1022.253,595.9811;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;46;-410.5999,363.7003;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-987.601,280.2999;Float;False;3;3;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;48;-214.7009,455.5003;Float;False;Constant;_Float0;Float 0;10;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-221.8995,295.4003;Float;False;2;2;0;FLOAT4;0.0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleDivideOpNode;47;4.399062,295.8001;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.CommentaryNode;94;192.099,517.4788;Float;False;469.3426;303.1207;Comment;2;82;13;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;9;-1296.8,-359.3;Float;True;Property;_Normal;Normal;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Image;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;49;190.699,50.90028;Float;True;2;2;0;COLOR;0.0;False;1;FLOAT4;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;13;211.099,705.5995;Float;False;Property;_Opacity;Opacity;5;0;0.7;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-439.0002,-136.1994;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;518.4417,590.4788;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;50;334.4008,-268.9995;Float;False;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;784.8002,-137.9001;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Created/VFX/Shield;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;64;2;20;0
WireConnection;35;0;27;0
WireConnection;35;1;38;0
WireConnection;55;0;21;0
WireConnection;55;1;54;0
WireConnection;65;0;35;0
WireConnection;65;1;64;0
WireConnection;69;0;65;0
WireConnection;16;2;20;0
WireConnection;16;3;55;0
WireConnection;5;0;6;0
WireConnection;78;0;69;0
WireConnection;28;0;35;0
WireConnection;28;1;16;0
WireConnection;4;1;5;0
WireConnection;44;0;28;0
WireConnection;75;0;78;0
WireConnection;7;0;4;0
WireConnection;7;1;8;0
WireConnection;12;1;5;0
WireConnection;40;0;8;0
WireConnection;40;1;7;0
WireConnection;40;2;44;0
WireConnection;76;0;75;0
WireConnection;46;0;40;0
WireConnection;15;0;12;0
WireConnection;15;1;8;4
WireConnection;15;2;76;0
WireConnection;45;0;15;0
WireConnection;45;1;46;0
WireConnection;47;0;45;0
WireConnection;47;1;48;0
WireConnection;9;1;5;0
WireConnection;49;0;40;0
WireConnection;49;1;47;0
WireConnection;41;0;7;0
WireConnection;41;1;40;0
WireConnection;82;0;49;0
WireConnection;82;1;13;0
WireConnection;50;0;9;0
WireConnection;0;1;50;0
WireConnection;0;2;41;0
WireConnection;0;9;82;0
ASEEND*/
//CHKSM=3F9C2F535F66F1CFD07F55B886428AF4716AD043