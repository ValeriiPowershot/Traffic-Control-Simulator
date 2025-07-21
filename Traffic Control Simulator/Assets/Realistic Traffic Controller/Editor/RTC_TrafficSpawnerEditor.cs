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

[CustomEditor(typeof(RTC_TrafficSpawner))]
public class RTC_TrafficSpawnerEditor : Editor {

    RTC_TrafficSpawner prop;

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmos(RTC_TrafficSpawner spawner, GizmoType gizmoType) {

        if ((gizmoType & GizmoType.Selected) != 0) {

            Gizmos.color = Color.green;

        } else {

            Gizmos.color = Color.green * .75f;

        }

        Gizmos.DrawWireSphere(spawner.transform.position, spawner.radius);
        Gizmos.DrawWireSphere(spawner.transform.position, spawner.closeRadius);

        Gizmos.matrix = spawner.transform.localToWorldMatrix;
        Gizmos.color = Color.green;

    }

    public override void OnInspectorGUI() {

        prop = (RTC_TrafficSpawner)target;
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("trafficVehicles"), new GUIContent("Spawnable Traffic Vehicles", "Spawnable traffic vehicles."), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"), new GUIContent("Radius", "Spawnable radius. Vehicles out of range would be disabled."), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("closeRadius"), new GUIContent("Close Radius", "Vehicles won't be spawned in this radius."), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoCalculateCloseRadius"), new GUIContent("Auto Calculate Close Radius", "Calculates the closer radiues related to the radius. Simply dividing by 2."), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ignoreCloseRadiusOnEnable"), new GUIContent("Ignore Close Radius On Enable", "Ignores close radius when enabled to populate the scene."), false);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("mainCamera"), new GUIContent("Main Camera", "Main camera will be used to positioning."), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("followType"), new GUIContent("Follow Type", "Follow the main camera, or setparent to it?"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumActiveVehicles"), new GUIContent("Maximum Active Vehicles", "Maximum possible active vehicles."), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("checkTrafficPerSecond"), new GUIContent("Check Traffic Per Second", "Checks the traffic vehicles per second. Using lower values will give you best accurate, but sufficate the performance."), false);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dontSpawnBehindCamera"), new GUIContent("Don't Spawn Behind Camera", "Won't spawn vehicles at behind of the camera."), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("disableVehiclesBehindCamera"), new GUIContent("Disable Vehicles Behind Camera", "Disables vehicles at behind of the camera."), false);

        if (prop.disableVehiclesBehindCamera) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToDisable"), new GUIContent("Distance To Disable Vehicles Behind Camera", "Distance to disable vehicles behind the camera if it's enabled."), false);
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();

        GUI.enabled = false;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnedTrafficVehicles"), new GUIContent("Spawned Traffic Vehicles", "Spawned traffic vehicles by this spawner. If you want to get all active traffic vehicles, get them from the RTC_SceneManager."), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waypointsInRadius"), new GUIContent("Waypoints In Radius", "Waypoints in radius."), true);

        GUI.enabled = true;

        EditorGUILayout.Space();

        DrawFooter();

        Repaint();

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void DrawFooter() {

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("BoneCracker Games", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Realistic Traffic Controller " + RTC_Version.version, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndHorizontal();

    }

}
