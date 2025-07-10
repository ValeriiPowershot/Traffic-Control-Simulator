using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Gley.TrafficSystem.Editor
{
    public class TrafficConnectionDrawer : Drawer
    {
        private List<ConnectionCurve> selectedConnections = new List<ConnectionCurve>();
        private TrafficConnectionData connectionData;

        internal delegate void WaypointClicked(Road road, int lane);
        internal event WaypointClicked onWaypointClicked;
        void TriggerWaypointClickedEvent(Road road, int lane)
        {
            if (onWaypointClicked != null)
            {
                onWaypointClicked(road, lane);
            }
        }


        internal TrafficConnectionDrawer Initialize(TrafficConnectionData connectionData)
        {  
            this.connectionData = connectionData;
            base.Initialize(connectionData);
            return this;
        }


        internal List<ConnectionCurve> ShowAllConnections(List<Road> allRoads, bool viewLabels, Color connectorLaneColor, Color anchorPointColor, Color roadConnectorColor, Color selectedRoadConnectorColor, Color disconnectedColor, float waypointDistance, Color textColor, Color waypointColor,
            bool outConnectors, Road selectedRoad, int selectedLane)
        {
            style.normal.textColor = textColor;
            var allConnections = connectionData.GetAllConnections();
            UpdateInViewProperty(allConnections);
            for (int i = 0; i < allRoads.Count; i++)
            {
                DrawConnectors(allRoads[i], roadConnectorColor, selectedRoadConnectorColor, disconnectedColor, waypointDistance, outConnectors, selectedRoad, selectedLane);
            }

            int nr = 0;
            for (int i = 0; i < allConnections.Length; i++)
            {
                if (allConnections[i].inView)
                {
                    nr++;
                    if (allConnections[i].draw == true)
                    {
                        DrawConnection(allConnections[i], viewLabels, connectorLaneColor, anchorPointColor);
                    }
                    if (allConnections[i].drawWaypoints == true)
                    {
                        Handles.color = waypointColor;
                        DrawWaypoints(allConnections[i]);
                    }
                }
            }
            if (nr != selectedConnections.Count)
            {
                UpdateSelectedConnections(allConnections);
            }
            return selectedConnections;
        }


        private void UpdateInViewProperty(ConnectionCurve[] connectionCurves)
        {
            GleyUtilities.SetCamera();
            if (cameraMoved)
            {
                cameraMoved = false;
                for (int i = 0; i < connectionCurves.Length; i++)
                {
                    if (GleyUtilities.IsPointInView(connectionCurves[i].inPosition) || GleyUtilities.IsPointInView(connectionCurves[i].outPosition))
                    {
                        connectionCurves[i].inView = true;
                    }
                    else
                    {
                        connectionCurves[i].inView = false;
                    }
                }
            }
        }


        private void UpdateSelectedConnections(ConnectionCurve[] allConnections)
        {
            selectedConnections = new List<ConnectionCurve>();
            for (int i = 0; i < allConnections.Length; i++)
            {
                if (allConnections[i].inView == true)
                {
                    selectedConnections.Add(allConnections[i]);
                }
            }
        }


        private void DrawWaypoints(ConnectionCurve connectionCurve)
        {
            var allWaypoints = connectionData.GetWaypoints(connectionCurve);
            Vector3[] positions = new Vector3[allWaypoints.Length + 2];
            positions[0] = connectionCurve.GetOutConnector().transform.position;
            positions[positions.Length - 1] = connectionCurve.GetInConnector().transform.position;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                positions[i + 1] = allWaypoints[i].position;
                DrawTriangle(positions[i], positions[i + 1]);
            }
            DrawTriangle(positions[positions.Length - 2], positions[positions.Length - 1]);
            Handles.DrawPolyLine(positions);
        }


        private void DrawConnectors(Road road, Color roadConnectorColor, Color selectedRoadConnectorColor, Color disconnectedColor, float waypointDistance, bool outConnectors, Road selectedRoad, int selectedLane)
        {
            float size;
            for (int lane = 0; lane < road.lanes.Count; lane++)
            {
                var outConnectorPosition = road.lanes[lane].laneEdges.outConnector.transform.position;
                if (outConnectors)
                {
                    size = Customizations.GetRoadConnectorSize(SceneView.lastActiveSceneView.camera.transform.position, outConnectorPosition);

                    if (road.lanes[lane].laneEdges.outConnector.neighbors.Count == 0)
                    {
                        Handles.color = disconnectedColor;
                    }
                    else
                    {
                        Handles.color = roadConnectorColor;
                    }
                    if (Handles.Button(outConnectorPosition, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), size, size, Handles.DotHandleCap))
                    {
                        TriggerWaypointClickedEvent(road, lane);
                    }
                }
                else
                {
                    if (lane == selectedLane && selectedRoad == road)
                    {
                        size = Customizations.GetRoadConnectorSize(SceneView.lastActiveSceneView.camera.transform.position, outConnectorPosition);
                        Handles.color = selectedRoadConnectorColor;
                        if (Handles.Button(outConnectorPosition, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), size, size, Handles.DotHandleCap))
                        {
                            TriggerWaypointClickedEvent(null, -1);
                            return;
                        }
                    }

                    var inConnector = road.lanes[lane].laneEdges.inConnector;
                    size = Customizations.GetRoadConnectorSize(SceneView.lastActiveSceneView.camera.transform.position, inConnector.transform.position);

                    if (inConnector.prev.Count == 0)
                    {
                        Handles.color = disconnectedColor;
                    }
                    else
                    {
                        Handles.color = roadConnectorColor;
                    }
                    if (Handles.Button(inConnector.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), size, size, Handles.DotHandleCap))
                    {
                        TriggerWaypointClickedEvent(road, lane);
                    }
                }
            }
        }


        private void DrawConnection(ConnectionCurve connection, bool viewLabel, Color connectorLaneColor, Color anchorPointColor)
        {
            Path curve = connection.GetCurve();
            for (int i = 0; i < curve.NumSegments; i++)
            {
                Vector3[] points = curve.GetPointsInSegment(i, connection.GetOffset());
                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
                Handles.DrawBezier(points[0], points[3], points[1], points[2], connectorLaneColor, null, 2);
            }

            for (int i = 0; i < curve.NumPoints; i++)
            {
                if (i % 3 != 0)
                {
                    float handleSize = Customizations.GetAnchorPointSize(SceneView.lastActiveSceneView.camera.transform.position, curve.GetPoint(i, connection.GetOffset()));
                    Handles.color = anchorPointColor;
                    Vector3 newPos = curve.GetPoint(i, connection.GetOffset());
#if UNITY_2019 || UNITY_2020 || UNITY_2021
                    newPos = Handles.FreeMoveHandle(curve.GetPoint(i, connection.GetOffset()), Quaternion.identity, handleSize, Vector2.zero, Handles.SphereHandleCap);
#else
                    newPos = Handles.FreeMoveHandle(curve.GetPoint(i, connection.GetOffset()), handleSize, Vector2.zero, Handles.SphereHandleCap);
#endif
                    newPos.y = curve.GetPoint(i, connection.GetOffset()).y;
                    if (curve.GetPoint(i, connection.GetOffset()) != newPos)
                    {
                        Undo.RecordObject(connectionData.GetConnectionPool(connection), "Move connection point");
                        curve.MovePoint(i, newPos - connection.GetOffset());
                    }
                }
            }

            if (viewLabel)
            {
                Handles.Label(connection.GetOutConnector().gameObject.transform.position, connection.GetName(), style);
            }
        }
    }
}