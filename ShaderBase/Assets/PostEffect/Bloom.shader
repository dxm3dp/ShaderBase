Shader "Custom/Bloom"
{
    Properties
    {
        _MainTex("Base(RGB)", 2D) = "white"{}
    }

    CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;

        struct v2f
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        v2f vert(appdata_img i)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(i.vertex);
            o.uv = i.texcoord;
            return o;
        }

        half3 Sample(float2 uv)
        {
            return tex2D(_MainTex, uv).rgb;
        }

        half3 SampleBox(float2 uv)
        {
            float4 o = _MainTex_TexelSize.xyxy * float2(-1, 1).xxyy;
            half3 s = Sample(uv + o.xy) + Sample(uv + o.zy) + 
                Sample(uv + o.xw) + Sample(uv + o.zw);
            return s * 0.25f;
        }

        half3 SampleBox(float2 uv, float delta)
        {
            float4 o = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;
            half3 s = Sample(uv + o.xy) + Sample(uv + o.zy) + 
                Sample(uv + o.xw) + Sample(uv + o.zw);
            return s * 0.25f;
        }
    ENDCG

    SubShader
    {
        Cull Off
        ZTest Off
        ZWrite Off

        pass// 0
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                half4 frag(v2f i) : SV_TARGET
                {
                    return half4(SampleBox(i.uv, 1), 1);
                }
            ENDCG
        }

        pass// 1
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                half4 frag(v2f i) : SV_TARGET
                {
                    return half4(SampleBox(i.uv, 0.5), 1);
                }
            ENDCG
        }
    }
}