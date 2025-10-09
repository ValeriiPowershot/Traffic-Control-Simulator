//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright � 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Realistic_Traffic_Controller.Scripts;

[CustomEditor(typeof(RTC_SceneManager))]
public class RTC_SceneManagerEditor : Editor {

    RTC_SceneManager prop;
    Color guiColor;

    private void OnEnable() {

        guiColor = GUI.color;

    }

    public override void OnInspectorGUI() {

        GUI.skin = (GUISkin)Resources.Load("RTC_GUISkin", typeof(GUISkin));
        prop = (RTC_SceneManager)target;
        serializedObject.Update();

        if (prop.transform.GetSiblingIndex() != 0 && GUILayout.Button("Raise!"))
            prop.transform.SetSiblingIndex(0);

        DrawDefaultInspector();
        GetEverything();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.yellow;

        EditorGUILayout.BeginVertical();

        GUILayout.Label("Update Everything");

        if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_Refresh")))
            UpdateEverything();

        EditorGUILayout.EndVertical();

        GUI.color = Color.green;

        EditorGUILayout.BeginVertical();

        GUILayout.Label("Add New Lane");

        if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_AddRoad")))
            CreateNewLane();

        EditorGUILayout.EndVertical();

        GUI.color = guiColor;

        EditorGUILayout.EndHorizontal();

        if (FindObjectOfType<RTC_TrafficSpawner>() == null)
            EditorGUILayout.HelpBox("RTC_TrafficSpawner is missing. Traffic vehicles will not be spawned without this. You can craete it from the toolbar menu.", MessageType.Warning);

        EditorGUILayout.Space();

        DrawFooter();

        Repaint();

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void GetEverything() {

        prop.allLanes = FindObjectsOfType<RTC_Lane>();
        prop.allWaypoints = FindObjectsOfType<RTC_Waypoint>();

    }

    private void CreateNewLane() {

        Selection.activeGameObject = RTC.CreateNewLane().gameObject;

    }

    private void UpdateEverything() {

        RTC.UpdateEverything();

    }

    private void DrawFooter() {

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("BoneCracker Games", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Realistic Traffic Controller " + RTC_Version.version, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndHorizontal();

    }

}
