// SKGames vertical fog object controller. Copyright (c) 2018 Sergey Klimenko. 18.05.2018

using System;
using UnityEngine;

[AddComponentMenu("SKGames/Object Fog Controller")]
[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class ObjectFogController : MonoBehaviour
{
    public enum FogSpace
    {
        Local = 0,
        World = 1
    }

    public                                    Color    mainColor;
    [Header("Main fog configuration")] public FogSpace fogSimulationSpace;
    public                                    Color    fogColor;
    public                                    float    fogMinimalHeight;
    public                                    float    fogMaximalHeight;
    [Range(0.01f, 20.0f)] public              float    fogFalloff;

    [Space(), Header("Emission fog configuration")]
    public Color fogEmissionColor;
    [Range(0.0f,  100.0f)] public float fogEmissionPower;
    [Range(0.01f, 20.0f)]  public float fogEmissionFalloff;
    [ColorUsageAttribute(false, true, -10f, 10f, -10f, 10f)] public                        Color emissionColor = Color.black;
    [Range(0.0f, 1.0f)] public    float emissionPower = 0.0f;

    [Space()] [Header("Standard fog configuration")]
    public bool combineWithStandardFog;
    [Tooltip("Forward only")] public bool overrideStandardFogColor;

    [Space()] [Header("Fog animation configuration")]
    public bool useFogAnimation;
    public float fogWaveSpeedX;
    public float fogWaveSpeedZ;
    public float fogWaveAmplitudeX;
    public float fogWaveAmplitudeZ;
    public float fogWaveFreqX;
    public float fogWaveFreqZ;

    [HideInInspector] public MeshRenderer          renderer;
    [HideInInspector] public MaterialPropertyBlock mpb;

    [Space()] [HideInInspector] public bool overridedFromGlobalController = false;

    private void Awake()
    {
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        if (renderer == null)
        {
            renderer = GetComponent<MeshRenderer>();
        }
    }

    public void Update()
    {
        if (!(overridedFromGlobalController && GlobalObjectFogController.Exists) && mpb != null && renderer != null)
        {
            renderer.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", mainColor);
            mpb.SetFloat("_FogRelativeWorldOrLocal", (float) fogSimulationSpace);
            mpb.SetColor("_FogColor", fogColor);
            mpb.SetFloat("_FogMin",     fogMinimalHeight);
            mpb.SetFloat("_FogMax",     fogMaximalHeight);
            mpb.SetFloat("_FogFalloff", fogFalloff);
            mpb.SetColor("_FogEmissionColor", fogEmissionColor);
            mpb.SetFloat("_FogEmissionPower", fogEmissionPower);
            mpb.SetColor("_EmissionColor", emissionColor);
            mpb.SetFloat("_EmissionPower",      emissionPower);
            mpb.SetFloat("_FogEmissionFalloff", fogEmissionFalloff);
            mpb.SetFloat("_STANDARD_FOG",       combineWithStandardFog ? 1 : 0);
            mpb.SetFloat("_OVERRIDE_FOG_COLOR", overrideStandardFogColor ? 1 : 0);
            mpb.SetFloat("_ANIMATION",          useFogAnimation ? 1 : 0);
            mpb.SetFloat("_FogWaveSpeedX",      fogWaveSpeedX);
            mpb.SetFloat("_FogWaveSpeedZ",      fogWaveSpeedZ);
            mpb.SetFloat("_FogWaveAmplitudeX",  fogWaveAmplitudeX);
            mpb.SetFloat("_FogWaveAmplitudeZ",  fogWaveAmplitudeZ);
            mpb.SetFloat("_FogWaveFreqX",       fogWaveFreqX);
            mpb.SetFloat("_FogWaveFreqZ",       fogWaveFreqZ);
            renderer.SetPropertyBlock(mpb);
        }
    }

    private void OnEnable()
    {
        if (mpb != null && renderer != null)
        {
            renderer.GetPropertyBlock(mpb);
            mainColor = renderer.sharedMaterial.GetColor("_Color");
            fogSimulationSpace = (FogSpace) Enum.Parse(typeof(FogSpace),
                renderer.sharedMaterial.GetFloat("_FogRelativeWorldOrLocal").ToString());
            fogColor                 = renderer.sharedMaterial.GetColor("_FogColor");
            fogMinimalHeight         = renderer.sharedMaterial.GetFloat("_FogMin");
            fogMaximalHeight         = renderer.sharedMaterial.GetFloat("_FogMax");
            fogFalloff               = renderer.sharedMaterial.GetFloat("_FogFalloff");
            fogEmissionColor         = renderer.sharedMaterial.GetColor("_FogEmissionColor");
            fogEmissionPower         = renderer.sharedMaterial.GetFloat("_FogEmissionPower");
            emissionColor            = renderer.sharedMaterial.GetColor("_EmissionColor");
            emissionPower            = renderer.sharedMaterial.GetFloat("_EmissionPower");
            fogEmissionFalloff       = renderer.sharedMaterial.GetFloat("_FogEmissionFalloff");
            combineWithStandardFog   = renderer.sharedMaterial.GetFloat("_STANDARD_FOG") > 0.5f;
            overrideStandardFogColor = renderer.sharedMaterial.GetFloat("_OVERRIDE_FOG_COLOR") > 0.5f;
            useFogAnimation          = renderer.sharedMaterial.GetFloat("_ANIMATION") > 0.5f;
            fogWaveSpeedX            = renderer.sharedMaterial.GetFloat("_FogWaveSpeedX");
            fogWaveSpeedZ            = renderer.sharedMaterial.GetFloat("_FogWaveSpeedZ");
            fogWaveAmplitudeX        = renderer.sharedMaterial.GetFloat("_FogWaveAmplitudeX");
            fogWaveAmplitudeZ        = renderer.sharedMaterial.GetFloat("_FogWaveAmplitudeZ");
            fogWaveFreqX             = renderer.sharedMaterial.GetFloat("_FogWaveFreqX");
            fogWaveFreqZ             = renderer.sharedMaterial.GetFloat("_FogWaveFreqZ");
            if (GlobalObjectFogController.Exists)
            {
                if (GlobalObjectFogController.Exists)
                {
                    GlobalObjectFogController.AddFogController(this);
                }
            }

            GlobalObjectFogController.AddFogController(this);
        }
        else
        {
            mpb      = new MaterialPropertyBlock();
            renderer = GetComponent<MeshRenderer>();
            renderer.GetPropertyBlock(mpb);
            mainColor = renderer.sharedMaterial.GetColor("_Color");
            fogSimulationSpace = (FogSpace) Enum.Parse(typeof(FogSpace),
                renderer.sharedMaterial.GetFloat("_FogRelativeWorldOrLocal").ToString());
            fogColor                 = renderer.sharedMaterial.GetColor("_FogColor");
            fogMinimalHeight         = renderer.sharedMaterial.GetFloat("_FogMin");
            fogMaximalHeight         = renderer.sharedMaterial.GetFloat("_FogMax");
            fogFalloff               = renderer.sharedMaterial.GetFloat("_FogFalloff");
            fogEmissionColor         = renderer.sharedMaterial.GetColor("_FogEmissionColor");
            fogEmissionPower         = renderer.sharedMaterial.GetFloat("_FogEmissionPower");
            emissionColor            = renderer.sharedMaterial.GetColor("_EmissionColor");
            emissionPower            = renderer.sharedMaterial.GetFloat("_EmissionPower");
            fogEmissionFalloff       = renderer.sharedMaterial.GetFloat("_FogEmissionFalloff");
            combineWithStandardFog   = renderer.sharedMaterial.GetFloat("_STANDARD_FOG") > 0.5f;
            overrideStandardFogColor = renderer.sharedMaterial.GetFloat("_OVERRIDE_FOG_COLOR") > 0.5f;
            useFogAnimation          = renderer.sharedMaterial.GetFloat("_ANIMATION") > 0.5f;
            fogWaveSpeedX            = renderer.sharedMaterial.GetFloat("_FogWaveSpeedX");
            fogWaveSpeedZ            = renderer.sharedMaterial.GetFloat("_FogWaveSpeedZ");
            fogWaveAmplitudeX        = renderer.sharedMaterial.GetFloat("_FogWaveAmplitudeX");
            fogWaveAmplitudeZ        = renderer.sharedMaterial.GetFloat("_FogWaveAmplitudeZ");
            fogWaveFreqX             = renderer.sharedMaterial.GetFloat("_FogWaveFreqX");
            fogWaveFreqZ             = renderer.sharedMaterial.GetFloat("_FogWaveFreqZ");
            if (GlobalObjectFogController.Exists)
            {
                if (GlobalObjectFogController.Exists)
                {
                    GlobalObjectFogController.AddFogController(this);
                }
            }

            GlobalObjectFogController.AddFogController(this);
        }
    }

    private void OnDisable()
    {
        if (mpb != null && renderer != null)
        {
            mpb.Clear();
            renderer.SetPropertyBlock(mpb);
            if (GlobalObjectFogController.Exists)
            {
                GlobalObjectFogController.RemoveFogController(this);
            }
        }
    }
}