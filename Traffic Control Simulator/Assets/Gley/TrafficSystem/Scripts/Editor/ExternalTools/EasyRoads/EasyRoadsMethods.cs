#if GLEY_EASYROADS_TRAFFIC
using EasyRoads3Dv3;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gley.TrafficSystem.Internal;

namespace Gley.TrafficSystem.Editor
{
    public class EasyRoadsMethods : UnityEditor.Editor
    {
        const string EasyRoadsWaypointsHolder = "GleyEasyRoadsWaypoints";

        private static List<GenericIntersectionSettings> allGleyIntersections;
        private static List<Waypoint> points;
        private static List<Transform> waypointParents;
        private static List<Waypoint> connectors;
        private static List<Transform> connectionParents;
        private static List<Transform> allWaypoints;
        private static List<Transform> allConnectors;
        private static ERCrossings[] allERIntersections;


        public static void ExtractWaypoints(IntersectionType intersectionType, float greenLightTime, float yellowLightTime, bool linkLanes, int waypointDistance, List<int> vehicleTypes)
        {
            //destroy existing roads
            DestroyImmediate(GameObject.Find(EasyRoadsWaypointsHolder));

            //create scene hierarchy
            ERRoadNetwork roadNetwork = new ERRoadNetwork();
            ERRoad[] roads = roadNetwork.GetRoadObjects();
            Debug.Log("Roads: " + roads.Length);

            points = new List<Waypoint>();
            waypointParents = new List<Transform>();
            allWaypoints = new List<Transform>();
            allConnectors = new List<Transform>();
            connectors = new List<Waypoint>();
            connectionParents = new List<Transform>();
            allGleyIntersections = new List<GenericIntersectionSettings>();
            Transform holder = new GameObject(EasyRoadsWaypointsHolder).transform;
            Transform waypointsHolder = new GameObject("WaypointsHolder").transform;
            Transform intersectionHolder = new GameObject(Constants.intersectionHolderName).transform;

            waypointsHolder.SetParent(holder);
            intersectionHolder.SetParent(holder);

            AddIntersections(intersectionHolder, intersectionType, greenLightTime, yellowLightTime);

            //extract information from EasyRoads
            for (int i = 0; i < roads.Length; i++)
            {

                if (!roads[i].roadScript.isSideObject)
                {
                    GameObject road = new GameObject("Road_" + i);
                    road.transform.SetParent(waypointsHolder);
                    GameObject lanesHolder = new GameObject("Lanes");
                    Transform connectorsHolder = new GameObject("Connectors").transform;
                    lanesHolder.transform.SetParent(road.transform);
                    connectorsHolder.SetParent(road.transform);

                    if (roads[i].GetLaneCount() > 0)
                    {
                        float laneWidth = roads[i].GetWidth() / roads[i].GetLaneCount();
                        ExtractLaneWaypoints(roads[i].GetLeftLaneCount(), lanesHolder, roads[i], ERLaneDirection.Left, i, laneWidth);
                        ExtractLaneWaypoints(roads[i].GetRightLaneCount(), lanesHolder, roads[i], ERLaneDirection.Right, i, laneWidth);
                        ExtractConnectors(roads[i].GetLaneCount(), roads[i], connectorsHolder, i,laneWidth);
                    }
                    else
                    {
                        Debug.LogError("No lane data found for " + roads[i].gameObject + ". Make sure this road hat at least one lane inside Lane Info tab.", roads[i].gameObject);
                    }
                }
            }

            //convert extracted information to waypoints
            CreateTrafficWaypoints(vehicleTypes);

            LinkAllWaypoints(waypointsHolder);

            if (linkLanes)
            {
                LinkOvertakeLanes(waypointsHolder, waypointDistance);
            }

            CreateConnectorWaypoints(vehicleTypes);

            LinkAllConnectors(waypointsHolder);

            LinkConnectorsToRoadWaypoints();

            AssignIntersections(intersectionType);

            RemoveNonRequiredWaypoints();

            Debug.Log("total waypoints generated " + allWaypoints.Count);

            Debug.Log("Done generating waypoints for Easy Roads");
        }


