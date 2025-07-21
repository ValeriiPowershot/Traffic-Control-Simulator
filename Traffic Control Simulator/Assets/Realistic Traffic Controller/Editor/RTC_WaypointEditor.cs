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

[CanEditMultipleObjects]
[CustomEditor(typeof(RTC_Waypoint))]
public class RTC_WaypointEditor : Editor {

    RTC_Waypoint prop;
    Color guiColor;
    static bool debug;
    static bool listCloserWaypoints;

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmos(RTC_Waypoint waypoint, GizmoType gizmoType) {

        if (!waypoint.ignoreGizmos) {

            if ((gizmoType & GizmoType.Selected) != 0) {

                Gizmos.color = RTC_Settings.Instance.selectedWaypointColor;

            } else {

                if (waypoint.firstWaypoint)
                    Gizmos.color = RTC_Settings.Instance.firstWaypointColor;
                else if (waypoint.lastWaypoint)
                    Gizmos.color = RTC_Settings.Instance.lastWaypointColor;
                else
                    Gizmos.color = RTC_Settings.Instance.unselectedWaypointColor;

                Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, .5f);

            }

            Gizmos.DrawSphere(waypoint.transform.position, waypoint.radius);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = RTC_Settings.Instance.textColor;

            if (SceneView.lastActiveSceneView && Vector3.Distance(waypoint.transform.position, SceneView.lastActiveSceneView.camera.transform.position) < RTC_Settings.Instance.textDistance)
                Handles.Label((waypoint.transform.position + Vector3.up * 3f) + (Vector3.forward * -0f), waypoint.transform.parent.name + "_" + (waypoint.transform.GetSiblingIndex() + 0).ToString(), style);

        }

        Gizmos.matrix = waypoint.transform.localToWorldMatrix;
        Gizmos.color = RTC_Settings.Instance.arrowColor;

