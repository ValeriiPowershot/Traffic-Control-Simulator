//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(RTC_CarController))]
public class RTC_CarControllerEditor : Editor {

    RTC_CarController prop;

    static bool drivetrain;
    static bool gearbox;
    static bool raycasts;
    static bool inputs;
    static bool navigation;
    static bool audio;
    static bool lights;
    static bool others;
    static bool events;
    static bool paints;
    static bool wheels;

    Texture iconDrivetrain;
    Texture iconGearbox;
    Texture iconRaycasts;
    Texture iconInputs;
    Texture iconNavigation;
    Texture iconAudio;
    Texture iconLights;
    Texture iconOthers;
    Texture iconEvents;
    Texture iconPaints;
    Texture iconWheels;

    public void Awake() {

        iconDrivetrain = (Texture)Resources.Load("Editor Icons/Icon_Drivetrain", typeof(Texture));
        iconGearbox = (Texture)Resources.Load("Editor Icons/Icon_Gearbox", typeof(Texture));
        iconRaycasts = (Texture)Resources.Load("Editor Icons/Icon_Raycasts", typeof(Texture));
        iconInputs = (Texture)Resources.Load("Editor Icons/Icon_Inputs", typeof(Texture));
        iconNavigation = (Texture)Resources.Load("Editor Icons/Icon_Navigation", typeof(Texture));
        iconAudio = (Texture)Resources.Load("Editor Icons/Icon_Audio", typeof(Texture));
        iconLights = (Texture)Resources.Load("Editor Icons/Icon_Lights", typeof(Texture));
        iconOthers = (Texture)Resources.Load("Editor Icons/Icon_Others", typeof(Texture));
        iconEvents = (Texture)Resources.Load("Editor Icons/Icon_Events", typeof(Texture));
        iconPaints = (Texture)Resources.Load("Editor Icons/Icon_Paints", typeof(Texture));
        iconWheels = (Texture)Resources.Load("Editor Icons/Icon_Wheels", typeof(Texture));

    }

    private void ToolbarMenu() {

        EditorGUILayout.BeginHorizontal();

        if (drivetrain)
            GUI.color = Color.green;

        if (GUILayout.Button(iconDrivetrain)) {

            bool previous = drivetrain;
            CloseAllMenus();
            drivetrain = !previous;

        }

        GUI.color = Color.white;

        if (wheels)
            GUI.color = Color.green;

        if (GUILayout.Button(iconWheels)) {

            bool previous = wheels;
            CloseAllMenus();
            wheels = !previous;

        }

        GUI.color = Color.white;

        if (gearbox)
            GUI.color = Color.green;

        if (GUILayout.Button(iconGearbox)) {

            bool previous = gearbox;
            CloseAllMenus();
            gearbox = !previous;

        }

        GUI.color = Color.white;

        if (raycasts)
            GUI.color = Color.green;

        if (GUILayout.Button(iconRaycasts)) {

            bool previous = raycasts;
            CloseAllMenus();
            raycasts = !previous;

        }

        GUI.color = Color.white;

        if (inputs)
            GUI.color = Color.green;

        if (GUILayout.Button(iconInputs)) {

            bool previous = inputs;
            CloseAllMenus();
            inputs = !previous;

        }

        GUI.color = Color.white;

        if (navigation)
            GUI.color = Color.green;

        if (GUILayout.Button(iconNavigation)) {

            bool previous = navigation;
            CloseAllMenus();
            navigation = !previous;

        }

        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.white;

        if (audio)
            GUI.color = Color.green;

        if (GUILayout.Button(iconAudio)) {

            bool previous = audio;
            CloseAllMenus();
            audio = !previous;

        }

        GUI.color = Color.white;

        if (lights)
            GUI.color = Color.green;

        if (GUILayout.Button(iconLights)) {

            bool previous = lights;
            CloseAllMenus();
            lights = !previous;

        }

        GUI.color = Color.white;

        if (others)
            GUI.color = Color.green;

        if (GUILayout.Button(iconOthers)) {

            bool previous = others;
            CloseAllMenus();
            others = !previous;

        }

        GUI.color = Color.white;

        if (events)
            GUI.color = Color.green;

        if (GUILayout.Button(iconEvents)) {

            bool previous = events;
            CloseAllMenus();
            events = !previous;

        }

        GUI.color = Color.white;

        if (paints)
            GUI.color = Color.green;

        if (GUILayout.Button(iconPaints)) {

            bool previous = paints;
            CloseAllMenus();
            paints = !previous;

        }

        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();

    }

