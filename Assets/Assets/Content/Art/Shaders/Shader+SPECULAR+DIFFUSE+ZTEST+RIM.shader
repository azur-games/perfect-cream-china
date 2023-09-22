// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VR/SPECULAR+DIFFUSE+ZTEST+RIM" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Texture Color", Color) = (1,1,1,1)

        _LightY ("Light Y", Range(0, 360)) = 0.0

        _DiffuseColor ("Diffuse Color", Color) = (1,1,1,1)

       	_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
		_Shininess ("Specular Shininess", Range(0, 1)) = 0.5
		_SpecIntensity ("Specular Intensity", Range(0, 1)) = 1

		_RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0, 10)) = 3.0
    }


	SubShader 
	{
		Tags { "Queue" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
       	ZWrite Off

		Pass
		{
			ZWrite On
			ColorMask 0
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "VR.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _Color; // Texture Color

			uniform float _LightY;

			uniform float4 _DiffuseColor; // Diffuse Color

			uniform float4 _SpecColor; // Specular Color
			uniform float _Shininess; // Specular Shininess
			uniform float _SpecIntensity; // Specular Intensity

			uniform float4 _RimColor;
			uniform float _RimPower;


			struct vertexInput {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 normal : NORMAL;

				float2 uvMain : TEXCOORD0;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 color : COLOR;

				float2 uvMain : TEXCOORD0;
				float4 diffuse : TEXCOORD1;
				float4 spec : TEXCOORD2;
				float4 rimLighting : TEXCOORD3;
			};



			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				output.pos = UnityObjectToClipPos(input.vertex);
				output.uvMain = input.uvMain;
				output.color = input.color;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);

				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, input.vertex).xyz);
				float3 lightDirection = getLightRotation(50.0 / 180.0 * PI, _LightY / 180.0 * PI);
				float attenuation = 1.0;

				float3 diffuseReflection = lerp(_DiffuseColor.rgb, float3(1.0, 1.0, 1.0),
				clamp(attenuation * max(0.0, dot(normalDirection, lightDirection)), 0.0, 1.0));

				float4 specularReflection;

				if (dot(normalDirection, lightDirection) < 0.0) {
					specularReflection = float4(0.0, 0.0, 0.0, 0.0);
				} else {	
					specularReflection = attenuation 
					* _SpecColor * _SpecIntensity * pow(max(0.0, dot(
					reflect(-lightDirection, normalDirection),
					viewDirection)), (_Shininess * 199.0 + 1.0));
				}

				output.spec = specularReflection;
				output.diffuse = float4(diffuseReflection, 1.0);

				float rim = 1 - saturate(dot(normalize(viewDirection), normalDirection));
				output.rimLighting = float4(attenuation * _RimColor.xyz, 1) * pow(rim, _RimPower);

				return output;
			}


			fixed4 frag(vertexOutput input) : COLOR
			{
				float4 clearColor = tex2D(_MainTex, input.uvMain) * _Color * input.color;
				float4 resultColor = (clearColor + input.spec) * input.diffuse + input.rimLighting;

				return resultColor;
			}

			ENDCG
		}
	}
}

