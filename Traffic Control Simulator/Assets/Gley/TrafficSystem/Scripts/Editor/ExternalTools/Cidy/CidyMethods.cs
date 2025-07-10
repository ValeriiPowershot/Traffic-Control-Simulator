#if GLEY_CIDY_TRAFFIC
using CiDy;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Gley.TrafficSystem.Internal;

namespace Gley.TrafficSystem.Editor
{
    public class CidyMethods : UnityEditor.Editor
    {
        struct TrafficLight
        {
            public Transform lightObject;
            public int intersectionIndex;
            public int roadIndex;

            public TrafficLight(Transform lightObject, int intersectionIndex, int roadIndex)
            {
                this.lightObject = lightObject;
                this.intersectionIndex = intersectionIndex;
                this.roadIndex = roadIndex;
            }
        }

        const string CidyWaypointsHolder = "GleyCidyWaypoints";

        private static List<GenericIntersectionSettings> allGleyIntersections;
        private static List<TrafficLight> trafficLights;
        private static List<Transform> allWaypoints;
        private static List<Transform> allConnectors;
        private static List<Waypoint> connectors;

        internal static void ExtractWaypoints(IntersectionType intersectionType, float greenLightTime, float yellowLightTime, int maxSpeed, List<int> vehicleTypes, int waypointDistance)
        {
            DestroyImmediate(GameObject.Find(CidyWaypointsHolder));

            allWaypoints = new List<Transform>();
            allConnectors = new List<Transform>();
            allGleyIntersections = new List<GenericIntersectionSettings>();
            connectors = new List<Waypoint>();
            trafficLights = new List<TrafficLight>();

            Transform holder = new GameObject(CidyWaypointsHolder).transform;
            Transform waypointsHolder = new GameObject("WaypointsHolder").transform;
            Transform intersectionHolder = new GameObject(Constants.intersectionHolderName).transform;

            waypointsHolder.SetParent(holder);
            intersectionHolder.SetParent(holder);

            CiDyGraph graph = FindObjectOfType<CiDyGraph>();
            graph.BuildTrafficData();

            //extract road waypoints
            List<GameObject> roads = graph.roads;
            for (int i = 0; i < roads.Count; i++)
            {
                GameObject road = new GameObject("Road_" + i);
                road.transform.SetParent(waypointsHolder);
                CiDyRoad cidyRoad = roads[i].GetComponent<CiDyRoad>();
                ExtractLaneWaypoints(cidyRoad.leftRoutes.routes, road, "Left", i, maxSpeed, cidyRoad.laneWidth, vehicleTypes);
                ExtractLaneWaypoints(cidyRoad.rightRoutes.routes, road, "Right", i, maxSpeed, cidyRoad.laneWidth, vehicleTypes);
            }

            //extract connectors
            List<CiDyNode> nodes = graph.masterGraph;
            for (int i = 0; i < nodes.Count; i++)
            {
                CiDyNode node = nodes[i];
                float laneWidth = node.connectedRoads[0].laneWidth;
                switch (node.type)
                {
                    case CiDyNode.IntersectionType.continuedSection:
                        GameObject lane = new GameObject("Connector" + i);
                        lane.transform.SetParent(waypointsHolder);
                        lane.transform.position = node.position;
                        for (int j = 0; j < node.leftRoutes.routes.Count; j++)
                        {
                            ExtractLaneConnectors(node.leftRoutes.routes[j], lane.transform, j, i, maxSpeed, laneWidth, -1, vehicleTypes);
                        }
                        for (int j = 0; j < node.rightRoutes.routes.Count; j++)
                        {
                            ExtractLaneConnectors(node.rightRoutes.routes[j], lane.transform, j, i, maxSpeed, laneWidth, -1, vehicleTypes);
                        }
                        break;
                    case CiDyNode.IntersectionType.culDeSac:
                        lane = new GameObject("CulDeSac" + i);
                        lane.transform.SetParent(waypointsHolder);
                        lane.transform.position = node.position;
                        for (int j = 0; j < node.leftRoutes.routes.Count; j++)
                        {
                            ExtractLaneConnectors(node.leftRoutes.routes[j], lane.transform, j, i, maxSpeed, laneWidth, -1, vehicleTypes);
                        }
                        for (int j = 0; j < node.rightRoutes.routes.Count; j++)
                        {
                            ExtractLaneConnectors(node.rightRoutes.routes[j], lane.transform, j, i, maxSpeed, laneWidth, -1, vehicleTypes);
                        }
                        break;
                    case CiDyNode.IntersectionType.tConnect:
                        lane = new GameObject("Intersection" + i);
                        lane.transform.SetParent(waypointsHolder);
                        lane.transform.position = node.position;
                        allGleyIntersections.Add(AddIntersection(intersectionHolder, intersectionType, greenLightTime, yellowLightTime, node.name, node.position));
                        for (int j = 0; j < node.intersectionRoutes.intersectionRoutes.Count; j++)
                        {
                            AddTrafficlights(new TrafficLight(node.intersectionRoutes.intersectionRoutes[j].light, allGleyIntersections.Count - 1, node.intersectionRoutes.intersectionRoutes[j].sequenceIndex));
                            ExtractLaneConnectors(node.intersectionRoutes.intersectionRoutes[j].route, lane.transform, j, node.intersectionRoutes.intersectionRoutes[j].sequenceIndex, maxSpeed, laneWidth, allGleyIntersections.Count - 1, vehicleTypes);
                        }
                        break;
                }

            }

            LinkAllWaypoints(waypointsHolder);

            LinkOvertakeLanes(waypointsHolder, waypointDistance);

            LinkConnectorsToRoadWaypoints();

            AssignIntersections(intersectionType);

            if (intersectionType == IntersectionType.TrafficLights)
            {
                AssignTrafficLights();
            }

            RemoveNonRequiredWaypoints();
        }


