// SKGames vertical fog editor GUI. Copyright (c) 2018 Sergey Klimenko. 13.05.2018

using UnityEditor;
using UnityEngine;

public class HeightFogGUI : ShaderGUI
{
    private static class Styles
    {
        public static string primaryProperties = "Primary properties"; 
        public static string fogProperties = "Fog properties";
        public static string fogAnimationProperties = "Fog animation properties";
        public static string standartFogProperties = "Standard fog properties";
        public static GUIContent mainText = new GUIContent("Albedo");
        public static GUIContent normalMapText = new GUIContent("Normal map");
        public static GUIContent aoMapText = new GUIContent("AO map");
        public static GUIContent emissionColor = new GUIContent("Emission color"); 
        public static GUIContent specTexture = new GUIContent("Specular map");
        public static GUIContent specularPower = new GUIContent("Power");
        public static GUIContent metTexture = new GUIContent("Metallic map");
        public static GUIContent metallicPower = new GUIContent("Power");
        public static GUIContent fogSpace = new GUIContent("Fog space");
        public static GUIContent fogColor = new GUIContent("Fog color");
        public static GUIContent fogMinMax = new GUIContent("Fog height");
        public static GUIContent fogFalloff = new GUIContent("Fog falloff");
        public static GUIContent fogEmissionColor = new GUIContent("Fog emission color");
        public static GUIContent fogEmissionPower = new GUIContent("Fog emission power");
        public static GUIContent fogEmissionFalloff = new GUIContent("Fog emission falloff");
        public static GUIContent combineSyandartFog = new GUIContent("Combine with standard fog");
        public static GUIContent overrideStandartFogColor = new GUIContent("Override standard fog color"); 
        public static GUIContent useAnimation = new GUIContent("Use fog animation"); 
        public static GUIContent fogSpeed = new GUIContent("Speed");
        public static GUIContent fogAmp = new GUIContent("Amplitude");
        public static GUIContent fogFreq = new GUIContent("Frequency");
        public static GUIContent source = new GUIContent("Source");
    }

    private enum FogSpace
    {
        Local = 0,
        World = 1
    }

    private enum Channel
    {
        R = 0,
        G = 1,
        B = 2,
        A = 3,
        AlbedoAlpha = 4
    }

    MaterialProperty mainColor = null;
    MaterialProperty emissionColor = null;
    MaterialProperty emissionPower = null;
    MaterialProperty specularPower = null;
    MaterialProperty specTexture = null;
    MaterialProperty specSrc = null;
    MaterialProperty metallicPower = null;
    MaterialProperty metTexture = null;
    MaterialProperty metSrc= null;
    MaterialProperty normalPower = null;
    MaterialProperty mainTexture = null;
    MaterialProperty normalTexture = null;
    MaterialProperty aoTexture = null;
    MaterialProperty fogSpace = null;
    MaterialProperty fogColor = null;
    MaterialProperty fogMin = null;
    MaterialProperty fogMax = null;
    MaterialProperty fogEmissionColor = null;
    MaterialProperty fogEmissionPower = null;
    MaterialProperty fogEmissionFalloff = null;
    MaterialProperty fogFalloff = null;
    MaterialProperty standartFog = null;
    MaterialProperty standartFogOverride = null;
    MaterialProperty useAnimation = null;
    MaterialProperty xSpeed = null;
    MaterialProperty zSpeed = null;
    MaterialProperty xAmplitude = null;
    MaterialProperty zAmplitude = null;
    MaterialProperty xFreq = null;
    MaterialProperty zFreq = null;

    private MaterialEditor m_MaterialEditor;
    private bool m_FirstTimeApply = true;
    private bool m_BakeEmission = false;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        GetProperties(props);
        m_MaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        if (m_FirstTimeApply)
        {
            MaterialChanged(material);
            m_FirstTimeApply = false;
        }

