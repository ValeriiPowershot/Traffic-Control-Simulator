#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Editor;
using Gley.PedestrianSystem.Internal;
#endif
using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class IntersectionWindow : TrafficSetupWindow
    {
        protected enum WindowActions
        {
            None,
            AssignRoadWaypoints,
            AssignPedestrianWaypoints,
            AddDirectionWaypoints,
            AddExitWaypoints
        }

#if GLEY_PEDESTRIAN_SYSTEM
        protected List<PedestrianWaypointSettings> pedestrianWaypoints;
        protected List<PedestrianWaypointSettings> directionWaypoints;
        protected PedestrianWaypointData pedestrianWaypointData;
        protected PedestrianWaypointDrawer pedestrianWaypointDrawer;
#endif
        protected List<IntersectionStopWaypointsSettings> stopWaypoints;
        protected List<WaypointSettings> exitWaypoints;
        protected GenericIntersectionSettings selectedIntersection;
        protected WindowActions currentAction;
        protected int selectedRoad;
        protected bool hideWaypoints;

        private TrafficWaypointData trafficWaypointData;
        private TrafficWaypointDrawer trafficWaypointDrawer;
        private IntersectionData intersectionData;
        private IntersectionDrawer intersectionDrawer;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            selectedIntersection = SettingsWindow.GetSelectedIntersection();
            stopWaypoints = selectedIntersection.GetAssignedWaypoints();
            exitWaypoints = selectedIntersection.GetExitWaypoints();


            trafficWaypointData = CreateInstance<TrafficWaypointData>().Initialize();
            trafficWaypointDrawer = CreateInstance<TrafficWaypointDrawer>().Initialize(trafficWaypointData);
            intersectionData = CreateInstance<IntersectionData>().Initialize();
            intersectionDrawer = CreateInstance<IntersectionDrawer>().Initialize(intersectionData);
#if GLEY_PEDESTRIAN_SYSTEM
            pedestrianWaypoints = selectedIntersection.GetPedestrianWaypoints();
            directionWaypoints = selectedIntersection.GetDirectionWaypoints();
            pedestrianWaypointData = CreateInstance<PedestrianWaypointData>().Initialize();
            pedestrianWaypointDrawer = CreateInstance<PedestrianWaypointDrawer>().Initialize(pedestrianWaypointData);
#endif

            selectedRoad = -1;
            name = selectedIntersection.name;
            currentAction = WindowActions.None;

            trafficWaypointDrawer.onWaypointClicked += TrafficWaypointClicked;
            return this;
        }

        public override void DrawInScene()
        {
            base.DrawInScene();
            switch (currentAction)
            {
                case WindowActions.None:
                    intersectionDrawer.DrawExitWaypoints(selectedIntersection, editorSave.editorColors.exitWaypointsColor);
                    intersectionDrawer.DrawStopWaypoints(selectedIntersection, int.MaxValue, editorSave.editorColors.stopWaypointsColor, editorSave.editorColors.labelColor);
#if GLEY_PEDESTRIAN_SYSTEM
                    intersectionDrawer.DrawPedestrianWaypoints(selectedIntersection, int.MaxValue, editorSave.editorColors.stopWaypointsColor);
                    intersectionDrawer.DrawDirectionWaypoints(selectedIntersection, editorSave.editorColors.exitWaypointsColor);
#endif
                    break;

                case WindowActions.AssignRoadWaypoints:
                    trafficWaypointDrawer.ShowIntersectionWaypoints(editorSave.editorColors.waypointColor);
                    intersectionDrawer.DrawStopWaypoints(selectedIntersection, selectedRoad, editorSave.editorColors.stopWaypointsColor, editorSave.editorColors.labelColor);
                    break;

                case WindowActions.AddExitWaypoints:
                    trafficWaypointDrawer.ShowIntersectionWaypoints(editorSave.editorColors.waypointColor);
                    intersectionDrawer.DrawExitWaypoints(selectedIntersection, editorSave.editorColors.exitWaypointsColor);
                    intersectionDrawer.DrawStopWaypoints(selectedIntersection, int.MaxValue, editorSave.editorColors.stopWaypointsColor, editorSave.editorColors.labelColor);
                    break;

#if GLEY_PEDESTRIAN_SYSTEM
                case WindowActions.AssignPedestrianWaypoints:
                    pedestrianWaypointDrawer.ShowIntersectionWaypoints(editorSave.editorColors.waypointColor);
                    intersectionDrawer.DrawPedestrianWaypoints(selectedIntersection, selectedRoad, editorSave.editorColors.stopWaypointsColor);
                    if (selectedRoad != -1)
                    {
                        intersectionDrawer.DrawStopWaypoints(selectedIntersection, selectedRoad, editorSave.editorColors.stopWaypointsColor, editorSave.editorColors.labelColor);
                    }
                    break;

                case WindowActions.AddDirectionWaypoints:
                    pedestrianWaypointDrawer.DrawPossibleDirectionWaypoints(selectedIntersection.GetPedestrianWaypoints(), editorSave.editorColors.waypointColor);
                    intersectionDrawer.DrawPedestrianWaypoints(selectedIntersection, int.MaxValue, editorSave.editorColors.stopWaypointsColor);
                    intersectionDrawer.DrawDirectionWaypoints(selectedIntersection, editorSave.editorColors.exitWaypointsColor);
                    break;
#endif
            }
        }


        private void TrafficWaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            switch (currentAction)
            {
                case WindowActions.AssignRoadWaypoints:
                    AddWaypointToList(clickedWaypoint, stopWaypoints[selectedRoad].roadWaypoints);
                    break;
                case WindowActions.AddExitWaypoints:
                    AddWaypointToList(clickedWaypoint, exitWaypoints);
                    break;
            }
            SceneView.RepaintAll();
            SettingsWindowBase.TriggerRefreshWindowEvent();
        }


        protected void DisplayList<T>(List<T> list, ref bool globalDraw) where T : WaypointSettingsBase
        {
            Color oldColor;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    continue;
                }
                EditorGUILayout.BeginHorizontal();

                list[i] = (T)EditorGUILayout.ObjectField(list[i], typeof(T), true);

                oldColor = GUI.backgroundColor;
                if (list[i].draw == true)
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View"))
                {
                    ViewWaypoint(list[i], ref globalDraw);
                }
                GUI.backgroundColor = oldColor;

                if (GUILayout.Button("Delete"))
                {
                    list.RemoveAt(i);
                    SceneView.RepaintAll();
                }
                EditorGUILayout.EndHorizontal();
            }
        }


        private void ViewWaypoint(WaypointSettingsBase waypoint, ref bool globalDraw)
        {
            waypoint.draw = !waypoint.draw;
            if (waypoint.draw == false)
            {
                globalDraw = false;
            }
            SceneView.RepaintAll();
        }


        protected void ViewAllDirectionWaypoints()
        {
#if GLEY_PEDESTRIAN_SYSTEM
            editorSave.showDirectionWaypoints = !editorSave.showDirectionWaypoints;
            for (int i = 0; i < directionWaypoints.Count; i++)
            {
                directionWaypoints[i].draw = editorSave.showDirectionWaypoints;
            }
#endif
        }


        protected void ViewAllExitWaypoints()
        {
            editorSave.showExitWaypoints = !editorSave.showExitWaypoints;
            for (int i = 0; i < exitWaypoints.Count; i++)
            {
                exitWaypoints[i].draw = editorSave.showExitWaypoints;
            }
        }


        protected void AddLightObjects(string title, List<GameObject> objectsList)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(title + ":");
            for (int i = 0; i < objectsList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                objectsList[i] = (GameObject)EditorGUILayout.ObjectField(objectsList[i], typeof(GameObject), true);

                if (GUILayout.Button("Delete"))
                {
                    objectsList.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add " + title + " Objects"))
            {
                objectsList.Add(null);
            }
            EditorGUILayout.EndVertical();
        }


        protected void AddWaypointToList<T>(T waypoint, List<T> listToAdd) where T : WaypointSettingsBase
        {
            if (!listToAdd.Contains(waypoint))
            {
                waypoint.draw = true;
                listToAdd.Add(waypoint);
            }
            else
            {
                listToAdd.Remove(waypoint);
            }
        }


        protected void SaveSettings()
        {
            selectedIntersection.gameObject.name = name;
            if (stopWaypoints.Count > 0)
            {
                Vector3 position = new Vector3();
                int nr = 0;
                for (int i = 0; i < stopWaypoints.Count; i++)
                {
                    for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
                    {
                        position += stopWaypoints[i].roadWaypoints[j].transform.position;
                        nr++;
                    }
                }
                selectedIntersection.transform.position = position / nr;
            }
        }


        protected void Cancel()
        {
            selectedRoad = -1;
            currentAction = WindowActions.None;
            SceneView.RepaintAll();
        }


        public override void DestroyWindow()
        {
            trafficWaypointDrawer.onWaypointClicked -= TrafficWaypointClicked;

            DestroyImmediate(trafficWaypointData);
            DestroyImmediate(trafficWaypointDrawer);
            DestroyImmediate(intersectionData);
            DestroyImmediate(intersectionDrawer);
#if GLEY_PEDESTRIAN_SYSTEM
            DestroyImmediate(pedestrianWaypointData);
            DestroyImmediate(pedestrianWaypointDrawer);
#endif
            base.DestroyWindow();
        }
    }
}
