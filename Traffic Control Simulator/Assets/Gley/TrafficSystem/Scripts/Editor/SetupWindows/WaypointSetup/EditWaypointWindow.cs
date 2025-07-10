using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class EditWaypointWindow : TrafficSetupWindow
    {
        protected struct VehicleDisplay
        {
            public Color color;
            public int vehicle;
            public bool active;
            public bool view;

            public VehicleDisplay(bool active, int vehicle, Color color)
            {
                this.active = active;
                this.vehicle = vehicle;
                this.color = color;
                view = false;
            }
        }


        protected enum ListToAdd
        {
            None,
            Neighbors,
            OtherLanes,
            GiveWayWaypoints
        }

        private readonly float scrollAdjustment = 230;

        private TrafficWaypointData trafficWaypointData;
        private TrafficWaypointDrawer waypointDrawer;
        private VehicleDisplay[] vehicleDisplay;
        private WaypointSettings selectedWaypoint;
        private WaypointSettings clickedWaypoint;
        private ListToAdd selectedList;
        private int nrOfAllowedVehicles;
        private int maxSpeed;
        private int priority;
        private int penalty;
      


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            
            selectedWaypoint = SettingsWindow.GetSelectedWaypoint();
            vehicleDisplay = SetCarDisplay();
            maxSpeed = selectedWaypoint.maxSpeed;
            priority = selectedWaypoint.priority;
            penalty = selectedWaypoint.penalty;
            trafficWaypointData = CreateInstance<TrafficWaypointData>().Initialize();
            waypointDrawer = CreateInstance<TrafficWaypointDrawer>().Initialize(trafficWaypointData);
            waypointDrawer.onWaypointClicked += WaypointClicked;
            
            return this;
        }


        public override void DrawInScene()
        {
            base.DrawInScene();

            if (selectedList != ListToAdd.None)
            {
                waypointDrawer.DrawWaypointsForLink(selectedWaypoint, selectedWaypoint.neighbors, selectedWaypoint.otherLanes, editorSave.editorColors.waypointColor);
            }

            waypointDrawer.DrawCurrentWaypoint(selectedWaypoint, editorSave.editorColors.selectedWaypointColor, editorSave.editorColors.waypointColor, editorSave.editorColors.laneChangeColor, editorSave.editorColors.prevWaypointColor, editorSave.editorColors.complexGiveWayColor);

            for (int i = 0; i < vehicleDisplay.Length; i++)
            {
                if (vehicleDisplay[i].view)
                {
                    waypointDrawer.ShowWaypointsWithVehicle(vehicleDisplay[i].vehicle, vehicleDisplay[i].color);
                }
            }

            if (clickedWaypoint)
            {
                waypointDrawer.DrawSelectedWaypoint(clickedWaypoint, editorSave.editorColors.selectedRoadConnectorColor);
            }
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUI.BeginChangeCheck();
            editorSave.editorColors.selectedWaypointColor = EditorGUILayout.ColorField("Selected Color ", editorSave.editorColors.selectedWaypointColor);
            editorSave.editorColors.waypointColor = EditorGUILayout.ColorField("Neighbor Color ", editorSave.editorColors.waypointColor);
            editorSave.editorColors.laneChangeColor = EditorGUILayout.ColorField("Lane Change Color ", editorSave.editorColors.laneChangeColor);
            editorSave.editorColors.prevWaypointColor = EditorGUILayout.ColorField("Previous Color ", editorSave.editorColors.prevWaypointColor);
            editorSave.editorColors.complexGiveWayColor = EditorGUILayout.ColorField("Required Free Waypoints ", editorSave.editorColors.complexGiveWayColor);

            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Select Waypoint"))
            {
                Selection.activeGameObject = selectedWaypoint.gameObject;
            }

            base.TopPart();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUI.BeginChangeCheck();
            if (selectedList == ListToAdd.None)
            {
                selectedWaypoint.giveWay = EditorGUILayout.Toggle(new GUIContent("Give Way", "Vehicle will stop when reaching this waypoint and check if next waypoint is free before continuing"), selectedWaypoint.giveWay);

                EditorGUILayout.BeginHorizontal();
                selectedWaypoint.complexGiveWay = EditorGUILayout.Toggle(new GUIContent("Complex Give Way", "Vehicle will stop when reaching this waypoint check if all selected waypoints are free before continue"), selectedWaypoint.complexGiveWay);
                if (selectedWaypoint.complexGiveWay)
                {
                    if (GUILayout.Button("Pick Required Free Waypoints"))
                    {
                        //PickFreeWaypoints();
                        selectedList = ListToAdd.GiveWayWaypoints;
                    }
                }
                EditorGUILayout.EndHorizontal();

                selectedWaypoint.zipperGiveWay = EditorGUILayout.Toggle(new GUIContent("Zipper Give Way", "Vehicles will stop before reaching this waypoint and continue randomly one at the time"), selectedWaypoint.zipperGiveWay);
                selectedWaypoint.triggerEvent = EditorGUILayout.Toggle(new GUIContent("Trigger Event", "If a vehicle reaches this, it will trigger an event"), selectedWaypoint.triggerEvent);
                if (selectedWaypoint.triggerEvent == true)
                {
                    selectedWaypoint.eventData = EditorGUILayout.TextField(new GUIContent("Event Data", "This string will be sent as a parameter for the event"), selectedWaypoint.eventData);
                }

                EditorGUILayout.BeginHorizontal();
                maxSpeed = EditorGUILayout.IntField(new GUIContent("Max speed", "The maximum speed allowed in this waypoint"), maxSpeed);
                if (GUILayout.Button("Set Speed"))
                {
                    if (maxSpeed != 0)
                    {
                        selectedWaypoint.speedLocked = true;
                    }
                    else
                    {
                        selectedWaypoint.speedLocked = false;
                    }
                    SetSpeed();
                }
                EditorGUILayout.EndHorizontal();




                EditorGUILayout.BeginHorizontal();
                priority = EditorGUILayout.IntField(new GUIContent("Spawn priority", "If the priority is higher, the vehicles will have higher chances to spawn on this waypoint"), priority);
                if (GUILayout.Button("Set Priority"))
                {
                    if (priority != 0)
                    {
                        selectedWaypoint.priorityLocked = true;
                    }
                    else
                    {
                        selectedWaypoint.priorityLocked = false;
                    }
                    SetPriority();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                penalty = EditorGUILayout.IntField(new GUIContent("Waypoint penalty", "Used for path finding. If penalty is higher vehicles are less likely to pick this route"), penalty);
                if (GUILayout.Button("Set Penalty "))
                {
                    if (penalty != 0)
                    {
                        selectedWaypoint.penaltyLocked = true;
                    }
                    else
                    {
                        selectedWaypoint.penaltyLocked = false;
                    }
                    SetPenalty();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Allowed vehicles: ", "Only the following vehicles can pass through this waypoint"), EditorStyles.boldLabel);
                EditorGUILayout.Space();

                for (int i = 0; i < nrOfAllowedVehicles; i++)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    vehicleDisplay[i].active = EditorGUILayout.Toggle(vehicleDisplay[i].active, GUILayout.MaxWidth(20));
                    EditorGUILayout.LabelField(((VehicleTypes)i).ToString());
                    vehicleDisplay[i].color = EditorGUILayout.ColorField(vehicleDisplay[i].color, GUILayout.MaxWidth(80));
                    Color oldColor = GUI.backgroundColor;
                    if (vehicleDisplay[i].view)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View", GUILayout.MaxWidth(64)))
                    {
                        vehicleDisplay[i].view = !vehicleDisplay[i].view;
                    }
                    GUI.backgroundColor = oldColor;

                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Set"))
                {
                    SetCars();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            MakeListOperations("Neighbors", "From this waypoint a moving agent can continue to the following ones", selectedWaypoint.neighbors, ListToAdd.Neighbors);
            
            EditorGUILayout.Space();
            MakeListOperations("Other Lanes", "Connections to other lanes, used for overtaking", selectedWaypoint.otherLanes, ListToAdd.OtherLanes);

            if (selectedList == ListToAdd.GiveWayWaypoints)
            {
                EditorGUILayout.Space();
                MakeListOperations("Pick Required Free Waypoints", "Waypoints required to be free for Complex Give Way", selectedWaypoint.giveWayList, ListToAdd.GiveWayWaypoints);
            }
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }

        private void SetCars()
        {
            List<VehicleTypes> result = new List<VehicleTypes>();
            for (int i = 0; i < vehicleDisplay.Length; i++)
            {
                if (vehicleDisplay[i].active)
                {
                    result.Add((VehicleTypes)vehicleDisplay[i].vehicle);
                }
            }
            selectedWaypoint.allowedCars = result;
            if (result.Count > 0)
            {
                selectedWaypoint.carsLocked = true;
            }
            else
            {
                selectedWaypoint.carsLocked = false;
            }
            List<WaypointSettings> waypointList = new List<WaypointSettings>();
            SetCarType(waypointList, selectedWaypoint.allowedCars, selectedWaypoint.neighbors);
        }


        private void DeleteWaypoint(WaypointSettingsBase waypoint, ListToAdd list)
        {
            switch (list)
            {
                case ListToAdd.Neighbors:
                    waypoint.prev.Remove(selectedWaypoint);
                    selectedWaypoint.neighbors.Remove(waypoint);
                    break;
                case ListToAdd.OtherLanes:
                    selectedWaypoint.otherLanes.Remove((WaypointSettings)waypoint);
                    break;
                case ListToAdd.GiveWayWaypoints:
                    selectedWaypoint.giveWayList.Remove((WaypointSettings)waypoint);
                    break;
            }
            clickedWaypoint = null;
            SceneView.RepaintAll();
        }

        private void AddNeighbor(WaypointSettingsBase neighbor)
        {
            if (!selectedWaypoint.neighbors.Contains(neighbor))
            {
                selectedWaypoint.neighbors.Add(neighbor);
                neighbor.prev.Add(selectedWaypoint);
            }
            else
            {
                neighbor.prev.Remove(selectedWaypoint);
                selectedWaypoint.neighbors.Remove(neighbor);
            }
        }


        private void AddOtherLanes(WaypointSettings waypoint)
        {
            if (!selectedWaypoint.otherLanes.Contains(waypoint))
            {
                selectedWaypoint.otherLanes.Add(waypoint);
            }
            else
            {
                selectedWaypoint.otherLanes.Remove(waypoint);
            }
        }


        private void AddGiveWayWaypoints(WaypointSettings waypoint)
        {
            if (!selectedWaypoint.giveWayList.Contains(waypoint))
            {
                selectedWaypoint.giveWayList.Add(waypoint);
            }
            else
            {
                selectedWaypoint.giveWayList.Remove(waypoint);
            }
        }

        private void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            if (leftClick)
            {
                if (selectedList == ListToAdd.Neighbors)
                {
                    AddNeighbor(clickedWaypoint);
                }

                if (selectedList == ListToAdd.OtherLanes)
                {
                    AddOtherLanes(clickedWaypoint);
                }

                if (selectedList == ListToAdd.GiveWayWaypoints)
                {
                    AddGiveWayWaypoints(clickedWaypoint);
                }

                if (selectedList == ListToAdd.None)
                {
                    window.SetActiveWindow(typeof(EditWaypointWindow), false);
                }
            }
            SettingsWindowBase.TriggerRefreshWindowEvent();
        }

        private void MakeListOperations<T>(string title, string description, List<T> listToEdit, ListToAdd listType) where T:WaypointSettingsBase
        {
            //if (listType == ListToAdd.GiveWayWaypoints)
            //    return;
            if (selectedList == listType || selectedList == ListToAdd.None)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent(title, description), EditorStyles.boldLabel);
                EditorGUILayout.Space();
                for (int i = 0; i < listToEdit.Count; i++)
                {
                    if (listToEdit[i] == null)
                    {
                        listToEdit.RemoveAt(i);
                        i--;
                        continue;
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(listToEdit[i].name);
                    Color oldColor = GUI.backgroundColor;
                    if (listToEdit[i] == clickedWaypoint)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View", GUILayout.MaxWidth(64)))
                    {
                        if (listToEdit[i] == clickedWaypoint)
                        {
                            clickedWaypoint = null;
                        }
                        else
                        {
                            ViewWaypoint(listToEdit[i]);
                        }
                    }
                    GUI.backgroundColor = oldColor;
                    if (GUILayout.Button("Delete", GUILayout.MaxWidth(64)))
                    {
                        DeleteWaypoint(listToEdit[i], listType);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                if (selectedList == ListToAdd.None)
                {
                    if (GUILayout.Button("Add/Remove " + title))
                    {
                        //baseWaypointDrawer.Initialize();
                        selectedList = listType;
                    }
                }
                else
                {
                    if (GUILayout.Button("Done"))
                    {
                        selectedList = ListToAdd.None;
                        SceneView.RepaintAll();
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }

        private void SetCarType(List<WaypointSettings> waypointList, List<VehicleTypes> carTypes, List<WaypointSettingsBase> neighbors)
        {
            if (carTypes == null || carTypes.Count == 0)
            {
                return;
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                var neighbor = (WaypointSettings)neighbors[i];
                if (!waypointList.Contains(neighbor))
                {
                    if (!neighbor.carsLocked)
                    {
                        waypointList.Add(neighbor);
                        neighbor.allowedCars = carTypes;
                        EditorUtility.SetDirty(neighbors[i]);
                        SetCarType(waypointList, carTypes, neighbors[i].neighbors);
                    }
                }
            }
        }



        private VehicleDisplay[] SetCarDisplay()
        {
            nrOfAllowedVehicles = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            VehicleDisplay[] carDisplay = new VehicleDisplay[nrOfAllowedVehicles];
            for (int i = 0; i < nrOfAllowedVehicles; i++)
            {
                carDisplay[i] = new VehicleDisplay(selectedWaypoint.allowedCars.Contains((VehicleTypes)i), i, Color.white);
            }
            return carDisplay;
        }



         private void ViewWaypoint(WaypointSettingsBase waypoint)
        {
            clickedWaypoint = (WaypointSettings)waypoint;
            GleyUtilities.TeleportSceneCamera(waypoint.transform.position);
        }


        private void SetSpeed()
        {
            List<WaypointSettings> waypointList = new List<WaypointSettings>();
            selectedWaypoint.maxSpeed = maxSpeed;
            SetSpeed(waypointList, selectedWaypoint.maxSpeed, selectedWaypoint.neighbors.Cast<WaypointSettings>().ToList());
        }


        private void SetSpeed(List<WaypointSettings> waypointList, int speed, List<WaypointSettings> neighbors)
        {
            if (speed == 0)
            {
                return;
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!waypointList.Contains(neighbors[i]))
                {
                    if (!neighbors[i].speedLocked)
                    {
                        waypointList.Add(neighbors[i]);
                        neighbors[i].maxSpeed = speed;
                        EditorUtility.SetDirty(neighbors[i]);
                        SetSpeed(waypointList, speed, neighbors[i].neighbors.Cast<WaypointSettings>().ToList());
                    }
                }
            }
        }


        private void SetPriority()
        {
            List<WaypointSettings> waypointList = new List<WaypointSettings>();
            selectedWaypoint.priority = priority;
            SetPriority(waypointList, selectedWaypoint.priority, selectedWaypoint.neighbors.Cast<WaypointSettings>().ToList());
        }


        private void SetPriority(List<WaypointSettings> waypointList, int priority, List<WaypointSettings> neighbors)
        {
            if (priority == 0)
            {
                return;
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!waypointList.Contains(neighbors[i]))
                {
                    if (!neighbors[i].priorityLocked)
                    {
                        waypointList.Add(neighbors[i]);
                        neighbors[i].priority = priority;
                        EditorUtility.SetDirty(neighbors[i]);
                        SetPriority(waypointList, priority, neighbors[i].neighbors.Cast<WaypointSettings>().ToList());
                    }
                }
            }
        }


        private void SetPenalty()
        {
            List<WaypointSettings> waypointList = new List<WaypointSettings>();
            selectedWaypoint.penalty = penalty;
            SetPenalty(waypointList, selectedWaypoint.penalty, selectedWaypoint.neighbors.Cast<WaypointSettings>().ToList());
        }


        private void SetPenalty(List<WaypointSettings> waypointList, int penalty, List<WaypointSettings> neighbors)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!waypointList.Contains(neighbors[i]))
                {
                    if (!neighbors[i].penaltyLocked)
                    {  
                        waypointList.Add(neighbors[i]);
                        neighbors[i].penalty = penalty;
                        EditorUtility.SetDirty(neighbors[i]);
                        SetPenalty(waypointList, penalty, neighbors[i].neighbors.Cast<WaypointSettings>().ToList());
                    }
                    else
                    {
                        Debug.Log(neighbors[i].name + " is penalty locked");
                    }
                }
            }
        }



        public override void DestroyWindow()
        {
            waypointDrawer.onWaypointClicked -= WaypointClicked;
            DestroyImmediate(trafficWaypointData);
            DestroyImmediate(waypointDrawer);
            if (selectedWaypoint)
            {
                EditorUtility.SetDirty(selectedWaypoint);
            }
            base.DestroyWindow();
        }
    }
}
