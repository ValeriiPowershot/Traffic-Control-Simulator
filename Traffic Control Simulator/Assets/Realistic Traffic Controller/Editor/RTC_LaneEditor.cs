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
using System.Threading.Tasks;

[CanEditMultipleObjects]
[CustomEditor(typeof(RTC_Lane))]
public class RTC_LaneEditor : Editor {

    public RTC_Lane waypointManager;
    private Color guiColor;

    public static bool creatingNextWaypoint;
    public static bool creatingPreviousWaypoint;
    public static bool creatingNewWaypoint;
    public static bool showWaypoints;
    public static bool showInfo;
    public static bool showTypes;
    public static bool showGizmos;

    private float createWithRadius = 1.5f;
    private float createWithSpeed = 50f;

    private void OnEnable() {

        guiColor = GUI.color;
        EditorApplication.update += EditorUpdate;
        SceneView.duringSceneGui += OnScene;

    }

    private void OnDisable() {

        EditorApplication.update -= EditorUpdate;
        SceneView.duringSceneGui -= OnScene;

    }

    private void EditorUpdate() {

        if (creatingNextWaypoint || creatingPreviousWaypoint || creatingNewWaypoint)
            Selection.activeGameObject = waypointManager.gameObject;

    }

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmos(RTC_Lane lane, GizmoType gizmoType) {

        if ((gizmoType & GizmoType.Selected) != 0)
            Gizmos.color = RTC_Settings.Instance.selectedLaneColor;
        else
            Gizmos.color = RTC_Settings.Instance.unselectedLaneColor;

        if (lane && lane.waypoints != null) {

            for (int i = 0; i < lane.waypoints.Length; i++) {

                if (lane.waypoints[i] != null) {

                    if (lane.waypoints[i].nextWaypoint)
                        Gizmos.DrawLine(lane.waypoints[i].transform.position, lane.waypoints[i].nextWaypoint.transform.position);

                    if (lane.waypoints[i].interConnectionWaypoint)
                        Gizmos.DrawLine(lane.waypoints[i].transform.position, lane.waypoints[i].interConnectionWaypoint.transform.position);

                }

            }

        }

    }

