using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PostEffect))]
public sealed class PostEffectEditor : Editor
{
    SerializedProperty downSampleShader;
    SerializedProperty brightPassShader;
    SerializedProperty blurPassShader;
    SerializedProperty wavePassShader;
    SerializedProperty combinePassShader;

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
        bloomThresholdColor = serObj.FindProperty("bloomThresholdColor");
        bloomBlurSpread = serObj.FindProperty("bloomBlurSpread");

        enableColorCurve = serObj.FindProperty("enableColorCurve");
        redChannelCurve = serObj.FindProperty("redChannelCurve");
        greenChannelCurve = serObj.FindProperty("greenChannelCurve");
        blueChannelCurve = serObj.FindProperty("blueChannelCurve");

        enableSaturation = serObj.FindProperty("enableSaturation");
        saturation = serObj.FindProperty("saturation");

        enableVignette = serObj.FindProperty("enableVignette");
        vignetteIntensity = serObj.FindProperty("vignetteIntensity");

        enableBlur = serObj.FindProperty("enableBlur");
        blurSpread = serObj.FindProperty("blurSpread");
        waveStrength = serObj.FindProperty("waveStrength");
    }

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
        if(wavePassShader.objectReferenceValue == null)
        {
            wavePassShader.objectReferenceValue = Shader.Find("PostEffect/WavePass");
        }
        if(combinePassShader.objectReferenceValue == null)
        {
            combinePassShader.objectReferenceValue = Shader.Find("PostEffect/CombinePass");
        }

        // Shaders
        downSampleShader.isExpanded =
            EditorGUILayout.ToggleLeft("Show Shaders", downSampleShader.isExpanded);
        if(downSampleShader.isExpanded)
        {
            EditorGUILayout.PropertyField(downSampleShader);
            EditorGUILayout.PropertyField(brightPassShader);
            EditorGUILayout.PropertyField(blurPassShader);
            EditorGUILayout.PropertyField(wavePassShader);
            EditorGUILayout.PropertyField(combinePassShader);
        }

        // Bloom
        enableBloom.boolValue =
            EditorGUILayout.ToggleLeft(enableBloom.displayName, enableBloom.boolValue);
        if(enableBloom.boolValue)
        {
            EditorGUILayout.PropertyField(bloomBlendMode);
            EditorGUILayout.PropertyField(bloomIntensity);
            EditorGUILayout.PropertyField(bloomThreshold);
            EditorGUILayout.PropertyField(bloomThresholdColor);
            EditorGUILayout.PropertyField(bloomBlurSpread);
        }

        // Color curve
        enableColorCurve.boolValue =
            EditorGUILayout.ToggleLeft(enableColorCurve.displayName, enableColorCurve.boolValue);
        if(enableColorCurve.boolValue)
        {
            EditorGUILayout.PropertyField(redChannelCurve);
            EditorGUILayout.PropertyField(greenChannelCurve);
            EditorGUILayout.PropertyField(blueChannelCurve);
        }

        // Saturation
        enableSaturation.boolValue =
            EditorGUILayout.ToggleLeft(enableSaturation.displayName, enableSaturation.boolValue);
        if(enableSaturation.boolValue)
        {
            EditorGUILayout.PropertyField(saturation);
        }

        // Vignette
        enableVignette.boolValue =
            EditorGUILayout.ToggleLeft(enableVignette.displayName, enableVignette.boolValue);
        if(enableVignette.boolValue)
        {
            EditorGUILayout.PropertyField(vignetteIntensity);
        }

        // Blur
        enableBlur.boolValue =
            EditorGUILayout.ToggleLeft(enableBlur.displayName, enableBlur.boolValue);
        if(enableBlur.boolValue)
        {
            EditorGUILayout.PropertyField(blurSpread);
            EditorGUILayout.PropertyField(waveStrength);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
