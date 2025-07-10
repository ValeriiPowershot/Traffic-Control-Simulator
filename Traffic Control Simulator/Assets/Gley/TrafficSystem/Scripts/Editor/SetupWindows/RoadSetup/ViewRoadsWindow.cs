using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class ViewRoadsWindow : TrafficSetupWindow
    {
        private const float nothingSelectedValue = 199;
        private const float viewLanesValue = 219;
        private const float viewWaypointsValue = 239;

        private List<Road> roadsOfInterest;

        private TrafficRoadCreator trafficRoadCreator;
        private TrafficRoadData trafficRoadData;
        private TrafficRoadDrawer trafficRoadDrawer;
        private TrafficLaneData trafficLaneData;
        private TrafficLaneDrawer trafficLaneDrawer;
        private TrafficConnectionCreator trafficConnectionCreator;
        private TrafficConnectionData trafficConnectionData;
        private TrafficWaypointCreator trafficWaypointCreator;

        private string drawButton = "Draw All Roads";
        private float scrollAdjustment;
        private bool drawAllRoads;
        private int nrOfRoads;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);

            trafficRoadData = CreateInstance<TrafficRoadData>().Initialize();
            trafficLaneData = CreateInstance<TrafficLaneData>().Initialize(trafficRoadData);
            trafficConnectionData = CreateInstance<TrafficConnectionData>().Initialize(trafficRoadData);

            trafficWaypointCreator = CreateInstance<TrafficWaypointCreator>().Initialize();
            trafficRoadCreator = CreateInstance<TrafficRoadCreator>().Initialize(trafficRoadData);
            trafficConnectionCreator = CreateInstance<TrafficConnectionCreator>().Initialize(trafficConnectionData, trafficWaypointCreator);
           
            trafficRoadDrawer = CreateInstance<TrafficRoadDrawer>().Initialize(trafficRoadData);       
            trafficLaneDrawer = CreateInstance<TrafficLaneDrawer>().Initialize(trafficLaneData);

            return this;
        }


        public override void DrawInScene()
        {
            base.DrawInScene();
            roadsOfInterest = trafficRoadDrawer.ShowAllRoads(MoveTools.None, editorSave.editorColors.roadColor, editorSave.editorColors.anchorPointColor, editorSave.editorColors.controlPointColor, editorSave.editorColors.labelColor, editorSave.viewLabels);

            if (roadsOfInterest.Count != nrOfRoads)
            {
                nrOfRoads = roadsOfInterest.Count;
                SettingsWindowBase.TriggerRefreshWindowEvent();
            }
            if (editorSave.viewRoadLanes)
            {
                for (int i = 0; i < nrOfRoads; i++)
                {
                    trafficLaneDrawer.DrawAllLanes(roadsOfInterest[i], editorSave.viewRoadWaypoints, editorSave.viewRoadLaneChanges, editorSave.viewLabels, editorSave.editorColors.laneColor, editorSave.editorColors.waypointColor, editorSave.editorColors.disconnectedColor, editorSave.editorColors.laneChangeColor, editorSave.editorColors.labelColor);
                }
            }
        }


        protected override void TopPart()
        {
            base.TopPart();

            if (GUILayout.Button(drawButton))
            {
                drawAllRoads = !drawAllRoads;
                if (drawAllRoads == true)
                {
                    drawButton = "Clear All";
                }
                else
                {
                    drawButton = "Draw All Roads";
                }

                trafficRoadDrawer.SetDrawProperty(drawAllRoads);
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();
            editorSave.editorColors.roadColor = EditorGUILayout.ColorField("Road Color", editorSave.editorColors.roadColor);

            if (editorSave.viewLabels)
            {
                EditorGUILayout.BeginHorizontal();
                editorSave.viewLabels = EditorGUILayout.Toggle("View Labels", editorSave.viewLabels, GUILayout.Width(TOGGLE_WIDTH));
                editorSave.editorColors.labelColor = EditorGUILayout.ColorField(editorSave.editorColors.labelColor);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                editorSave.viewLabels = EditorGUILayout.Toggle("View Labels", editorSave.viewLabels);
            }

            if (editorSave.viewRoadLanes)
            {
                scrollAdjustment = viewLanesValue;
                EditorGUILayout.BeginHorizontal();
                editorSave.viewRoadLanes = EditorGUILayout.Toggle("View Lanes", editorSave.viewRoadLanes, GUILayout.Width(TOGGLE_WIDTH));
                editorSave.editorColors.laneColor = EditorGUILayout.ColorField(editorSave.editorColors.laneColor);
                EditorGUILayout.EndHorizontal();

                if (editorSave.viewRoadWaypoints)
                {
                    scrollAdjustment = viewWaypointsValue;
                    EditorGUILayout.BeginHorizontal();
                    editorSave.viewRoadWaypoints = EditorGUILayout.Toggle("View Waypoints", editorSave.viewRoadWaypoints, GUILayout.Width(TOGGLE_WIDTH));
                    editorSave.editorColors.waypointColor = EditorGUILayout.ColorField(editorSave.editorColors.waypointColor);
                    EditorGUILayout.EndHorizontal();

                    if (editorSave.viewRoadLaneChanges)
                    {
                        EditorGUILayout.BeginHorizontal();
                        editorSave.viewRoadLaneChanges = EditorGUILayout.Toggle("View Lane Changes", editorSave.viewRoadLaneChanges, GUILayout.Width(TOGGLE_WIDTH));
                        editorSave.editorColors.laneChangeColor = EditorGUILayout.ColorField(editorSave.editorColors.laneChangeColor);
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        editorSave.viewRoadLaneChanges = EditorGUILayout.Toggle("View Lane Changes", editorSave.viewRoadLaneChanges);
                    }
                }
                else
                {
                    editorSave.viewRoadWaypoints = EditorGUILayout.Toggle("View Waypoints", editorSave.viewRoadWaypoints);
                }
            }
            else
            {
                scrollAdjustment = nothingSelectedValue;
                editorSave.viewRoadLanes = EditorGUILayout.Toggle("View Lanes", editorSave.viewRoadLanes);
            }

            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
            EditorGUILayout.Space();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            if (roadsOfInterest != null)
            {
                if (roadsOfInterest.Count == 0)
                {
                    EditorGUILayout.LabelField("Nothing in view");
                }
                for (int i = 0; i < roadsOfInterest.Count; i++)
                {
                    DisplayRoad(roadsOfInterest[i]);
                }
            }
            GUILayout.EndScrollView();
        }


        private void DisplayRoad(Road road)
        {
            if (road == null)
                return;
            EditorGUILayout.BeginHorizontal();
            road.draw = EditorGUILayout.Toggle(road.draw, GUILayout.Width(TOGGLE_DIMENSION));
            GUILayout.Label(road.gameObject.name);

            if (GUILayout.Button("View"))
            {
                GleyUtilities.TeleportSceneCamera(road.transform.position);
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("Select"))
            {
                SelectWaypoint(road);
            }
            if (GUILayout.Button("Delete"))
            {
                EditorGUI.BeginChangeCheck();
                if (EditorUtility.DisplayDialog("Delete " + road.name + "?", "Are you sure you want to delete " + road.name + "? \nYou cannot undo this operation.", "Delete", "Cancel"))
                {
                    DeleteCurrentRoad(road);
                }
                EditorGUI.EndChangeCheck();
            }

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndHorizontal();
        }


        private void SelectWaypoint(Road road)
        {
            SettingsWindow.SetSelectedRoad(road);
            window.SetActiveWindow(typeof(EditRoadWindow), true);
        }


        private void DeleteCurrentRoad(Road road)
        {
            trafficConnectionCreator.DeleteConnectionsWithThisRoad(road);
            trafficRoadCreator.DeleteCurrentRoad(road);
            Undo.undoRedoPerformed += UndoPerformed;
        }


        protected void UndoPerformed()
        {
            Undo.undoRedoPerformed -= UndoPerformed;
        }


        public override void DestroyWindow()
        {
            Undo.undoRedoPerformed -= UndoPerformed;
            base.DestroyWindow();

            DestroyImmediate(trafficRoadData);
            DestroyImmediate(trafficLaneData);
            DestroyImmediate(trafficConnectionData);

            DestroyImmediate(trafficWaypointCreator);
            DestroyImmediate(trafficConnectionCreator);
            DestroyImmediate(trafficRoadCreator);

            DestroyImmediate(trafficRoadDrawer);
            DestroyImmediate(trafficLaneDrawer);
        }
    }
}
