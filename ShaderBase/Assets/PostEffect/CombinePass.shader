﻿Shader "PostEffect/CombinePass"
{
    Properties
    {
        _MainTex ("Base(RGB)", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    #pragma multi_compile _ _BLOOM_ADD _BLOOM_SCREEN
    #pragma multi_compile _ _SATURATION
    #pragma multi_compile _ _COLOR_CURVE
    #pragma multi_compile _ _VIGNETTE_INTENSITY
    #pragma multi_compile _ _VIGNETTE_BLUR

    struct v2f
    {
        float4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
    };

    sampler2D _MainTex;

    #if _BLOOM_ADD || _BLOOM_SCREEN
        sampler2D _BloomTex;
        half _BloomIntensity;
    #endif

    #if _SATURATION
        fixed _Saturation;
    #endif

    #if _COLOR_CURVE
        sampler2D _CurveTex;
    #endif

    #if _VIGNETTE_INTENSITY
        fixed _VignetteIntensity;
    #endif

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord.xy;
        return o;
    }

    fixed4 frag(v2f i) : SV_TARGET
    {
        fixed4 color = tex2D(_MainTex, i.uv);

        #if _BLOOM_ADD
            half4 bloom = tex2D(_BloomTex, i.uv);
            color = _BloomIntensity * bloom + color;
        #endif

        #if _BLOOM_SCREEN
            half4 bloom = tex2D(_BloomTex, i.uv) * _BloomIntensity;
            color = 1 - (1 - bloom) * (1 - color);
        #endif

        #if _COLOR_CURVE
            fixed3 red = tex2D(_CurveTex, half2(color.r, 0.5 / 4.0)).rgb * fixed3(1, 0, 0);
            fixed3 green = tex2D(_CurveTex, half2(color.g, 1.5 / 4.0)).rgb * fixed3(0, 1, 0);
            fixed3 blue = tex2D(_CurveTex, half2(color.b, 2.5 / 4.0)).rgb * fixed3(0, 0, 1);
            color = red + green + blue;
        #endif

        #if _SATURATION
            fixed lum = Luminance(color.rgb);
            color.rgb = lerp(fixed3(lum, lum, lum), color.rgb, _Saturation);
        #endif

        #if _VIGNETTE_INTENSITY
            
        #endif

        return color;
    }

    ENDCG

    SubShader
    {
        Cull Off
        ZTest Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    Fallback off
}
