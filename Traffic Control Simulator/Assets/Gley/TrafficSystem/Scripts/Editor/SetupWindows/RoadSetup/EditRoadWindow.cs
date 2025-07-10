using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class EditRoadWindow : TrafficSetupWindow
    {
        private const float maxValue = 480;
        private const float minValue = 340;

        private bool[] allowedCarIndex;

        private TrafficRoadData roadData;
        private TrafficLaneData laneData;
        private TrafficConnectionData connectionData;
        private TrafficRoadDrawer roadDrawer;
        private TrafficLaneDrawer laneDrawer;
        private TrafficWaypointCreator waypointCreator;
        private TrafficLaneCreator laneCreator;
        private TrafficConnectionCreator connectionCreator;
        private Road selectedRoad;
        private MoveTools moveTool;
        private float scrollAdjustment;
        private int nrOfCars;
        private bool showCustomizations;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);

            roadData = CreateInstance<TrafficRoadData>().Initialize();
            laneData = CreateInstance<TrafficLaneData>().Initialize(roadData);
            connectionData = CreateInstance<TrafficConnectionData>().Initialize(roadData);

            roadDrawer = CreateInstance<TrafficRoadDrawer>().Initialize(roadData);
            laneDrawer = CreateInstance<TrafficLaneDrawer>().Initialize(laneData);

            waypointCreator = CreateInstance<TrafficWaypointCreator>().Initialize();
            laneCreator = CreateInstance<TrafficLaneCreator>().Initialize(laneData, waypointCreator);
            connectionCreator = CreateInstance<TrafficConnectionCreator>().Initialize(connectionData, waypointCreator);

            selectedRoad = SettingsWindow.GetSelectedRoad();
            selectedRoad.justCreated = false;
            moveTool = editorSave.moveTool;
            nrOfCars = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            allowedCarIndex = new bool[nrOfCars];
            for (int i = 0; i < allowedCarIndex.Length; i++)
            {
                if (editorSave.globalCarList.Contains((VehicleTypes)i))
                {
                    allowedCarIndex[i] = true;
                }
            }
            return this;
        }


        public override void DrawInScene()
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }

            roadDrawer.DrawPath(selectedRoad, moveTool,editorSave.editorColors.roadColor, editorSave.editorColors.anchorPointColor, editorSave.editorColors.controlPointColor, editorSave.editorColors.labelColor, true);
            laneDrawer.DrawAllLanes(selectedRoad, editorSave.viewRoadWaypoints, editorSave.viewRoadLaneChanges, editorSave.viewLabels, editorSave.editorColors.laneColor, editorSave.editorColors.waypointColor, editorSave.editorColors.disconnectedColor, editorSave.editorColors.laneChangeColor, editorSave.editorColors.labelColor);

            base.DrawInScene();
        }

        protected override void TopPart()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Press SHIFT + Left Click to add a road point");
            EditorGUILayout.LabelField("Press SHIFT + Right Click to remove a road point");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            editorSave.viewRoadWaypoints = EditorGUILayout.Toggle("View Waypoints", editorSave.viewRoadWaypoints);
            editorSave.viewRoadLaneChanges = EditorGUILayout.Toggle("View Lane Changes", editorSave.viewRoadLaneChanges);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            selectedRoad.nrOfLanes = EditorGUILayout.IntField("Nr of lanes", selectedRoad.nrOfLanes);
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                selectedRoad.UpdateLaneNumber(editorSave.maxSpeed, System.Enum.GetValues(typeof(VehicleTypes)).Length);
            }

            selectedRoad.laneWidth = EditorGUILayout.FloatField("Lane width (m)", selectedRoad.laneWidth);
            selectedRoad.waypointDistance = EditorGUILayout.FloatField("Waypoint distance ", selectedRoad.waypointDistance);

            EditorGUI.BeginChangeCheck();
            moveTool = (MoveTools)EditorGUILayout.EnumPopup("Select move tool ", moveTool);
            showCustomizations = EditorGUILayout.Toggle("Change Colors ", showCustomizations);
            if (showCustomizations == true)
            {
                scrollAdjustment = maxValue;
                editorSave.editorColors.roadColor = EditorGUILayout.ColorField("Road Color", editorSave.editorColors.roadColor);
                editorSave.editorColors.laneColor = EditorGUILayout.ColorField("Lane Color", editorSave.editorColors.laneColor);
                editorSave.editorColors.waypointColor = EditorGUILayout.ColorField("Waypoint Color", editorSave.editorColors.waypointColor);
                editorSave.editorColors.disconnectedColor = EditorGUILayout.ColorField("Disconnected Color", editorSave.editorColors.disconnectedColor);
                editorSave.editorColors.laneChangeColor = EditorGUILayout.ColorField("Lane Change Color", editorSave.editorColors.laneChangeColor);
                editorSave.editorColors.controlPointColor = EditorGUILayout.ColorField("Control Point Color", editorSave.editorColors.controlPointColor);
                editorSave.editorColors.anchorPointColor = EditorGUILayout.ColorField("Anchor Point Color", editorSave.editorColors.anchorPointColor);
            }
            else
            {
                scrollAdjustment = minValue;
            }
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            base.TopPart();
        }


        protected override void ScrollPart(float width, float height)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Global Lane Settings", EditorStyles.boldLabel);
            GUILayout.Label("Allowed Vehicle Types:");
            for (int i = 0; i < nrOfCars; i++)
            {
                allowedCarIndex[i] = EditorGUILayout.Toggle(((VehicleTypes)i).ToString(), allowedCarIndex[i]);
            }
            if (GUILayout.Button("Apply Global Vehicle Settings"))
            {
                ApplyGlobalCarSettings();
            }

            EditorGUILayout.BeginHorizontal();
            editorSave.maxSpeed = EditorGUILayout.IntField("Global Max Speed", editorSave.maxSpeed);
            if (GUILayout.Button("Apply Speed"))
            {
                SetSpeedOnLanes(selectedRoad, editorSave.maxSpeed);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Apply All Settings"))
            {
                ApplyGlobalCarSettings();
            }
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Individual Lane Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (selectedRoad.lanes != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                for (int i = 0; i < selectedRoad.lanes.Count; i++)
                {
                    if (selectedRoad.lanes[i].laneDirection == true)
                    {
                        DrawLaneButton(i);
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                for (int i = 0; i < selectedRoad.lanes.Count; i++)
                {
                    if (selectedRoad.lanes[i].laneDirection == false)
                    {
                        DrawLaneButton(i);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.EndScrollView();

            base.ScrollPart(width, height);
        }


        protected override void BottomPart()
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }

            if (GUILayout.Button("Generate waypoints"))
            {
                editorSave.viewRoadWaypoints = true;

                if (selectedRoad.nrOfLanes <= 0)
                {
                    Debug.LogError("Nr of lanes has to be >0");
                    return;
                }

                if (selectedRoad.waypointDistance <= 0)
                {
                    Debug.LogError("Waypoint distance needs to be >0");
                    return;
                }

                if (selectedRoad.laneWidth <= 0)
                {
                    Debug.LogError("Lane width has to be >0");
                    return;
                }
                connectionCreator.DeleteConnectionsWithThisRoad(selectedRoad);
                laneCreator.GenerateWaypoints(selectedRoad, window.GetGroundLayer());

                EditorUtility.SetDirty(selectedRoad);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.BeginHorizontal();
            selectedRoad.otherLaneLinkDistance = EditorGUILayout.IntField("Link distance", (selectedRoad).otherLaneLinkDistance);
            if (selectedRoad.otherLaneLinkDistance < 1)
            {
                selectedRoad.otherLaneLinkDistance = 1;
            }
            if (GUILayout.Button("Link other lanes"))
            {
                editorSave.viewRoadWaypoints = true;
                editorSave.viewRoadLaneChanges = true;
                laneCreator.LinkOtherLanes(selectedRoad);
                EditorUtility.SetDirty(selectedRoad);
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Unlink other lanes"))
            {
                laneCreator.UnLinckOtherLanes((Road)selectedRoad);
                EditorUtility.SetDirty(selectedRoad);
                AssetDatabase.SaveAssets();
            }

            base.BottomPart();
        }


        public override void MouseMove(Vector3 mousePosition)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }
            base.MouseMove(mousePosition);
            roadDrawer.SelectSegmentIndex(selectedRoad, mousePosition);
        }


        public override void LeftClick(Vector3 mousePosition, bool clicked)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }

            roadDrawer.AddPathPoint(mousePosition, selectedRoad);
            base.LeftClick(mousePosition, clicked);
        }


        public override void RightClick(Vector3 mousePosition)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }
            roadDrawer.Delete(selectedRoad, mousePosition);
            base.RightClick(mousePosition);
        }


        private void ApplyGlobalCarSettings()
        {
            SetSpeedOnLanes(selectedRoad, editorSave.maxSpeed);
            for (int i = 0; i < selectedRoad.lanes.Count; i++)
            {
                for (int j = 0; j < allowedCarIndex.Length; j++)
                {
                    selectedRoad.lanes[i].allowedCars[j] = allowedCarIndex[j];
                }
            }
        }


        private void SetSpeedOnLanes(Road selectedRoad, int maxSpeed)
        {
            for (int i = 0; i < selectedRoad.lanes.Count; i++)
            {
                selectedRoad.lanes[i].laneSpeed = maxSpeed;
            }
        }


        private void DrawLaneButton(int currentLane)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal();
            selectedRoad.lanes[currentLane].laneSpeed = EditorGUILayout.IntField("Lane " + currentLane + ", Lane Speed:", selectedRoad.lanes[currentLane].laneSpeed);
            string buttonLebel = "<--";
            if (selectedRoad.lanes[currentLane].laneDirection == false)
            {
                buttonLebel = "-->";
            }
            if (GUILayout.Button(buttonLebel))
            {
                selectedRoad.lanes[currentLane].laneDirection = !selectedRoad.lanes[currentLane].laneDirection;
                connectionCreator.DeleteConnectionsWithThisLane(selectedRoad, currentLane);
                laneCreator.SwitchLaneDirection(selectedRoad, currentLane);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Allowed vehicle types on this lane:");
            for (int i = 0; i < nrOfCars; i++)
            {
                if (i >= selectedRoad.lanes[currentLane].allowedCars.Length)
                {
                    selectedRoad.lanes[currentLane].UpdateAllowedCars(nrOfCars);
                }
                selectedRoad.lanes[currentLane].allowedCars[i] = EditorGUILayout.Toggle(((VehicleTypes)i).ToString(), selectedRoad.lanes[currentLane].allowedCars[i]);
            }
            EditorGUILayout.EndVertical();
        }


        public override void DestroyWindow()
        {
            editorSave.moveTool = moveTool;
            editorSave.globalCarList = new List<VehicleTypes>();
            for (int i = 0; i < allowedCarIndex.Length; i++)
            {
                if (allowedCarIndex[i] == true)
                {
                    editorSave.globalCarList.Add((VehicleTypes)i);
                }
            }
            editorSave.nrOfLanes = selectedRoad.nrOfLanes;
            editorSave.laneWidth = selectedRoad.laneWidth;
            editorSave.waypointDistance = selectedRoad.waypointDistance;
            editorSave.otherLaneLinkDistance = selectedRoad.otherLaneLinkDistance;

            DestroyImmediate(roadData);
            DestroyImmediate(laneData);
            DestroyImmediate(connectionData);
            DestroyImmediate(waypointCreator);
            DestroyImmediate(connectionCreator);
            DestroyImmediate(laneCreator);
            DestroyImmediate(roadDrawer);
            DestroyImmediate(laneDrawer);

            base.DestroyWindow();
        }
    }
}