        private static void RemoveNonRequiredWaypoints()
        {
            for (int j = allWaypoints.Count - 1; j >= 0; j--)
            {
                if (allWaypoints[j].GetComponent<WaypointSettings>().neighbors.Count == 0)
                {
                    DestroyImmediate(allWaypoints[j].gameObject);
                }
            }
        }


        private static void AssignIntersections(IntersectionType intersectionType)
        {
            for (int i = 0; i < connectors.Count; i++)
            {
                if (connectors[i].listIndex != -1)
                {
                    if (intersectionType == IntersectionType.Priority)
                    {
                        PriorityIntersectionSettings currentIntersection = (PriorityIntersectionSettings)allGleyIntersections[connectors[i].listIndex];
                        if (connectors[i].enter == true)
                        {
                            WaypointSettings waypointToAdd = allConnectors[i].GetComponent<WaypointSettings>();
                            if (waypointToAdd.prev.Count > 0)
                            {
                                waypointToAdd = (WaypointSettings)waypointToAdd.prev[0];
                                AssignEnterWaypoints(currentIntersection.enterWaypoints, waypointToAdd);
                            }
                            else
                            {
                                Debug.Log(waypointToAdd.name + " has no previous waypoints", waypointToAdd);
                            }
                        }

                        if (connectors[i].exit)
                        {
                            if (currentIntersection.exitWaypoints == null)
                            {
                                currentIntersection.exitWaypoints = new List<WaypointSettings>();
                            }
                            WaypointSettings waypointToAdd = allConnectors[i].GetComponent<WaypointSettings>();
                            if (waypointToAdd.neighbors.Count > 0)
                            {
                                waypointToAdd = (WaypointSettings)waypointToAdd.neighbors[0];
                                if (!currentIntersection.exitWaypoints.Contains(waypointToAdd))
                                {
                                    currentIntersection.exitWaypoints.Add(waypointToAdd);
                                }
                            }
                            else
                            {
                                Debug.Log(waypointToAdd.name + " has no neighbors.", waypointToAdd);
                            }
                        }
                    }
                    else
                    {
                        TrafficLightsIntersectionSettings currentIntersection = (TrafficLightsIntersectionSettings)allGleyIntersections[connectors[i].listIndex];
                        if (connectors[i].enter == true)
                        {
                            WaypointSettings waypoint = allConnectors[i].GetComponent<WaypointSettings>();
                            if (waypoint.prev.Count > 0)
                            {
                                AssignEnterWaypoints(currentIntersection.stopWaypoints, (WaypointSettings)allConnectors[i].GetComponent<WaypointSettings>().prev[0]);
                            }
                            else
                            {
                                Debug.Log(waypoint.name + " is not properly linked", waypoint);
                            }
                        }
                    }
                }
            }
        }


        private static void AssignEnterWaypoints(List<IntersectionStopWaypointsSettings> enterWaypoints, WaypointSettings waypointToAdd)
        {
            if (enterWaypoints == null)
            {
                enterWaypoints = new List<IntersectionStopWaypointsSettings>();
            }
            string roadName = waypointToAdd.name.Split('-')[0];
            int index = -1;

            for (int j = 0; j < enterWaypoints.Count; j++)
            {
                if (enterWaypoints[j].roadWaypoints.Count > 0)
                {
                    if (enterWaypoints[j].roadWaypoints[0].name.Contains(roadName))
                    {
                        index = j;
                    }
                }
            }
            if (index == -1)
            {
                enterWaypoints.Add(new IntersectionStopWaypointsSettings());
                index = enterWaypoints.Count - 1;
                enterWaypoints[index].roadWaypoints = new List<WaypointSettings>();
            }

            if (!enterWaypoints[index].roadWaypoints.Contains(waypointToAdd))
            {
                enterWaypoints[index].roadWaypoints.Add(waypointToAdd);
            }
        }


