using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficWaypointDrawer : Drawer
    {
        private TrafficWaypointData trafficWaypointData;
        private List<WaypointSettings> pathProblems = new List<WaypointSettings>();
        private Dictionary<VehicleTypes, string> vehicleTypesToString = new Dictionary<VehicleTypes, string>();
        private Dictionary<int, string> priorityToString = new Dictionary<int, string>();
        private Dictionary<int, string> speedToString = new Dictionary<int, string>();
        private GUIStyle speedStyle;
        private GUIStyle carsStyle;
        private GUIStyle priorityStyle;
        private Quaternion towardsCamera;
        private bool colorChanged;


        internal delegate void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick);
        internal event WaypointClicked onWaypointClicked;
        void TriggerWaypointClickedEvent(WaypointSettings clickedWaypoint, bool leftClick)
        {
            SettingsWindow.SetSelectedWaypoint(clickedWaypoint);
            if (onWaypointClicked != null)
            {
                onWaypointClicked(clickedWaypoint, leftClick);
            }
        }


        internal TrafficWaypointDrawer Initialize(TrafficWaypointData trafficWaypointData)
        {
            this.trafficWaypointData = trafficWaypointData;
            base.Initialize(trafficWaypointData);


            speedStyle = new GUIStyle();
            carsStyle = new GUIStyle();
            priorityStyle = new GUIStyle();
            vehicleTypesToString = new Dictionary<VehicleTypes, string>();
            var allTypes = Enum.GetValues(typeof(VehicleTypes)).Cast<VehicleTypes>();

            foreach (var vehicleType in allTypes)
            {
                vehicleTypesToString.Add(vehicleType, vehicleType.ToString());
            }

            var allWaypoints = trafficWaypointData.GetAllWaypoints();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (!priorityToString.ContainsKey(allWaypoints[i].priority))
                {
                    priorityToString.Add(allWaypoints[i].priority, allWaypoints[i].priority.ToString());
                }

                if (!speedToString.ContainsKey(allWaypoints[i].maxSpeed))
                {
                    speedToString.Add(allWaypoints[i].maxSpeed, allWaypoints[i].maxSpeed.ToString());
                }
            }

            return this;
        }


        internal void DrawWaypointsForLink(WaypointSettings currentWaypoint, List<WaypointSettingsBase> neighborsList, List<WaypointSettings> otherLinesList, Color waypointColor)
        {
            colorChanged = true;
            var allWaypoints = trafficWaypointData.GetAllWaypoints();
            UpdateInViewProperty(allWaypoints);
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i] != currentWaypoint && !neighborsList.Contains(allWaypoints[i]) && !otherLinesList.Contains(allWaypoints[i]))
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                    }
                }
            }
        }


        internal void ShowIntersectionWaypoints(Color waypointColor)
        {
            ShowAllWaypoints(waypointColor, true, false, default, false, default, false, default, false, default);
        }


        internal void ShowAllWaypoints(Color waypointColor, bool showConnections, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor, bool showPriority, Color priorityColor)
        {
            var allWaypoints = trafficWaypointData.GetAllWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            if (showSpeed)
            {
                speedStyle.normal.textColor = speedColor;
            }
            if (showCars)
            {
                carsStyle.normal.textColor = carsColor;
            }
            if (showPriority)
            {
                priorityStyle.normal.textColor = priorityColor;
            }

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], showConnections, waypointColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor, showPriority, priorityColor, false, default, true);
                }
            }
        }


        internal void ShowWaypointsWithPenalty(int penalty, Color color)
        {
            var allWaypoints = trafficWaypointData.GetAllWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i].penalty == penalty)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, color, false, default, false, default, false, default, false, default, false, default, false);
                    }
                }
            }
        }


        internal void ShowWaypointsWithPriority(int priority, Color color)
        {
            var allWaypoints = trafficWaypointData.GetAllWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i].priority == priority)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, color, false, default, false, default, false, default, false, default, false, default, false);
                    }
                }
            }
        }


        internal void ShowWaypointsWithVehicle(int vehicle, Color color)
        {
            var allWaypoints = trafficWaypointData.GetAllWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i].allowedCars.Contains((VehicleTypes)vehicle))
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, color, false, default, false, default, false, default, false, default, false, default, false);
                    }
                }
            }
        }


        internal void ShowWaypointsWithSpeed(int speed, Color color)
        {
            var allWaypoints = trafficWaypointData.GetAllWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i].maxSpeed == speed)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, color, false, default, false, default, false, default, false, default, false, default, false);
                    }
                }
            }
        }


        internal void ShowDisconnectedWaypoints(Color waypointColor)
        {
            var allWaypoints = trafficWaypointData.GetDisconnectedWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                }
            }
        }


        internal void ShowVehicleEditedWaypoints(Color waypointColor, Color carsColor)
        {
            var allWaypoints = trafficWaypointData.GetVehicleEditedWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            carsStyle.normal.textColor = carsColor;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, true, default, false, default, false, default, false, default, true);
                }
            }
        }


        internal void ShowSpeedEditedWaypoints(Color waypointColor, Color speedColor)
        {
            var allWaypoints = trafficWaypointData.GetSpeedEditedWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            speedStyle.normal.textColor = speedColor;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, true, default, false, default, false, default, false, default, false, default, true);
                }
            }
        }


        internal void ShowPriorityEditedWaypoints(Color waypointColor, Color priorityColor)
        {
            var allWaypoints = trafficWaypointData.GetPriorityEditedWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            priorityStyle.normal.textColor = priorityColor;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, true, default, false, default, true);
                }
            }
        }


        internal void ShowGiveWayWaypoints(Color waypointColor)
        {
            var allWaypoints = trafficWaypointData.GetGiveWayWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                }
            }
        }


        internal void ShowComplexGiveWayWaypoints(Color waypointColor)
        {
            var allWaypoints = trafficWaypointData.GetComplexGiveWayWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                }
            }
        }


        internal void ShowZipperGiveWayWaypoints(Color waypointColor)
        {
            var allWaypoints = trafficWaypointData.GetZipperGiveWayWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                }
            }
        }


        internal void ShowEventWaypoints(Color waypointColor)
        {
            var allWaypoints = trafficWaypointData.GetEventWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                    Handles.Label(allWaypoints[i].position, (allWaypoints[i]).eventData);
                }
            }
        }


        internal WaypointSettings[] ShowVehiclePathProblems(Color waypointColor, Color carsColor)
        {
            var allWaypoints = trafficWaypointData.GetAllWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            carsStyle.normal.textColor = carsColor;
            pathProblems = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    int nr = allWaypoints[i].allowedCars.Count;
                    for (int j = 0; j < allWaypoints[i].allowedCars.Count; j++)
                    {
                        for (int k = 0; k < allWaypoints[i].neighbors.Count; k++)
                        {
                            if (((WaypointSettings)allWaypoints[i].neighbors[k]).allowedCars.Contains(allWaypoints[i].allowedCars[j]))
                            {
                                nr--;
                                break;
                            }
                        }
                    }
                    if (nr != 0)
                    {
                        pathProblems.Add(allWaypoints[i]);
                        DrawCompleteWaypoint(allWaypoints[i], true, waypointColor, false, default, true, default, false, default, false, default, false, default, true);

                        for (int k = 0; k < allWaypoints[i].neighbors.Count; k++)
                        {
                            for (int j = 0; j < ((WaypointSettings)allWaypoints[i].neighbors[k]).allowedCars.Count; j++)
                            {
                                DrawCompleteWaypoint((WaypointSettings)allWaypoints[i].neighbors[k], false, waypointColor, false, default, true, default, false, default, false, default, false, default, true);
                            }
                        }
                    }
                }

            }
            return pathProblems.ToArray();
        }


        internal void ShowPenaltyEditedWaypoints(Color waypointColor)
        {
            var allWaypoints = trafficWaypointData.GetPenlatyEditedWaypoints();
            colorChanged = true;
            UpdateInViewProperty(allWaypoints);
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                }
            }
        }


        internal void DrawCurrentWaypoint(WaypointSettings waypoint, Color selectedColor, Color waypointColor, Color otherLaneColor, Color prevColor, Color giveWayColor)
        {
            colorChanged = true;

            DrawCompleteWaypoint(waypoint, true, selectedColor, false, default, false, default, true, otherLaneColor, false, default, true, prevColor, true);

            for (int i = 0; i < waypoint.neighbors.Count; i++)
            {
                DrawCompleteWaypoint((WaypointSettings)waypoint.neighbors[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
            }

            colorChanged = true;
            for (int i = 0; i < waypoint.prev.Count; i++)
            {

                DrawCompleteWaypoint((WaypointSettings)waypoint.prev[i], false, prevColor, false, default, false, default, false, default, false, default, false, default, true);
            }

            colorChanged = true;
            for (int i = 0; i < waypoint.otherLanes.Count; i++)
            {

                DrawCompleteWaypoint((WaypointSettings)waypoint.otherLanes[i], false, otherLaneColor, false, default, false, default, false, default, false, default, false, default, true);
            }

            colorChanged = true;
            for (int i = 0; i < waypoint.giveWayList.Count; i++)
            {

                DrawCompleteWaypoint((WaypointSettings)waypoint.giveWayList[i], true, giveWayColor, false, default, false, default, false, default, false, default, false, default, true);
            }
        }


        internal void DrawSelectedWaypoint(WaypointSettings selectedWaypoint, Color color)
        {
            colorChanged = true;
            DrawCompleteWaypoint(selectedWaypoint, false, color, false, default, false, default, false, default, false, default, false, default, false);
        }


        private void UpdateInViewProperty(WaypointSettings[] selectedWaypoints)
        {
            GleyUtilities.SetCamera();
            if (cameraMoved)
            {
                cameraMoved = false;
                for (int i = 0; i < selectedWaypoints.Length; i++)
                {
                    if (GleyUtilities.IsPointInView(selectedWaypoints[i].position))
                    {
                        selectedWaypoints[i].inView = true;
                    }
                    else
                    {
                        selectedWaypoints[i].inView = false;
                    }
                }
                if (Camera.current != null)
                {
                    towardsCamera = Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up);
                }
            }
        }


        private void DrawCompleteWaypoint(WaypointSettings waypoint, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor, bool showPriority, Color priorityColor, bool drawPrev, Color prevColor, bool showDirection)
        {
            SetHandleColor(connectionColor, colorChanged);
            if (colorChanged == true)
            {
                colorChanged = false;
            }
            //clickable button
            if (Handles.Button(waypoint.position, towardsCamera, 0.5f, 0.5f, Handles.DotHandleCap))
            {
                TriggerWaypointClickedEvent(waypoint, Event.current.button == 0);
            }

            if (showConnections)
            {
                for (int i = 0; i < waypoint.neighbors.Count; i++)
                {
                    DrawConnection(waypoint.position, waypoint.neighbors[i].position, showDirection);
                }
            }

            if (drawPrev)
            {
                colorChanged = true;
                SetHandleColor(prevColor, colorChanged);
                for (int i = 0; i < waypoint.prev.Count; i++)
                {
                    DrawConnection(waypoint.position, waypoint.prev[i].position, false);
                }
            }

            if (drawOtherLanes)
            {
                for (int i = 0; i < waypoint.otherLanes.Count; i++)
                {
                    colorChanged = true;
                    SetHandleColor(otherLanesColor, colorChanged);
                    DrawConnection(waypoint.position, waypoint.otherLanes[i].position, true);
                }
            }

            if (showSpeed)
            {
                ShowSpeed(waypoint);
            }
            if (showCars)
            {
                ShowCars(waypoint);
            }
            if (showPriority)
            {
                ShowPriority(waypoint);
            }
        }


        private void ShowCars(WaypointSettings waypoint)
        {
            string text = string.Empty;
            for (int j = 0; j < waypoint.allowedCars.Count; j++)
            {
                text += vehicleTypesToString[waypoint.allowedCars[j]] + "\n";
            }
            Handles.Label(waypoint.position, text, carsStyle);
        }


        private void ShowSpeed(WaypointSettings waypoint)
        {
            Handles.Label(waypoint.position, speedToString[waypoint.maxSpeed], speedStyle);
        }


        private void ShowPriority(WaypointSettings waypoint)
        {
            Handles.Label(waypoint.position, priorityToString[waypoint.priority], priorityStyle);
        }


        private void SetHandleColor(Color newColor, bool colorChanged)
        {
            if (colorChanged)
            {
                Handles.color = newColor;
            }
        }


        private void DrawConnection(Vector3 start, Vector3 end, bool drawDirection)
        {
            Handles.DrawLine(start, end);
            if (drawDirection)
            {
                DrawTriangle(start, end);
            }
        }
    }
}
