﻿Shader "GameCult/MaskyMixPro Tessellated" {
	Properties{
		_EdgeLength("Edge length", Range(2,50)) = 5
		_Phong("Phong Strengh", Range(-1,1)) = 0
		_SlopeDirection("Slope Test Direction", Vector) = (0,1,0,0)
		_SlopeLimit("Slope Limit", Range(-1.0, 1.0)) = 0.5
		_SlopeBlend("Slope Blend", Float) = 0
		_HeightLimit("Height Limit", Float) = 0
		_HeightBlend("Height Blend", Float) = 0
		_HeightLimit2("Height Limit 2", Float) = 0
		_HeightBlend2("Height Blend 2", Float) = 0
		_Displacement("Displacement", Range(0, 1.0)) = 0.3
		_DispOffset("Disp Offset", Range(-1, 1)) = 0.5
		_Color("Color", color) = (1,1,1,0)
		_Metallic("Metallic", Range(0, 1)) = 0.5
		_Glossiness("Smoothness", Range(0, 1)) = 0.5
		_MainTex("Base (RGB)", 2D) = "white" {}
	_MetalGloss("Metallic (R), Smoothness (A)", 2D) = "white" {}
	//          _Occlusion ("Ambient Occlusion", 2D) = "white" {}
	_DispTex("Disp Texture", 2D) = "gray" {}
	_NormalMap("Normalmap", 2D) = "bump" {}
	_TexSlope("Slope Base (RGB)", 2D) = "white" {}
	_MetalGlossSlope("Slope Metallic (R), Smoothness (A)", 2D) = "white" {}
	//         _OcclusionSlope ("Slope Ambient Occlusion", 2D) = "white" {}
	_DispTexSlope("Slope Disp Texture", 2D) = "gray" {}
	_NormalMapSlope("Slope Normalmap", 2D) = "bump" {}
	_TexHeight("Height Base (RGB)", 2D) = "white" {}
	_MetalGlossHeight("Height Metallic (R), Smoothness (A)", 2D) = "white" {}
	//        _OcclusionHeight ("Height Ambient Occlusion", 2D) = "white" {}
	_DispTexHeight("Height Disp Texture", 2D) = "gray" {}
	_NormalMapHeight("Height Normalmap", 2D) = "bump" {}
	_TexHeight2("Height 2 Base (RGB)", 2D) = "white" {}
	_MetalGlossHeight2("Height 2 Metallic (R), Smoothness (A)", 2D) = "white" {}
	//       _OcclusionHeight2 ("Height 2 Ambient Occlusion", 2D) = "white" {}
	_DispTexHeight2("Height 2 Disp Texture", 2D) = "gray" {}
	_NormalMapHeight2("Height 2 Normalmap", 2D) = "bump" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 300

		CGPROGRAM
#pragma surface surf Standard addshadow fullforwardshadows vertex:disp tessellate:tessEdge tessphong:_Phong
		//#include "FreeTess_Tessellator.cginc"
#include "Tessellation.cginc"

		struct appdata {
		float4 vertex : POSITION;
		float4 tangent : TANGENT;
		float3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
		float2 texcoord2 : TEXCOORD2;

		// we will use this to pass custom data to the surface function
		fixed4 color : COLOR;
	};

	struct Input {
		float2 uv_MainTex;
		// It is necessary to explicitly define the color variable here, as otherwise
		// it will not be visible to the surface shader
		float4 color : COLOR;
	};

	sampler2D _DispTex;
	uniform float4 _DispTex_ST;
	sampler2D _MainTex;
	sampler2D _MetalGloss;
	//            sampler2D _Occlusion;
	sampler2D _NormalMap;

	sampler2D _DispTexSlope;
	sampler2D _TexSlope;
	sampler2D _MetalGlossSlope;
	//            sampler2D _OcclusionSlope;
	sampler2D _NormalMapSlope;

	sampler2D _DispTexHeight;
	sampler2D _TexHeight;
	sampler2D _MetalGlossHeight;
	//            sampler2D _OcclusionHeight;
	sampler2D _NormalMapHeight;

	sampler2D _DispTexHeight2;
	sampler2D _TexHeight2;
	sampler2D _MetalGlossHeight2;
	//            sampler2D _OcclusionHeight2;
	sampler2D _NormalMapHeight2;

	fixed4 _Color;
	float _Metallic;
	float _Glossiness;
	float _Phong;
	float _EdgeLength;
	float _Displacement;
	float _DispOffset;

	float4 _SlopeDirection;
	float _SlopeLimit;
	float _SlopeBlend;
	float _HeightLimit;
	float _HeightLimit2;
	float _HeightBlend;
	float _HeightBlend2;

	float4 tessEdge(appdata v0, appdata v1, appdata v2)
	{
		return UnityEdgeLengthBasedTessCull(v0.vertex, v1.vertex, v2.vertex, _EdgeLength, _Displacement*abs(_DispOffset) * 2 * unity_ObjectToWorld[0].x);
	}

	void disp(inout appdata v)
	{
		v.color = float4(smoothstep(_SlopeLimit - _SlopeBlend,_SlopeLimit + _SlopeBlend,dot(_SlopeDirection.xyz,v.normal)),
			smoothstep(_HeightLimit - _HeightBlend,_HeightLimit + _HeightBlend,v.vertex.z),
			smoothstep(_HeightLimit2 - _HeightBlend2,_HeightLimit2 + _HeightBlend2,v.vertex.z),0);
		float d = tex2Dlod(_DispTex, float4(v.texcoord.xy * _DispTex_ST.xy + _DispTex_ST.zw,0,0)).r;
		d = lerp(d, tex2Dlod(_DispTexHeight, float4(v.texcoord.xy * _DispTex_ST.xy + _DispTex_ST.zw,0,0)).r, v.color.g);
		d = lerp(d, tex2Dlod(_DispTexHeight2, float4(v.texcoord.xy * _DispTex_ST.xy + _DispTex_ST.zw,0,0)).r, v.color.b);
		d = lerp(d, tex2Dlod(_DispTexSlope, float4(v.texcoord.xy * _DispTex_ST.xy + _DispTex_ST.zw,0,0)).r, v.color.r);
		d = (d - 0.5 + _DispOffset) * _Displacement;
		v.vertex.xyz += v.normal * d;
		//v.texcoord1 = float2(0,0);
	}

	void surf(Input IN, inout SurfaceOutputStandard o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		half2 ms = tex2D(_MetalGloss, IN.uv_MainTex).ra;
		//half oc = tex2D (_Occlusion, IN.uv_MainTex).r;
		half4 n = tex2D(_NormalMap, IN.uv_MainTex);

		c = lerp(c, tex2D(_TexHeight, IN.uv_MainTex) * _Color, IN.color.g);
		ms = lerp(ms, tex2D(_MetalGlossHeight, IN.uv_MainTex).ra, IN.color.g);
		//oc = lerp(oc, tex2D (_OcclusionHeight, IN.uv_MainTex).r, IN.color.g);
		n = lerp(n, tex2D(_NormalMapHeight, IN.uv_MainTex), IN.color.g);

		c = lerp(c, tex2D(_TexHeight2, IN.uv_MainTex) * _Color, IN.color.b);
		ms = lerp(ms, tex2D(_MetalGlossHeight2, IN.uv_MainTex).ra, IN.color.b);
		//oc = lerp(oc, tex2D (_OcclusionHeight2, IN.uv_MainTex).r, IN.color.b);
		n = lerp(n, tex2D(_NormalMapHeight2, IN.uv_MainTex), IN.color.b);

		c = lerp(c, tex2D(_TexSlope, IN.uv_MainTex) * _Color, IN.color.r);
		ms = lerp(ms, tex2D(_MetalGlossSlope, IN.uv_MainTex).ra, IN.color.r);
		//oc = lerp(oc, tex2D (_OcclusionSlope, IN.uv_MainTex).r, IN.color.r);
		n = lerp(n, tex2D(_NormalMapSlope, IN.uv_MainTex), IN.color.r);

		o.Albedo = c.rgb;
		o.Metallic = ms.r * _Metallic;
		o.Smoothness = ms.g *_Glossiness;
		//o.Occlusion = oc.r;
		o.Normal = UnpackNormal(n);
	}
	ENDCG
	}
		FallBack "Standard"
}