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
            ENDCG
        }
    }
}
