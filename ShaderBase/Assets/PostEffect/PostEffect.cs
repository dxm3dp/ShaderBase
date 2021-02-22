using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public sealed class PostEffect : MonoBehaviour
{
    static int threshholdID = -1;
    static int offsetsID = -1;
    static int bloomTexID = -1;
    static int bloomIntensityID = -1;
    static int saturationID = -1;
    static int curveTexID = -1;
    static int vignetteIntensityID = -1;
    static int waveStengthID = -1;

    [Tooltip("The shader for down sample")]
    Shader downSampleShader;

    [Tooltip("The shader for brightPass")]
    Shader brightPassShader;

    [Tooltip("The shader for blur pass")]
    Shader blurPassShader;

    [Tooltip("The shader for combine pass")]
    Shader combinePassShader;

    [Tooltip("The shader for wave pass")]
    Shader wavePassShader;


    [Tooltip("whether to enable the bloom")]
    bool enableBloom;

    [Tooltip("The bloom blend mode")]
    BloomBlendMode bloomBlendMode = BloomBlendMode.Add;

    [Tooltip("The bloom intensity")]
    float bloomIntensity = 0.5f;

    [Tooltip("The bloom threshold")]
    [Range(-0.05f, 4f)]
    float bloomThreshold = 0.5f;

    [Tooltip("The bloom threshold color")]
    Color bloomThresoldColor = Color.white;

    [Tooltip("The bloom blur spread")]
    float bloomBlurSpread = 2.5f;

    [Tooltip("Whether to enable saturation control")]
    bool enableColorCurve;

    [Tooltip("The color correction curve for red channel")]
    AnimationCurve redChannelCurve =
        new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    [Tooltip("The color correction curve for green channel")]
    AnimationCurve greenChannelCurve =
        new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    [Tooltip("The color correction curve for blue channel")]
    AnimationCurve blueChannelCurve =
        new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    [Tooltip("Whether to enable saturation control")]
    bool enableSaturation;

    [Tooltip("The saturation for the image")]
    [Range(0f, 5f)]
    float saturation = 1f;

    [Tooltip("Whether to enable vignette")]
    bool enableVignette;

    [Tooltip("The intensity for vignette")]
    float vignetteIntensity = 0.375f;

    [Tooltip("Whether to enable blur the screen")]
    bool enableBlur;

    [Tooltip("The blur spread")]
    [Range(0f, 10f)]
    float blurSpread = 2.5f;

    [Tooltip("The wave strength")]
    [Range(0f, 1f)]
    float waveStrength = 0.02f;

    static int OffsetsID
    {
        get
        {
            if(offsetsID == -1)
            {
                offsetsID = Shader.PropertyToID("_Offsets");
            }
            return offsetsID;
        }
    }
    static int BloomTexID
    {
        get
        {
            if(bloomTexID == -1)
            {
                bloomTexID = Shader.PropertyToID("_BloomTex");
            }
            return bloomTexID;
        }
    }
    static int BloomIntensityID
    {
        get
        {
            if(bloomIntensityID == -1)
            {
                bloomIntensityID = Shader.PropertyToID("_BloomIntensity");
            }
            return bloomIntensityID;
        }
    }
    static int ThreshholdID
    {
        get
        {
            if(threshholdID == -1)
            {
                threshholdID = Shader.PropertyToID("_Threshhold");
            }
            return threshholdID;
        }
    }
    static int WaveStrengthID
    {
        get
        {
            if(waveStengthID == -1)
            {
                waveStengthID = Shader.PropertyToID("_WaveStrength");
            }
            return waveStengthID;
        }
    }
    static int SaturationID
    {
        get
        {
            if(saturationID == -1)
            {
                saturationID = Shader.PropertyToID("_Saturation");
            }
            return saturationID;
        }
    }
    static int CurveTexID
    {
        get
        {
            if(curveTexID == -1)
            {
                curveTexID = Shader.PropertyToID("_CurveTex");
            }
            return curveTexID;
        }
    }
    static int VignetteIntensityID
    {
        get
        {
            if(vignetteIntensityID == -1)
            {
                vignetteIntensityID = Shader.PropertyToID("_VignetteIntensity");
            }
            return vignetteIntensityID;
        }
    }

    public bool EnableBloom
    {
        get
        {
            return enableBloom;
        }
        set
        {
            if(enableBloom != value)
            {
                enableBloom = value;
                rebuildResource = true;
                CheckEnabled();
            }
        }
    }
    public bool EnableColorCurve
    {
        get
        {
            return this.enableColorCurve;
        }
        set
        {
            if(enableColorCurve != value)
            {
                enableColorCurve = value;
                rebuildResource = true;
                CheckEnabled();
            }
        }
    }
    public bool EnableSaturation
    {
        get
        {
            return enableSaturation;
        }
        set
        {
            if(enableSaturation != value)
            {
                enableSaturation = value;
                rebuildResource = true;
                CheckEnabled();
            }
        }
    }
    public bool EnableVignette
    {
        get
        {
            return enableVignette;
        }
        set
        {
            if(enableVignette != value)
            {
                enableVignette = value;
                rebuildResource = true;
                CheckEnabled();
            }
        }
    }
    public bool EnableBlur
    {
        get
        {
            return enableBlur;
        }
        set
        {
            if(enableBlur != value)
            {
                enableBlur = value;
                rebuildResource = true;
                CheckEnabled();
            }
        }
    }
    public float BlurSpread
    {
        get
        {
            return blurSpread;
        }
        set
        {
            blurSpread = value;
        }
    }
    public float WaveStrength
    {
        get
        {
            return waveStrength;
        }
        set
        {
            waveStrength = value;
        }
    }

    bool rebuildResource = true;
    Material downSampleMaterial;
    Material brightPassMaterial;
    Material blurPassMaterial;
    Material wavePassMaterial;
    Material combinePassMaterial;
    Texture2D curveTex;
    new Camera camera;

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void OnEnable()
    {
        CheckEnabled();
        if(enabled)
        {
            SetupResource();
            rebuildResource = false;
            CheckEnabled();
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(!enableBloom && !enableSaturation && !enableColorCurve
            && !enableVignette && !enableBlur)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if(rebuildResource)
        {
            SetupResource();
            rebuildResource = false;
        }

        RenderTexture blur4 = null;
        if(enableBloom)
        {
            var doHdr = camera.allowHDR;
            var rtFormat = (doHdr) ? RenderTextureFormat.ARGBHalf :
                RenderTextureFormat.Default;
            var rtW2 = source.width / 2;
            var rtH2 = source.height / 2;
            var rtW4 = source.width / 4;
            var rtH4 = source.height / 4;

            float widthOverHeight = (1f * source.width) / (1f * source.height);
            float oneOverBaseSize = 1f / 512f;

            // down sample
            var halfRezColorDown = RenderTexture.GetTemporary(rtW2, rtH2, 0, rtFormat);
            Graphics.Blit(source, halfRezColorDown);

            var quarterRezColor = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
            Graphics.Blit(halfRezColorDown, quarterRezColor, downSampleMaterial, 0);
            RenderTexture.ReleaseTemporary(halfRezColorDown);

            // cut colors 
            var secondQuarterRezColor = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
            var threshColor = bloomThreshold * bloomThresoldColor;
            brightPassMaterial.SetVector(ThreshholdID, threshColor);
            Graphics.Blit(quarterRezColor, secondQuarterRezColor, brightPassMaterial, 0);
            RenderTexture.ReleaseTemporary(quarterRezColor);

            // vertical blur
            blur4 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
            Vector4 offset = new Vector4(0f, bloomBlurSpread * oneOverBaseSize, 0f, 0f);
            blurPassMaterial.SetVector(OffsetsID, offset);
            Graphics.Blit(secondQuarterRezColor, blur4, blurPassMaterial, 0);
            RenderTexture.ReleaseTemporary(secondQuarterRezColor);
            secondQuarterRezColor = blur4;

            // horizontal blur
            blur4 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
            offset = new Vector4((bloomBlurSpread / widthOverHeight) * oneOverBaseSize, 0f, 0f, 0f);
            blurPassMaterial.SetVector(OffsetsID, offset);
            Graphics.Blit(secondQuarterRezColor, blur4, blurPassMaterial, 0);
            RenderTexture.ReleaseTemporary(secondQuarterRezColor);

            combinePassMaterial.SetTexture(BloomTexID, blur4);
            combinePassMaterial.SetFloat(BloomIntensityID, bloomIntensity);
        }
        else
        {

        }
    }

    void SetupResource()
    {
        if(enableBloom)
        {
            if(downSampleMaterial == null)
            {
                downSampleMaterial = new Material(downSampleShader);
            }
            if(brightPassMaterial == null)
            {
                brightPassMaterial = new Material(brightPassShader);
            }
            if(blurPassMaterial == null)
            {
                blurPassMaterial = new Material(blurPassShader);
            }
        }

        if(enableBlur)
        {
            if(blurPassMaterial == null)
            {
                blurPassMaterial = new Material(blurPassShader);
            }
            if(wavePassMaterial == null)
            {
                wavePassMaterial = new Material(wavePassShader);
            }
        }

        if(combinePassMaterial == null)
        {
            combinePassMaterial = new Material(combinePassShader);
        }

        if(enableBloom)
        {
            switch(bloomBlendMode)
            {
                case BloomBlendMode.Add:
                    combinePassMaterial.EnableKeyword("_BLOOM_ADD");
                    combinePassMaterial.DisableKeyword("_BLOOM_SCREEN");
                    break;
                case BloomBlendMode.Screen:
                    combinePassMaterial.DisableKeyword("_BLOOM_ADD");
                    combinePassMaterial.EnableKeyword("_BLOOM_SCREEN");
                    break;
            }
        }
        else
        {
            combinePassMaterial.DisableKeyword("_BLOOM_ADD");
            combinePassMaterial.DisableKeyword("_BLOOM_SCREEN");
        }

        if(enableColorCurve)
        {
            if(curveTex == null)
            {
                curveTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
                curveTex.hideFlags = HideFlags.DontSave;
                curveTex.wrapMode = TextureWrapMode.Clamp;
                curveTex.filterMode = FilterMode.Bilinear;
            }
            if(redChannelCurve != null
                && greenChannelCurve != null
                && blueChannelCurve != null)
            {
                for(int i = 0; i < 256; ++i)
                {
                    var k = (float)i / 256;

                    var rCh = Mathf.Clamp(redChannelCurve.Evaluate(k), 0f, 1f);
                    var gCh = Mathf.Clamp(greenChannelCurve.Evaluate(k), 0f, 1f);
                    var bCh = Mathf.Clamp(blueChannelCurve.Evaluate(k), 0f, 1f);

                    curveTex.SetPixel(i, 0, new Color(rCh, rCh, rCh));
                    curveTex.SetPixel(i, 1, new Color(gCh, gCh, gCh));
                    curveTex.SetPixel(i, 2, new Color(bCh, bCh, bCh));
                }
                curveTex.Apply();
            }
            combinePassMaterial.EnableKeyword("_COLOR_CURVE");
        }
        else
        {
            combinePassMaterial.DisableKeyword("_COLOR_CURVE");
        }

        if(enableSaturation)
        {
            combinePassMaterial.EnableKeyword("_SATURATION");
        }
        else
        {
            combinePassMaterial.DisableKeyword("_SATURATION");
        }

        if(enableVignette)
        {
            combinePassMaterial.EnableKeyword("_VIGNETTE_INTENSITY");
        }
        else
        {
            combinePassMaterial.DisableKeyword("_VIGNETTE_INTENSITY");
        }
    }

    void CheckEnabled()
    {
        enabled = EnableBloom || EnableColorCurve || EnableSaturation
            || EnableVignette || EnableBlur;
    }

    public void DoBlurSpread(float endValue, float duration)
    {
        DOTween.To(
            () => blurSpread,
            v =>
            {
                blurSpread = v;
            },
            endValue,
            duration
        );
    }

    public void DoWave(float endValue, float duration)
    {
        DOTween.To(
            () => waveStrength,
            v =>
            {
                waveStrength = v;
            },
            endValue,
            duration
        );
    }

    public void DoVignette(float endValue, float duration)
    {
        DOTween.To(
            () => vignetteIntensity,
            v =>
            {
                vignetteIntensity = v;
            },
            endValue,
            duration
        );
    }

    public void DoBloom(float endValue, float duration)
    {
        EnableBloom = true;
        Sequence quence = DOTween.Sequence();
        quence.Append(
            DOTween.To(
                () => bloomThreshold,
                v =>
                {
                    bloomThreshold = v;
                },
                endValue,
                duration
            )
        );
    }

    public void DoSaturation(float endValue, float duration)
    {
        EnableSaturation = true;
        Sequence quence = DOTween.Sequence();
        quence.Append(
            DOTween.To(
                () => saturation,
                v =>
                {
                    saturation = v;
                },
                endValue,
                duration
            )
        );
    }

    public void DoDreaming(float blurSpread, float duration01, float wave, float duration02)
    {
        EnableBlur = true;
        Sequence quence01 = DOTween.Sequence();
        quence01.Append(
            DOTween.To(
                () => blurSpread,
                v =>
                {
                    blurSpread = v;
                },
                blurSpread,
                duration01
            )
        );
        quence01.AppendInterval(0.5f);
        quence01.Append(
            DOTween.To(
                () => blurSpread,
                v =>
                {
                    blurSpread = v;
                },
                0,
                duration01
            )
        );
        Sequence quence02 = DOTween.Sequence();
        quence02.Append(
            DOTween.To(
                () => waveStrength,
                vignetteIntensity =>
                {
                },
                wave,
                duration02
            )
        );
        quence02.AppendInterval(0.5f);
        quence02.Append(
            DOTween.To(
                () => waveStrength,
                v =>
                {
                    waveStrength = v;
                },
                0,
                duration02
            )
        );
    }

    enum BloomBlendMode
    {
        // Blend the bloom with the image using screen mode
        Screen = 0,
        // Blend the bloom with the image using add mode
        Add = 1,
    }
}