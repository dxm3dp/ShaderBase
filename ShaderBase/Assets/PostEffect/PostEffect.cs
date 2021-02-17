using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public sealed class PostEffect : MonoBehaviour
{
    [Tooltip("The shader for blur pass")]
    Shader blurPassShader;
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
    [Range(0f, 10f)]
    float blurSpread = 2.5f;

    Material blurPassMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    { }

    enum BloomBlendMode
    {
        // Blend the bloom with the image using screen mode
        Screen = 0,
        // Blend the bloom with the image using add mode
        Add = 1,
    }
}