// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/BagScreen"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_VFX("VFX", Color) = (1,0,0,0)
		_Force("Force", Range( 0 , 20)) = 1
		_Normal("Normal", 2D) = "white" {}
		_Icon("Icon", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Icon;
		uniform float4 _Icon_ST;
		uniform float4 _VFX;
		uniform float _Force;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float4 tex2DNode12 = tex2D( _Normal, uv_Normal );
			float3 temp_cast_0 = (( tex2DNode12.r + tex2DNode12.g )).xxx;
			o.Normal = temp_cast_0;
			float2 uv_Icon = i.uv_texcoord * _Icon_ST.xy + _Icon_ST.zw;
			float4 tex2DNode15 = tex2D( _Icon, uv_Icon );
			float4 temp_cast_1 = (tex2DNode15.r).xxxx;
			float4 lerpResult20 = lerp( temp_cast_1 , _VFX , tex2DNode15.a);
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelFinalVal9 = (0.0 + 1.0*pow( 1.0 - dot( ase_worldNormal, worldViewDir ) , 5.0));
			o.Emission = ( tex2DNode12 + ( ( lerpResult20 * tex2D( _Icon, uv_Icon ).a ) + ( float4( 0,0,0,0 ) + ( ( _Force * _VFX ) * fresnelFinalVal9 ) ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
7;29;1352;692;2192.334;755.3086;2.957756;True;True
Node;AmplifyShaderEditor.CommentaryNode;10;-1218.121,469.4293;Float;False;743.1998;475.8499;Bloom VFX;5;8;9;5;4;6;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1168.121,519.4296;Float;False;Property;_Force;Force;1;0;1;0;20;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;4;-1131.521,620.4294;Float;False;Property;_VFX;VFX;0;0;1,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;15;-1123.605,-53.36761;Float;True;Property;_Icon;Icon;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Image;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-846.5217,603.2295;Float;False;2;2;0;FLOAT;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.FresnelNode;9;-892.8222,766.2794;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;51;-738.4582,212.9126;Float;True;Property;_TextureSample0;Texture Sample 0;4;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Image;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;20;-702.0793,-28.01889;Float;True;3;0;FLOAT;0.0;False;1;COLOR;0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-643.9218,742.2292;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-317.2737,-28.26474;Float;True;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-322.38,203.4971;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;12;-622.8546,-381.072;Float;True;Property;_Normal;Normal;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Image;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;44;6.090953,-27.50192;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;54;-294.3352,-215.8874;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-1.183293,-351.2817;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;53;171.0784,-216.4036;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;446.0082,-373.8098;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Created/BagScreen;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;8;0
WireConnection;5;1;4;0
WireConnection;20;0;15;1
WireConnection;20;1;4;0
WireConnection;20;2;15;4
WireConnection;6;0;5;0
WireConnection;6;1;9;0
WireConnection;50;0;20;0
WireConnection;50;1;51;4
WireConnection;14;1;6;0
WireConnection;44;0;50;0
WireConnection;44;1;14;0
WireConnection;54;0;12;0
WireConnection;48;0;12;1
WireConnection;48;1;12;2
WireConnection;53;0;54;0
WireConnection;53;1;44;0
WireConnection;0;1;48;0
WireConnection;0;2;53;0
ASEEND*/
//CHKSM=053C7554B964BF05C55B86D5A15CCCF2BCC13A21