Shader "Playgendary/ConfitureFalling"
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

            uniform float heights[200];

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

            vertexOutput vert(vertexInput v)
            {
                vertexOutput output;

                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject;

                int index = (int)(v.color.a * 49.0f + 0.01f);

                float height = heights[index];
                height = -min(height * 0.5f, 0.0f);
                height *= height;
                height *= height;
                height *= 4.0f;
                height -= 2.5f;

                float odd = 1.0f;// saturate(index & 0x03);
                height += odd * saturate(height * 100.0f) * 100.0f;
                
                //height = odd;
                float4 localPosition = v.vertex + float4(v.uv.x, -height, v.uv.y, 0.0f);
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
