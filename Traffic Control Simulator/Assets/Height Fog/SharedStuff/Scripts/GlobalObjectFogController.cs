// SKGames vertical fog global object controller. Copyright (c) 2018 Sergey Klimenko. 18.05.2018
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[AddComponentMenu("SKGames/Global Fog Controller")]
[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class GlobalObjectFogController: MonoBehaviour
{
    public enum FogSpace
    {
        Local = 0,
        World = 1
    }

    public Color mainColor = Color.red;
    [Header("Main fog configuration")]
    public ObjectFogController.FogSpace fogSimulationSpace = ObjectFogController.FogSpace.World;
    public Color fogColor = Color.white;
    public float fogMinimalHeight = -5f;
    public float fogMaximalHeight = 1;
    [Range(0.01f, 20.0f)]
    public float fogFalloff = 5f;

    [Space()]
    [Header("Emission fog configuration")]
    public Color fogEmissionColor;
    [Range(0.0f, 100.0f)]
    public float fogEmissionPower = 1f;
    [Range(0.01f, 20.0f)]
    public float fogEmissionFalloff = 0.01f;
    [ColorUsageAttribute(false, true, -10f, 10f, -10f, 10f)] public Color emissionColor = Color.black;
    [Range(0.0f, 1.0f)]
    public float emissionPower = 0.0f;

    [Space()]
    [Header("Standard fog configuration")]
    public bool combineWithStandardFog = false;
    [Tooltip("Forward only")]
    public bool overrideStandardFogColor = false;

    [Space()]
    [Header("Fog animation configuration")]
    public bool useFogAnimation = false;
    public float fogWaveSpeedX = 0f;
    public float fogWaveSpeedZ = 0f;
    public float fogWaveAmplitudeX = 0f;
    public float fogWaveAmplitudeZ = 0f;
    public float fogWaveFreqX = 0f;
    public float fogWaveFreqZ = 0f;
    [HideInInspector]
    public List<ObjectFogController> controllers;
    public static bool Exists {
        get { return instance != null && instance.enabled; }
    }

    private static GlobalObjectFogController instance;
    private Camera cam;

    private void OnEnable()
    {
        if (GlobalObjectFogController.Exists && GlobalObjectFogController.instance != this)
        {
            DestroyImmediate(this);
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Can't add manager.", "Only one manager allowed on scene! (Manager exists on: " + GameObject.FindObjectOfType<GlobalObjectFogController>().name + ")", "Ok", "");
#endif
        }
        if (instance == null)
        {
            instance = this;
        }

        ObjectFogController[] _fogControllers = GameObject.FindObjectsOfType<ObjectFogController>();
        for (int i = 0; i < _fogControllers.Length; i++)
        {
            if (controllers == null)
            {
                controllers = new List<ObjectFogController>();
            }

            if (!controllers.Contains(_fogControllers[i]))
            {
                controllers.Add(_fogControllers[i]);
            }
        }

        cam = Camera.main;
        if (cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }
        if (cam == null)
        {
            Debug.LogError("Can't find camera!");
        }
    }

    private void OnDisable()
    {
        if (controllers != null)
        {
            controllers.Clear();
        }
    }

    public static void AddFogController(ObjectFogController c)
    {
        if (instance == null)
            return;

        if (instance.controllers == null)
        {
            instance.controllers = new List<ObjectFogController>();
        }
        if (!instance.controllers.Contains(c))
        {
            instance.controllers.Add(c);
        }
    }

    public static void RemoveFogController(ObjectFogController c)
    {
        if (instance == null)
            return;

        if (instance.controllers != null)
        {
            if (instance.controllers.Contains(c))
            {
                instance.controllers.Remove(c);
            }
        }
    }

    private void Update()
    {
        if (controllers == null || controllers != null && controllers.Count == 0)
            return;

        if (cam != null)
        {
            if (cam.clearFlags != CameraClearFlags.Color)
            {
                cam.clearFlags = CameraClearFlags.Color;
            }

            if (cam.backgroundColor != fogColor)
            {
                cam.backgroundColor = fogColor;
            }
        }

        for (int i = 0; i < controllers.Count; i++)
        {
            if (!controllers[i].overridedFromGlobalController)
                continue;

            controllers[i].renderer.GetPropertyBlock(controllers[i].mpb);
            controllers[i].mpb.SetColor("_Color", mainColor);
            controllers[i].mpb.SetFloat("_FogRelativeWorldOrLocal", (float)fogSimulationSpace);
            controllers[i].mpb.SetColor("_FogColor", fogColor);
            controllers[i].mpb.SetFloat("_FogMin",             fogMinimalHeight);
            controllers[i].mpb.SetFloat("_FogMax",             fogMaximalHeight);
            controllers[i].mpb.SetFloat("_FogFalloff",         fogFalloff);
            controllers[i].mpb.SetColor("_FogEmissionColor", fogEmissionColor);
            controllers[i].mpb.SetFloat("_FogEmissionPower",   fogEmissionPower);
            controllers[i].mpb.SetColor("_EmissionColor", emissionColor);
            controllers[i].mpb.SetFloat("_EmissionPower", emissionPower);
            controllers[i].mpb.SetFloat("_FogEmissionFalloff", fogEmissionFalloff);
            controllers[i].mpb.SetFloat("_STANDARD_FOG",       combineWithStandardFog ? 1 : 0);
            controllers[i].mpb.SetFloat("_OVERRIDE_FOG_COLOR", overrideStandardFogColor ? 1 : 0);
            controllers[i].mpb.SetFloat("_ANIMATION",          useFogAnimation ? 1 : 0);
            controllers[i].mpb.SetFloat("_FogWaveSpeedX",      fogWaveSpeedX);
            controllers[i].mpb.SetFloat("_FogWaveSpeedZ",      fogWaveSpeedZ);
            controllers[i].mpb.SetFloat("_FogWaveAmplitudeX",  fogWaveAmplitudeX);
            controllers[i].mpb.SetFloat("_FogWaveAmplitudeZ",  fogWaveAmplitudeZ);
            controllers[i].mpb.SetFloat("_FogWaveFreqX",       fogWaveFreqX);
            controllers[i].mpb.SetFloat("_FogWaveFreqZ",       fogWaveFreqZ);
            controllers[i].renderer.SetPropertyBlock(controllers[i].mpb);
        }
    }
}
