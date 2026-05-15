#if UNITY_2018_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

class HeightFogLWRPGUI : BaseShaderGUI
{
    private enum FogSpace
    {
        Local = 0,
        World = 1
    }

    public enum WorkflowMode
    {
        Specular = 0,
        Metallic
    }

    public enum SmoothnessMapChannel
    {
        SpecularMetallicAlpha,
        AlbedoAlpha,
    }

    private static class Styles
    {
        public static GUIContent albedoText =
            new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
        public static GUIContent specularMapText =
            new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");
        public static GUIContent metallicMapText =
            new GUIContent("Metallic", "Metallic (R) and Smoothness (A)");
        public static GUIContent smoothnessText = new GUIContent("Smoothness", "Smoothness value");
        public static GUIContent smoothnessScaleText =
            new GUIContent("Smoothness", "Smoothness scale factor");
        public static GUIContent smoothnessMapChannelText =
            new GUIContent("Source", "Smoothness texture and channel");
        public static GUIContent highlightsText =
            new GUIContent("Specular Highlights", "Specular Highlights");
        public static GUIContent reflectionsText = new GUIContent("Reflections", "Glossy Reflections");
        public static GUIContent normalMapText   = new GUIContent("Normal Map",  "Normal Map");
        public static GUIContent occlusionText   = new GUIContent("Occlusion",   "Occlusion (G)");
        public static GUIContent emissionText    = new GUIContent("Color",       "Emission (RGB)");
        public static GUIContent bumpScaleNotSupported =
            new GUIContent("Bump scale is not supported on mobile platforms");
        public static GUIContent fixNow = new GUIContent("Fix now");

        public static          string   surfaceProperties              = "Surface Properties";
        public static GUIContent workflowModeText               = new GUIContent("Workflow Mode", "Workflow Mode");
        public static readonly string[] workflowNames                  = Enum.GetNames(typeof(WorkflowMode));
        public static readonly string[] metallicSmoothnessChannelNames = {"Metallic Alpha", "Albedo Alpha"};
        public static readonly string[] specularSmoothnessChannelNames = {"Specular Alpha", "Albedo Alpha"};

        public static string primaryProperties      = "Primary properties";
        public static string fogProperties          = "Fog properties";
        public static string fogAnimationProperties = "Fog animation properties";
        public static string standartFogProperties  = "Standard fog properties";


        public static GUIContent fogSpace                 = new GUIContent("Fog space");
        public static GUIContent fogColor                 = new GUIContent("Fog color");
        public static GUIContent fogMinMax                = new GUIContent("Fog height");
        public static GUIContent fogFalloff               = new GUIContent("Fog falloff");
        public static GUIContent fogEmissionColor = new GUIContent("Fog emission color"); 
        public static GUIContent fogEmissionPower         = new GUIContent("Fog emission power"); 
        public static GUIContent fogEmissionFalloff       = new GUIContent("Fog emission falloff");
        public static GUIContent combineSyandartFog       = new GUIContent("Combine with standard fog");
        public static GUIContent overrideStandartFogColor = new GUIContent("Override standard fog color");
        public static GUIContent useAnimation             = new GUIContent("Use fog animation");
        public static GUIContent fogSpeed                 = new GUIContent("Speed");
        public static GUIContent fogAmp                   = new GUIContent("Amplitude");
        public static GUIContent fogFreq                  = new GUIContent("Frequency");
        public static GUIContent source                   = new GUIContent("Source");
    }

    private MaterialProperty workflowMode;

    private MaterialProperty albedoColor;
    private MaterialProperty albedoMap;

    private MaterialProperty smoothness;
    private MaterialProperty smoothnessScale;
    private MaterialProperty smoothnessMapChannel;

    private MaterialProperty metallic;
    private MaterialProperty specColor;
    private MaterialProperty metallicGlossMap;
    private MaterialProperty specGlossMap;
    private MaterialProperty highlights;
    private MaterialProperty reflections;

    private MaterialProperty bumpScale;
    private MaterialProperty bumpMap;
    private MaterialProperty occlusionStrength;
    private MaterialProperty occlusionMap;
    private MaterialProperty emissionColorForRendering;
    private MaterialProperty emissionMap;

