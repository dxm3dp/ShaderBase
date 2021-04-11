Shader "Custom/PBRTest"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Albedo(RGB)", 2D) = "white"{}
        _MetallicTex("Metallic(R), Smoothness(A)", 2D) = "white"{}
        _Metallic("Metallic", Range(0, 1)) = 1.0
        _Glossiness("Smoothness", Range(0, 1)) = 1.0
        [Normal]_Normal("NormalMap", 2D) = "bump"{}
        _OcclussionTex("Occlusion", 2D) = "white"{}
        _AO("AO", Range(0, 1)) = 1.0
        _Emission("Emission", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags {"LightMode"="ForwardBase"}
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma target 3.0

            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MetallicTex;
            fixed _Metallic;
            fixed _Glossiness;
            fixed _AO;
            half3 _Emission;
            sampler2D _Normal;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 tSpace0 : TEXCOORD2;
                float3 tSpace1 : TEXCOORD3;
                float3 tSpace2 : TEXCOORD4;

                UNITY_FOG_COORDS(5)
                UNITY_SHADOW_COORDS(6)

                #if UNITY_SHOULD_SAMPLE_SH
                    half3 sh : TEXCOORD7;
                #endif
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.pos = UnityObjectToClipPos(v.vertex);
                // o.uv.xy = 
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half3 worldTangent = UnityObjectToWorldDir(v.tangent);
                half3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w * 
                    unity_WorldTransformParams.w;
                
                o.tSpace0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
                o.tSpace1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
                o.tSpace2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);

                UNITY_TRANSFER_LIGHTING(o, v.texcoord1.xy);

                UNITY_TRANSFER_FOG(o, o.pos);

                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                half3 normalTex = UnpackNormal(tex2D(_Normal, i.uv));
                half3 worldNormal = half3(dot(i.tSpace0, normalTex), 
                    dot(i.tSpace1, normalTex), dot(i.tSpace2, normalTex));
                worldNormal = normalize(worldNormal);
                
                fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

                SurfaceOutputStandard o;
                UNITY_INITIALIZE_OUTPUT(SurfaceOutputStandard, o);
                // 对颜色贴图进行采样
                fixed4 AlbedoColorSampler = tex2D(_MainTex, i.uv) * _Color;
                o.Albedo = AlbedoColorSampler.rgb;
                // 自发光
                o.Emission = _Emission;
                // 对 Metallic-Smoothness 贴图进行进行采样
                fixed4 MetallicSmoothnessSampler = tex2D(_MetallicTex, i.uv);
                // r通道乘以控制系数并赋予金属度
                o.Metallic = MetallicSmoothnessSampler.r * _Metallic;
                // a通道乘以控制系数并赋予光滑度
                o.Smoothness = MetallicSmoothnessSampler.a * _Glossiness;
                // 单独赋予透明度
                o.Alpha = AlbedoColorSampler.a;
                // 对AO贴图进行采样乘以控制系数并赋予AO
                o.Occlusion = tex2D(_OcclussionTex, i.uv) * _AO;
                o.Normal = worldNormal;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

                UnityGI gi;
                UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
                gi.indirect.diffuse = 0;
                gi.indirect.specular = 0;
                gi.light.color = _LightColor0.rgb;
                gi.light.dir = lightDir;

                UnityGIInput giInput;
                UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
                giInput.light = gi.light;
                giInput.worldPos = i.worldPos;
                giInput.worldViewDir = worldViewDir;
                giInput.atten = atten;

                //反射探针相关
                giInput.probeHDR[0] = unity_SpecCube0_HDR;
                giInput.probeHDR[1] = unity_SpecCube1_HDR;
                #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
                    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
                #endif
                #ifdef UNITY_SPECCUBE_BOX_PROJECTION
                    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
                    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
                    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
                    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
                    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif


                return fixed4(1,1,1,1);
            }

            ENDCG
        }
    }
}
