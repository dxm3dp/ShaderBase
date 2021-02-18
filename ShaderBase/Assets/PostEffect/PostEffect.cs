using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public sealed class PostEffect : MonoBehaviour
{
    //https://stackoverflow.com/questions/34407922/setting-c-sharp-formatting-options-for-omnisharp-on-visual-studio-code
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
            if (offsetsID == -1)
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
            if (bloomTexID == -1)
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
            if (bloomIntensityID == -1)
            {
                bloomIntensityID = Shader.PropertyToID("_BloomIntensity");
            }
            return bloomIntensityID;
        }
    }

    Material blurPassMaterial;
    new Camera camera;

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    { 
        if (!enableBloom)
        {
            Graphics.Blit(source, destination);
            return;
        }

        RenderTexture blur4 = null;
        if (enableBloom)
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

            // cut colors 
            var secondQuarterRezColor = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);

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

        }
    }

    void SetupResource()
    {
        if (enableBloom)
        {
            if (blurPassMaterial == null)
            {
                blurPassMaterial = new Material(blurPassShader);
            }
        }
    }

    enum BloomBlendMode
    {
        // Blend the bloom with the image using screen mode
        Screen = 0,
        // Blend the bloom with the image using add mode
        Add = 1,
    }
}