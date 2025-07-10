using Gley.UrbanAssets.Editor;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class WaypointSetupWindow : SetupWindowBase
    {
        protected override void ScrollPart(float width, float height)
        {
            EditorGUILayout.LabelField("Select action:");
            EditorGUILayout.Space();

            if (GUILayout.Button("Show All Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowAllWaypoints), true);     
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Disconnected Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowDisconnectedWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Vehicle Edited Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowVehicleTypeEditedWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Speed Edited Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowSpeedEditedWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Priority Edited Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowPriorityEditedWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Give Way Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowGiveWayWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Complex Give Way Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowComplexGiveWayWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Zipper Give Way Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowZipperGiveWayWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Event Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowEventWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Vehicle Path Problems"))
            {
                window.SetActiveWindow(typeof(ShowVehiclePathProblems), true);
            }
            EditorGUILayout.Space();

            base.ScrollPart(width, height);
        }
    }
}