    MaterialProperty fogSpace            = null;
    MaterialProperty fogColor            = null;
    MaterialProperty fogMin              = null;
    MaterialProperty fogMax              = null;
    MaterialProperty fogEmissionColor = null;
    MaterialProperty fogEmissionPower    = null;
    MaterialProperty fogEmissionFalloff  = null;
    MaterialProperty fogFalloff          = null;
    MaterialProperty standartFog         = null;
    MaterialProperty standartFogOverride = null;
    MaterialProperty useAnimation        = null;
    MaterialProperty xSpeed              = null;
    MaterialProperty zSpeed              = null;
    MaterialProperty xAmplitude          = null;
    MaterialProperty zAmplitude          = null;
    MaterialProperty xFreq               = null;
    MaterialProperty zFreq               = null;

    public override void FindProperties(MaterialProperty[] properties)
    {
        //base.FindProperties(properties);

        workflowMode = FindProperty("_WorkflowMode", properties);
        albedoColor  = FindProperty("_Color",        properties);
        albedoMap    = FindProperty("_MainTex",      properties);

        smoothness           = FindProperty("_Glossiness",               properties);
        smoothnessScale      = FindProperty("_GlossMapScale",            properties, false);
        smoothnessMapChannel = FindProperty("_SmoothnessTextureChannel", properties, false);

        metallic         = FindProperty("_Metallic",           properties);
        specColor        = FindProperty("_SpecColor",          properties);
        metallicGlossMap = FindProperty("_MetallicGlossMap",   properties);
        specGlossMap     = FindProperty("_SpecGlossMap",       properties);
        highlights       = FindProperty("_SpecularHighlights", properties);
        reflections      = FindProperty("_GlossyReflections",  properties);

        bumpScale                 = FindProperty("_BumpScale",         properties);
        bumpMap                   = FindProperty("_BumpMap",           properties);
        occlusionStrength         = FindProperty("_OcclusionStrength", properties);
        occlusionMap              = FindProperty("_OcclusionMap",      properties);
        emissionColorForRendering = FindProperty("_EmissionColor",     properties);
        emissionMap               = FindProperty("_EmissionMap",       properties);

        fogSpace            = FindProperty("_FogRelativeWorldOrLocal", properties);
        fogColor            = FindProperty("_FogColor",                properties);
        fogMin              = FindProperty("_FogMin",                  properties);
        fogMax              = FindProperty("_FogMax",                  properties);
        fogEmissionColor = FindProperty("_FogEmissionColor",        properties); 
        fogEmissionPower    = FindProperty("_FogEmissionPower",        properties); 
         fogEmissionFalloff  = FindProperty("_FogEmissionFalloff",      properties);
        fogFalloff          = FindProperty("_FogFalloff",              properties);
        standartFog         = FindProperty("_STANDARD_FOG",            properties);
        standartFogOverride = FindProperty("_OVERRIDE_FOG_COLOR",      properties);
        useAnimation        = FindProperty("_ANIMATION",               properties);
        xSpeed              = FindProperty("_FogWaveSpeedX",           properties);
        zSpeed              = FindProperty("_FogWaveSpeedZ",           properties);
        xAmplitude          = FindProperty("_FogWaveAmplitudeX",       properties);
        zAmplitude          = FindProperty("_FogWaveAmplitudeZ",       properties);
        xFreq               = FindProperty("_FogWaveFreqX",            properties);
        zFreq               = FindProperty("_FogWaveFreqZ",            properties);
    }
    
    public override void MaterialChanged(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        material.shaderKeywords = null;
        SetupMaterialBlendMode(material);
        SetMaterialKeywords(material);
    }
    
    public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
    {  
        foreach (var obj in  materialEditor.targets)
            MaterialChanged((Material)obj);
    }