        private static void AddTrafficlights(TrafficLight trafficLight)
        {
            if (!trafficLights.Contains(trafficLight))
            {
                trafficLights.Add(trafficLight);
            }
        }


        private static void AssignTrafficLights()
        {
            for (int i = 0; i < allGleyIntersections.Count; i++)
            {
                TrafficLightsIntersectionSettings currentIntersection = (TrafficLightsIntersectionSettings)allGleyIntersections[i];

                if (currentIntersection.stopWaypoints != null)
                {
                    for (int j = 0; j < currentIntersection.stopWaypoints.Count; j++)
                    {
                        List<TrafficLight> currentRoadLights = trafficLights.Where(cond => cond.intersectionIndex == i && cond.roadIndex == j).ToList();
                        for (int k = 0; k < currentRoadLights.Count; k++)
                        {
                            Transform colorObject = currentRoadLights[k].lightObject.Find("RedLight");
                            if (colorObject != null)
                            {
                                EnableRenderer(colorObject.GetComponent<Renderer>());
                                currentIntersection.stopWaypoints[j].redLightObjects.Add(colorObject.gameObject);
                            }
                            colorObject = currentRoadLights[k].lightObject.Find("YellowLight");
                            if (colorObject != null)
                            {
                                EnableRenderer(colorObject.GetComponent<Renderer>());
                                currentIntersection.stopWaypoints[j].yellowLightObjects.Add(colorObject.gameObject);
                            }
                            colorObject = currentRoadLights[k].lightObject.Find("GreenLight");
                            if (colorObject != null)
                            {
                                EnableRenderer(colorObject.GetComponent<Renderer>());
                                currentIntersection.stopWaypoints[j].greenLightObjects.Add(colorObject.gameObject);
                            }
                        }
                    }
                }
            }
        }


