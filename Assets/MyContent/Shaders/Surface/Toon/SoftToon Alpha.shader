// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/Toon/SoftToonAlpha"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_ASEOutlineWidth( "Outline Width", Float ) = 0.001
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,1)
		[Header(SoftToon)]
		_Cutofblend("Cut of blend", Range( 0.01 , 3)) = 0.4
		_Cutoffangle("Cut off angle", Range( -1 , 1)) = 0.03
		_ColorAlbedo("ColorAlbedo", Color) = (1,1,1,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_Alpha("Alpha", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:outlineVertexDataFunc
		uniform fixed4 _ASEOutlineColor;
		uniform fixed _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline fixed4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return fixed4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o ) { o.Emission = _ASEOutlineColor.rgb; o.Alpha = 1; }
		ENDCG
		

		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
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
		uniform float _Alpha;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).xyz;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 ase_vertex4Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_objectlightDir = normalize( ObjSpaceLightDir( ase_vertex4Pos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float dotResult12_g1 = dot( -ase_objectlightDir , ase_vertexNormal );
			float temp_output_14_0_g1 = smoothstep( ( _Cutoffangle + _Cutofblend ) , ( _Cutoffangle - _Cutofblend ) , dotResult12_g1 );
			float4 blendOpSrc316 = ( tex2D( _Albedo, uv_Albedo ) * _ColorAlbedo );
			float4 blendOpDest316 = ( ( _LightColor0 * temp_output_14_0_g1 ) + ( _LightColor0 * ( 1.0 - temp_output_14_0_g1 ) * 0.05553238 ) );
			o.Emission = ( ( ( saturate( ( blendOpSrc316 * blendOpDest316 ) )) / 1.5 ) * _Alpha ).rgb;
			o.Alpha = ( _Alpha * tex2D( _Albedo, uv_Albedo ).a );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred 

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
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
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
7;29;1352;692;1076.354;186.6869;1.503842;True;True
Node;AmplifyShaderEditor.ColorNode;308;-814.0145,323.9727;Float;False;Property;_ColorAlbedo;ColorAlbedo;4;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;306;-861.4153,140.1725;Float;True;Property;_Albedo;Albedo;5;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Image;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;319;-230.7457,104.6132;Float;False;246.2002;151.3;Screen;1;316;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;318;70.96758,96.1018;Float;False;445.5153;236.875;ASE dobles all your emission color automatically;2;317;315;;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;337;-346.0853,417.1269;Float;False;SoftToon;0;;1;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;309;-516.0153,143.5725;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.BlendOpsNode;316;-185.1459,147.0124;Float;False;Multiply;True;2;0;COLOR;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;315;102.1544,232.367;Float;False;Constant;_DivEmission;DivEmission;3;0;1.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;317;370.4011,153.4672;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;332;255.9938,416.5723;Float;False;Property;_Alpha;Alpha;7;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;336;129.6932,504.8809;Float;True;Property;_TextureSample0;Texture Sample 0;9;0;None;True;0;False;white;Auto;False;Instance;306;Auto;Image;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;133;241.7904,-107.3857;Float;True;Property;_Normal;Normal;6;0;Assets/Art/Shaders/Texture/ShieldTileNormal.png;True;0;False;white;Auto;False;Object;-1;Auto;Image;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;333;563.9938,257.5723;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;335;557.851,472.6307;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;747.3,110.6001;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Created/Toon/SoftToonAlpha;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;5;10;25;True;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;True;0.001;0,0,0,1;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;309;0;306;0
WireConnection;309;1;308;0
WireConnection;316;0;309;0
WireConnection;316;1;337;0
WireConnection;317;0;316;0
WireConnection;317;1;315;0
WireConnection;333;0;317;0
WireConnection;333;1;332;0
WireConnection;335;0;332;0
WireConnection;335;1;336;4
WireConnection;0;1;133;0
WireConnection;0;2;333;0
WireConnection;0;9;335;0
ASEEND*/
//CHKSM=7DF18D5C727482B5AA98FFA3BD14B235173431B5