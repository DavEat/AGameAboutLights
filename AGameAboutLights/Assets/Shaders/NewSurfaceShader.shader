﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "own/markercelll"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MainColor("_MainColor", Color) = (1,1,1,1)
		_StartFade("StartFade", float) = 0.35
	}
		SubShader
		{
			Tags{ "DisableBatching" = "true" "Queue" = "Transparent" }
			Pass
			{
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MaskTex;
				uniform fixed4 _MainColor;
				uniform float _Scale;
				float _StartFade;


				struct vertOut
				{
					float4 pos : SV_POSITION;
					float2 tex : TEXCOORD0;
					float3 lpos : TEXCOORD2;
				};


				vertOut vert(appdata_base input)
				{
					vertOut output;


					fixed4 pos = input.vertex;

					output.lpos = pos;
					output.pos = UnityObjectToClipPos(pos);
					output.tex = input.texcoord;
					return output;
				}

				fixed4 frag(vertOut input) : COLOR0
				{
					fixed4 output = fixed4(0,0,0,0);
					//output.a = 1;
					//output.rgb = input.lpos;
					output.rgb = _MainColor.rgb;

					if (input.lpos.y > _StartFade)
						output.r = 0.1f;					

					return output;
				}
			ENDCG
		}
		}
}