        ShaderPropertiesGUI(material);
    }

    public void GetProperties(MaterialProperty[] props)
    {
        mainColor = FindProperty("_Color", props);
        emissionColor = FindProperty("_EmissionColor", props);
        emissionPower = FindProperty("_EmissionPower", props);
        specularPower = FindProperty("_SpecularPower", props);
        specTexture = FindProperty("_SpecularTex", props);
        specSrc = FindProperty("_SpecChannel", props);
        metallicPower = FindProperty("_Metallic", props);
        metTexture = FindProperty("_MetallicTex", props);
        metSrc = FindProperty("_MetChannel", props);
        normalPower = FindProperty("_NormalAmount", props);
        mainTexture = FindProperty("_MainTex", props);
        normalTexture = FindProperty("_NormalMap", props);
        aoTexture = FindProperty("_AOMap", props);
        fogSpace = FindProperty("_FogRelativeWorldOrLocal", props);
        fogColor = FindProperty("_FogColor", props);
        fogMin = FindProperty("_FogMin", props);
        fogMax = FindProperty("_FogMax", props);
        fogEmissionColor = FindProperty("_FogEmissionColor", props); 
        fogEmissionPower = FindProperty("_FogEmissionPower", props);
         fogEmissionFalloff = FindProperty("_FogEmissionFalloff", props);
        fogFalloff = FindProperty("_FogFalloff", props);
        standartFog = FindProperty("_STANDARD_FOG", props);
        standartFogOverride = FindProperty("_OVERRIDE_FOG_COLOR", props);
        useAnimation = FindProperty("_ANIMATION", props);
        xSpeed = FindProperty("_FogWaveSpeedX", props);
        zSpeed = FindProperty("_FogWaveSpeedZ", props);
        xAmplitude = FindProperty("_FogWaveAmplitudeX", props);
        zAmplitude = FindProperty("_FogWaveAmplitudeZ", props);
        xFreq = FindProperty("_FogWaveFreqX", props);
        zFreq = FindProperty("_FogWaveFreqZ", props);
    }

    public void ShaderPropertiesGUI(Material material)
    {
        EditorGUIUtility.labelWidth = 0f;
        EditorGUI.BeginChangeCheck();
        {
            GUILayout.Label(Styles.primaryProperties, EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            Rect a = EditorGUILayout.GetControlRect();
            a.xMax = 100f;
            m_MaterialEditor.TexturePropertyMiniThumbnail(a, mainTexture, Styles.mainText.text, "");
            mainColor.colorValue = EditorGUILayout.ColorField(mainColor.colorValue, GUILayout.Width(50f));
            GUILayout.Label("", GUILayout.Width(50f));
            GUILayout.EndHorizontal();
            m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, normalTexture, normalTexture.textureValue != null ? normalPower : null);
            if (normalPower.floatValue != 1 && (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS))
                if (m_MaterialEditor.HelpBoxWithButton(
                    new GUIContent("Bump scale is not supported on mobile platforms"),
                    new GUIContent("Fix Now")))
                {
                    normalPower.floatValue = 1;
                }
            m_MaterialEditor.TexturePropertySingleLine(Styles.aoMapText, aoTexture);

            a = EditorGUILayout.GetControlRect();
            m_MaterialEditor.TexturePropertyMiniThumbnail(a, specTexture, Styles.specTexture.text, "");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(10f));
            GUILayout.Label(Styles.specularPower, GUILayout.Width(120f));
            specularPower.floatValue = EditorGUILayout.Slider(specularPower.floatValue, 0.0f, 1f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(10f));
            GUILayout.Label(Styles.source, GUILayout.Width(120f));
            specSrc.floatValue = (float)(Channel)EditorGUILayout.EnumPopup((Channel)specSrc.floatValue);
            EditorGUILayout.EndHorizontal();

            a      = EditorGUILayout.GetControlRect();
            m_MaterialEditor.TexturePropertyMiniThumbnail(a, metTexture, Styles.metTexture.text, "");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("",                   GUILayout.Width(10f));
            GUILayout.Label(Styles.metallicPower, GUILayout.Width(120f));
            metallicPower.floatValue = EditorGUILayout.Slider(metallicPower.floatValue, 0.0f, 1f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(10f));
            GUILayout.Label(Styles.source, GUILayout.Width(120f));
            metSrc.floatValue = (float)(Channel)EditorGUILayout.EnumPopup((Channel)metSrc.floatValue);
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Styles.emissionColor.text);
            emissionColor.colorValue = EditorGUILayout.ColorField(emissionColor.colorValue, GUILayout.MaxWidth(50f));
            emissionPower.floatValue = EditorGUILayout.FloatField("", emissionPower.floatValue, GUILayout.MaxWidth(50f));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Bake emission");
            m_BakeEmission = EditorGUILayout.Toggle(m_BakeEmission, GUILayout.MaxWidth(50f));
            if (m_BakeEmission)
            {
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            else
            {
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            }
            GUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            m_MaterialEditor.TextureScaleOffsetProperty(mainTexture);
            if (EditorGUI.EndChangeCheck())
            {
                normalTexture.textureScaleAndOffset = mainTexture.textureScaleAndOffset;
                aoTexture.textureScaleAndOffset     = mainTexture.textureScaleAndOffset;
            }

            EditorGUILayout.Space();
            GUILayout.Label(Styles.fogProperties, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Styles.fogSpace, GUILayout.Width(130f));
            fogSpace.floatValue = (float)(FogSpace)EditorGUILayout.EnumPopup((FogSpace)fogSpace.floatValue);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Styles.fogColor, GUILayout.Width(130f));
            fogColor.colorValue = EditorGUILayout.ColorField(fogColor.colorValue);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Styles.fogMinMax, GUILayout.Width(130f));
            GUILayout.Label("Min", GUILayout.MaxWidth(30f), GUILayout.MinWidth(30f));
            fogMin.floatValue = EditorGUILayout.FloatField(fogMin.floatValue, GUILayout.MinWidth(1f));
            GUILayout.Label("Max", GUILayout.MaxWidth(30f), GUILayout.MinWidth(30f));
            fogMax.floatValue = EditorGUILayout.FloatField(fogMax.floatValue, GUILayout.MinWidth(1f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Styles.fogFalloff, GUILayout.Width(130f));
            fogFalloff.floatValue = EditorGUILayout.Slider(fogFalloff.floatValue, 0.01f, 20f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Styles.fogEmissionColor, GUILayout.Width(130f));
            fogEmissionColor.colorValue = EditorGUILayout.ColorField(GUIContent.none, fogEmissionColor.colorValue,
                false, false, true, new ColorPickerHDRConfig(-10, 10, -10, 10));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Styles.fogEmissionPower, GUILayout.Width(130f));
            fogEmissionPower.floatValue = EditorGUILayout.Slider(fogEmissionPower.floatValue, 0.0f, 100f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Styles.fogEmissionFalloff, GUILayout.Width(130f));
            fogEmissionFalloff.floatValue = EditorGUILayout.Slider(fogEmissionFalloff.floatValue, 0.01f, 20f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.Label(Styles.fogAnimationProperties, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Styles.useAnimation, GUILayout.Width(130f));
            bool useAnimationState = EditorGUILayout.Toggle(useAnimation.floatValue > 0.5f);
            useAnimation.floatValue = useAnimationState ? 1f : 0f;
            EditorGUILayout.EndHorizontal();
            if (useAnimationState)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Styles.fogSpeed, GUILayout.Width(130f));
                GUILayout.Label("X", GUILayout.Width(12f));
                xSpeed.floatValue = EditorGUILayout.FloatField(xSpeed.floatValue, GUILayout.MinWidth(1f));
                GUILayout.Label("Z", GUILayout.Width(12f));
                zSpeed.floatValue = EditorGUILayout.FloatField(zSpeed.floatValue, GUILayout.MinWidth(1f));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Styles.fogAmp, GUILayout.Width(130f));
                GUILayout.Label("X", GUILayout.Width(12f));
                xAmplitude.floatValue = EditorGUILayout.FloatField(xAmplitude.floatValue, GUILayout.MinWidth(1f));
                GUILayout.Label("Z", GUILayout.Width(12f));
                zAmplitude.floatValue = EditorGUILayout.FloatField(zAmplitude.floatValue, GUILayout.MinWidth(1f));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Styles.fogFreq, GUILayout.Width(130f));
                GUILayout.Label("X", GUILayout.Width(12f));
                xFreq.floatValue = EditorGUILayout.FloatField(xFreq.floatValue, GUILayout.MinWidth(1f));
                GUILayout.Label("Z", GUILayout.Width(12f));
                zFreq.floatValue = EditorGUILayout.FloatField(zFreq.floatValue, GUILayout.MinWidth(1f));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            GUILayout.Label(Styles.standartFogProperties, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Styles.combineSyandartFog, GUILayout.Width(230f));
            bool combineWithStandartFog = EditorGUILayout.Toggle(standartFog.floatValue == 1);
            standartFog.floatValue = combineWithStandartFog ? 1 : 0;
            EditorGUILayout.EndHorizontal();
            if (combineWithStandartFog)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Styles.overrideStandartFogColor, GUILayout.Width(230f));
                bool overrideColor = EditorGUILayout.Toggle(standartFogOverride.floatValue == 1);
                standartFogOverride.floatValue = overrideColor ? 1 : 0;
                EditorGUILayout.EndHorizontal();
                GUILayout.Label("(forward only)", GUILayout.Width(230f));
            }
            else
            {
                standartFogOverride.floatValue = 0;
            }

            GUILayout.Label("Advanced Options", EditorStyles.boldLabel);

            m_MaterialEditor.EnableInstancingField();

            GUILayout.Label("Copyright (c) 2018 Sergey Klimenko.", new GUIStyle()
            {
                alignment   = TextAnchor.MiddleCenter,
                clipping    = TextClipping.Clip,
                fontSize    = 10,
                fixedHeight = 12,
                normal =
                {
                    textColor = Color.gray
                }
            });
            EditorGUILayout.EndVertical();
        }
        if (EditorGUI.EndChangeCheck())
        {
           foreach (var obj in m_MaterialEditor.targets)
                MaterialChanged((Material)obj);
        }
    }

    static void SetMaterialKeywords(Material material)
    {
        SetKeyword(material, "_NORMALMAP", material.GetTexture("_NormalMap"));
        MaterialEditor.FixupEmissiveFlag(material);
        bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
        SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);
    }

    static void MaterialChanged(Material material)
    {
        SetMaterialKeywords(material);
    }

    static void SetKeyword(Material m, string keyword, bool state)
    {
        if (state)
            m.EnableKeyword(keyword);
        else
            m.DisableKeyword(keyword);
    }
}
