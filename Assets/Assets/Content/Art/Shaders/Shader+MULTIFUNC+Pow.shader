Shader "Playgendary/Shader+MULTIFUNC+Pow"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}          

        [Toggle(F_MAIN_TEXTURE_TRANSITION)] _F_texture_transition("Main texture transition", Int) = 0 //Doesn't work with triplanar mapping and distinct tiling/offset
        [HideInInspector] _SecondMainTex("Second Texture", 2D) = "white" {}  
        [HideInInspector] _TransitionFactor("Transition Factor", Range(0, 1)) = 0     
        
        [Toggle(F_DIFFUSE)] _F_diffuse("Diffuse", Int) = 0
        [HideInInspector] _DiffuseRampTex ("Diffuse Ramp Texture", 2D) = "white" {}
        
        [Toggle(F_TRIPLANAR_MAPPING)] _F_TriplanarMapping("Triplanar Mapping", Int) = 0
        
        [Toggle(F_SPECULAR)] _F_specular("Specular", Int) = 0
        [HideInInspector] _SpecTex ("Specular Texture", 2D) =  "white" {} 
        [HideInInspector] _SpecShininess ("Specular Shininess", Range(0, 1)) = 0.5
        [HideInInspector] _SpecIntensity ("Specular Intensity", Range(0, 1)) = 1       
        
        [Toggle(F_GRADIENT)] _F_gradient("Gradient", Int) = 0
        [HideInInspector] _GradientTex ("Gradient Texture", 2D) = "white" {}
        [HideInInspector] _GradientLimits ("Gradient Limits", Vector) = (0,0,0,0)
        
        [Toggle(F_EMISSION)] _F_emission("Emission", Int) = 0
        [HideInInspector] _EmissionMap("Emission Map", 2D) = "black" {}
        [HideInInspector] _EmissionIntensity("Emission Intensity", Range(0, 1)) = 1
        
        [Toggle(F_RIM)] _F_rim("Rim", Int) = 0
        [HideInInspector] _RimColor ("Rim Color", Color) = (1,1,1,1)
        [HideInInspector] _RimPower ("Rim Power", Range(0, 10)) = 3.0
        
        [Toggle(F_REFLECTION)] _F_reflection("Reflection", Int) = 0
        [HideInInspector] _ReflectionTex ("Reflection Texture", 2D) =  "white" {} 
        [HideInInspector] _ReflectionCube ("Reflection Cube", Cube) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
        LOD 100

        Pass
        {
            CGPROGRAM
            
            #pragma shader_feature F_DIFFUSE
            #pragma shader_feature F_TRIPLANAR_MAPPING
            #pragma shader_feature F_SPECULAR
            #pragma shader_feature F_GRADIENT
            #pragma shader_feature F_EMISSION
            #pragma shader_feature F_RIM
            #pragma shader_feature F_REFLECTION
            #pragma shader_feature F_MAIN_TEXTURE_TRANSITION
            
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight        

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
            
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 worldNormal : NORMAL;  
                
                float2 diffuseGradientUV : TEXCOORD2;       
                fixed4 color : TEXCOORD3;                     
                
                UNITY_FOG_COORDS(6)
                
                LIGHTING_COORDS(4,5)
            };
            
            
            sampler2D _MainTex;
            fixed4 _MainColor;   
            fixed4 _ShadowColor;
            float4 _MainTex_ST;     

            #if F_MAIN_TEXTURE_TRANSITION
            sampler2D _SecondMainTex;
            float _TransitionFactor;
            #endif    
            
            #if F_DIFFUSE
            uniform fixed4 _DiffuseColor;
            sampler1D _DiffuseRampTex;
            #endif
                        
            #if F_SPECULAR
            sampler2D _SpecTex;
            float _SpecShininess;
            float _SpecIntensity;
            #endif
                        
            #if F_GRADIENT
            sampler2D _GradientTex;
            float4 _GradientLimits;
            #endif
            
            #if F_EMISSION
            uniform sampler2D _EmissionMap;
            float _EmissionIntensity;
            #endif
            
            #if F_RIM
            uniform fixed4 _RimColor;
            uniform float _RimPower;
            #endif
            
            #if F_REFLECTION
            uniform sampler2D _ReflectionTex;
            uniform samplerCUBE _ReflectionCube;
            uniform fixed4 _ReflectionColor;             
            #endif
            
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
                
                output.pos = UnityObjectToClipPos(localPosition);
                output.uv = TRANSFORM_TEX(v.uv, _MainTex);
                output.posWorld = mul(modelMatrix, localPosition);
                output.worldNormal = UnityObjectToWorldNormal(v.normal);
                output.color = v.color;                        
                
                UNITY_TRANSFER_FOG(output, output.pos);                
                
                #if F_GRADIENT
                float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
                float factor = (worldVertex.y - _GradientLimits.x) / (_GradientLimits.y - _GradientLimits.x);                          
                output.diffuseGradientUV = half2(0, factor);    
                #endif                                                                                                                               
                                                                                                        
                TRANSFER_VERTEX_TO_FRAGMENT(output)                                             
                
                return output;
            }
            
            
            fixed4 frag(vertexOutput i) : SV_Target
            {
                half3 worldNormal = normalize(i.worldNormal);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
                
                float attenuation = 1.0;
                      
                fixed4 resultColor;          
                #if F_TRIPLANAR_MAPPING
                fixed4 cX = tex2D(_MainTex, TRANSFORM_TEX(i.posWorld.yz, _MainTex));
                fixed4 cY = tex2D(_MainTex, TRANSFORM_TEX(i.posWorld.xz, _MainTex));
                fixed4 cZ = tex2D(_MainTex, TRANSFORM_TEX(i.posWorld.xy, _MainTex));
                
                half3 blend = abs(worldNormal);
                blend /= blend.x + blend.y + blend.z + 0.001f;
                resultColor = cX * blend.x + cY * blend.y + cZ * blend.z;
                #elif F_MAIN_TEXTURE_TRANSITION
                resultColor  = lerp(tex2D(_MainTex, i.uv), tex2D(_SecondMainTex, i.uv), _TransitionFactor) * _MainColor;
                #else
                resultColor = tex2D(_MainTex, i.uv) * _MainColor;
                #endif
                                
               
                #if F_SPECULAR
                float3 specularReflection;
                
                if (dot(worldNormal, lightDirection) < 0.0) 
                {
                    specularReflection = float3(0.0, 0.0, 0.0);
                } 
                else 
                {
                    fixed4 specularColor = tex2D(_SpecTex, i.uv);

                    specularReflection = attenuation 
                    * specularColor.rgb * _SpecIntensity * pow(max(0.0, dot(
                    reflect(-lightDirection, worldNormal),
                    viewDirection)), (_SpecShininess * 199.0 + 1.0));
                }
                
                resultColor.rgb += specularReflection; 
                #endif              
               
                #if F_DIFFUSE
                half diff = 0.5 - dot(worldNormal, lightDirection) * 0.5;
                fixed3 diffuseReflection = tex1D(_DiffuseRampTex, diff).rgb;            
                resultColor.rgb *= diffuseReflection * tex1D(_DiffuseRampTex, 1.0 - LIGHT_ATTENUATION(i)).rgb;
                #else
                float att = LIGHT_ATTENUATION(i);
                resultColor.rgb = resultColor.rgb * att + _ShadowColor * (1.0f - att);
                #endif      
                

                #if F_GRADIENT      
                resultColor *= tex2D(_GradientTex, i.diffuseGradientUV);                
                #endif
                
                
                #if F_REFLECTION
                fixed4 reflectionColor = tex2D(_ReflectionTex, i.uv);
                
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.posWorld));                
                half3 worldReflection = reflect(-worldViewDir, worldNormal);

                resultColor.rgb += texCUBE(_ReflectionCube, worldReflection).rgb * reflectionColor.r;
                #endif
                             
                
                #if F_EMISSION
                resultColor.rgb += tex2D(_EmissionMap, i.uv).rgb * _EmissionIntensity;
                #endif
                
                
                #if F_RIM
                float rim = 1 - saturate(dot(normalize(viewDirection), worldNormal));
                float3 rimLighting = attenuation * _RimColor.rgb * pow(rim, _RimPower);
                resultColor.rgb += rimLighting;
                #endif  

                resultColor *= i.color;
                
                
                UNITY_APPLY_FOG(i.fogCoord, resultColor);          
                
                return resultColor;                                                               
                                                                                                               
            }
            ENDCG
        }          
                     
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"                           
    }
    
    CustomEditor "MultifuncShaderEditor"
}