        private static void LinkConnectorsToRoadWaypoints()
        {
            for (int i = 0; i < allConnectors.Count; i++)
            {
                if (allConnectors[i].name.Contains(Gley.UrbanAssets.Internal.Constants.connectionEdgeName))
                {
                    for (int j = 0; j < allWaypoints.Count; j++)
                    {
                        if (Vector3.Distance(allConnectors[i].position, allWaypoints[j].position) < 0.01f)
                        {
                            WaypointSettings connectorScript = allConnectors[i].GetComponent<WaypointSettings>();
                            WaypointSettings waypointScript = allWaypoints[j].GetComponent<WaypointSettings>();

                            if (connectorScript.prev.Count == 0)
                            {
                                connectorScript.prev = waypointScript.prev;
                                waypointScript.prev[0].neighbors.Remove(waypointScript);
                                waypointScript.prev[0].neighbors.Add(connectorScript);

                            }

                            if (connectorScript.neighbors.Count == 0)
                            {
                                connectorScript.neighbors = waypointScript.neighbors;
                                if (waypointScript.neighbors.Count > 0)
                                {
                                    waypointScript.neighbors[0].prev.Add(connectorScript);
                                }
                                //else
                                //{
                                //    Debug.Log(waypointScript.name + " has no neighbors", waypointScript);
                                //}
                            }
                            break;
                        }
                    }
                }
            }
        }


        private static void CreateConnectorWaypoints(List<int> vehicleTypes)
        {
            TrafficWaypointCreator waypointCreator = CreateInstance<TrafficWaypointCreator>().Initialize();

            for (int i = 0; i < connectors.Count; i++)
            {
                allConnectors.Add(waypointCreator.CreateWaypoint(connectionParents[i], connectors[i].position, connectors[i].name, vehicleTypes, connectors[i].maxSpeed, connectors[i].laneWidth));
            }
        }


        private static void CreateTrafficWaypoints(List<int> vehicleTypes)
        {
            TrafficWaypointCreator waypointCreator = CreateInstance<TrafficWaypointCreator>().Initialize();
            for (int i = 0; i < points.Count; i++)
            {
                allWaypoints.Add(waypointCreator.CreateWaypoint(waypointParents[i], points[i].position, points[i].name, vehicleTypes, points[i].maxSpeed, points[i].laneWidth));
            }
        }


        private static void AddIntersections(Transform intersectionHolder, IntersectionType intersectionType, float greenLightTime, float yellowLightTime)
        {
            allERIntersections = FindObjectsOfType<ERCrossings>();
            for (int i = 0; i < allERIntersections.Length; i++)
            {
                GameObject intersection = new GameObject(allERIntersections[i].name);
                intersection.transform.SetParent(intersectionHolder);
                intersection.transform.position = allERIntersections[i].gameObject.transform.position;
                GenericIntersectionSettings intersectionScript = null;
                switch (intersectionType)
                {
                    case IntersectionType.Priority:
                        intersectionScript = intersection.AddComponent<PriorityIntersectionSettings>();
                        break;
                    case IntersectionType.TrafficLights:
                        intersectionScript = intersection.AddComponent<TrafficLightsIntersectionSettings>();
                        ((TrafficLightsIntersectionSettings)intersectionScript).greenLightTime = greenLightTime;
                        ((TrafficLightsIntersectionSettings)intersectionScript).yellowLightTime = yellowLightTime;
                        break;
                    default:
                        Debug.LogWarning(intersectionType + " not supported");
                        break;
                }

                allGleyIntersections.Add(intersectionScript);
            }
        }


        private static void LinkAllConnectors(Transform holder)
        {
            for (int r = 0; r < holder.childCount; r++)
            {
                for (int i = 0; i < holder.GetChild(r).GetChild(1).childCount; i++)
                {
                    Transform laneHolder = holder.GetChild(r).GetChild(1).GetChild(i);
                    LinkWaypoints(laneHolder);
                }
            }
        }


