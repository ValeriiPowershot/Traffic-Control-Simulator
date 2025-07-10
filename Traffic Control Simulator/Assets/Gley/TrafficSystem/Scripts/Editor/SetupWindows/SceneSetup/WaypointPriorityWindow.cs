using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class WaypointPriorityWindow : TrafficSetupWindow
    {
        private List<int> priorities;
        private float scrollAdjustment = 104;
        private TrafficWaypointData trafficWaypointData;
        private TrafficWaypointDrawer waypointDrawer;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            trafficWaypointData = CreateInstance<TrafficWaypointData>().Initialize();
            waypointDrawer = CreateInstance<TrafficWaypointDrawer>().Initialize(trafficWaypointData);
            priorities = GetDifferentPriorities(trafficWaypointData.GetAllWaypoints());
            if (editorSave.priorityRoutes.routesColor.Count < priorities.Count)
            {
                int nrOfColors = priorities.Count - editorSave.priorityRoutes.routesColor.Count;
                for (int i = 0; i < nrOfColors; i++)
                {
                    editorSave.priorityRoutes.routesColor.Add(Color.white);
                    editorSave.priorityRoutes.active.Add(true);
                }
            }

            waypointDrawer.onWaypointClicked += WaypointClicked;
            return this;
        }

        private List<int> GetDifferentPriorities(WaypointSettings[] allWaypoints)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (!result.Contains(allWaypoints[i].priority))
                {
                    result.Add(allWaypoints[i].priority);
                }
            }
            return result;
        }
        private void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        public override void DrawInScene()
        {
            for (int i = 0; i < priorities.Count; i++)
            {
                if (editorSave.priorityRoutes.active[i])
                {
                    waypointDrawer.ShowWaypointsWithPriority(priorities[i], editorSave.priorityRoutes.routesColor[i]);
                }
            }

            base.DrawInScene();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUILayout.LabelField("Waypoint Priorities: ");
            for (int i = 0; i < priorities.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(priorities[i].ToString(), GUILayout.MaxWidth(50));
                editorSave.priorityRoutes.routesColor[i] = EditorGUILayout.ColorField(editorSave.priorityRoutes.routesColor[i]);
                Color oldColor = GUI.backgroundColor;
                if (editorSave.priorityRoutes.active[i])
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View"))
                {
                    editorSave.priorityRoutes.active[i] = !editorSave.priorityRoutes.active[i];
                    SceneView.RepaintAll();
                }

                GUI.backgroundColor = oldColor;
                EditorGUILayout.EndHorizontal();
            }

            base.ScrollPart(width, height);
            EditorGUILayout.EndScrollView();
        }


        public override void DestroyWindow()
        {
            waypointDrawer.onWaypointClicked -= WaypointClicked;

            DestroyImmediate(trafficWaypointData);
            DestroyImmediate(waypointDrawer);
            base.DestroyWindow();
        }
    }
}