    public override void OnInspectorGUI() {

        GUI.skin = (GUISkin)Resources.Load("RTC_GUISkin", typeof(GUISkin));
        waypointManager = (RTC_Lane)target;
        serializedObject.Update();

        if (Selection.objects.Length <= 1)
            UpdateLane();

        if (Selection.objects.Length <= 1)
            Buttons();
        else
            EditorGUILayout.HelpBox("Lane configuration are disabled for multiple selection.", MessageType.Warning);

        GUI.color = guiColor;

        EditorGUILayout.Space();

        EditorGUILayout.EndFoldoutHeaderGroup();

        showTypes = EditorGUILayout.Foldout(showTypes, "Types");

        if (showTypes) {

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("supportedLight"), new GUIContent("Light Vehicles", "Light vehicles can use this lane"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("supportedMedium"), new GUIContent("Medium Vehicles", "Medium vehicles can use this lane"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("supportedHeavy"), new GUIContent("Heavy Vehicles", "Heavy vehicles can use this lane"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("supportedSedan"), new GUIContent("Sedan Vehicles", "Sedan vehicles can use this lane"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("supportedTaxi"), new GUIContent("Taxi Vehicles", "Taxi vehicles can use this lane"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("supportedBus"), new GUIContent("Bus Vehicles", "Bus vehicles can use this lane"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("supportedExotic"), new GUIContent("Exotic Vehicles", "Exotic vehicles can use this lane"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("supportedVan"), new GUIContent("Van Vehicles", "Van vehicles can use this lane"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("supportedPolice"), new GUIContent("Police Vehicles", "Police vehicles can use this lane"), false);

            EditorGUI.indentLevel--;

        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        if (Selection.objects.Length <= 1) {

            showWaypoints = EditorGUILayout.Foldout(showWaypoints, "Waypoints");

            if (showWaypoints) {

                EditorGUI.indentLevel++;

                for (int i = 0; i < waypointManager.waypoints.Length; i++) {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(waypointManager.waypoints[i], typeof(RTC_Waypoint), true);

                    float orgWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 55f;
                    waypointManager.waypoints[i].radius = EditorGUILayout.FloatField("Radius", waypointManager.waypoints[i].radius, GUILayout.Width(80f));
                    waypointManager.waypoints[i].targetSpeed = EditorGUILayout.FloatField("Speed", waypointManager.waypoints[i].targetSpeed, GUILayout.Width(80f));
                    waypointManager.waypoints[i].wait = EditorGUILayout.FloatField("Wait", waypointManager.waypoints[i].wait, GUILayout.Width(80f));
                    EditorGUIUtility.labelWidth = orgWidth;

                    GUI.color = Color.red;

                    if (GUILayout.Button("X", GUILayout.Width(20f)))
                        RemoveWaypoint(i);

                    GUI.color = guiColor;

                    EditorGUILayout.EndHorizontal();

                }

                EditorGUI.indentLevel--;

            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            showInfo = EditorGUILayout.Foldout(showInfo, "Info");

            if (showInfo) {

                EditorGUI.indentLevel++;

                float averageSpeed = 0f;

                for (int i = 0; i < waypointManager.waypoints.Length; i++) {

                    averageSpeed += waypointManager.waypoints[i].targetSpeed;

                }

                averageSpeed /= Mathf.Clamp(waypointManager.waypoints.Length, 1, Mathf.Infinity);

                EditorGUILayout.LabelField("Lane has " + waypointManager.waypoints.Length.ToString() + " waypoints at total with average speed of " + averageSpeed.ToString() + ".");

                float totalLength = 0f;

                for (int i = 0; i < waypointManager.waypoints.Length; i++)
                    totalLength += waypointManager.waypoints[i].distanceToNextWaypoint;

                totalLength /= Mathf.Clamp(waypointManager.waypoints.Length, 1, Mathf.Infinity);

                EditorGUILayout.LabelField("Total Length: " + totalLength.ToString() + " Meters.");

                EditorGUI.indentLevel--;

            }

            EditorGUILayout.EndFoldoutHeaderGroup();

        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        showGizmos = EditorGUILayout.Foldout(showGizmos, "Gizmos");

        if (showGizmos) {

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("hideMiddleWaypointsGizmos"), new GUIContent("Hide Middle Waypoints Gizmos", "Hide Middle Waypoints Gizmos."), false);

            EditorGUI.indentLevel--;

        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        DrawFooter();

        Repaint();

        if (GUI.changed)
            EditorUtility.SetDirty(waypointManager);

        serializedObject.ApplyModifiedProperties();

    }

    private void Buttons() {

        EditorGUILayout.BeginVertical(GUI.skin.window);

        GUI.color = Color.red;

        if (creatingNewWaypoint || creatingNextWaypoint || creatingPreviousWaypoint) {

            if (GUILayout.Button("Cancel"))
                DisableCreateMode();

        }

        if (creatingNewWaypoint) {

            GUILayout.Label("Creating new first waypoint now.\nHold left shift and click on your scene to create a new waypoint now.");

            EditorGUILayout.EndVertical();

            Repaint();

            if (GUI.changed)
                EditorUtility.SetDirty(waypointManager);

            serializedObject.ApplyModifiedProperties();

            return;

        }

        if (creatingNextWaypoint) {

            GUILayout.Label("Creating new next waypoint now.\nHold left shift and click on your scene to create a new waypoint now.");

            EditorGUILayout.EndVertical();

            Repaint();

            if (GUI.changed)
                EditorUtility.SetDirty(waypointManager);

            serializedObject.ApplyModifiedProperties();

            return;

        }

        if (creatingPreviousWaypoint) {

            GUILayout.Label("Creating new previous waypoint now.\nHold left shift and click on your scene to create a new waypoint now.");

            EditorGUILayout.EndVertical();

            Repaint();

            if (GUI.changed)
                EditorUtility.SetDirty(waypointManager);

            serializedObject.ApplyModifiedProperties();

            return;

        }

        GUI.color = guiColor;

        EditorGUILayout.BeginHorizontal();

        if (waypointManager.waypoints.Length < 1) {

            if (!creatingNewWaypoint) {

                GUI.color = Color.green;

                if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_AddWaypoint"))) {

                    creatingNewWaypoint = !creatingNewWaypoint;
                    creatingNextWaypoint = false;
                    creatingPreviousWaypoint = false;

                }

            } else {

                GUI.color = Color.red;

                if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_Cancel"))) {

                    creatingNewWaypoint = !creatingNewWaypoint;
                    creatingNextWaypoint = false;
                    creatingPreviousWaypoint = false;

                }

            }

            GUI.color = guiColor;

            if (creatingNewWaypoint) {

                float orgWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 45f;
                createWithRadius = EditorGUILayout.FloatField("Radius", createWithRadius, GUILayout.Width(100f));
                createWithSpeed = EditorGUILayout.FloatField("Speed", createWithSpeed, GUILayout.Width(100f));
                EditorGUIUtility.labelWidth = orgWidth;

            }

        } else {

            if (!creatingNextWaypoint) {

                GUI.color = Color.green;

                if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_AddWaypoint"))) {

                    creatingNextWaypoint = !creatingNextWaypoint;
                    creatingNewWaypoint = false;
                    creatingPreviousWaypoint = false;

                    createWithRadius = waypointManager.waypoints[waypointManager.waypoints.Length - 1].radius;
                    createWithSpeed = waypointManager.waypoints[waypointManager.waypoints.Length - 1].targetSpeed;

                }

            } else {

                GUI.color = Color.red;

                if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_Cancel"))) {

                    creatingNextWaypoint = !creatingNextWaypoint;
                    creatingNewWaypoint = false;
                    creatingPreviousWaypoint = false;

                    createWithRadius = waypointManager.waypoints[waypointManager.waypoints.Length - 1].radius;
                    createWithSpeed = waypointManager.waypoints[waypointManager.waypoints.Length - 1].targetSpeed;

                }

            }

            GUI.color = guiColor;

            if (creatingNextWaypoint) {

                float orgWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 45f;
                createWithRadius = EditorGUILayout.FloatField("Radius", createWithRadius, GUILayout.Width(100f));
                createWithSpeed = EditorGUILayout.FloatField("Speed", createWithSpeed, GUILayout.Width(100f));
                EditorGUIUtility.labelWidth = orgWidth;

            }

            if (!creatingPreviousWaypoint) {

                GUI.color = Color.green;

                if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_AddWaypointPrevious"))) {

                    creatingPreviousWaypoint = !creatingPreviousWaypoint;
                    creatingNextWaypoint = false;
                    creatingNewWaypoint = false;

                    createWithRadius = waypointManager.waypoints[0].radius;
                    createWithSpeed = waypointManager.waypoints[0].targetSpeed;

                }

            } else {

                GUI.color = Color.red;

                if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_AddWaypoint"))) {

                    creatingPreviousWaypoint = !creatingPreviousWaypoint;
                    creatingNextWaypoint = false;
                    creatingNewWaypoint = false;

                    createWithRadius = waypointManager.waypoints[0].radius;
                    createWithSpeed = waypointManager.waypoints[0].targetSpeed;

                }

            }



            GUI.color = guiColor;

            if (creatingPreviousWaypoint) {

                float orgWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 45f;
                createWithRadius = EditorGUILayout.FloatField("Radius", createWithRadius, GUILayout.Width(100f));
                createWithSpeed = EditorGUILayout.FloatField("Speed", createWithSpeed, GUILayout.Width(100f));
                EditorGUIUtility.labelWidth = orgWidth;

            }

            GUI.color = Color.blue;

            if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_Focus")))
                RTC_EditorCoroutines.Execute(FocusTo(waypointManager.waypoints[0].gameObject));

            GUI.color = guiColor;

        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.window);
        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.cyan;

        if (waypointManager.waypoints.Length > 2) {

            if (waypointManager.waypoints[0].previousWaypoint == null && waypointManager.waypoints[waypointManager.waypoints.Length - 1].nextWaypoint == null) {

                if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_CloseCircuit")))
                    CloseCircuit();

            } else {

                if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_BreakCircuit")))
                    BreakCircuit();

            }

        }

        if (waypointManager.waypoints.Length >= 2) {

            if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_ReverseCircuit")))
                ReverseCircuit();

        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        GUI.color = guiColor;

        if (waypointManager.waypoints.Length >= 2) {

            EditorGUILayout.BeginVertical(GUI.skin.window);
            EditorGUILayout.BeginHorizontal();

            GUI.color = Color.yellow;

            if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_Align_X")))
                AlignX();

            if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_Align_Y")))
                AlignY();

            if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_Align_Z")))
                AlignZ();

            GUI.color = guiColor;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.Space();

        if (waypointManager.waypoints.Length >= 1) {

            GUI.color = Color.red;

            if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_Waypoint_Remove")))
                RemoveLastWaypoint();

            GUI.color = guiColor;

        }

        GUI.color = Color.green;

        if (GUILayout.Button((Texture)Resources.Load("Editor Icons/Icon_AddRoad")))
            CreateNewLane();

        GUI.color = guiColor;

    }

    private void OnSceneGUI() {

        if (waypointManager && waypointManager.waypoints != null) {

            for (int i = 0; i < waypointManager.waypoints.Length; i++) {

                if (waypointManager.waypoints[i]) {

                    EditorGUI.BeginChangeCheck();

                    Vector3 waypointPosition = Handles.PositionHandle(waypointManager.waypoints[i].transform.position, Quaternion.identity);

                    if (EditorGUI.EndChangeCheck()) {

                        Undo.RecordObject(waypointManager.waypoints[i], "Changed Waypoint");
                        waypointManager.waypoints[i].waypointPosition = waypointPosition;
                        waypointManager.waypoints[i].transform.position = waypointPosition;

                    }

                }

            }

        }

        Event e = Event.current;

        if (e != null) {

            if (e.isMouse && e.shift && e.type == EventType.MouseDown) {

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 5000.0f)) {

                    if (creatingNewWaypoint)
                        CreateNewWaypoint(hit.point);

                    if (creatingNextWaypoint)
                        CreateNextWaypoint(hit.point);

                    if (creatingPreviousWaypoint)
                        CreatePreviousWaypoint(hit.point);

                }

            }

        }

    }

    private void OnScene(SceneView sceneView) {

        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect((Screen.width - 250) / 2f, 10f, 500f, 500f));

        guiColor = GUI.color;
        GUI.color = Color.red;

        if (creatingNewWaypoint)
            GUILayout.Label("Creating New Waypoint Mode\nYou can't select other gameobjects now.\nPress 'Cancel' to exit mode.", EditorStyles.boldLabel);

        if (creatingNextWaypoint)
            GUILayout.Label("Creating Next Waypoint Mode\nYou can't select other gameobjects now.\nPress 'Cancel' to exit mode.", EditorStyles.boldLabel);

        if (creatingPreviousWaypoint)
            GUILayout.Label("Creating Previous Waypoint Mode\nYou can't select other gameobjects now.\nPress 'Cancel' to exit mode.", EditorStyles.boldLabel);

        GUI.color = guiColor;

        GUILayout.EndArea();

        Handles.EndGUI();

    }

    private void CreateNewWaypoint(Vector3 position) {

        RTC.CreateNewWaypoint(waypointManager, position, createWithRadius, createWithSpeed);
        RTC_EditorCoroutines.Execute(DisableNewCreation());
        EditorUtility.SetDirty(waypointManager);

    }

    private void CreatePreviousWaypoint(Vector3 position) {

        RTC.CreatePreviousWaypoint(waypointManager, position, createWithRadius, createWithSpeed);
        EditorUtility.SetDirty(waypointManager);

    }

    private void CreateNextWaypoint(Vector3 position) {

        RTC.CreateNextWaypoint(waypointManager, position, createWithRadius, createWithSpeed);
        EditorUtility.SetDirty(waypointManager);

    }

    private IEnumerator FocusTo(GameObject focusTo) {

        Selection.activeGameObject = focusTo;
        yield return new WaitForSecondsRealtime(.02f);
        EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
        yield return new WaitForSecondsRealtime(.02f);
        Selection.activeGameObject = waypointManager.gameObject;

    }

    private void CloseCircuit() {

        Undo.RegisterCompleteObjectUndo(waypointManager.waypoints, "Closed Circuit");
        RTC.CloseCircuit(waypointManager);

    }

    private void BreakCircuit() {

        Undo.RegisterCompleteObjectUndo(waypointManager.waypoints, "Breaked Circuit");
        RTC.BreakCircuit(waypointManager);

    }

    private void ReverseCircuit() {

        Undo.RegisterCompleteObjectUndo(waypointManager.waypoints, "Reverse Circuit");
        RTC.ReverseCircuit(waypointManager);

    }

    private void RemoveLastWaypoint() {

        RTC_Waypoint lastWaypoint = waypointManager.waypoints[waypointManager.waypoints.Length - 1];
        Undo.DestroyObjectImmediate(lastWaypoint.gameObject);

    }

    private void RemoveWaypoint(int index) {

        RTC_Waypoint currentWaypoint = waypointManager.waypoints[index];
        RTC_Waypoint nextWaypoint = currentWaypoint.nextWaypoint;
        RTC_Waypoint previousWaypoint = currentWaypoint.previousWaypoint;

        if (nextWaypoint && previousWaypoint) {

            nextWaypoint.previousWaypoint = previousWaypoint;
            previousWaypoint.nextWaypoint = nextWaypoint;

        }

        Undo.DestroyObjectImmediate(currentWaypoint.gameObject);

    }

    private void AlignX() {

        bool answer = EditorUtility.DisplayDialog("Aligning All Waypoints", "All waypoints will be aligned by this axis. Are you sure?", "Yes", "No");

        if (answer) {

            Undo.RegisterCompleteObjectUndo(waypointManager.waypoints, "Aligned X");
            RTC.AlignX(waypointManager);

        }

    }

    private void AlignY() {

        bool answer = EditorUtility.DisplayDialog("Aligning All Waypoints", "All waypoints will be aligned by this axis. Are you sure?", "Yes", "No");

        if (answer) {

            Undo.RegisterCompleteObjectUndo(waypointManager.waypoints, "Aligned Y");
            RTC.AlignY(waypointManager);

        }

    }

    private void AlignZ() {

        bool answer = EditorUtility.DisplayDialog("Aligning All Waypoints", "All waypoints will be aligned by this axis. Are you sure?", "Yes", "No");

        if (answer) {

            Undo.RegisterCompleteObjectUndo(waypointManager.waypoints, "Aligned Z");
            RTC.AlignZ(waypointManager);

        }

    }

    private void CreateNewLane() {

        Selection.activeGameObject = RTC.CreateNewLane().gameObject;

    }

    private void UpdateLane() {

        waypointManager.UpdateLane();

    }

    private IEnumerator DisableNewCreation() {

        yield return new WaitForSecondsRealtime(.5f);
        creatingNewWaypoint = false;
        creatingNextWaypoint = true;

    }

    public static void DisableCreateMode() {

        creatingNewWaypoint = false;
        creatingNextWaypoint = false;
        creatingPreviousWaypoint = false;

    }

    private void DrawFooter() {

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("BoneCracker Games", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Realistic Traffic Controller " + RTC_Version.version, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndHorizontal();

    }

}
