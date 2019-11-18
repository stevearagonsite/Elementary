// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/Toon/SoftToon"
{
	Properties
	{
		[Header(SoftToon)]
		_Cutofblend("Cut of blend", Range( 0.01 , 3)) = 0.4
		_Cutoffangle("Cut off angle", Range( -1 , 1)) = 0.03
		_HitPosition("Hit Position", Vector) = (0,0,0,0)
		_ColorAlbedo("ColorAlbedo", Color) = (1,1,1,0)
		_RadiusDamege("RadiusDamege", Float) = 0
		_Albedo("Albedo", 2D) = "white" {}
		_ImpactVector("ImpactVector", Vector) = (0,0,0,0)
		_Normal("Normal", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _ColorAlbedo;
		uniform float _Cutoffangle;
		uniform fixed _Cutofblend;
		uniform float3 _HitPosition;
		uniform float _RadiusDamege;
		uniform float3 _ImpactVector;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float clampResult329 = clamp( ( distance( ase_worldPos , _HitPosition ) / _RadiusDamege ) , 0.0 , 1.0 );
			v.vertex.xyz += ( clampResult329 * _ImpactVector );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 ase_vertex4Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_objectlightDir = normalize( ObjSpaceLightDir( ase_vertex4Pos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float dotResult12_g4 = dot( -ase_objectlightDir , ase_vertexNormal );
			float smoothstepResult14_g4 = smoothstep( ( _Cutoffangle + _Cutofblend ) , ( _Cutoffangle - _Cutofblend ) , dotResult12_g4);
			float4 blendOpSrc316 = ( tex2D( _Albedo, uv_Albedo ) * _ColorAlbedo );
			float4 blendOpDest316 = ( ( _LightColor0 * smoothstepResult14_g4 ) + ( _LightColor0 * ( 1.0 - smoothstepResult14_g4 ) * 0.05553238 ) );
			o.Emission = ( ( saturate( ( blendOpSrc316 * blendOpDest316 ) )) / 1.5 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
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
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				float4 texcoords01 : TEXCOORD4;
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
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
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
				surfIN.uv_texcoord.xy = IN.texcoords01.xy;
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
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13701
1687;55;1177;948;365.8965;241.392;1.3;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;324;-401.316,550.8521;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;325;-387.4518,717.224;Float;False;Property;_HitPosition;Hit Position;4;0;0,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;328;-248.8093,897.4595;Float;False;Property;_RadiusDamege;RadiusDamege;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;326;-167.3562,627.1058;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;306;-861.4153,140.1725;Float;True;Property;_Albedo;Albedo;5;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;308;-814.0145,323.9727;Float;False;Property;_ColorAlbedo;ColorAlbedo;4;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;327;49.2732,689.4951;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.FunctionNode;323;-378.507,391.4724;Float;False;SoftToon;0;;4;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;309;-516.0153,143.5725;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.CommentaryNode;318;70.96758,96.1018;Float;False;445.5153;236.875;ASE dobles all your emission color automatically;2;317;315;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;319;-230.7457,104.6132;Float;False;246.2002;151.3;Screen;1;316;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;329;231.2423,689.4951;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;315;102.1544,232.367;Float;False;Constant;_DivEmission;DivEmission;3;0;1.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;316;-185.1459,147.0124;Float;False;Multiply;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.Vector3Node;330;189.6493,869.7313;Float;False;Property;_ImpactVector;ImpactVector;6;0;0,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;331;446.139,831.6042;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleDivideOpNode;317;370.4011,153.4672;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;133;241.7904,-107.3857;Float;True;Property;_Normal;Normal;6;0;Assets/Art/Shaders/Texture/ShieldTileNormal.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;746.3,111.6001;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Created/Toon/SoftToon;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;5;10;25;True;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0.01;1,0,0,1;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;326;0;324;0
WireConnection;326;1;325;0
WireConnection;327;0;326;0
WireConnection;327;1;328;0
WireConnection;309;0;306;0
WireConnection;309;1;308;0
WireConnection;329;0;327;0
WireConnection;316;0;309;0
WireConnection;316;1;323;0
WireConnection;331;0;329;0
WireConnection;331;1;330;0
WireConnection;317;0;316;0
WireConnection;317;1;315;0
WireConnection;0;1;133;0
WireConnection;0;2;317;0
WireConnection;0;11;331;0
ASEEND*/
//CHKSM=3F6A47A65183A41E41A2072F7193052995D633F0