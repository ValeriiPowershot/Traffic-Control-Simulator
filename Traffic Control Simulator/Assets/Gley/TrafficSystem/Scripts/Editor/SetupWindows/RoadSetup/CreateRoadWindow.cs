using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class CreateRoadWindow : TrafficSetupWindow
    {
        private List<Road> roadsOfInterest;
        private TrafficRoadCreator trafficRoadCreator;
        private TrafficRoadData trafficRoadData;
        private TrafficRoadDrawer trafficRoadDrawer;
        private TrafficLaneData trafficLaneData;
        private TrafficLaneDrawer trafficLaneDrawer;
        private Vector3 firstClick;
        private Vector3 secondClick;
        private int nrOfRoads;

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            trafficRoadData = CreateInstance<TrafficRoadData>().Initialize();
            trafficLaneData = CreateInstance<TrafficLaneData>().Initialize(trafficRoadData);
            
            trafficRoadCreator = CreateInstance<TrafficRoadCreator>().Initialize(trafficRoadData);
                   
            trafficRoadDrawer = CreateInstance<TrafficRoadDrawer>().Initialize(trafficRoadData);            
            trafficLaneDrawer = CreateInstance<TrafficLaneDrawer>().Initialize(trafficLaneData);

            base.Initialize(windowProperties, window);
            return this;
        }


        public override void DrawInScene()
        {
            if (firstClick != Vector3.zero)
            {
                Handles.SphereHandleCap(0, firstClick, Quaternion.identity, 1, EventType.Repaint);
            }

            if (editorSave.viewOtherRoads)
            {
                roadsOfInterest= trafficRoadDrawer.ShowAllRoads(MoveTools.None, editorSave.editorColors.roadColor, editorSave.editorColors.anchorPointColor, editorSave.editorColors.controlPointColor, editorSave.editorColors.labelColor, editorSave.viewLabels);

                if (roadsOfInterest.Count != nrOfRoads)
                {
                    nrOfRoads = roadsOfInterest.Count;
                    SettingsWindowBase.TriggerRefreshWindowEvent();
                }
                if(editorSave.viewRoadLanes)
                {
                    for (int i = 0; i < nrOfRoads; i++)
                    {
                        trafficLaneDrawer.DrawAllLanes(roadsOfInterest[i], editorSave.viewRoadWaypoints, editorSave.viewRoadLaneChanges, editorSave.viewLabels, editorSave.editorColors.laneColor, editorSave.editorColors.waypointColor, editorSave.editorColors.disconnectedColor, editorSave.editorColors.laneChangeColor, editorSave.editorColors.labelColor);
                    }
                }
            }
            base.DrawInScene();
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("Press SHIFT + Left Click to add a road point");
            EditorGUILayout.LabelField("Press SHIFT + Right Click to remove a road point");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("If you are not able to draw, make sure your ground/road is on the layer marked as Road inside Layer Setup");
            EditorGUILayout.Space();
            editorSave.leftSideTraffic = EditorGUILayout.Toggle("LeftSideTraffic", editorSave.leftSideTraffic);
        }


        protected override void ScrollPart(float width, float height)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            editorSave.viewOtherRoads = EditorGUILayout.Toggle("View Other Roads", editorSave.viewOtherRoads, GUILayout.Width(TOGGLE_WIDTH));
            editorSave.editorColors.roadColor = EditorGUILayout.ColorField(editorSave.editorColors.roadColor);
            EditorGUILayout.EndHorizontal();

            if (editorSave.viewOtherRoads)
            {
                EditorGUILayout.BeginHorizontal();
                editorSave.viewLabels = EditorGUILayout.Toggle("View Labels", editorSave.viewLabels, GUILayout.Width(TOGGLE_WIDTH));
                editorSave.editorColors.labelColor = EditorGUILayout.ColorField(editorSave.editorColors.labelColor);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                editorSave.viewRoadLanes = EditorGUILayout.Toggle("View Lanes", editorSave.viewRoadLanes, GUILayout.Width(TOGGLE_WIDTH));
                editorSave.editorColors.laneColor = EditorGUILayout.ColorField(editorSave.editorColors.laneColor);
                EditorGUILayout.EndHorizontal();

                if (editorSave.viewRoadLanes)
                {
                    EditorGUILayout.BeginHorizontal();
                    editorSave.viewRoadWaypoints = EditorGUILayout.Toggle("View Waypoints", editorSave.viewRoadWaypoints, GUILayout.Width(TOGGLE_WIDTH));
                    editorSave.editorColors.waypointColor = EditorGUILayout.ColorField(editorSave.editorColors.waypointColor);
                    editorSave.editorColors.disconnectedColor = EditorGUILayout.ColorField(editorSave.editorColors.disconnectedColor);
                    if (editorSave.viewRoadWaypoints == false)
                    {
                        editorSave.viewRoadLaneChanges = false;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    editorSave.viewRoadLaneChanges = EditorGUILayout.Toggle("View Lane Changes", editorSave.viewRoadLaneChanges, GUILayout.Width(TOGGLE_WIDTH));
                    if (editorSave.viewRoadLaneChanges == true)
                    {
                        editorSave.viewRoadWaypoints = true;
                    }
                    editorSave.editorColors.laneChangeColor = EditorGUILayout.ColorField(editorSave.editorColors.laneChangeColor);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
            base.ScrollPart(width, height);
        }

        public override void LeftClick(Vector3 mousePosition, bool clicked)
        {
            if (firstClick == Vector3.zero)
            {
                firstClick = mousePosition;
            }
            else
            {
                secondClick = mousePosition;
                CreateRoad();
            }
            base.LeftClick(mousePosition, clicked);
        }

        void CreateRoad()
        {
            Road selectedRoad = trafficRoadCreator.Create(
                Constants.trafficWaypointsHolderName, 
                editorSave.nrOfLanes, 
                editorSave.laneWidth, 
                editorSave.waypointDistance, 
                Constants.roadName, 
                firstClick, 
                secondClick,
                editorSave.maxSpeed, 
                System.Enum.GetValues(typeof(VehicleTypes)).Length, 
                editorSave.leftSideTraffic,
                editorSave.otherLaneLinkDistance);
            SettingsWindow.SetSelectedRoad(selectedRoad);
            window.SetActiveWindow(typeof(EditRoadWindow), false);
            firstClick = Vector3.zero;
            secondClick = Vector3.zero;
        }

        public override void UndoAction()
        {
            base.UndoAction();
            if (secondClick == Vector3.zero)
            {
                if (firstClick != Vector3.zero)
                {
                    firstClick = Vector3.zero;
                }
            }
        }

        public override void DestroyWindow()
        {
            DestroyImmediate(trafficRoadData);
            DestroyImmediate(trafficLaneData);
     
            DestroyImmediate(trafficRoadCreator);

            DestroyImmediate(trafficRoadDrawer);
            DestroyImmediate(trafficLaneDrawer);
            base.DestroyWindow();
        }
    }
}