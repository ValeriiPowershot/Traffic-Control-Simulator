#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Internal;
#endif
using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficLightsCrossingWindow : IntersectionWindow
    {
        private readonly float scrollAdjustment = 231;

        private List<GameObject> pedestrianRedLightObjects = new List<GameObject>();
        private List<GameObject> pedestrianGreenLightObjects = new List<GameObject>();
        private TrafficLightsCrossingSettings selectedTrafficLightsCrossing;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            selectedTrafficLightsCrossing = selectedIntersection as TrafficLightsCrossingSettings;
#if GLEY_PEDESTRIAN_SYSTEM
            pedestrianRedLightObjects = selectedTrafficLightsCrossing.pedestrianRedLightObjects;
            pedestrianGreenLightObjects = selectedTrafficLightsCrossing.pedestrianGreenLightObjects;
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

            selectedTrafficLightsCrossing.greenLightTime = EditorGUILayout.FloatField("Green Light Time", selectedTrafficLightsCrossing.greenLightTime);
            selectedTrafficLightsCrossing.yellowLightTime = EditorGUILayout.FloatField("Yellow Light Time", selectedTrafficLightsCrossing.yellowLightTime);
            selectedTrafficLightsCrossing.redLightTime = EditorGUILayout.FloatField("Red Light Time", selectedTrafficLightsCrossing.redLightTime);
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
                    AddWaypointToList(clickedWaypoint, directionWaypoints);
                    break;
                case WindowActions.AssignPedestrianWaypoints:
                    AddWaypointToList(clickedWaypoint, pedestrianWaypoints);
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
            ViewPedestrianWaypoints();
            EditorGUILayout.Space();

            ViewDirectionWaypoints();
            EditorGUILayout.Space();
#endif
        }


        #region StopWaypoints
        private void ViewAssignedStopWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Stop waypoints:", "The vehicle will stop at this point until the intersection allows it to continue. " +
               "\nEach road that enters in intersection should have its own set of stop waypoints"));
            Color oldColor;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                EditorGUILayout.Space();
                DisplayList(stopWaypoints[i].roadWaypoints, ref stopWaypoints[i].draw);
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();


                if (GUILayout.Button("Assign Road"))
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

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space();
        }


        private void AddTrafficWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Stop waypoints:", "The vehicle will stop at this point until the intersection allows it to continue. " +
               "\nEach road that enters in intersection should have its own set of stop waypoints"));
            Color oldColor;


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Road " + (selectedRoad + 1));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            DisplayList(stopWaypoints[selectedRoad].roadWaypoints, ref stopWaypoints[selectedRoad].draw);
            EditorGUILayout.Space();

            oldColor = GUI.backgroundColor;
            if (stopWaypoints[selectedRoad].draw == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Road Waypoints"))
            {
                ViewRoadWaypoints(selectedRoad);

            }
            GUI.backgroundColor = oldColor;

            EditorGUILayout.Space();
            AddLightObjects("Red Light", stopWaypoints[selectedRoad].redLightObjects);
            AddLightObjects("Yellow Light", stopWaypoints[selectedRoad].yellowLightObjects);
            AddLightObjects("Green Light", stopWaypoints[selectedRoad].greenLightObjects);
            EditorGUILayout.Space();
            if (GUILayout.Button("Done"))
            {
                Cancel();
            }

            EditorGUILayout.EndVertical();
        }


        private void ViewRoadWaypoints(int i)
        {
            stopWaypoints[i].draw = !stopWaypoints[i].draw;
            for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
            {
                stopWaypoints[i].roadWaypoints[j].draw = stopWaypoints[i].draw;
            }
        }
        #endregion

#if GLEY_PEDESTRIAN_SYSTEM
        #region PedestrianWaypoints
        private void ViewPedestrianWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Pedestrian waypoints:", "Pedestrian waypoints are used for waiting before crossing the road. " +
                "Pedestrians will stop on those waypoints and wait for green color."));


            EditorGUILayout.EndHorizontal(); EditorGUILayout.Space();
            DisplayList(pedestrianWaypoints, ref editorSave.showPedestrianWaypoints);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Pedestrian Waypoints"))
            {
                selectedRoad = -1;
                currentAction = WindowActions.AssignPedestrianWaypoints;
            }
            Color oldColor = GUI.backgroundColor;
            if (editorSave.showPedestrianWaypoints == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Pedestrian Waypoints"))
            {
                ViewAllPedestrianWaypoints();
            }
            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }


        private void AddPedestrianWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Pedestrian stop waypoints:", "pedestrians will stop at this point until the intersection allows them to continue. " +
            "\nEach crossing in intersection should have its own set of stop waypoints corresponding to its road"));

            DisplayList(pedestrianWaypoints, ref editorSave.showPedestrianWaypoints);

            EditorGUILayout.Space();
            Color oldColor = GUI.backgroundColor;
            if (editorSave.showPedestrianWaypoints == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Pedestrian Waypoints"))
            {
                ViewAllPedestrianWaypoints();
            }
            GUI.backgroundColor = oldColor;

            AddLightObjects("Red Light - Pedestrians", pedestrianRedLightObjects);
            AddLightObjects("Green Light - Pedestrians", pedestrianGreenLightObjects);

            if (GUILayout.Button("Done"))
            {
                Cancel();
            }
            EditorGUILayout.EndVertical();
        }


        private void ViewAllPedestrianWaypoints()
        {
            editorSave.showPedestrianWaypoints = !editorSave.showPedestrianWaypoints;
            for (int i = 0; i < pedestrianWaypoints.Count; i++)
            {
                pedestrianWaypoints[i].draw =editorSave.showPedestrianWaypoints;
            }
        }
        #endregion


        #region DirectionWaypoints
        private void ViewDirectionWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Crossing direction waypoints:", "For each stop waypoint a direction needs to be specified\n" +
                "Only if a pedestrian goes to that direction it will stop, otherwise it will pass through stop waypoint"));
            EditorGUILayout.Space();
            DisplayList(directionWaypoints, ref editorSave.showDirectionWaypoints);

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

            DisplayList(directionWaypoints, ref editorSave.showDirectionWaypoints);

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
