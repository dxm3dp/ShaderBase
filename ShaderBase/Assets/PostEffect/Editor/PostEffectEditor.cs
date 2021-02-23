using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PostEffect))]
public sealed class PostEffectEditor : Editor
{
    SerializedProperty downSampleShader;
    SerializedProperty brightPassShader;
    SerializedProperty blurPassShader;
    SerializedProperty combinePassShader;
    SerializedProperty wavePassShader;

    SerializedProperty enableBloom;
    SerializedProperty bloomBlendMode;
    SerializedProperty bloomIntensity;
    SerializedProperty bloomThreshold;
    SerializedProperty bloomThresholdColor;
    SerializedProperty bloomBlurSpread;

    SerializedProperty enableColorCurve;
    SerializedProperty redChannelCurve;
    SerializedProperty greenChannelCurve;
    SerializedProperty blueChannelCurve;

    SerializedProperty enableSaturation;
    SerializedProperty saturation;

    SerializedProperty enableVignette;
    SerializedProperty vignetteIntensity;

    SerializedProperty enableBlur;
    SerializedProperty blurSpread;
    SerializedProperty waveStrength;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if(downSampleShader.objectReferenceValue == null)
        {
            downSampleShader.objectReferenceValue = Shader.Find("PostEffect/DownSample");
        }

        if(brightPassShader.objectReferenceValue == null)
        {
            brightPassShader.objectReferenceValue = Shader.Find("PostEffect/BrightPass");
        }

        if(blurPassShader.objectReferenceValue == null)
        {
            blurPassShader.objectReferenceValue = Shader.Find("PostEffect/BlurPass");
        }

        if(combinePassShader.objectReferenceValue == null)
        {
            combinePassShader.objectReferenceValue = Shader.Find("PostEffect/CombinePass");
        }

        if(wavePassShader.objectReferenceValue == null)
        {
            wavePassShader.objectReferenceValue = Shader.Find("PostEffect/WavePass");
        }
    }

    void OnEnable()
    {
        var serObj = serializedObject;
        downSampleShader = serObj.FindProperty("downSampleShader");
        blurPassShader = serObj.FindProperty("blurPassShader");
        brightPassShader = serObj.FindProperty("brightPassShader");
        combinePassShader = serObj.FindProperty("combinePassShader");
        wavePassShader = serObj.FindProperty("wavePassShader");

        enableBloom = serObj.FindProperty("enableBloom");
        bloomBlendMode = serObj.FindProperty("bloomBlendMode");
        bloomIntensity = serObj.FindProperty("bloomIntensity");
        bloomThreshold = serObj.FindProperty("bloomThreshold");
    }
}