        if (waypoint.previousWaypoint)
            Gizmos.DrawMesh((Mesh)Resources.Load("Arrow", typeof(Mesh)), waypoint.transform.InverseTransformPoint(Vector3.Lerp(waypoint.transform.position, waypoint.previousWaypoint.transform.position, .5f) + Vector3.up * 2f), Quaternion.identity, new Vector3(3f, 3f, 3f));

    }

    private void OnEnable() {

        guiColor = GUI.color;

    }

    public override void OnInspectorGUI() {

        GUI.skin = (GUISkin)Resources.Load("RTC_GUISkin", typeof(GUISkin));
        prop = (RTC_Waypoint)target;
        serializedObject.Update();

        if (prop.waypointPosition != prop.transform.position)
            prop.connectedLane.UpdateLane();

        prop.waypointPosition = prop.transform.position;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("nextWaypoint"), new GUIContent("Next Waypoint", "Next waypoint"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("previousWaypoint"), new GUIContent("Previous Waypoint", "Previous waypoint"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interConnectionWaypoint"), new GUIContent("InterConnection Waypoint", "InterConnection waypoint"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"), new GUIContent("Radius", "Radius"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetSpeed"), new GUIContent("Target Speed", "Target speed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("wait"), new GUIContent("Wait", "Wait at this waypoint for x seconds."));

        EditorGUILayout.Space();

        GUI.color = Color.green;

        if (Selection.objects.Length <= 1 && prop.index != (prop.connectedLane.transform.childCount - 1) && GUILayout.Button("Create Next Waypoint"))
            CreateNextWaypoint();

        GUI.color = Color.yellow;

        if (Selection.objects.Length <= 1 && GUILayout.Button("Select Connected Lane"))
            Selection.activeGameObject = prop.connectedLane.gameObject;

        GUI.color = Color.red;

        if (Selection.objects.Length <= 1 && GUILayout.Button("Remove Waypoint"))
            RemoveWaypoint();

        GUI.color = guiColor;

        if (Selection.objects.Length <= 1) {

            listCloserWaypoints = EditorGUILayout.ToggleLeft("List Closer Waypoints To Connect", listCloserWaypoints);

            if (listCloserWaypoints) {

                EditorGUILayout.HelpBox("Only possible and closer waypoints are listed. If waypoint you want to connect is not in the list, angle or distance of the waypoint is not eligible.", MessageType.Info);

                RTC_Waypoint[] waypoints = FindObjectsOfType<RTC_Waypoint>();
                List<RTC_Waypoint> closestWaypoints = new List<RTC_Waypoint>();

                for (int i = 0; i < waypoints.Length; i++) {

                    bool waypointBehind;

                    if (Vector3.Dot((waypoints[i].transform.position - prop.transform.position).normalized, prop.transform.forward) > .5f)
                        waypointBehind = false;
                    else
                        waypointBehind = true;

                    if (!waypoints[i].ignoreGizmos && !waypointBehind && waypoints[i] != prop && Vector3.Distance(waypoints[i].transform.position, prop.transform.position) < 20f)
                        closestWaypoints.Add(waypoints[i]);

                }

                for (int i = 0; i < closestWaypoints.Count; i++) {

                    if (GUILayout.Button("Connect To " + "[" + closestWaypoints[i].transform.parent.name + "_" + closestWaypoints[i].transform.GetSiblingIndex().ToString() + "]" + " As Next")) {

                        prop.nextWaypoint = closestWaypoints[i];
                        EditorUtility.SetDirty(prop);

                    }

                    if (GUILayout.Button("Connect To " + "[" + closestWaypoints[i].transform.parent.name + "_" + closestWaypoints[i].transform.GetSiblingIndex().ToString() + "]" + " As Previous")) {

                        prop.previousWaypoint = closestWaypoints[i];
                        EditorUtility.SetDirty(prop);

                    }

                    if (GUILayout.Button("Connect To " + "[" + closestWaypoints[i].transform.parent.name + "_" + closestWaypoints[i].transform.GetSiblingIndex().ToString() + "]" + " As InterConnection")) {

                        prop.interConnectionWaypoint = closestWaypoints[i];
                        EditorUtility.SetDirty(prop);

                    }

                    EditorGUILayout.Space();

                }

            }

        }

        EditorGUILayout.Space();

        debug = EditorGUILayout.ToggleLeft("Debug Info", debug);

        if (debug) {

            GUI.enabled = false;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("connectedLane"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("index"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("firstWaypoint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lastWaypoint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("desiredSpeedForNextWaypoint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("desiredSpeedForInterConnectionWaypoint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToNextWaypoint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToInterConnectionWaypoint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("angleToNextWaypoint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("angleToInterConnectionWaypoint"));

            GUI.enabled = true;

        }

        EditorGUILayout.Space();

        prop.ignoreGizmos = EditorGUILayout.ToggleLeft("Ignore Gizmos", prop.ignoreGizmos);

        EditorGUILayout.Space();

        DrawFooter();

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        Repaint();
        serializedObject.ApplyModifiedProperties();

    }

    private void CreateNextWaypoint() {

        GameObject newWaypoint = new GameObject("Waypoint" + (prop.index + 1).ToString(), typeof(RTC_Waypoint));
        newWaypoint.transform.SetParent(prop.connectedLane.transform, false);
        newWaypoint.transform.position = prop.transform.position;
        newWaypoint.transform.position += prop.transform.forward * 15f;
        RTC_Waypoint newWaypointComponent = newWaypoint.GetComponent<RTC_Waypoint>();

        newWaypointComponent.connectedLane = prop.connectedLane;
        newWaypointComponent.radius = prop.radius;
        newWaypointComponent.targetSpeed = prop.targetSpeed;

        newWaypointComponent.nextWaypoint = prop.nextWaypoint;
        newWaypointComponent.previousWaypoint = prop;
        newWaypointComponent.nextWaypoint.previousWaypoint = newWaypointComponent;

        prop.transform.name = "Waypoint" + "_";
        prop.nextWaypoint = newWaypointComponent;

        RTC.InsertWaypoint(prop.connectedLane, newWaypointComponent, prop.index);

        EditorUtility.SetDirty(prop);

        Selection.activeGameObject = prop.connectedLane.gameObject;

    }

    private void RemoveWaypoint() {

        RTC_Waypoint previousWaypoint = prop.previousWaypoint;
        RTC_Waypoint nextWaypoint = prop.nextWaypoint;

        if (previousWaypoint && nextWaypoint) {

            previousWaypoint.nextWaypoint = nextWaypoint;
            nextWaypoint.previousWaypoint = previousWaypoint;

        }

        Undo.DestroyObjectImmediate(prop.gameObject);

    }

    private void DrawFooter() {

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("BoneCracker Games", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Realistic Traffic Controller " + RTC_Version.version, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndHorizontal();

    }

}
