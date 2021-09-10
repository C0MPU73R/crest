// Crest Ocean System

// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

Shader "Crest/Hidden/Water Boundary Geometry"
{
	SubShader
	{
		CGINCLUDE
		#pragma vertex Vert
		#pragma fragment Frag

		#include "UnityCG.cginc"

		struct Attributes
		{
			float3 positionOS : POSITION;
		};

		struct Varyings
		{
			float4 positionCS : SV_POSITION;
		};

		Varyings Vert(Attributes input)
		{
			Varyings o;
			o.positionCS = UnityObjectToClipPos(input.positionOS);
			return o;
		}

		half4 Frag(Varyings input) : SV_Target
		{
			return 1.0;
		}
		ENDCG

		Pass
		{
			Name "Outer Boundary"
			Cull Back

			CGPROGRAM
			ENDCG
		}

		Pass
		{
			Name "Inner Boundary"
			Cull Front

			CGPROGRAM
			ENDCG
		}
	}
}
