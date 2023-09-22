Shader "Playgendary/ShapeProgressShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FillingColor ("FillingColor", Color) = (0, 0, 0, 1)
        _FillingProgress("FillingProgress", float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        LOD 100
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _FillingColor;
            float _FillingProgress;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float filled = saturate(50.0f * (i.uv.y - _FillingProgress));
                
                col.rgb = lerp(col.rgb, _FillingColor.rgb, filled) * col.a;

                return col;
            }
            ENDCG
        }
    }
}
