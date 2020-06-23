// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/2sides"
{
	Properties
	{
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_Cutoff( "Mask Clip Value", Float ) = 0.084
		_FrontFacesColor("Front Faces Color", Color) = (1,0,0,0)
		_FrontFacesAlbedo("Front Faces Albedo", 2D) = "white" {}
		_FrontFacesNormal("Front Faces Normal", 2D) = "bump" {}
		_FrontFacesMetallic("Front Faces Metallic", 2D) = "white" {}
		_BackFacesColor("Back Faces Color", Color) = (0,0.04827571,1,0)
		_BackFacesAlbedo("Back Faces Albedo", 2D) = "white" {}
		_Smooth("Smooth", Float) = 0
		_Metal("Metal", Range( 0 , 1)) = 0
		_BackFacesNormal("Back Faces Normal", 2D) = "bump" {}
		_BackFacesMetallic("Back Faces Metallic", 2D) = "white" {}
		_OpacityMask("Opacity Mask", 2D) = "white" {}
		[HDR]_Emission("Emission", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		Stencil
		{
			Ref 1
			CompFront Always
			PassFront Replace
		}
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#include "Assets/VertExmotion/Shaders/VertExmotion.cginc"
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform sampler2D _FrontFacesNormal;
		uniform float4 _FrontFacesNormal_ST;
		uniform sampler2D _BackFacesNormal;
		uniform float4 _BackFacesNormal_ST;
		uniform float4 _FrontFacesColor;
		uniform sampler2D _FrontFacesAlbedo;
		uniform float4 _FrontFacesAlbedo_ST;
		uniform float4 _BackFacesColor;
		uniform sampler2D _BackFacesAlbedo;
		uniform float4 _BackFacesAlbedo_ST;
		uniform float4 _Emission;
		uniform sampler2D _FrontFacesMetallic;
		uniform float4 _FrontFacesMetallic_ST;
		uniform sampler2D _BackFacesMetallic;
		uniform float4 _BackFacesMetallic_ST;
		uniform float _Metal;
		uniform float _Smooth;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform sampler2D _OpacityMask;
		uniform float4 _OpacityMask_ST;
		uniform float _Cutoff = 0.084;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += VertExmotionASE(v);
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_FrontFacesNormal = i.uv_texcoord * _FrontFacesNormal_ST.xy + _FrontFacesNormal_ST.zw;
			float3 FrontFacesNormal51 = UnpackNormal( tex2D( _FrontFacesNormal, uv_FrontFacesNormal ) );
			float2 uv_BackFacesNormal = i.uv_texcoord * _BackFacesNormal_ST.xy + _BackFacesNormal_ST.zw;
			float3 BackFacesNormal54 = UnpackNormal( tex2D( _BackFacesNormal, uv_BackFacesNormal ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult20 = dot( ase_worldNormal , ase_worldViewDir );
			float FaceSign48 = (1.0 + (sign( dotResult20 ) - -1.0) * (0.0 - 1.0) / (1.0 - -1.0));
			float3 lerpResult64 = lerp( FrontFacesNormal51 , BackFacesNormal54 , FaceSign48);
			o.Normal = lerpResult64;
			float2 uv_FrontFacesAlbedo = i.uv_texcoord * _FrontFacesAlbedo_ST.xy + _FrontFacesAlbedo_ST.zw;
			float4 FrontFacesAlbedo44 = ( _FrontFacesColor * tex2D( _FrontFacesAlbedo, uv_FrontFacesAlbedo ) );
			float2 uv_BackFacesAlbedo = i.uv_texcoord * _BackFacesAlbedo_ST.xy + _BackFacesAlbedo_ST.zw;
			float4 BackFacesAlbedo47 = ( _BackFacesColor * tex2D( _BackFacesAlbedo, uv_BackFacesAlbedo ) );
			float4 lerpResult24 = lerp( FrontFacesAlbedo44 , BackFacesAlbedo47 , FaceSign48);
			o.Albedo = lerpResult24.rgb;
			o.Emission = _Emission.rgb;
			float2 uv_FrontFacesMetallic = i.uv_texcoord * _FrontFacesMetallic_ST.xy + _FrontFacesMetallic_ST.zw;
			float4 FrontMetallic74 = tex2D( _FrontFacesMetallic, uv_FrontFacesMetallic );
			float2 uv_BackFacesMetallic = i.uv_texcoord * _BackFacesMetallic_ST.xy + _BackFacesMetallic_ST.zw;
			float4 BackMetallic76 = tex2D( _BackFacesMetallic, uv_BackFacesMetallic );
			float4 lerpResult78 = lerp( FrontMetallic74 , BackMetallic76 , FaceSign48);
			o.Metallic = ( lerpResult78 * _Metal ).r;
			o.Smoothness = _Smooth;
			float2 uv_OpacityMask = i.uv_texcoord * _OpacityMask_ST.xy + _OpacityMask_ST.zw;
			float OpacityMask56 = tex2D( _OpacityMask, uv_OpacityMask ).a;
			float temp_output_58_0 = OpacityMask56;
			float3 temp_cast_3 = (temp_output_58_0).xxx;
			o.Translucency = temp_cast_3;
			o.Alpha = 1;
			clip( temp_output_58_0 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustom keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

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
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandardCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardCustom, o )
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
Version=17101
0;0;1920;1019;565.7632;476.3589;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;49;-1836.759,699.2838;Inherit;False;1094.131;402.4268;Comment;6;20;22;23;48;19;41;Face Sign (0 = Front, 1 = Back);1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;41;-1786.759,749.2839;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;19;-1761.539,917.7107;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;20;-1528.508,844.4049;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;52;-1810.675,-1065.252;Inherit;False;870.9222;707.2373;Comment;6;43;44;28;42;50;51;Front Faces;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;55;-909.7165,-1070.798;Inherit;False;865.924;714.2354;Comment;6;45;46;47;29;53;54;Back Faces;1,1,1,1;0;0
Node;AmplifyShaderEditor.SignOpNode;22;-1360.956,856.0175;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;75;-844.3566,-263.737;Inherit;True;Property;_BackFacesMetallic;Back Faces Metallic;17;0;Create;True;0;0;False;0;None;326a8885c4a02ad4a962c465f578e702;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;73;-1711.391,-277.6025;Inherit;True;Property;_FrontFacesMetallic;Front Faces Metallic;11;0;Create;True;0;0;False;0;None;326a8885c4a02ad4a962c465f578e702;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;29;-818.2955,-1020.797;Float;False;Property;_BackFacesColor;Back Faces Color;12;0;Create;True;0;0;False;0;0,0.04827571,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;45;-859.7165,-832.3703;Inherit;True;Property;_BackFacesAlbedo;Back Faces Albedo;13;0;Create;True;0;0;False;0;None;6ef69127653462f4aa7cb141db445315;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;42;-1760.675,-819.4415;Inherit;True;Property;_FrontFacesAlbedo;Front Faces Albedo;9;0;Create;True;0;0;False;0;None;6ef69127653462f4aa7cb141db445315;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;23;-1198.453,837.8569;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;28;-1742.167,-1015.252;Float;False;Property;_FrontFacesColor;Front Faces Color;8;0;Create;True;0;0;False;0;1,0,0,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-1368.146,-277.6025;Float;False;FrontMetallic;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;53;-807.7463,-586.5619;Inherit;True;Property;_BackFacesNormal;Back Faces Normal;16;0;Create;True;0;0;False;0;None;635100153fbea7447aaa9a58a21a9710;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;76;-512.4836,-250.1911;Float;False;BackMetallic;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;50;-1734.204,-588.0151;Inherit;True;Property;_FrontFacesNormal;Front Faces Normal;10;0;Create;True;0;0;False;0;None;635100153fbea7447aaa9a58a21a9710;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-976.6271,834.203;Float;False;FaceSign;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-469.2787,-860.2587;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1392.549,-883.5837;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;57;-676.4657,727.2352;Inherit;False;626.0693;280;Comment;2;56;27;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-188.4406,417.4052;Inherit;False;48;FaceSign;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-219.4406,288.4052;Inherit;False;76;BackMetallic;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;-221.4406,188.4052;Inherit;False;74;FrontMetallic;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-1191.753,-883.5831;Float;False;FrontFacesAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-475.8735,-573.016;Float;False;BackFacesNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;-290.7924,-860.2587;Float;False;BackFacesAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;27;-626.4657,777.2352;Inherit;True;Property;_OpacityMask;Opacity Mask;18;0;Create;True;0;0;False;0;None;6ef69127653462f4aa7cb141db445315;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-1392.172,-588.0151;Float;False;FrontFacesNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;92.06564,-443.866;Inherit;False;48;FaceSign;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;69.94625,-166.7323;Inherit;False;48;FaceSign;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;44.01965,-257.4734;Inherit;False;54;BackFacesNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;82;366.5594,276.4052;Inherit;False;Property;_Metal;Metal;15;0;Create;True;0;0;False;0;0;0.843;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;45.73564,-534.6071;Inherit;False;47;BackFacesAlbedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;68.09934,-630.6602;Inherit;False;44;FrontFacesAlbedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-290.3963,839.452;Float;False;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;62;45.97994,-351.6714;Inherit;False;51;FrontFacesNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;78;151.3811,249.9991;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;67;307.6212,-160.2447;Inherit;False;Property;_Emission;Emission;19;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;58;35.12917,-14.52736;Inherit;False;56;OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;64;303.545,-293.6472;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertExmotionASENode;66;260.5704,67.78078;Inherit;False;0;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;24;331.738,-491.5313;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;698.2368,193.6411;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;86;614.2368,104.6411;Inherit;False;Property;_Smooth;Smooth;14;0;Create;True;0;0;False;0;0;1.63;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1029.206,-293.0633;Float;False;True;2;ASEMaterialInspector;0;0;Standard;Custom/2sides;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.084;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;14;all;True;True;True;True;0;False;-1;True;1;False;-1;255;False;-1;255;False;-1;7;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;1,0.4344827,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;;7;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;1;Include;;True;0d37e36ddb6c45c4db973485a27eadd4;Custom;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;41;0
WireConnection;20;1;19;0
WireConnection;22;0;20;0
WireConnection;23;0;22;0
WireConnection;74;0;73;0
WireConnection;76;0;75;0
WireConnection;48;0;23;0
WireConnection;46;0;29;0
WireConnection;46;1;45;0
WireConnection;43;0;28;0
WireConnection;43;1;42;0
WireConnection;44;0;43;0
WireConnection;54;0;53;0
WireConnection;47;0;46;0
WireConnection;51;0;50;0
WireConnection;56;0;27;4
WireConnection;78;0;79;0
WireConnection;78;1;80;0
WireConnection;78;2;81;0
WireConnection;64;0;62;0
WireConnection;64;1;63;0
WireConnection;64;2;65;0
WireConnection;24;0;59;0
WireConnection;24;1;60;0
WireConnection;24;2;61;0
WireConnection;85;0;78;0
WireConnection;85;1;82;0
WireConnection;0;0;24;0
WireConnection;0;1;64;0
WireConnection;0;2;67;0
WireConnection;0;3;85;0
WireConnection;0;4;86;0
WireConnection;0;7;58;0
WireConnection;0;10;58;0
WireConnection;0;11;66;0
ASEEND*/
//CHKSM=542A03A51808BC9B228462D325EE6908D4657579