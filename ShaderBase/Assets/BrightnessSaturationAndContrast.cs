using System;
using UnityEngine;

public class BrightnessSaturationAndContrast : PostEffectBase
{
    public Shader briSatConShader;
    public Material material
    {
        get
        {
            briSatConMaterial = CheckShaderAndCreateMaterial(briSatConShader, briSatConMaterial);
            return briSatConMaterial;
        }
    }

    [Range(0f, 3f)]
    public float brightness = 1f;
    [Range(0f, 3f)]
    public float saturation = 1f;
    [Range(0f, 3f)]
    public float contrast = 1f;

    Material briSatConMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            material.SetFloat("_Brightness", brightness);
            material.SetFloat("_Saturation", saturation);
            material.SetFloat("_Contrast", contrast);
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}