        private static void LinkAllWaypoints(Transform holder)
        {
            for (int r = 0; r < holder.childCount; r++)
            {
                for (int i = 0; i < holder.GetChild(r).GetChild(0).childCount; i++)
                {
                    Transform laneHolder = holder.GetChild(r).GetChild(0).GetChild(i);
                    LinkWaypoints(laneHolder);
                }
            }
        }


        private static void LinkOvertakeLanes(Transform holder, int waypointDistance)
        {
            for (int i = 0; i < holder.childCount; i++)
            {
                for (int j = 0; j < holder.GetChild(i).GetChild(0).childCount; j++)
                {
                    Transform firstLane = holder.GetChild(i).GetChild(0).GetChild(j);
                    int laneToLink = j - 1;
                    if (laneToLink >= 0)
                    {
                        LinkLanes(firstLane, holder.GetChild(i).GetChild(0).GetChild(laneToLink), waypointDistance);
                    }
                    laneToLink = j + 1;
                    if (laneToLink < holder.GetChild(i).GetChild(0).childCount)
                    {
                        LinkLanes(firstLane, holder.GetChild(i).GetChild(0).GetChild(laneToLink), waypointDistance);
                    }
                }
            }
        }


        private static void LinkLanes(Transform firstLane, Transform secondLane, int waypointDistance)
        {
            if (secondLane.name.Split('_')[2] == firstLane.name.Split('_')[2])
            {
                LinkLaneWaypoints(firstLane, secondLane, waypointDistance);
            }
        }


        private static void LinkLaneWaypoints(Transform currentLane, Transform otherLane, int waypointDistance)
        {
            for (int i = 0; i < currentLane.childCount; i++)
            {
                int otherLaneIndex = i + waypointDistance;
                if (otherLaneIndex < currentLane.childCount - 1)
                {
                    WaypointSettings currentLaneWaypoint = currentLane.GetChild(i).GetComponent<WaypointSettings>();
                    WaypointSettings otherLaneWaypoint = otherLane.GetChild(otherLaneIndex).GetComponent<WaypointSettings>();
                    currentLaneWaypoint.otherLanes.Add(otherLaneWaypoint);
                }
            }
        }


        private static void LinkWaypoints(Transform laneHolder)
        {
            WaypointSettings previousWaypoint = laneHolder.GetChild(0).GetComponent<WaypointSettings>();
            for (int j = 1; j < laneHolder.childCount; j++)
            {
                string waypointName = laneHolder.GetChild(j).name;
                WaypointSettings waypointScript = laneHolder.GetChild(j).GetComponent<WaypointSettings>();
                if (previousWaypoint != null)
                {
                    previousWaypoint.neighbors.Add(waypointScript);
                    waypointScript.prev.Add(previousWaypoint);
                }
                if (!waypointName.Contains("Output"))
                {
                    previousWaypoint = waypointScript;
                }
                else
                {
                    previousWaypoint = null;
                }
            }
        }


