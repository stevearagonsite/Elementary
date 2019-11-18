// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Created/Toon/Disolve"
{
	Properties
	{
		_AlbedoColor("Albedo Color", Color) = (1,1,1,0)
		_Albedo("Albedo", 2D) = "white" {}
		_ToonRamp("Toon Ramp", 2D) = "white" {}
		_ControlShadow("ControlShadow", Range( 0 , 1)) = 0
		_NoiseTexture("NoiseTexture", 2D) = "white" {}
		_BorderThickness("BorderThickness", Float) = 0.01
		_EmissionColor("EmissionColor", Color) = (1,0,0,0)
		_Cutoff( "Mask Clip Value", Float ) = 0
		_Mask("Mask", Range( 0 , 1)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		Offset  0 , 1
		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Add
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			float3 worldPos;
		};

		uniform float4 _AlbedoColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _ToonRamp;
		uniform float _ControlShadow;
		uniform sampler2D _NoiseTexture;
		uniform float4 _NoiseTexture_ST;
		uniform float _BorderThickness;
		uniform float _Mask;
		uniform float4 _EmissionColor;
		uniform float _Cutoff = 0;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float dotResult3 = dot( i.worldNormal , ase_worldlightDir );
			float2 temp_cast_0 = (saturate( (dotResult3*0.5 + 0.5) )).xx;
			o.Albedo = ( ( _AlbedoColor * tex2D( _Albedo, uv_Albedo ) ) * ( tex2D( _ToonRamp, temp_cast_0 ) + _ControlShadow ) ).rgb;
			float2 uv_NoiseTexture = i.uv_texcoord * _NoiseTexture_ST.xy + _NoiseTexture_ST.zw;
			float4 tex2DNode66 = tex2D( _NoiseTexture, uv_NoiseTexture );
			float4 temp_cast_2 = (( _BorderThickness + _Mask )).xxxx;
			float4 temp_output_68_0 = step( tex2DNode66 , temp_cast_2 );
			o.Emission = ( temp_output_68_0 * _EmissionColor ).rgb;
			o.Alpha = 1;
			clip( tex2DNode66.r - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

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
				float3 worldNormal : TEXCOORD1;
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
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
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
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
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
Version=13501
1927;46;1666;974;1116.652;399.0606;1.3;True;False
Node;AmplifyShaderEditor.CommentaryNode;48;-1853.299,-160.7;Float;False;584.5353;436.0284;Comment;3;3;1;2;N . L;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;1;-1741.299,-112.7;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;2;-1789.299,47.30001;Float;False;1;0;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.CommentaryNode;51;-1246.099,-103.6995;Float;False;723.599;290;Also know as Lambert Wrap or Half Lambert;3;5;4;15;Diffuse Wrap;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;3;-1453.299,-48.7;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-1196.099,71.30051;Float;False;Constant;_WrapperValue;Wrapper Value;0;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ScaleAndOffsetNode;4;-930.6974,-53.69949;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;76;-634.3523,575.9395;Float;False;Property;_Mask;Mask;8;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;69;-565.1661,491.4731;Float;False;Property;_BorderThickness;BorderThickness;5;0;0.01;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;15;-697.4999,-54.09733;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-224,-80;Float;True;Property;_ToonRamp;Toon Ramp;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;57;-268.9664,-511.0154;Float;False;Property;_AlbedoColor;Albedo Color;0;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;66;-551.7179,262.8319;Float;True;Property;_NoiseTexture;NoiseTexture;4;0;Assets/Shaders/Texture/noise.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;43;-352.7859,-340.3509;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-331.4503,491.4401;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;54;-224.2505,136.9424;Float;False;Property;_ControlShadow;ControlShadow;3;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;72;61.14927,590.2397;Float;False;Property;_EmissionColor;EmissionColor;6;0;1,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;55.07486,-358.3414;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StepOpNode;68;-181.8303,375.7119;Float;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;56;107.0046,29.26566;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;250.1611,-106.3859;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TFHCGrayscale;70;59.78387,307.3798;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;308.1497,380.9399;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;713.1438,-100.3854;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Created/Toon/Disolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;0;True;0;1;Custom;0;True;True;0;True;TransparentCutout;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;Add;Add;0;False;0.02;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;7;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;4;2;5;0
WireConnection;15;0;4;0
WireConnection;6;1;15;0
WireConnection;75;0;69;0
WireConnection;75;1;76;0
WireConnection;58;0;57;0
WireConnection;58;1;43;0
WireConnection;68;0;66;0
WireConnection;68;1;75;0
WireConnection;56;0;6;0
WireConnection;56;1;54;0
WireConnection;42;0;58;0
WireConnection;42;1;56;0
WireConnection;70;0;68;0
WireConnection;71;0;68;0
WireConnection;71;1;72;0
WireConnection;0;0;42;0
WireConnection;0;2;71;0
WireConnection;0;10;66;0
ASEEND*/
//CHKSM=8A8D68641D5AFB502A5DDD2B12CB2F51191FE5C4