#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Internal;
#endif
using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class PriorityIntersectionWindow : IntersectionWindow
    {
        private readonly float scrollAdjustment = 171;

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
#if GLEY_PEDESTRIAN_SYSTEM
            pedestrianWaypointDrawer.onWaypointClicked += PedestrianWaypointClicked;
#endif
            return this;
        }


        protected override void TopPart()
        {
            name = EditorGUILayout.TextField("Intersection Name", name);
            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }
            EditorGUI.BeginChangeCheck();
            hideWaypoints = EditorGUILayout.Toggle("Hide Waypoints ", hideWaypoints);
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                window.BlockClicks(!hideWaypoints);
            }
            base.TopPart();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            switch (currentAction)
            {
                case WindowActions.None:
                    IntersectionOverview();
                    break;
                case WindowActions.AssignRoadWaypoints:
                    AddTrafficWaypoints();
                    break;
                case WindowActions.AddExitWaypoints:
                    AddExitWaypoints();
                    break;
#if GLEY_PEDESTRIAN_SYSTEM
                case WindowActions.AddDirectionWaypoints:
                    AddDirectionWaypoints();
                    break;
                case WindowActions.AssignPedestrianWaypoints:
                    AddPedestrianWaypoints();
                    break;
#endif
            }
            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }


#if GLEY_PEDESTRIAN_SYSTEM
        private void PedestrianWaypointClicked(PedestrianWaypointSettings clickedWaypoint, bool leftClick)
        {
            switch (currentAction)
            {
                case WindowActions.AddDirectionWaypoints:
                    int road = GetRoadFromWaypoint(clickedWaypoint);
                    AddWaypointToList(clickedWaypoint, stopWaypoints[road].directionWaypoints);
                    directionWaypoints = selectedIntersection.GetDirectionWaypoints();
                    break;
                case WindowActions.AssignPedestrianWaypoints:
                    AddWaypointToList(clickedWaypoint, stopWaypoints[selectedRoad].pedestrianWaypoints);
                    pedestrianWaypoints = selectedIntersection.GetPedestrianWaypoints();
                    break;
            }
            SceneView.RepaintAll();
            SettingsWindowBase.TriggerRefreshWindowEvent();
        }
#endif


        private void IntersectionOverview()
        {
            ViewAssignedStopWaypoints();
            EditorGUILayout.Space();      
#if GLEY_PEDESTRIAN_SYSTEM
            ViewDirectionWaypoints();
            EditorGUILayout.Space();
#endif
            ViewExitWaypoints();
        }
        #region StopWaypoints


        private void ViewAssignedStopWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            Color oldColor;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Road " + (i + 1));
                EditorGUILayout.LabelField(new GUIContent("Stop waypoints:", "The vehicle will stop at this point until the intersection allows it to continue. " +
                "\nEach road that enters in intersection should have its own set of stop waypoints"));

                DisplayList(stopWaypoints[i].roadWaypoints, ref stopWaypoints[i].draw);
#if GLEY_PEDESTRIAN_SYSTEM
                EditorGUILayout.LabelField("Pedestrian waypoints:");
                DisplayList(stopWaypoints[i].pedestrianWaypoints, ref stopWaypoints[i].draw);
#endif
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Assign Road Waypoints"))
                {
                    selectedRoad = i;
                    currentAction = WindowActions.AssignRoadWaypoints;
                }

                oldColor = GUI.backgroundColor;
                if (stopWaypoints[i].draw == true)
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View Road Waypoints"))
                {
                    ViewRoadWaypoints(i);
                }
                GUI.backgroundColor = oldColor;

                if (GUILayout.Button("Delete Road"))
                {
                    stopWaypoints.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
#if GLEY_PEDESTRIAN_SYSTEM
                if (GUILayout.Button("Assign Pedestrian Waypoints"))
                {
                    selectedRoad = i;
                    currentAction = WindowActions.AssignPedestrianWaypoints;
                }
#endif
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Add Road"))
            {
                stopWaypoints.Add(new IntersectionStopWaypointsSettings());
            }
            EditorGUILayout.EndVertical();
        }



        private void AddTrafficWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Road " + (selectedRoad + 1));
            EditorGUILayout.LabelField(new GUIContent("Stop waypoints:", "The vehicle will stop at this point until the intersection allows it to continue. " +
            "\nEach road that enters in intersection should have its own set of stop waypoints"));

            DisplayList(stopWaypoints[selectedRoad].roadWaypoints, ref stopWaypoints[selectedRoad].draw);

            EditorGUILayout.Space();
            Color oldColor = GUI.backgroundColor;
            if (stopWaypoints[selectedRoad].draw == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Road Waypoints"))
            {
                ViewRoadWaypoints(selectedRoad);
            }
            GUI.backgroundColor = oldColor;

            if (GUILayout.Button("Done"))
            {
                Cancel();
            }
            EditorGUILayout.EndVertical();
        }