    private void CloseAllMenus() {

        drivetrain = false;
        gearbox = false;
        raycasts = false;
        inputs = false;
        navigation = false;
        audio = false;
        lights = false;
        others = false;
        events = false;
        paints = false;
        wheels = false;

    }

    public override void OnInspectorGUI() {

        prop = (RTC_CarController)target;
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal(GUI.skin.window);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vehicleType"), new GUIContent("Vehicle Type", "Type of the vehicle. You can specify each lane with these types."), false);
        EditorGUILayout.EndHorizontal();

        ToolbarMenu();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.window);

        if (drivetrain) {

            EditorGUILayout.LabelField("Drivetrain", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("COM"), new GUIContent("COM", "Center of mass of the vehicle. Will be positioned automatically when you select all four wheel models."), false);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineRunning"), new GUIContent("Engine Running", "Engine is running now?"), false);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineTorque"), new GUIContent("Engine Torque", "Engine torque directly will be applied to the traction wheels."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineTorqueCurve"), new GUIContent("Engine Torque Curve", "Engine torque curve."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoGenerateEngineRPMCurve"), new GUIContent("Auto Generate Engine RPM Curve", "Auto generate engine RPM curve."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxEngineTorqueAtRPM"), new GUIContent("Maximum Engine Torque At RPM", "Maximum engine torque at RPM."), false);

            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentEngineRPM"), new GUIContent("Current Engine RPM", "Current engine rpm."), false);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("minEngineRPM"), new GUIContent("Minimum Engine RPM", "Minimum engine rpm."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxEngineRPM"), new GUIContent("Maximum Engine RPM", "Maximum engine rpm."), false);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("differentialRatio"), new GUIContent("Differential Ratio", "Differential ratio."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gearShiftThreshold"), new GUIContent("Gear Shift Threshold", "Gear shift threshold."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gearShiftUpRPM"), new GUIContent("Gear Shift Up RPM", "Gear shift up rpm."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gearShiftDownRPM"), new GUIContent("Gear Shift Down RPM", "Gear shift down rpm."), false);

            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentGear"), new GUIContent("Current Gear", "Current gear."), false);
            GUI.enabled = true;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumSpeed"), new GUIContent("Maximum Speed", "Maximum speed."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeTorque"), new GUIContent("Brake Torque", "Brake torque directly will be applied to the braked wheels."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("steerAngle"), new GUIContent("Steer Angle", "Maximum steer angle of the steering wheels."), false);

        }

        if (wheels) {

            EditorGUILayout.LabelField("Wheels", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wheels"), new GUIContent("Wheels", "All wheels of the vehicle. Select wheel models, and create their wheelcolliders."), true);
            EditorGUI.indentLevel--;

            if (Selection.gameObjects.Length <= 1) {

                if (prop.wheels != null) {

                    bool allWheelsSelected = true;

                    for (int i = 0; i < prop.wheels.Length; i++) {

                        if (prop.wheels[i].wheelModel == null)
                            allWheelsSelected = false;

                    }

                    if (allWheelsSelected && GUILayout.Button("Create WheelColliders"))
                        CreateWheelColliders();

                }

            }

        }

        if (gearbox) {

            EditorGUILayout.LabelField("Gearbox", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gearRatios"), new GUIContent("Gear Ratios", "Gear ratios."), true);
            EditorGUI.indentLevel--;

        }

        if (raycasts) {

            EditorGUILayout.LabelField("Raycasts", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("CarSpawnIndex"), new GUIContent("CarSpawnIndex", "Car Index."), false);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useRaycasts"), new GUIContent("Use Raycast", "Raycast will be used to detect obstacles at front of the vehicle."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastLayermask"), new GUIContent("Raycast Layermask", "Raycast will use these layers to detect obstacles."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastDistance"), new GUIContent("Raycast Distance", "Raycast distance."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastDistanceRate"), new GUIContent("Raycast Distance Rate", "Length of the raycast will be adjusted by speed of the vehicle."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastOrigin"), new GUIContent("Raycast Origin", "Origin point of the raycast."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnPositionOffset"), new GUIContent("Spawn Position Offset", "Spawn position offset."), false);

        }

        if (inputs) {

            EditorGUILayout.LabelField("Inputs", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothInputs"), new GUIContent("Smooth Inputs", "Inputs of the vehicle will be smoothed with 10x Time.deltaTime."), false);

            GUI.enabled = false;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("throttleInput"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeInput"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("steerInput"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("clutchInput"), false);

            GUI.enabled = true;

        }

        if (navigation) {

            EditorGUILayout.LabelField("Navigation", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("lookAhead"), new GUIContent("Look Ahead", "Offset for navigator point."), false);

            EditorGUILayout.Space();
            GUI.enabled = false;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentWaypoint"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nextWaypoint"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pastWaypoint"), false);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("interconnecting"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("desiredSpeed"), false);

            GUI.enabled = true;

        }

        if (audio) {

            EditorGUILayout.LabelField("Audio", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineSoundOn"), new GUIContent("Engine Sound On", "Engine sound on clip will be played on throttle press."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engineSoundOff"), new GUIContent("Engine Sound Off", "Engine sound off clip will be played on throttle release."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minAudioRadius"), new GUIContent("Minimum Audio Radius", "Minimum audio radius."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAudioRadius"), new GUIContent("Maximum Audio Radius", "Maximum audio radius."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minAudioVolume"), new GUIContent("Minimum Audio Volume", "Minimum audio volume."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAudioVolume"), new GUIContent("Maximum Audio Volume", "Maximum audio volume."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minAudioPitch"), new GUIContent("Minimum Audio Pitch", "Minimum audio pitch."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAudioPitch"), new GUIContent("Maximum Audio Pitch", "Maximum audio pitch."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("horn"), new GUIContent("Horn", "Horn will be played on unexpected stops."), false);

        }

        if (lights) {

            EditorGUILayout.LabelField("Lights", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("turnSignalsSystem"), new GUIContent("", ""), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lights"), new GUIContent("Lights", "All lights of the vehicle. Select lights, and set their types."), true);
            EditorGUI.indentLevel--;

        }

        if (others) {

            EditorGUILayout.LabelField("Others", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bounds"), new GUIContent("Bounds", "Bounds of the vehicle."), true);
            EditorGUI.indentLevel--;

            if (Selection.gameObjects.Length <= 1) {

                if (GUILayout.Button("Calculate Bounds"))
                    prop.CalculateBounds();

            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("canCrash"), new GUIContent("Can Crash", "Vehicle won't move after an accident."), false);

            if (prop.canCrash) {

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("crashImpact"), new GUIContent("Crash Impact", "Crash impact."), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("disableAfterCrash"), new GUIContent("Disable After Crash", "Disable after crash."), false);

                EditorGUI.indentLevel--;

            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("canTakeover"), new GUIContent("Takeover", "Tries to pass the obstacle."), false);

            if (prop.canTakeover)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("timeNeededToTakeover"), new GUIContent("Time Needed To Takeover", "How long vehicle will wait?"), false);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("optimization"), new GUIContent("Optimization", "Optimization will be used to disable aligning wheels, sounds, and other heavier process."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceForLOD"), new GUIContent("Distance For LOD", "Distance for enabling optimization features."), false);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isNight"), new GUIContent("Is Night", "Headlights will be enabled on night."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("checkUpsideDown"), new GUIContent("Check Upside Down", "Checks the vehicle if upside down and resets it."), false);

            EditorGUILayout.Space();

        }

        if (events) {

            EditorGUILayout.LabelField("Events", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("outputEvent_OnEnable"), new GUIContent("On Enable"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outputEvent_OnDisable"), new GUIContent("On Disable"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outputEvent_OnCollision"), new GUIContent("On Collision"), false);

        }

        if (paints) {

            EditorGUILayout.LabelField("Paints", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("paints"), new GUIContent("Paints", "Paints."), true);
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();





        EditorGUILayout.BeginVertical(GUI.skin.box);

        bool colliderFound = false;
        bool wheelsFound = true;
        bool missingWheelModelsFound = false;
        bool missingWheelCollidersFound = false;
        bool tractionWheelsFound = false;
        bool brakeWheelsFound = false;
        bool steeringWheelsFound = false;

        if (prop.wheels == null || (prop.wheels != null && prop.wheels.Length < 1))
            wheelsFound = false;

        if (prop.wheels != null) {

            for (int i = 0; i < prop.wheels.Length; i++) {

                if (prop.wheels[i] != null && prop.wheels[i].wheelModel == null)
                    missingWheelModelsFound = true;

                if (prop.wheels[i] != null && prop.wheels[i].wheelCollider == null)
                    missingWheelCollidersFound = true;

                if (prop.wheels[i] != null && prop.wheels[i].isTraction)
                    tractionWheelsFound = true;

                if (prop.wheels[i] != null && prop.wheels[i].isBraking)
                    brakeWheelsFound = true;

                if (prop.wheels[i] != null && prop.wheels[i].isSteering)
                    steeringWheelsFound = true;

            }

        }

        Collider[] colliders = prop.GetComponentsInChildren<Collider>();

        for (int i = 0; i < colliders.Length; i++) {

            if (colliders[i] is not WheelCollider && colliders[i] is not SphereCollider && colliders[i] is not CapsuleCollider)
                colliderFound = true;

        }

        if (!colliderFound)
            EditorGUILayout.HelpBox("Missing body collider, please create a body collider for this vehicle! You can add meshcollider to the main body.", MessageType.Error);

        if (!wheelsFound)
            EditorGUILayout.HelpBox("Missing wheels, please create at least four wheels for this vehicle!", MessageType.Error);

        if (missingWheelModelsFound)
            EditorGUILayout.HelpBox("Missing wheel models, please check wheel model of your wheels!", MessageType.Error);

        if (missingWheelCollidersFound)
            EditorGUILayout.HelpBox("Missing wheel colliders, please check wheel collider of your wheels! You can recreate them after selecting wheel models.", MessageType.Error);

        if (!tractionWheelsFound)
            EditorGUILayout.HelpBox("Traction wheels are missing, please check your wheels. Be sure to have one traction wheel at least! Otherwise vehicle won't move.", MessageType.Error);

        if (!brakeWheelsFound)
            EditorGUILayout.HelpBox("Brake wheels are missing, please check your wheels. Be sure to have one brake wheel at least! Otherwise vehicle won't stop.", MessageType.Error);

        if (!steeringWheelsFound)
            EditorGUILayout.HelpBox("Steering wheels are missing, please check your wheels. Be sure to have one steering wheel at least! Otherwise vehicle won't steer.", MessageType.Error);

        EditorGUILayout.EndVertical();

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        bool isPersistent = EditorUtility.IsPersistent(prop.gameObject);
        bool isIsolated = false;

        if (StageUtility.GetStage(prop.gameObject) != StageUtility.GetMainStage())
            isIsolated = true;

        if (isPersistent || isIsolated)
            GUI.enabled = false;

        if (Application.isPlaying)
            GUI.enabled = false;

        if (PrefabUtility.GetCorrespondingObjectFromSource(prop.gameObject) == null) {

            if (GUILayout.Button("Create Prefab")) {

                PrefabUtility.SaveAsPrefabAssetAndConnect(prop.gameObject, "Assets/" + prop.gameObject.name + ".prefab", InteractionMode.UserAction);
                Debug.Log("New prefab of this vehicle has been created in the ''Assets/''.");

            }

        } else {

            if (GUILayout.Button("Save Prefab"))
                PrefabUtility.SaveAsPrefabAssetAndConnect(prop.gameObject, "Assets/" + prop.gameObject.name + ".prefab", InteractionMode.UserAction);

        }

        GUI.enabled = true;

        EditorGUILayout.Space();

        DrawFooter();
        Repaint();

        if (GUI.changed && RTC_Settings.Instance.trafficVehiclesLayer != "") {

            prop.gameObject.layer = LayerMask.NameToLayer(RTC_Settings.Instance.trafficVehiclesLayer);

            if (RTC_Settings.Instance.layerChildren) {

                foreach (Transform item in prop.transform)
                    item.gameObject.layer = LayerMask.NameToLayer(RTC_Settings.Instance.trafficVehiclesLayer);

            }

        }

        serializedObject.ApplyModifiedProperties();

    }

    private void CreateWheelColliders() {

        if (prop.transform.Find("Wheel Colliders"))
            DestroyImmediate(prop.transform.Find("Wheel Colliders").gameObject);

        WheelCollider[] allWheelColliders = prop.GetComponentsInChildren<WheelCollider>(true);

        for (int i = 0; i < allWheelColliders.Length; i++) {

            DestroyImmediate(allWheelColliders[i]);

        }

        RTC.CreateWheelColliders(prop);

    }

    private void DrawFooter() {

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("BoneCracker Games", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Realistic Traffic Controller " + RTC_Version.version, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndHorizontal();

    }

}
