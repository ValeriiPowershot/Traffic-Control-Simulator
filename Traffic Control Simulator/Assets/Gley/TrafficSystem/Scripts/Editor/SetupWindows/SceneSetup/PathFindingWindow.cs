using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class PathFindingWindow : TrafficSetupWindow
    {
        private WaypointSettings[] waypointsOfInterest;
        private TrafficWaypointData trafficWaypointData;
        private TrafficWaypointDrawer waypointDrawer;
        private float scrollAdjustment = 171;
        private List<int> penalties;
        private bool showPenaltyEditedWaypoints;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            trafficWaypointData = CreateInstance<TrafficWaypointData>().Initialize();
            waypointDrawer = CreateInstance<TrafficWaypointDrawer>().Initialize(trafficWaypointData);
            waypointsOfInterest = trafficWaypointData.GetPenlatyEditedWaypoints();
            penalties = GetDifferentPenalties(trafficWaypointData.GetAllWaypoints());
            if (editorSave.pathFindingRoutes.routesColor.Count < penalties.Count)
            {
                int nrOfColors = penalties.Count - editorSave.pathFindingRoutes.routesColor.Count;
                for (int i = 0; i < nrOfColors; i++)
                {
                    editorSave.pathFindingRoutes.routesColor.Add(Color.white);
                    editorSave.pathFindingRoutes.active.Add(true);
                }
            }
            waypointDrawer.onWaypointClicked += WaypointClicked;
            return this;
        }

        private List<int> GetDifferentPenalties(WaypointSettings[] allWaypoints)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (!result.Contains(allWaypoints[i].penalty))
                {
                    result.Add(allWaypoints[i].penalty);
                }
            }
            return result;
        }

        private void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        protected override void TopPart()
        {
            base.TopPart();
            editorSave.pathFindingEnabled = EditorGUILayout.Toggle("Enable Path Finding", editorSave.pathFindingEnabled);

            EditorGUI.BeginChangeCheck();
            showPenaltyEditedWaypoints = EditorGUILayout.Toggle("Show Edited Waypoints", showPenaltyEditedWaypoints);

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
           
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            if (showPenaltyEditedWaypoints)
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
                    EditorGUILayout.LabelField("No priority edited waypoints");
                }
            }
            else
            { 
                EditorGUILayout.LabelField("Waypoint Penalties: ");
                for (int i = 0; i < penalties.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(penalties[i].ToString(), GUILayout.MaxWidth(50));
                    editorSave.pathFindingRoutes.routesColor[i] = EditorGUILayout.ColorField(editorSave.pathFindingRoutes.routesColor[i]);
                    Color oldColor = GUI.backgroundColor;
                    if (editorSave.pathFindingRoutes.active[i])
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View"))
                    {
                        editorSave.pathFindingRoutes.active[i] = !editorSave.pathFindingRoutes.active[i];
                        SceneView.RepaintAll();
                    }

                    GUI.backgroundColor = oldColor;
                    EditorGUILayout.EndHorizontal();
                } 
            }
            base.ScrollPart(width, height);
            EditorGUILayout.EndScrollView();
        }


        protected void OpenEditWindow(int index)
        {
            SettingsWindow.SetSelectedWaypoint(waypointsOfInterest[index]);
            GleyUtilities.TeleportSceneCamera(waypointsOfInterest[index].transform.position);
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        public override void DrawInScene()
        {
            if (showPenaltyEditedWaypoints)
            {
                waypointDrawer.ShowPenaltyEditedWaypoints(editorSave.editorColors.waypointColor);
            }
            else
            {
                for (int i = 0; i < penalties.Count; i++)
                {
                    if (editorSave.pathFindingRoutes.active[i])
                    {
                        waypointDrawer.ShowWaypointsWithPenalty(penalties[i], editorSave.pathFindingRoutes.routesColor[i]);
                    }
                }
            }
            base.DrawInScene();
        }


        protected override void BottomPart()
        {
            if (GUILayout.Button("Save"))
            {
                    Save();
            }
            base.BottomPart();
        }


        private void Save()
        {
            Debug.Log("Save");
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