#if GLEY_PEDESTRIAN_SYSTEM
        private void AddPedestrianWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Road " + (selectedRoad + 1));
            EditorGUILayout.LabelField(new GUIContent("Pedestrian stop waypoints:", "pedestrians will stop at this point until the intersection allows them to continue. " +
            "\nEach crossing in intersection should have its own set of stop waypoints corresponding to its road"));

            DisplayList(stopWaypoints[selectedRoad].pedestrianWaypoints, ref stopWaypoints[selectedRoad].draw);

            EditorGUILayout.Space();
            Color oldColor = GUI.backgroundColor;
            if (stopWaypoints[selectedRoad].draw == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Road Waypoints"))
            {
                ViewRoadWaypoints(selectedRoad);
            }
            GUI.backgroundColor = oldColor;

            if (GUILayout.Button("Done"))
            {
                Cancel();
            }
            EditorGUILayout.EndVertical();
        }
#endif


        private void ViewRoadWaypoints(int i)
        {
            stopWaypoints[i].draw = !stopWaypoints[i].draw;
            for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
            {
                stopWaypoints[i].roadWaypoints[j].draw = stopWaypoints[i].draw;
            }
#if GLEY_PEDESTRIAN_SYSTEM
            for (int j = 0; j < stopWaypoints[i].pedestrianWaypoints.Count; j++)
            {
                stopWaypoints[i].pedestrianWaypoints[j].draw = stopWaypoints[i].draw;
            }
#endif
        }
        #endregion


        #region ExitWaypoints
        private void ViewExitWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Exit waypoints:", "When a vehicle touches an exit waypoint, it is no longer considered inside intersection.\n" +
                "For every lane that exits the intersection a single exit point should be marked"));
            EditorGUILayout.Space();
            DisplayList(exitWaypoints, ref editorSave.showExitWaypoints);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Exit Waypoints"))
            {
                selectedRoad = -1;
                currentAction = WindowActions.AddExitWaypoints;
            }
            Color oldColor = GUI.backgroundColor;
            if (editorSave.showExitWaypoints == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Exit Waypoints"))
            {
                ViewAllExitWaypoints();
            }
            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }


        private void AddExitWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Exit waypoints:", "When a vehicle touches an exit waypoint, it is no longer considered inside intersection.\n" +
                "For every lane that exits the intersection a single exit point should be marked"));
            EditorGUILayout.Space();
            DisplayList(exitWaypoints, ref editorSave.showExitWaypoints);
            EditorGUILayout.Space();
            Color oldColor = GUI.backgroundColor;
            if (editorSave.showExitWaypoints == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Exit Waypoints"))
            {
                ViewAllExitWaypoints();
            }
            GUI.backgroundColor = oldColor;

            if (GUILayout.Button("Done"))
            {
                Cancel();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

#if GLEY_PEDESTRIAN_SYSTEM
        #region DirectionWaypoints
        private void ViewDirectionWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Crossing direction waypoints:", "For each stop waypoint a direction needs to be specified\n" +
                "Only if a pedestrian goes to that direction it will stop, otherwise it will pass through stop waypoint"));
            EditorGUILayout.Space();

            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                DisplayList(stopWaypoints[i].directionWaypoints, ref editorSave.showDirectionWaypoints);
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Direction Waypoints"))
            {
                selectedRoad = -1;
                currentAction = WindowActions.AddDirectionWaypoints;
            }
            Color oldColor = GUI.backgroundColor;
            if (editorSave.showDirectionWaypoints == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Direction Waypoints"))
            {
                ViewAllDirectionWaypoints();
            }
            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void AddDirectionWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Crossing direction waypoints:", "For each stop waypoint a direction needs to be specified\n" +
                "Only if a pedestrian goes to that direction it will stop, otherwise it will pass through stop waypoint"));
            EditorGUILayout.Space();

            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                DisplayList(stopWaypoints[i].directionWaypoints, ref editorSave.showDirectionWaypoints);
            }
            EditorGUILayout.Space();

            Color oldColor = GUI.backgroundColor;
            if (editorSave.showDirectionWaypoints == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Direction Waypoints"))
            {
                ViewAllDirectionWaypoints();
            }
            GUI.backgroundColor = oldColor;

            if (GUILayout.Button("Done"))
            {
                Cancel();
            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        private int GetRoadFromWaypoint(PedestrianWaypointSettings waypoint)
        {
            int road = -1;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                for (int j = 0; j < stopWaypoints[i].pedestrianWaypoints.Count; j++)
                {
                    for (int k = 0; k < waypoint.prev.Count; k++)
                    {
                        if (waypoint.prev[k] == stopWaypoints[i].pedestrianWaypoints[j])
                        {
                            road = i;
                            break;
                        }

                    }
                    for (int k = 0; k < waypoint.neighbors.Count; k++)
                    {
                        if (waypoint.neighbors[k] == stopWaypoints[i].pedestrianWaypoints[j])
                        {
                            road = i;
                            break;
                        }
                    }
                }
            }
            return road;
        }
#endif


        public override void DestroyWindow()
        {
            EditorUtility.SetDirty(selectedIntersection);
#if GLEY_PEDESTRIAN_SYSTEM
            pedestrianWaypointDrawer.onWaypointClicked -= PedestrianWaypointClicked;
#endif
            base.DestroyWindow();
        }
    }
}
