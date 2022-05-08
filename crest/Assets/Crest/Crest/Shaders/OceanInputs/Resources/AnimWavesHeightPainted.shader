﻿// Crest Ocean System

// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

// Adds height from the Painting feature

// TODO this is actually writing into the depth cache - setting the water level! By luck y is the correct channel.

Shader "Hidden/Crest/Inputs/Animated Waves/Painted Height"
{
	SubShader
	{
		// Additive blend everywhere
		Blend One One
		ZWrite Off
		ZTest Always
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			//#pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"

			#include "../../OceanGlobals.hlsl"
			#include "../../OceanInputsDriven.hlsl"
			#include "../../OceanHelpersNew.hlsl"
			#include "../../FullScreenTriangle.hlsl"

			Texture2DArray _WaveBuffer;
			Texture2D _PaintedWavesData;

			CBUFFER_START(CrestPerOceanInput)
			int _WaveBufferSliceIndex;
			float _Weight;
			float _AverageWavelength;
			CBUFFER_END

			CBUFFER_START(CrestPerMaterial)
			float2 _PaintedWavesSize;
			float2 _PaintedWavesPosition;
			CBUFFER_END

			struct Attributes
			{
				uint VertexID : SV_VertexID;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 worldPosXZ : TEXCOORD0;
			};

			Varyings Vert(Attributes input)
			{
				Varyings o;
				o.positionCS = GetFullScreenTriangleVertexPosition(input.VertexID);

				const float2 worldPosXZ = UVToWorld(GetFullScreenTriangleTexCoord(input.VertexID), _LD_SliceIndex, _CrestCascadeData[_LD_SliceIndex] );

				o.worldPosXZ = worldPosXZ;

				return o;
			}

			half4 Frag( Varyings input ) : SV_Target
			{
				half result = 0.0;

				if (all(_PaintedWavesSize > 0.0))
				{
					float2 paintUV = (input.worldPosXZ - _PaintedWavesPosition) / _PaintedWavesSize + 0.5;
					// Check if in bounds
					if (all(saturate(paintUV) == paintUV))
					{
						result = _PaintedWavesData.Sample(LODData_linear_clamp_sampler, paintUV).x;
					}
				}

				return _Weight * float4(0.0, result, 0.0, 0.0);
			}
			ENDCG
		}
	}
}