        static void ExtractLaneWaypoints(int lanes, GameObject lanesHolder, ERRoad road, ERLaneDirection side, int r, float laneWidth)
        {
            if (lanes > 0)
            {
                for (int i = 0; i < lanes; i++)
                {
                    Vector3[] positions = road.GetLanePoints(i, side);
                    if (positions != null)
                    {
                        GameObject lane = new GameObject("Lane" + i);
                        lane.name = "Lane_" + lanesHolder.transform.childCount + "_" + side;
                        lane.transform.SetParent(lanesHolder.transform);


                        for (int j = 0; j < positions.Length; j++)
                        {
                            Waypoint waypoint = new Waypoint();
                            waypoint.name = "Road_" + r + "-" + Gley.UrbanAssets.Internal.Constants.laneNamePrefix + i + "-" + Gley.UrbanAssets.Internal.Constants.waypointNamePrefix + j;
                            waypoint.position = positions[j];
                            waypoint.maxSpeed = (int)road.GetSpeedLimit();
                            waypoint.laneWidth = laneWidth;
                            points.Add(waypoint);
                            waypointParents.Add(lane.transform);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No lane points found for " + road.gameObject.name + ", make sure Generate Lane Data is enabled from AI traffic", road.gameObject);
                    }
                }
            }
        }


        private static void ExtractConnectors(int lanes, ERRoad road, Transform lanesHolder, int roadIndex,float laneWidth)
        {
            bool endConnectorsFound = true;
            bool connectorsFound = true;
            GameObject connectorGameobect = null;
            for (int i = 0; i < lanes; i++)
            {
                int connectionIndex;
                ERConnection conn = road.GetConnectionAtEnd(out connectionIndex);
                if (conn != null)
                {
                    ERLaneConnector[] laneConnectors = conn.GetLaneData(connectionIndex, i);
                    if (laneConnectors != null)
                    {
                        ExtractLaneConnectors(conn, laneConnectors, lanesHolder, i, roadIndex, (int)road.GetSpeedLimit(), laneWidth);
                    }
                    else
                    {
                        connectorsFound = false;
                        connectorGameobect = conn.gameObject;
                    }
                }
                else
                {
                    endConnectorsFound = false;
                }

                conn = road.GetConnectionAtStart(out connectionIndex);
                if (conn != null)
                {
                    ERLaneConnector[] laneConnectors = conn.GetLaneData(connectionIndex, i);
                    if (laneConnectors != null)
                    {
                        ExtractLaneConnectors(conn, laneConnectors, lanesHolder, i, roadIndex, (int)road.GetSpeedLimit(), laneWidth);
                    }
                    else
                    {
                        connectorsFound = false;
                        connectorGameobect = conn.gameObject;
                    }
                }
                else
                {
                    endConnectorsFound = false;
                }
            }

            if (endConnectorsFound == false)
            {
                Debug.LogWarning(road.gameObject + " is not connected to anything ", road.gameObject);
            }

            if (connectorsFound == false)
            {
                Debug.LogWarning("No waypoint connectors found for " + connectorGameobect + ". You should connect it manually.", connectorGameobect);
            }
        }


        private static void ExtractLaneConnectors(ERConnection conn, ERLaneConnector[] laneConnectors, Transform lanesHolder, int laneIndex, int roadIndex, int speedLimit, float laneWidth)
        {

            if (laneConnectors != null)
            {
                for (int j = 0; j < laneConnectors.Length; j++)
                {
                    GameObject lane = new GameObject("Connector" + j);
                    lane.transform.SetParent(lanesHolder);
                    for (int k = 0; k < laneConnectors[j].points.Length; k++)
                    {
                        Waypoint waypoint = new Waypoint();
                        waypoint.listIndex = -1;
                        if (k == 0 || k == laneConnectors[j].points.Length - 1)
                        {
                            waypoint.name = "Road_" + roadIndex + "-" + Gley.UrbanAssets.Internal.Constants.laneNamePrefix + laneIndex + "-" + Gley.UrbanAssets.Internal.Constants.connectionEdgeName + k;
                            waypoint.listIndex = Array.FindIndex(allERIntersections, cond => cond.gameObject == conn.gameObject);
                            if (k == 0)
                            {
                                waypoint.enter = true;
                            }
                            else
                            {
                                waypoint.exit = true;
                            }
                        }
                        else
                        {
                            waypoint.name = "Road_" + roadIndex + "-" + Gley.UrbanAssets.Internal.Constants.laneNamePrefix + laneIndex + "-" + Gley.UrbanAssets.Internal.Constants.connectionWaypointName + k;
                        }

                        waypoint.position = laneConnectors[j].points[k];
                        waypoint.maxSpeed = speedLimit;
                        waypoint.laneWidth = laneWidth;
                        connectors.Add(waypoint);
                        connectionParents.Add(lane.transform);
                    }
                }
            }
        }
    }
}
#endif
