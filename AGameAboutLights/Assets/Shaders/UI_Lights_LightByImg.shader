Shader "UI Lights/LightByImg"
{
	Properties
	{
		_MainTex("Light Texture", 2D) = "white" {}
		_LightColor("Light Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_LightIntensity("Light Intensity", Range(0.0, 2.0)) = 1.0
	}

	SubShader
	{
		Pass
		{
			Blend DstColor One, Zero One
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			//user defined variables
			uniform sampler2D _MainTex;
			uniform float4 _LightColor;
			uniform float _LightIntensity;

			//in/out structs
			struct vertexInput
			{
				float4 vertex : POSITION;
				float2 uv_MainTex : TEXCOORD0;
			};
			struct vertexOutput
			{
				float4 vertex : POSITION;
				float2 uv_MainTex : TEXCOORD0;
			};

			//vertex
			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				o.uv_MainTex = v.uv_MainTex;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			//fragment
			float4 frag(vertexOutput i) : COLOR
			{
				float4 lightValue = tex2D(_MainTex, i.uv_MainTex);
				return _LightIntensity * _LightColor * lightValue;
			}
			ENDCG
		}
	}
}