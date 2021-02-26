Shader "PostEffect/BrightPass"
{
    Properties
    {
        _MainTex ("Base(RGB)", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct v2f
    {
        float4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
    };

    sampler2D _MainTex;
    half4 _Threshold;

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord;
        return o;
    }

    half4 frag(v2f i) : SV_TARGET
    {
        half4 color = tex2D(_MainTex, i.uv);
        color.rgb = max(half3(0, 0, 0), color.rgb - _Threshold.rgb);
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
