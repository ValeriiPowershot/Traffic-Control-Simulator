using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using UnityEditor;
using UnityEngine;


namespace Gley.TrafficSystem.Editor
{
    public class TrafficLaneDrawer : Drawer
    {
        private TrafficLaneData laneData;
        private WaypointSettings waypointScript;
        private Quaternion up = Quaternion.LookRotation(Vector3.up);
      

        internal TrafficLaneDrawer Initialize(TrafficLaneData laneData)
        {
            this.laneData = laneData;
            base.Initialize(laneData);
            return this;
        }


        internal void DrawAllLanes(Road road, bool drawWaypoints, bool drawLaneChange, bool drawLabels, Color laneColor, Color waypointColor, Color disconnectedColor, Color laneChangeColor, Color labelColor)
        {
            var lanes = laneData.GetRoadLanes(road);
            if (lanes == null)
            {
                return;
            }

            style.normal.textColor = labelColor;
            for (int i = 0; i < lanes.Length; i++)
            {
                DrawSingleLane(lanes[i], laneColor, drawWaypoints, waypointColor, drawLaneChange, laneChangeColor, drawLabels, labelColor, disconnectedColor);
            }
        }


        private void DrawSingleLane(LaneHolder<WaypointSettings> laneHolder, Color laneColor, bool drawWaypoints, Color waypointColor, bool drawLaneChange, Color laneChangeColor, bool drawLabels, Color labelsColor, Color disconnectedColor)
        {
            Vector3[] positions = new Vector3[laneHolder.waypoints.Length];
            for (int i = 0; i < laneHolder.waypoints.Length; i++)
            {
                positions[i] = laneHolder.waypoints[i].position;

                if (drawWaypoints)
                {
                    waypointScript = laneHolder.waypoints[i];

                    if (waypointScript.neighbors.Count == 0 || waypointScript.prev.Count == 0)
                    {
                        Handles.color = disconnectedColor;
                        DrawUnconnectedWaypoint(waypointScript.position);
                    }

                    if (drawLaneChange)
                    {
                        Handles.color = laneChangeColor;
                        for (int j = 0; j < waypointScript.otherLanes.Count; j++)
                        {
                            Handles.DrawLine(waypointScript.position, waypointScript.otherLanes[j].position);
                            DrawTriangle(waypointScript.position, waypointScript.otherLanes[j].position);
                        }
                    }

                    Handles.color = waypointColor;
                    for (int j = 0; j < waypointScript.neighbors.Count; j++)
                    {
                        DrawTriangle(waypointScript.position, waypointScript.neighbors[j].position);
                    }
                }
            }
            if (!drawWaypoints)
            {
                if (laneHolder.waypoints[0].prev.Count > 0)
                {
                    Handles.color = laneColor;
                    DrawTriangle(laneHolder.waypoints[0].prev[0].position, positions[0]);
                }
                if (drawLabels)
                {
                    Handles.color = labelsColor;
                    Handles.Label(positions[0], laneHolder.name, style);
                }

                if (laneHolder.waypoints[laneHolder.waypoints.Length - 1].prev.Count > 0)
                {
                    Handles.color = laneColor;
                    DrawTriangle(laneHolder.waypoints[laneHolder.waypoints.Length - 1].prev[0].position, positions[laneHolder.waypoints.Length - 1]);
                }
                if (drawLabels)
                {
                    Handles.color = labelsColor;
                    Handles.Label(positions[laneHolder.waypoints.Length - 1], laneHolder.name, style);
                }
                Handles.color = laneColor;
            }
            else
            {
                Handles.color = waypointColor;
            }
            Handles.DrawPolyLine(positions);
        }


        private void DrawUnconnectedWaypoint(Vector3 position)
        {
            Handles.ArrowHandleCap(0, position, up, 10, EventType.Repaint);
        }
    }
}
