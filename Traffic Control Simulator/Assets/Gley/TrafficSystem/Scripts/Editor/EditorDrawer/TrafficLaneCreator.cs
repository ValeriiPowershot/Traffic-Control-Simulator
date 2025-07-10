using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;
using Constants = Gley.UrbanAssets.Internal.Constants;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficLaneCreator : Creator
    {
        protected TrafficWaypointCreator waypointCreator;
        private TrafficLaneData laneData;

        internal TrafficLaneCreator Initialize(TrafficLaneData laneData, TrafficWaypointCreator waypointCreator)
        {
            this.waypointCreator = waypointCreator;
            this.laneData = laneData;
            return this;
        }


        internal void SwitchLaneDirection(Road road, int laneNumber)
        {         
            SwitchWaypointDirection(road.lanes[laneNumber].laneEdges.inConnector, road.lanes[laneNumber].laneEdges.outConnector);
            road.SwitchDirection(laneNumber);
        }


        internal void GenerateWaypoints(Road road, int groundLayerMask)
        {
            ClearOldWaypointConnections(road.transform);

            GleyPrefabUtilities.ClearAllChildObjects(road.transform);

            List<Transform> helpingPoints = SplitBezierIntoPoints.CreatePoints(road);

            AddFinalWaypoints(road, helpingPoints, groundLayerMask);

            LinkNeighbors(road);

            for (int i = 0; i < road.lanes.Count; i++)
            {
                if (road.lanes[i].laneDirection == true)
                {
                    SwitchLaneDirection(road, i);
                }
            }

            DeleteHelpingPoints(helpingPoints);
            GleyPrefabUtilities.ApplyPrefabInstance(road.gameObject);
            laneData.TriggerModifiedEvent();
        }


        internal void LinkOtherLanes(Road road)
        {
            int nrOfLanes = road.transform.Find(UrbanAssets.Internal.Constants.lanesHolderName).childCount;
            float maxLength = road.waypointDistance * 9;

            for (int i = 0; i < nrOfLanes; i++)
            {
                ClearLinks(road, i);
            }
            for (int i = 0; i < nrOfLanes; i++)
            {
                LinkSameDirectionLanes(road, i, nrOfLanes, maxLength);
            }
        }


        internal void UnLinckOtherLanes(Road road)
        {
            int nrOfLanes = road.transform.Find(UrbanAssets.Internal.Constants.lanesHolderName).childCount;

            for (int i = 0; i < nrOfLanes; i++)
            {
                ClearLinks(road, i);
            }
        }


        private Transform AddLaneHolder(Transform parent, string laneName)
        {
            GameObject lane = new GameObject(laneName);
            lane.transform.SetParent(parent, false);
            return lane.transform;
        }


        private bool PositionIsValid(List<Transform> helpingPoints, Vector3 waypointPosition, float limit)
        {
            for (int i = 0; i < helpingPoints.Count; i++)
            {
                if (Vector3.Distance(helpingPoints[i].position, waypointPosition) < limit)
                {
                    return false;
                }
            }
            return true;
        }


        private Vector3 PutWaypointOnRoad(Vector3 waypointPosition, Vector3 perpendicular, int groundLayermask)
        {
            if (GleyPrefabUtilities.EditingInsidePrefab())
            {
                if (GleyPrefabUtilities.GetScenePrefabRoot().scene.GetPhysicsScene().Raycast(waypointPosition + 5 * perpendicular, -perpendicular, out RaycastHit hitInfo, Mathf.Infinity, groundLayermask))
                {
                    return hitInfo.point;
                }
            }
            else
            {
                if (Physics.Raycast(waypointPosition + 5 * perpendicular, -perpendicular, out RaycastHit hitInfo, Mathf.Infinity, groundLayermask))
                {
                    return hitInfo.point;
                }
            }
            return waypointPosition;
        }


        private void DeleteHelpingPoints(List<Transform> helpingPoints)
        {
            DestroyImmediate(helpingPoints[0].transform.parent.gameObject);
        }


        private void LinkNeighbors(Road road)
        {
            for (int i = 0; i < road.nrOfLanes; i++)
            {
                Transform laneHolder = road.transform.Find(Constants.lanesHolderName).Find(Constants.laneNamePrefix + i);
                WaypointSettingsBase previousWaypoint = laneHolder.GetChild(0).GetComponent<WaypointSettingsBase>();
                for (int j = 1; j < laneHolder.childCount; j++)
                {
                    string waypointName = laneHolder.GetChild(j).name;
                    WaypointSettingsBase waypointScript = laneHolder.GetChild(j).GetComponent<WaypointSettingsBase>();
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
                if (road.path.IsClosed)
                {
                    WaypointSettingsBase first = laneHolder.GetChild(0).GetComponent<WaypointSettingsBase>();
                    WaypointSettingsBase last = laneHolder.GetChild(laneHolder.childCount - 1).GetComponent<WaypointSettingsBase>();
                    last.neighbors.Add(first);
                    first.prev.Add(last);
                }
            }
        }


        private void AddFinalWaypoints(Road road, List<Transform> helpingPoints, int roadLayerMask)
        {
            float startPosition;
            if (road.nrOfLanes % 2 == 0)
            {
                startPosition = -road.laneWidth / 2;
            }
            else
            {
                startPosition = 0;
            }

            int laneModifier = 0;

            Transform lanesHolder = new GameObject(Constants.lanesHolderName).transform;
            lanesHolder.SetParent(road.transform, false);

            for (int i = 0; i < road.nrOfLanes; i++)
            {
                Transform laneHolder = AddLaneHolder(lanesHolder, Constants.laneNamePrefix + i);
                if (i % 2 == 0)
                {
                    laneModifier = -laneModifier;
                }
                else
                {
                    laneModifier = Mathf.Abs(laneModifier) + 1;
                }

                List<Transform> finalPoints = new List<Transform>();
                string waypointName;
                Vector3 waypointPosition;

                for (int j = 0; j < helpingPoints.Count - 1; j++)
                {
                    waypointPosition = helpingPoints[j].position + (startPosition + laneModifier * road.laneWidth) * helpingPoints[j].right;
                    if (PositionIsValid(helpingPoints, waypointPosition, Mathf.Abs(startPosition + laneModifier * road.laneWidth) - 0.1f))
                    {
                        waypointPosition = PutWaypointOnRoad(waypointPosition, helpingPoints[j].up, roadLayerMask);
                        if (PositionIsValid(finalPoints, waypointPosition, road.waypointDistance))
                        {
                            waypointName = road.name + "-" + Constants.laneNamePrefix + i + "-" + Constants.waypointNamePrefix + j;
                            finalPoints.Add(waypointCreator.CreateWaypoint(laneHolder, waypointPosition, waypointName, road.GetAllowedCars(i), road.lanes[i].laneSpeed, road.laneWidth));
                        }
                    }
                }

                //add last point from the list
                if (!road.path.IsClosed)
                {
                    waypointPosition = helpingPoints[helpingPoints.Count - 1].position + (startPosition + laneModifier * road.laneWidth) * helpingPoints[helpingPoints.Count - 1].right;
                    waypointPosition = PutWaypointOnRoad(waypointPosition, helpingPoints[helpingPoints.Count - 1].up, roadLayerMask);
                    waypointName = road.name + "-" + Constants.laneNamePrefix + i + "-" + Constants.waypointNamePrefix + helpingPoints.Count;
                    finalPoints.Add(waypointCreator.CreateWaypoint(laneHolder, waypointPosition, waypointName, road.GetAllowedCars(i), road.lanes[i].laneSpeed, road.laneWidth));
                }

                road.AddLaneConnector(finalPoints[0].GetComponent<WaypointSettingsBase>(), finalPoints[finalPoints.Count - 1].GetComponent<WaypointSettingsBase>(), i);
            }
        }


        private void ClearOldWaypointConnections(Transform holder)
        {
            WaypointSettingsBase[] allWaypoints = holder.GetComponentsInChildren<WaypointSettingsBase>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                WaypointSettingsBase waypoint = allWaypoints[i];
                for (int j = 0; j < waypoint.neighbors.Count; j++)
                {
                    if (waypoint.neighbors[j] != null)
                    {
                        waypoint.neighbors[j].prev.Remove(waypoint);
                    }
                    else
                    {
                        Debug.LogError(waypoint.name + " has null neighbors", waypoint);
                    }
                }
                for (int j = 0; j < waypoint.prev.Count; j++)
                {
                    if (waypoint.prev[j] != null)
                    {
                        waypoint.prev[j].neighbors.Remove(waypoint);
                    }
                    else
                    {
                        Debug.LogError(waypoint.name + " has null prevs", waypoint);
                    }
                }
            }
        }


        private void SwitchWaypointDirection(WaypointSettingsBase startWaypoint, WaypointSettingsBase endWaypoint)
        {
            WaypointSettingsBase currentWaypoint = startWaypoint;
            bool continueSwitching = true;
            while (continueSwitching)
            {
                if (currentWaypoint == null)
                {
                    break;
                }
                if (currentWaypoint.neighbors == null)
                {
                    break;
                }

                if (currentWaypoint == endWaypoint)
                {
                    continueSwitching = false;
                }

                for (int i = currentWaypoint.neighbors.Count - 1; i >= 1; i--)
                {
                    currentWaypoint.neighbors[i].prev.Remove(currentWaypoint);
                    currentWaypoint.neighbors.RemoveAt(i);
                }

                for (int i = currentWaypoint.prev.Count - 1; i >= 1; i--)
                {
                    currentWaypoint.prev[i].neighbors.Remove(currentWaypoint);
                    currentWaypoint.prev.RemoveAt(i);
                }

                List<WaypointSettingsBase> aux = currentWaypoint.neighbors;
                currentWaypoint.neighbors = currentWaypoint.prev;
                currentWaypoint.prev = aux;
                if (currentWaypoint.prev.Count > 0)
                {
                    currentWaypoint = currentWaypoint.prev[0];
                }
            }
        }


        private void ClearLinks(Road road, int laneIndex)
        {
            Transform laneToLink = road.transform.Find(UrbanAssets.Internal.Constants.lanesHolderName).Find(UrbanAssets.Internal.Constants.laneNamePrefix + laneIndex);
            for (int i = 0; i < laneToLink.transform.childCount; i++)
            {
                laneToLink.transform.GetChild(i).GetComponent<WaypointSettings>().otherLanes = new List<WaypointSettings>();
            }
        }


        private void LinkSameDirectionLanes(Road road, int laneIndex, int nrOfLanes, float maxLength)
        {
            Transform currentLane = road.transform.Find(UrbanAssets.Internal.Constants.lanesHolderName).Find(UrbanAssets.Internal.Constants.laneNamePrefix + laneIndex);
            int[] neighbors = GetNeighbors(laneIndex, nrOfLanes);
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] != -1)
                {
                    if (road.lanes[neighbors[i]].laneDirection == road.lanes[laneIndex].laneDirection)
                    {
                        Transform otherLane = road.transform.Find(UrbanAssets.Internal.Constants.lanesHolderName).Find(UrbanAssets.Internal.Constants.laneNamePrefix + neighbors[i]);
                        int currentLaneCount = 0;
                        int otherLaneCount = 0;
                        if (currentLane.GetChild(0).name.Contains(UrbanAssets.Internal.Constants.outWaypointEnding))
                        {
                            currentLaneCount = currentLane.childCount - 1;
                            otherLaneCount = otherLane.childCount - 1;
                        }
                        int startIndex = 0;
                        int currentIndex = road.otherLaneLinkDistance;
                        int steps = 10;
                        for (int j = 0; j < currentLane.childCount; j++)
                        {
                            WaypointSettings currentLaneWaypoint = currentLane.GetChild(Mathf.Abs(currentLaneCount - j)).GetComponent<WaypointSettings>();

                            bool connected = false;
                            while (currentIndex < startIndex + steps && currentIndex < otherLane.childCount)
                            {
                                WaypointSettings otherLaneWaypoint = otherLane.GetChild(Mathf.Abs(otherLaneCount - currentIndex)).GetComponent<WaypointSettings>();
                                if (ConnectionIsValid(currentLaneWaypoint, otherLaneWaypoint, maxLength))
                                {
                                    currentLaneWaypoint.otherLanes.Add(otherLaneWaypoint);
                                    connected = true;
                                    break;
                                }
                                currentIndex++;
                            }
                            if (connected)
                            {
                                startIndex = j + 1 + road.otherLaneLinkDistance;
                                currentIndex = startIndex;
                            }
                            else
                            {
                                currentIndex = startIndex;
                            }
                        }
                    }
                }
            }
        }


        private bool ConnectionIsValid(WaypointSettings currentLaneWaypoint, WaypointSettings otherLaneWaypoint, float maxLength)
        {
            if (currentLaneWaypoint.neighbors.Count == 0 || otherLaneWaypoint.neighbors.Count == 0 || currentLaneWaypoint.prev.Count == 0)
            {
                return false;
            }

            float angle = Vector3.Angle(currentLaneWaypoint.neighbors[0].transform.position - currentLaneWaypoint.transform.position, otherLaneWaypoint.transform.position - currentLaneWaypoint.transform.position);
            if (angle > 45)
            {
                return false;
            }

            angle = Vector3.Angle(currentLaneWaypoint.transform.position - otherLaneWaypoint.transform.position, otherLaneWaypoint.neighbors[0].transform.position - otherLaneWaypoint.transform.position);
            if (angle < 135)
            {
                return false;
            }

            angle = Vector3.Angle(currentLaneWaypoint.prev[0].transform.position - currentLaneWaypoint.transform.position, otherLaneWaypoint.transform.position - currentLaneWaypoint.transform.position);
            if (angle < 115)
            {
                return false;
            }

            if (Vector3.Magnitude(currentLaneWaypoint.transform.position - otherLaneWaypoint.transform.position) > maxLength)
            {
                return false;
            }

            return true;
        }


        private int[] GetNeighbors(int laneIndex, int nrOfLanes)
        {
            int[] result = new int[2];
            if (laneIndex == 0)
            {
                result[0] = 1;
                result[1] = 2;
            }
            if (laneIndex == 1)
            {
                result[0] = 0;
                result[1] = 3;
            }

            if (laneIndex >= 2)
            {
                result[0] = laneIndex - 2;
                result[1] = laneIndex + 2;
            }
            if (result[1] >= nrOfLanes)
            {
                result[1] = -1;
            }
            return result;
        }
    }
}