        static void EnableRenderer(Renderer renderer)
        {
            if (renderer != null)
            {
                if (renderer.enabled == false)
                {
                    renderer.enabled = true;
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
                            AssignEnterWaypoints(currentIntersection.enterWaypoints, (WaypointSettings)allConnectors[i].GetComponent<WaypointSettings>().prev[0]);
                        }

                        if (connectors[i].exit)
                        {
                            if (currentIntersection.exitWaypoints == null)
                            {
                                currentIntersection.exitWaypoints = new List<WaypointSettings>();
                            }
                            WaypointSettings waypointToAdd = allConnectors[i].GetComponent<WaypointSettings>();
                            if (!currentIntersection.exitWaypoints.Contains(waypointToAdd))
                            {
                                currentIntersection.exitWaypoints.Add(waypointToAdd);
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


        private static GenericIntersectionSettings AddIntersection(Transform intersectionHolder, IntersectionType intersectionType, float greenLightTime, float yellowLightTime, string name, Vector3 position)
        {
            GameObject intersection = new GameObject(name);
            intersection.transform.SetParent(intersectionHolder);
            intersection.transform.position = position;
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

            return intersectionScript;
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


        private static void LinkConnectorsToRoadWaypoints()
        {
            for (int i = 0; i < allConnectors.Count; i++)
            {
                if (allConnectors[i].name.Contains(Gley.UrbanAssets.Internal.Constants.connectionEdgeName))
                {
                    bool found = false;
                    for (int j = 0; j < allWaypoints.Count; j++)
                    {
                        if (Vector3.Distance(allConnectors[i].position, allWaypoints[j].position) < 0.01f)
                        {
                            found = true;
                            WaypointSettings connectorScript = allConnectors[i].GetComponent<WaypointSettings>();
                            WaypointSettings waypointScript = allWaypoints[j].GetComponent<WaypointSettings>();

                            if (connectorScript.prev.Count == 0)
                            {
                                connectorScript.prev = waypointScript.prev;
                                waypointScript.prev[0].neighbors.Remove(waypointScript);
                                waypointScript.prev[0].neighbors.Add(connectorScript);
                                break;
                            }

                            if (connectorScript.neighbors.Count == 0)
                            {
                                connectorScript.neighbors = waypointScript.neighbors;
                                waypointScript.neighbors[0].prev.Add(connectorScript);
                                break;
                            }
                            found = false;
                        }

                    }
                    if (found == false)
                    {
                        Debug.Log("Not Found " + allConnectors[i].name, allConnectors[i]);
                    }
                }
            }
        }


        private static void ExtractLaneConnectors(CiDyRoute routeData, Transform node, int laneIndex, int roadIndex, int speedLimit, float laneWidth, int intersectionIndex, List<int> vehicleTypes)
        {
            Transform connectorsHolder = new GameObject("Connectors_" + laneIndex).transform;
            connectorsHolder.SetParent(node);
            List<Vector3> laneConnectors = routeData.waypoints;
            laneConnectors.AddRange(routeData.newRoutePoints);
            TrafficWaypointCreator waypointCreator = CreateInstance<TrafficWaypointCreator>().Initialize();

            for (int i = 0; i < laneConnectors.Count; i++)
            {
                Waypoint waypoint = new Waypoint();
                waypoint.listIndex = -1;
                if (i == 0 || i == laneConnectors.Count - 1)
                {
                    waypoint.listIndex = intersectionIndex;
                    waypoint.name = "Road_" + roadIndex + "-" + Gley.UrbanAssets.Internal.Constants.laneNamePrefix + laneIndex + "-" + Gley.UrbanAssets.Internal.Constants.connectionEdgeName + i;
                    if (i == 0)
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
                    waypoint.name = "Road_" + roadIndex + "-" + Gley.UrbanAssets.Internal.Constants.laneNamePrefix + laneIndex + "-" + Gley.UrbanAssets.Internal.Constants.connectionWaypointName + i;
                }
                waypoint.position = laneConnectors[i];
                waypoint.maxSpeed = speedLimit;
                waypoint.laneWidth = laneWidth;
                connectors.Add(waypoint);
                allConnectors.Add(waypointCreator.CreateWaypoint(connectorsHolder, waypoint.position, waypoint.name, vehicleTypes, waypoint.maxSpeed, waypoint.laneWidth));
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


        private static void LinkAllWaypoints(Transform holder)
        {
            for (int i = 0; i < holder.childCount; i++)
            {
                for (int j = 0; j < holder.GetChild(i).childCount; j++)
                {
                    Transform laneHolder = holder.GetChild(i).GetChild(j);
                    LinkWaypoints(laneHolder);
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


        static void ExtractLaneWaypoints(List<CiDyRoute> lanes, GameObject lanesHolder, string side, int roadIndex, int maxSpeed, float laneWidth, List<int> vehicleTypes)
        {
            if (lanes.Count > 0)
            {
                TrafficWaypointCreator waypointCreator = CreateInstance<TrafficWaypointCreator>().Initialize();
                for (int i = 0; i < lanes.Count; i++)
                {
                    GameObject lane = new GameObject("Lane" + i);
                    lane.name = "Lane_" + lanesHolder.transform.childCount + "_" + side;
                    lane.transform.SetParent(lanesHolder.transform);

                    List<Vector3> positions = lanes[i].waypoints;
                    positions.AddRange(lanes[i].newRoutePoints);
                    for (int k = 0; k < positions.Count; k++)
                    {
                        if (k > 0 && positions[k - 1] == positions[k])
                        {
                            continue;
                        }
                        Waypoint waypoint = new Waypoint();
                        waypoint.maxSpeed = maxSpeed;
                        waypoint.name = "Road_" + roadIndex + "-" + Gley.UrbanAssets.Internal.Constants.laneNamePrefix + i + "-" + Gley.UrbanAssets.Internal.Constants.waypointNamePrefix + k;
                        waypoint.position = positions[k];
                        waypoint.laneWidth = laneWidth;

                        allWaypoints.Add(waypointCreator.CreateWaypoint(lane.transform, waypoint.position, waypoint.name, vehicleTypes, waypoint.maxSpeed, waypoint.laneWidth));
                    }
                }
            }
        }


        private static void LinkOvertakeLanes(Transform holder, int waypointDistance)
        {
            for (int i = 0; i < holder.childCount; i++)
            {
                if (holder.GetChild(i).name.Contains("Road"))
                {
                    for (int j = 0; j < holder.GetChild(i).childCount; j++)
                    {
                        Transform firstLane = holder.GetChild(i).GetChild(j);
                        int laneToLink = j - 1;
                        if (laneToLink >= 0)
                        {
                            LinkLanes(firstLane, holder.GetChild(i).GetChild(laneToLink), waypointDistance);
                        }
                        laneToLink = j + 1;
                        if (laneToLink < holder.GetChild(i).childCount)
                        {
                            LinkLanes(firstLane, holder.GetChild(i).GetChild(laneToLink), waypointDistance);
                        }
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
    }
}
#endif