    public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
    {
        if (materialEditorIn == null)
            throw new ArgumentNullException("materialEditorIn");

        FindProperties(properties); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
        materialEditor = materialEditorIn;
        Material material = materialEditor.target as Material;
            
        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
        // material to a lightweight shader.
        if (m_FirstTimeApply)
        {
            OnOpenGUI(material, materialEditorIn);
            m_FirstTimeApply = false;
        }
        
        if (material == null)
            throw new ArgumentNullException("material");

        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;

        // Detect any changes to the material
        EditorGUI.BeginChangeCheck();
        {
            DoPopup(Styles.workflowModeText, workflowMode, Styles.workflowNames);

            GUILayout.Label(Styles.surfaceProperties, EditorStyles.boldLabel);

            DoAlbedoArea();
            DoMetallicSpecularArea();
            DoNormalArea();

            materialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap,
                occlusionMap.textureValue != null ? occlusionStrength : null);

            DoEmissionArea(material);
            EditorGUI.BeginChangeCheck();
            materialEditor.TextureScaleOffsetProperty(albedoMap);
            if (EditorGUI.EndChangeCheck())
                emissionMap.textureScaleAndOffset =
                    albedoMap
                        .textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake

            EditorGUILayout.Space();

            materialEditor.ShaderProperty(highlights,  Styles.highlightsText);
            materialEditor.ShaderProperty(reflections, Styles.reflectionsText);

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
            fogEmissionColor.colorValue = EditorGUILayout.ColorField(GUIContent.none, fogEmissionColor.colorValue, false, false, true);
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
                if (overrideColor)
                {
                    RenderSettings.fogColor = fogColor.colorValue;
                }
                standartFogOverride.floatValue = overrideColor ? 1 : 0;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                standartFogOverride.floatValue = 0;
            }

            GUILayout.Label("Advanced Options", EditorStyles.boldLabel);

            materialEditor.EnableInstancingField();

            GUILayout.Label("Copyright (c) 2018 Sergey Klimenko.", new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                clipping = TextClipping.Clip,
                fontSize = 10,
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
            foreach (var obj in materialEditor.targets)
                MaterialChanged((Material) obj);
        }
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        // _Emission property is lost after assigning Standard shader to the material
        // thus transfer it before assigning the new shader
        if (material.HasProperty("_Emission"))
        {
            material.SetColor("_EmissionColor", material.GetColor("_Emission"));
        }

        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
        {
            SetupMaterialBlendMode(material);
            return;
        }

        SurfaceType surfaceType = SurfaceType.Opaque;
        BlendMode   blendMode   = BlendMode.Alpha;
        if (oldShader.name.Contains("/Transparent/Cutout/"))
        {
            surfaceType = SurfaceType.Opaque;
            material.SetFloat("_AlphaClip", 1);
        }
        else if (oldShader.name.Contains("/Transparent/"))
        {
            // NOTE: legacy shaders did not provide physically based transparency
            // therefore Fade mode
            surfaceType = SurfaceType.Transparent;
            blendMode   = BlendMode.Alpha;
        }

        material.SetFloat("_Surface", (float) surfaceType);
        material.SetFloat("_Blend",   (float) blendMode);

        if (oldShader.name.Equals("Standard (Specular setup)"))
        {
            material.SetFloat("_WorkflowMode", (float) WorkflowMode.Specular);
            Texture texture = material.GetTexture("_SpecGlossMap");
            if (texture != null)
                material.SetTexture("_MetallicSpecGlossMap", texture);
        }
        else
        {
            material.SetFloat("_WorkflowMode", (float) WorkflowMode.Metallic);
            Texture texture = material.GetTexture("_MetallicGlossMap");
            if (texture != null)
                material.SetTexture("_MetallicSpecGlossMap", texture);
        }

