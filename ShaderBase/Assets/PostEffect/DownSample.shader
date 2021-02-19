Shader "PostEffect/DownSample"
{
    Properties
    {
        _MainTex ("Base(RGB)", 2D) = "white" {}
    }
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

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv[4] : TEXCOORD0;
            };

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;

            v2f vert (appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv[0] = v.texcoord.xy + _MainTex_TexelSize.xy * 0.5;
                o.uv[1] = v.texcoord.xy - _MainTex_TexelSize.xy * 0.5;
                o.uv[2] = v.texcoord.xy - _MainTex_TexelSize.xy * half2(1, -1) * 0.5;
                o.uv[3] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(1, -1) * 0.5;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 0;
                col += tex2D(_MainTex, i.uv[0].xy);
                col += tex2D(_MainTex, i.uv[1].xy);
                col += tex2D(_MainTex, i.uv[2].xy);
                col += tex2D(_MainTex, i.uv[3].xy);
                return col / 4;
            }
            ENDCG
        }
    }
    Fallback off
}
