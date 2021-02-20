Shader "PostEffect/WavePass"
{
    Properties
    {
        _MainTex ("Base(RGB)", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct v2f
    {
        half4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
        half4 uv01 : TEXCOORD1;
        half4 uv23 : TEXCOORD2;
        half4 uv45 : TEXCOORD3;
        half4 uv67 : TEXCOORD4;
    };

    sampler2D _MainTex;
    half4 _MainTex_TexelSize;
    half4 _Offsets;
    half4 _WaveStrength;

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv.xy = v.texcoord.xy;
        o.uv01 = v.texcoord.xyxy + _Offsets.xyxy * half4(1, 1, -1, -1);
        o.uv23 = v.texcoord.xyxy + _Offsets.xyxy * half4(1, 1, -1, -1) * 2.0;
        o.uv45 = v.texcoord.xyxy + _Offsets.xyxy * half4(1, 1, -1, -1) * 3.0;
        o.uv67 = v.texcoord.xyxy + _Offsets.xyxy * half4(1, 1, -1, -1) * 4.0;
        o.uv67 = v.texcoord.xyxy + _Offsets.xyxy * half4(1, 1, -1, -1) * 5.0;
        return o;
    }

    half4 frag(v2f i) : SV_TARGET
    {
        // 计算由uv点指向中点的向量
        half2 dv = half2(0.5, 0.5) - i.uv;
        // 根据屏幕长宽比对dv进行缩放
        dv = dv * half2(_ScreenParams.x / _ScreenParams.y, 1);
        // 计算像素点到中点的距离
        half dis = sqrt(dv.x * dv.x + dv.y * dv.y);
        // 
        half _distanceFactor = 2.0;
        half _totalFactor = 10.0;
        half _waveWidth = 0.25;
        half _curWaveDis = _WaveStrength;
        half sinFactor = sin(dis * _distanceFactor) * _totalFactor * 0.01;

        // 模糊处理
        half4 color = 0;
        color += 0.225 * tex2D(_MainTex, i.uv);
        color += 0.150 * tex2D(_MainTex, i.uv01.xy);
        color += 0.150 * tex2D(_MainTex, i.uv01.zw);
        color += 0.110 * tex2D(_MainTex, i.uv23.xy);
        color += 0.110 * tex2D(_MainTex, i.uv23.zw);
        color += 0.075 * tex2D(_MainTex, i.uv45.xy);
        color += 0.075 * tex2D(_MainTex, i.uv45.zw);
        color += 0.0525 * tex2D(_MainTex, i.uv67.xy);
        color += 0.0525 * tex2D(_MainTex, i.uv67.zw);
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
