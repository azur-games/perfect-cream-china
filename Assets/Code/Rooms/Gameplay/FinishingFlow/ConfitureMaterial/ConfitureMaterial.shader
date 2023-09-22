Shader "Playgendary/Confiture"
{
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"

            uniform float scales[200];

            struct vertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct vertexOutput
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float3 normal : TEXCOORD0;
            };

            /*uniform float4 pows[10];
            uniform float minHeight;

            float4 PreparePows(float4 pos)
            {
                return pos;
                float ampScale = saturate((pos.y - minHeight) * 0.5f) * 0.05f;
                float3 dpos = 0.0f;
                for (int i = 0; i < 10; i++)
                {
                    float4 pow = pows[i];
                    float w = pow.w;
                    float amp = saturate(w * 1000.0f);

                    float3 toPow = pow.xyz - pos.xyz;
                    float distSqr = dot(toPow, toPow);
                    float dist = sqrt(distSqr);

                    dpos += amp * ampScale * toPow * saturate(5.0f * (w * 10.0f - distSqr * 0.333f)) / distSqr;
                }

                dpos.y *= 0.015f;
                pos.xyz -= dpos;

                return pos;
            }*/

            vertexOutput vert(vertexInput v)
            {
                vertexOutput output;

                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject;

                float4 localPosition = v.vertex;

                float height = scales[(int)(v.color.a * 100.0f + 0.01f)];
                localPosition *= saturate(-height * 100.0f);

                output.pos = UnityObjectToClipPos(localPosition);
                output.normal = v.normal;
                output.color = v.color;

                return output;
            }

            fixed4 frag(vertexOutput i) : SV_Target
            {
                float3 ambient = ShadeSH9(half4(i.normal, 1));
                half nl = max(0, dot(i.normal, _WorldSpaceLightPos0.xyz));
                float3 diff = nl * _LightColor0.rgb;

                float4 clr = i.color;
                clr.rgb = clr.rgb * (diff + 0.8f) + ambient;

                return clr;
            }

            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }

    CustomEditor "MultifuncShaderEditor"
}
