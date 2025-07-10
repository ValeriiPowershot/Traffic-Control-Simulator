using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class SpeedRoutesSetupWindow : TrafficSetupWindow
    {
        private List<int> speeds;
        private float scrollAdjustment = 104;
        private TrafficWaypointData trafficWaypointData;
        private TrafficWaypointDrawer waypointDrawer;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            trafficWaypointData = CreateInstance<TrafficWaypointData>().Initialize();
            waypointDrawer = CreateInstance<TrafficWaypointDrawer>().Initialize(trafficWaypointData);
            speeds = GetDifferentSpeeds(trafficWaypointData.GetAllWaypoints());
            if (editorSave.speedRoutes.routesColor.Count < speeds.Count)
            {
                int nrOfColors = speeds.Count - editorSave.speedRoutes.routesColor.Count;
                for (int i = 0; i < nrOfColors; i++)
                {
                    editorSave.speedRoutes.routesColor.Add(Color.white);
                    editorSave.speedRoutes.active.Add(true);
                }
            }

            waypointDrawer.onWaypointClicked += WaypointClicked;
            return this;
        }

        private List<int> GetDifferentSpeeds(WaypointSettings[] allWaypoints)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (!result.Contains(allWaypoints[i].maxSpeed))
                {
                    result.Add(allWaypoints[i].maxSpeed);
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
            for (int i = 0; i < speeds.Count; i++)
            {
                if (editorSave.speedRoutes.active[i])
                {
                    waypointDrawer.ShowWaypointsWithSpeed(speeds[i], editorSave.speedRoutes.routesColor[i]);
                }
            }

            base.DrawInScene();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUILayout.LabelField("SpeedRoutes: ");
            for (int i = 0; i < speeds.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(speeds[i].ToString(), GUILayout.MaxWidth(50));
                editorSave.speedRoutes.routesColor[i] = EditorGUILayout.ColorField(editorSave.speedRoutes.routesColor[i]);
                Color oldColor = GUI.backgroundColor;
                if (editorSave.speedRoutes.active[i])
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View"))
                {
                    editorSave.speedRoutes.active[i] = !editorSave.speedRoutes.active[i];
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
