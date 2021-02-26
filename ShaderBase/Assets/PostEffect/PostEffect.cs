using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
public sealed class PostEffect : MonoBehaviour
{
    static int thresholdID = -1;
    static int offsetsID = -1;
    static int bloomTexID = -1;
    static int bloomIntensityID = -1;
    static int saturationID = -1;
    static int curveTexID = -1;
    static int vignetteIntensityID = -1;
    static int waveStengthID = -1;

    [SerializeField]
    Shader downSampleShader;

    [SerializeField]
    Shader brightPassShader;

    [SerializeField]
    Shader blurPassShader;

    [SerializeField]
    Shader wavePassShader;

    [SerializeField]
    Shader combinePassShader;

    [SerializeField]
    bool enableBloom;

    [SerializeField]
    BloomBlendMode bloomBlendMode = BloomBlendMode.Add;

    [SerializeField]
    float bloomIntensity = 0.5f;

    [SerializeField]
    [Range(-0.05f, 4f)]
    float bloomThreshold = 0.5f;

    [SerializeField]
    Color bloomThresholdColor = Color.white;

    [SerializeField]
    float bloomBlurSpread = 2.5f;

    [SerializeField]
    bool enableColorCurve;

    [SerializeField]
    AnimationCurve redChannelCurve =
        new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    [SerializeField]
    AnimationCurve greenChannelCurve =
        new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    [SerializeField]
    AnimationCurve blueChannelCurve =
        new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    [SerializeField]
    bool enableSaturation;

    [SerializeField]
    [Range(0f, 5f)]
    float saturation = 1f;

    [SerializeField]
    bool enableVignette;

    [SerializeField]
    float vignetteIntensity = 0.375f;

    [SerializeField]
    bool enableBlur;

    [SerializeField]
    [Range(0f, 10f)]
    float blurSpread = 2.5f;

    [SerializeField]
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
    static int ThresholdID
    {
        get
        {
            if(thresholdID == -1)
            {
                thresholdID = Shader.PropertyToID("_Threshold");
            }
            return thresholdID;
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
        CheckSupport();
        if(enabled)
        {
            SetupResource();
            rebuildResource = false;
            CheckEnabled();
        }
    }

    void CheckSupport()
    {
        if(combinePassShader == null || !combinePassShader.isSupported)
        {
            Debug.LogWarning("Does not support the combine pass shader.");
            enabled = false;
            return;
        }

        if(enableBloom)
        {
            if(downSampleShader == null || !downSampleShader.isSupported)
            {
                Debug.LogWarning("Does not support the down sample shader, turn off the bloom");
                enableBloom = false;
            }
        }

        if(enableBloom)
        {
            if(brightPassShader == null || !brightPassShader.isSupported)
            {
                Debug.LogWarning("Does not support the bright pass shader, turn off the bloom");
                enableBloom = false;
            }
        }

        if(enableBloom)
        {
            if(blurPassShader == null || !blurPassShader.isSupported)
            {
                Debug.LogWarning("Does not support the blur pass shader, turn off the bloom");
                enableBloom = false;
            }
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
            var threshColor = bloomThreshold * bloomThresholdColor;
            brightPassMaterial.SetVector(ThresholdID, threshColor);
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

        if(enableSaturation)
        {
            combinePassMaterial.SetFloat(SaturationID, saturation);
        }

        if(enableColorCurve)
        {
            combinePassMaterial.SetTexture(CurveTexID, curveTex);
        }

        if(enableVignette)
        {
            combinePassMaterial.SetFloat(VignetteIntensityID, vignetteIntensity);
        }

        if(enableBloom || enableSaturation || enableColorCurve || enableVignette)
        {
            if(enableBlur)
            {
                var temp1 = RenderTexture.GetTemporary(
                    source.width, source.height, 0, RenderTextureFormat.Default);
                var temp2 = RenderTexture.GetTemporary(
                    source.width, source.height, 0, RenderTextureFormat.Default);
                Graphics.Blit(source, temp1, combinePassMaterial, 0);

                float widthOverHeight = (1f * source.width) / (1f * source.height);
                float oneOverBaseSize = 1f / 512f;

                var offset = new Vector4(0f, blurSpread * oneOverBaseSize, 0f, 0f);
                blurPassMaterial.SetVector(OffsetsID, offset);
                Graphics.Blit(temp1, temp2, blurPassMaterial, 0);

                offset = new Vector4((blurSpread / widthOverHeight) * oneOverBaseSize, 0f, 0f, 0f);
                wavePassMaterial.SetVector(OffsetsID, offset);
                wavePassMaterial.SetVector(WaveStrengthID, new Vector4(waveStrength, waveStrength));
                Graphics.Blit(temp2, destination, wavePassMaterial, 0);

                RenderTexture.ReleaseTemporary(temp1);
                RenderTexture.ReleaseTemporary(temp2);
            }
            else
            {
                Graphics.Blit(source, destination, combinePassMaterial, 0);
            }
        }
        else
        {
            var temp = RenderTexture.GetTemporary(
                            source.width, source.height, 0, RenderTextureFormat.Default);

            float widthOverHeight = (1f * source.width) / (1f * source.height);
            float oneOverBaseSize = 1f / 512f;

            // vertical blur
            var offset = new Vector4(0.0f, this.blurSpread * oneOverBaseSize, 0.0f, 0.0f);
            this.blurPassMaterial.SetVector(OffsetsID, offset);
            Graphics.Blit(source, temp, this.blurPassMaterial, 0);

            // horizontal blur
            offset = new Vector4((this.blurSpread / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f);
            this.wavePassMaterial.SetVector(OffsetsID, offset);
            this.wavePassMaterial.SetVector(WaveStrengthID, new Vector4(this.waveStrength, this.waveStrength));
            Graphics.Blit(temp, destination, this.wavePassMaterial, 0);

            RenderTexture.ReleaseTemporary(temp);
        }

        if(blur4 != null)
        {
            RenderTexture.ReleaseTemporary(blur4);
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