        MaterialChanged(material);
    }

    void DoAlbedoArea()
    {
        materialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);
    }

    void DoNormalArea()
    {
        materialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap,
            bumpMap.textureValue != null ? bumpScale : null);
        if (bumpScale.floatValue != 1 &&
            UnityEditorInternal.InternalEditorUtility.IsMobilePlatform(EditorUserBuildSettings.activeBuildTarget))
            if (materialEditor.HelpBoxWithButton(Styles.bumpScaleNotSupported, Styles.fixNow))
                bumpScale.floatValue = 1;
    }

    void DoEmissionArea(Material material)
    {
        // Emission for GI?
        if (materialEditor.EmissionEnabledProperty())
        {
            bool hadEmissionTexture = emissionMap.textureValue != null;

            // Texture and HDR color controls
            materialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap, emissionColorForRendering,
                false);

            // If texture was assigned and color was black set color to white
            float brightness = emissionColorForRendering.colorValue.maxColorComponent;
            if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                emissionColorForRendering.colorValue = Color.white;

            // LW does not support RealtimeEmissive. We set it to bake emissive and handle the emissive is black right.
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            if (brightness <= 0f)
                material.globalIlluminationFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        }
    }

    void DoMetallicSpecularArea()
    {
        string[] metallicSpecSmoothnessChannelName;
        bool     hasGlossMap = false;
        if ((WorkflowMode) workflowMode.floatValue == WorkflowMode.Metallic)
        {
            hasGlossMap                       = metallicGlossMap.textureValue != null;
            metallicSpecSmoothnessChannelName = Styles.metallicSmoothnessChannelNames;
            materialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicGlossMap,
                hasGlossMap ? null : metallic);
        }
        else
        {
            hasGlossMap                       = specGlossMap.textureValue != null;
            metallicSpecSmoothnessChannelName = Styles.specularSmoothnessChannelNames;
            materialEditor.TexturePropertySingleLine(Styles.specularMapText, specGlossMap,
                hasGlossMap ? null : specColor);
        }

        bool showSmoothnessScale = hasGlossMap;
        if (smoothnessMapChannel != null)
        {
            int smoothnessChannel = (int) smoothnessMapChannel.floatValue;
            if (smoothnessChannel == (int) SmoothnessMapChannel.AlbedoAlpha)
                showSmoothnessScale = true;
        }

        int indentation = 2; // align with labels of texture properties
        materialEditor.ShaderProperty(showSmoothnessScale ? smoothnessScale : smoothness,
            showSmoothnessScale ? Styles.smoothnessScaleText : Styles.smoothnessText, indentation);

        int prevIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 3;
        if (smoothnessMapChannel != null)
            DoPopup(Styles.smoothnessMapChannelText, smoothnessMapChannel, metallicSpecSmoothnessChannelName);
        EditorGUI.indentLevel = prevIndentLevel;
    }

    static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
    {
        int ch = (int) material.GetFloat("_SmoothnessTextureChannel");
        if (ch == (int) SmoothnessMapChannel.AlbedoAlpha)
            return SmoothnessMapChannel.AlbedoAlpha;

        return SmoothnessMapChannel.SpecularMetallicAlpha;
    }

    static void SetMaterialKeywords(Material material)
    {
        // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
        // (MaterialProperty value might come from renderer material property block)
        bool isSpecularWorkFlow = (WorkflowMode) material.GetFloat("_WorkflowMode") == WorkflowMode.Specular;
        bool hasGlossMap        = false;
        if (isSpecularWorkFlow)
            hasGlossMap = material.GetTexture("_SpecGlossMap");
        else
            hasGlossMap = material.GetTexture("_MetallicGlossMap");

        CoreUtils.SetKeyword(material, "_SPECULAR_SETUP", isSpecularWorkFlow);

        CoreUtils.SetKeyword(material, "_METALLICSPECGLOSSMAP", hasGlossMap);
        CoreUtils.SetKeyword(material, "_SPECGLOSSMAP",         hasGlossMap && isSpecularWorkFlow);
        CoreUtils.SetKeyword(material, "_METALLICGLOSSMAP",     hasGlossMap && !isSpecularWorkFlow);

        CoreUtils.SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap"));

        CoreUtils.SetKeyword(material, "_SPECULARHIGHLIGHTS_OFF", material.GetFloat("_SpecularHighlights") == 0.0f);
        CoreUtils.SetKeyword(material, "_GLOSSYREFLECTIONS_OFF",  material.GetFloat("_GlossyReflections") == 0.0f);

        CoreUtils.SetKeyword(material, "_OCCLUSIONMAP", material.GetTexture("_OcclusionMap"));
        //CoreUtils.SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
        //CoreUtils.SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));

        CoreUtils.SetKeyword(material, "_RECEIVE_SHADOWS_OFF", material.GetFloat("_ReceiveShadows") == 0.0f);

        // A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
        // or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
        // The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
        MaterialEditor.FixupEmissiveFlag(material);
        bool shouldEmissionBeEnabled =
            (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
        CoreUtils.SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

        if (material.HasProperty("_SmoothnessTextureChannel"))
        {
            CoreUtils.SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
                GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
        }
    }
}
#endif