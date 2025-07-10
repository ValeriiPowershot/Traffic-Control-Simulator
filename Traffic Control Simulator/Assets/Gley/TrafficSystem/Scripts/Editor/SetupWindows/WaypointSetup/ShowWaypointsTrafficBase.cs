using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public abstract class ShowWaypointsTrafficBase : TrafficSetupWindow
    {
        protected float scrollAdjustment = 210;
        protected TrafficWaypointData trafficWaypointData;
        protected TrafficWaypointDrawer trafficWaypointDrawer;
        protected WaypointSettings[] waypointsOfInterest;
        

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            trafficWaypointData = CreateInstance<TrafficWaypointData>().Initialize();
            trafficWaypointDrawer = CreateInstance<TrafficWaypointDrawer>().Initialize(trafficWaypointData);
            trafficWaypointDrawer.onWaypointClicked += WaypointClicked;
            base.Initialize(windowProperties, window);
            return this;
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUI.BeginChangeCheck();
           

            EditorGUILayout.BeginHorizontal();
            editorSave.showConnections = EditorGUILayout.Toggle("Show Connections", editorSave.showConnections, GUILayout.Width(TOGGLE_WIDTH));
            editorSave.editorColors.waypointColor = EditorGUILayout.ColorField(editorSave.editorColors.waypointColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            editorSave.showOtherLanes = EditorGUILayout.Toggle("Show Lane Change", editorSave.showOtherLanes, GUILayout.Width(TOGGLE_WIDTH));
            editorSave.editorColors.laneChangeColor = EditorGUILayout.ColorField(editorSave.editorColors.laneChangeColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            editorSave.showSpeed = EditorGUILayout.Toggle("Show Speed", editorSave.showSpeed, GUILayout.Width(TOGGLE_WIDTH));
            editorSave.editorColors.speedColor = EditorGUILayout.ColorField(editorSave.editorColors.speedColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            editorSave.showVehicles = EditorGUILayout.Toggle("Show Cars", editorSave.showVehicles, GUILayout.Width(TOGGLE_WIDTH));
            editorSave.editorColors.agentColor = EditorGUILayout.ColorField(editorSave.editorColors.agentColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            editorSave.showPriority = EditorGUILayout.Toggle("Show Waypoint Priority", editorSave.showPriority, GUILayout.Width(TOGGLE_WIDTH));
            editorSave.editorColors.priorityColor = EditorGUILayout.ColorField(editorSave.editorColors.priorityColor);
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }

        protected override void ScrollPart(float width, float height)
        {
            if (waypointsOfInterest != null)
            {
                if (waypointsOfInterest.Length == 0)
                {
                    EditorGUILayout.LabelField("No " + GetWindowTitle());
                }
                for (int i = 0; i < waypointsOfInterest.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(waypointsOfInterest[i].name);
                    if (GUILayout.Button("View", GUILayout.Width(BUTTON_DIMENSION)))
                    {
                        GleyUtilities.TeleportSceneCamera(waypointsOfInterest[i].transform.position);
                        SceneView.RepaintAll();
                    }
                    if (GUILayout.Button("Edit", GUILayout.Width(BUTTON_DIMENSION)))
                    {
                        OpenEditWindow(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No " + GetWindowTitle());
            }
            base.ScrollPart(width, height);
        }


        protected void OpenEditWindow(int index)
        {
            SettingsWindow.SetSelectedWaypoint((WaypointSettings)waypointsOfInterest[index]);
            GleyUtilities.TeleportSceneCamera(waypointsOfInterest[index].transform.position);
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        protected virtual void WaypointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick)
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        public override void DestroyWindow()
        {
            trafficWaypointDrawer.onWaypointClicked -= WaypointClicked;

            DestroyImmediate(trafficWaypointData);
            DestroyImmediate(trafficWaypointDrawer);

            base.DestroyWindow();
        }
    }
}