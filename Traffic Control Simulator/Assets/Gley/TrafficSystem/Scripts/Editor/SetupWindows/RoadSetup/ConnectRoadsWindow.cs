using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class ConnectRoadsWindow : TrafficSetupWindow
    {
        private const float maxValue = 384;
        private const float minValue = 264;

        private List<ConnectionCurve> connectionsOfInterest;
        private List<Road> roadsOfInterest;

        private TrafficWaypointCreator waypointCreator;

        private TrafficRoadData roadData;
        private TrafficRoadDrawer roadDrawer;

        private TrafficLaneData laneData;
        private TrafficLaneDrawer laneDrawer;

        private TrafficConnectionCreator connectionCreator;
        private TrafficConnectionData connectionData;
        private TrafficConnectionDrawer connectionDrawer;

        private Road clickedRoad;
        private bool[] allowedCarIndex;
        private float scrollAdjustment;
        private int nrOfRoads;
        private int nrOfCars;
        private int nrOfConnections;
        private int clickedLane;
        private bool drawAllConnections;
        private bool showCustomizations;
        private bool drawOutConnectors; 


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);

            roadData = CreateInstance<TrafficRoadData>().Initialize();
            laneData = CreateInstance<TrafficLaneData>().Initialize(roadData);
            connectionData = CreateInstance<TrafficConnectionData>().Initialize(roadData);

            waypointCreator = CreateInstance<TrafficWaypointCreator>().Initialize();
            connectionCreator = CreateInstance<TrafficConnectionCreator>().Initialize(connectionData, waypointCreator);

            roadDrawer = CreateInstance<TrafficRoadDrawer>().Initialize(roadData);
            laneDrawer = CreateInstance<TrafficLaneDrawer>().Initialize(laneData);
            connectionDrawer = CreateInstance<TrafficConnectionDrawer>().Initialize(connectionData);

            nrOfCars = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            allowedCarIndex = new bool[nrOfCars];
            for (int i = 0; i < allowedCarIndex.Length; i++)
            {
                allowedCarIndex[i] = true;
            }

            connectionDrawer.onWaypointClicked += WaypointClicked;
            drawOutConnectors = true;
            return this;
        }

        public override void DrawInScene()
        {
            if(roadData.HasErrors())
            {
                return;
            }

            roadsOfInterest = roadDrawer.ShowAllRoads(MoveTools.None, editorSave.editorColors.roadColor, editorSave.editorColors.anchorPointColor, editorSave.editorColors.controlPointColor, editorSave.editorColors.labelColor, editorSave.viewLabels);

            if (roadsOfInterest.Count != nrOfRoads)
            {
                nrOfRoads = roadsOfInterest.Count;
                SettingsWindowBase.TriggerRefreshWindowEvent();
            }

            for (int i = 0; i < nrOfRoads; i++)
            {
                laneDrawer.DrawAllLanes(roadsOfInterest[i], editorSave.viewRoadWaypoints, editorSave.viewRoadLaneChanges, editorSave.viewLabels, editorSave.editorColors.laneColor, editorSave.editorColors.waypointColor, editorSave.editorColors.disconnectedColor, editorSave.editorColors.laneChangeColor, editorSave.editorColors.labelColor);
            }

            connectionsOfInterest = connectionDrawer.ShowAllConnections(roadsOfInterest, editorSave.viewLabels, editorSave.editorColors.connectorLaneColor, editorSave.editorColors.anchorPointColor,
               editorSave.editorColors.roadConnectorColor, editorSave.editorColors.selectedRoadConnectorColor, editorSave.editorColors.disconnectedColor, editorSave.waypointDistance, editorSave.editorColors.labelColor, editorSave.editorColors.waypointColor,
               drawOutConnectors, clickedRoad, clickedLane);

            if (connectionsOfInterest.Count != nrOfConnections)
            {
                nrOfConnections = connectionsOfInterest.Count;
                SettingsWindowBase.TriggerRefreshWindowEvent();
            }

            base.DrawInScene();
        }


        protected override void TopPart()
        {
            base.TopPart();
            string drawButton = "Draw All Connections";
            if (drawAllConnections == true)
            {
                drawButton = "Clear All";
            }

            if (GUILayout.Button(drawButton))
            {
                DrawButton();
            }

            EditorGUI.BeginChangeCheck();
            if (showCustomizations == false)
            {
                scrollAdjustment = minValue;
                showCustomizations = EditorGUILayout.Toggle("Change Colors ", showCustomizations);
                editorSave.viewLabels = EditorGUILayout.Toggle("View Labels", editorSave.viewLabels);
                editorSave.viewRoadWaypoints = EditorGUILayout.Toggle("View Waypoints", editorSave.viewRoadWaypoints);
                editorSave.viewRoadLaneChanges = EditorGUILayout.Toggle("View Lane Changes", editorSave.viewRoadLaneChanges);
            }
            else
            {
                scrollAdjustment = maxValue;
                showCustomizations = EditorGUILayout.Toggle("Change Colors ", showCustomizations);
                EditorGUILayout.BeginHorizontal();
                editorSave.viewLabels = EditorGUILayout.Toggle("View Labels", editorSave.viewLabels, GUILayout.Width(TOGGLE_WIDTH));
                editorSave.editorColors.labelColor = EditorGUILayout.ColorField(editorSave.editorColors.labelColor);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                editorSave.viewRoadWaypoints = EditorGUILayout.Toggle("View Waypoints", editorSave.viewRoadWaypoints, GUILayout.Width(TOGGLE_WIDTH));
                editorSave.editorColors.waypointColor = EditorGUILayout.ColorField(editorSave.editorColors.waypointColor);
                editorSave.editorColors.disconnectedColor = EditorGUILayout.ColorField(editorSave.editorColors.disconnectedColor);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                editorSave.viewRoadLaneChanges = EditorGUILayout.Toggle("View Lane Changes", editorSave.viewRoadLaneChanges, GUILayout.Width(TOGGLE_WIDTH));
                editorSave.editorColors.laneChangeColor = EditorGUILayout.ColorField(editorSave.editorColors.laneChangeColor);
                EditorGUILayout.EndHorizontal();


                editorSave.editorColors.roadColor = EditorGUILayout.ColorField("Road Color", editorSave.editorColors.roadColor);
                editorSave.editorColors.laneColor = EditorGUILayout.ColorField("Lane Color", editorSave.editorColors.laneColor);
                editorSave.editorColors.connectorLaneColor = EditorGUILayout.ColorField("Connector Lane Color", editorSave.editorColors.connectorLaneColor);
                editorSave.editorColors.anchorPointColor = EditorGUILayout.ColorField("Anchor Point Color", editorSave.editorColors.anchorPointColor);
                editorSave.editorColors.roadConnectorColor = EditorGUILayout.ColorField("Road Connector Color", editorSave.editorColors.roadConnectorColor);
                editorSave.editorColors.selectedRoadConnectorColor = EditorGUILayout.ColorField("Selected Connector Color", editorSave.editorColors.selectedRoadConnectorColor);
            }
            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }


        protected override void ScrollPart(float width, float height)
        {
            base.ScrollPart(width, height);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            if (connectionsOfInterest != null)
            {
                for (int i = 0; i < connectionsOfInterest.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    connectionsOfInterest[i].draw = EditorGUILayout.Toggle(connectionsOfInterest[i].draw, GUILayout.Width(TOGGLE_DIMENSION));
                    EditorGUILayout.LabelField(connectionsOfInterest[i].GetName());
                    Color oldColor = GUI.backgroundColor;
                    if (connectionsOfInterest[i].drawWaypoints == true)
                    {
                        GUI.backgroundColor = Color.green;
                    }

                    if (GUILayout.Button("Waypoints", GUILayout.Width(BUTTON_DIMENSION)))
                    {
                        connectionsOfInterest[i].drawWaypoints = !connectionsOfInterest[i].drawWaypoints;
                    }
                    GUI.backgroundColor = oldColor;

                    if (GUILayout.Button("View", GUILayout.Width(BUTTON_DIMENSION)))
                    {
                        View(i);
                    }

                    if (GUILayout.Button("Delete", GUILayout.Width(BUTTON_DIMENSION)))
                    {
                        if (EditorUtility.DisplayDialog("Delete " + connectionsOfInterest[i].name + "?", "Are you sure you want to delete " + connectionsOfInterest[i].name + "? \nYou cannot undo this operation.", "Delete", "Cancel"))
                        {
                            connectionCreator.DeleteConnection(connectionsOfInterest[i]);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();
            GUILayout.EndScrollView();
            EditorGUILayout.Space();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();
        }


        protected override void BottomPart()
        {
            editorSave.waypointDistance = EditorGUILayout.FloatField("Waypoint distance ", editorSave.waypointDistance);
            if (editorSave.waypointDistance <= 0)
            {
                Debug.LogWarning("Waypoint distance needs to be >0. will be set to 1 by default");
                editorSave.waypointDistance = 1;
            }

            if (GUILayout.Button("Generate Selected Connections"))
            {
                GenerateSelectedConnections();
            }
            base.BottomPart();
        }


        private void WaypointClicked(Road road, int lane)
        {
            if (drawOutConnectors == true)
            {
                drawOutConnectors = false;
                clickedRoad = road;
                clickedLane = lane;
            }
            else
            {
                drawOutConnectors = true;
                if (road != null)
                {
                    connectionCreator.CreateConnection(road.transform.parent.GetComponent<ConnectionPool>(), clickedRoad, clickedLane, road, lane, editorSave.waypointDistance);
                }
                clickedRoad = null;
                clickedLane = -1;

            }


        }


        private void DrawButton()
        {
            drawAllConnections = !drawAllConnections;
            for (int i = 0; i < connectionsOfInterest.Count; i++)
            {
                connectionsOfInterest[i].draw = drawAllConnections;
                if (drawAllConnections == false)
                {
                    connectionsOfInterest[i].drawWaypoints = false;
                }
            }
        }


        void GenerateSelectedConnections()
        {
            connectionCreator.GenerateConnections(connectionsOfInterest, editorSave.waypointDistance);
            SceneView.RepaintAll();
        }


        private void View(int curveIndex)
        {
            GleyUtilities.TeleportSceneCamera(connectionsOfInterest[curveIndex].GetOutConnector().gameObject.transform.position);
        }


        public override void DestroyWindow()
        {
            connectionDrawer.onWaypointClicked -= WaypointClicked;

            DestroyImmediate(roadData);
            DestroyImmediate(laneData);
            DestroyImmediate(connectionData);

            DestroyImmediate(waypointCreator);
            DestroyImmediate(connectionCreator);

            DestroyImmediate(roadDrawer);
            DestroyImmediate(laneDrawer);
            DestroyImmediate(connectionDrawer);

            base.DestroyWindow();
        }
    }
}
