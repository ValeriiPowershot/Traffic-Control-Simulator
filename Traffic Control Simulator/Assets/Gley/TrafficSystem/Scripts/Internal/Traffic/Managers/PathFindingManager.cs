using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    public class PathFindingManager : MonoBehaviour
    {
        private TrafficVehicles trafficVehicles;
        private WaypointManager waypointManager;
        private GridManager gridManager;
        private PathFindingData data;
        private PathFinding pathFinding;
        private List<PathFindingWaypoint> pathFindingWaypoints;
        private List<int> path;
        private PathFindingWaypoint startNode;
        private PathFindingWaypoint endNode;
        private bool debugPathFinding;


        public PathFindingManager Initialize(TrafficVehicles trafficVehicles, WaypointManager waypointManager, GridManager gridManager, bool debugPathFinding)
        {
            this.trafficVehicles = trafficVehicles;
            this.waypointManager = waypointManager;
            this.gridManager = gridManager;
            this.debugPathFinding = debugPathFinding;
            data = CurrentSceneData.GetSceneInstance().GetComponent<PathFindingData>();
            pathFinding = gameObject.GetComponent<PathFinding>();
            if (pathFinding == null)
            {
                pathFinding = gameObject.AddComponent<PathFinding>();
            }

            pathFindingWaypoints = data.allPathFindingWaypoints;
            pathFinding.Initialize(pathFindingWaypoints);
            return this;
        }


        internal void SetDestination(int vehicleIndex, Vector3 position)
        {
            if (data == null)
            {
                Debug.LogError("Path Finding Waypoints not found, Go to Tools -> Gley -> Traffic System -> Path Finding and enable Path Finding");
                return;
            }

            int currentVehicleWaypoint = waypointManager.GetTargetWaypointIndex(vehicleIndex);
            if (currentVehicleWaypoint < 0)
            {
                Debug.LogWarning($"Cannot find route to destination. Vehicle at index {vehicleIndex} is disabled");
                return;
            }

            int closestWaypointIndex = GetClosestWaypoint(position, trafficVehicles.GetVehicleType(vehicleIndex), pathFindingWaypoints);
            if (closestWaypointIndex < 0)
            {
                Debug.LogWarning("No waypoint found closer to destination");
            }
            List<int> path = pathFinding.FindPath(pathFindingWaypoints[currentVehicleWaypoint], pathFindingWaypoints[closestWaypointIndex], trafficVehicles.GetVehicleType(vehicleIndex));

            if (path != null)
            {
                waypointManager.SetAgentPath(vehicleIndex, new Queue<int>(path));
            }
            else
            {
                Debug.LogWarning($"No path found for vehicle {vehicleIndex} to {position}");
            }
        }


        internal List<int> GetPath(Vector3 startPosition, Vector3 endPosition, VehicleTypes vehicleType)
        {
            startNode = pathFindingWaypoints[GetClosestWaypoint(startPosition, vehicleType, pathFindingWaypoints)];
            endNode = pathFindingWaypoints[GetClosestWaypoint(endPosition, vehicleType, pathFindingWaypoints)];
            path = pathFinding.FindPath(startNode, endNode, vehicleType);
            if (path == null)
            {
                Debug.LogWarning($"No path found from {startPosition} to {endPosition}");
            }
            return path;
        }


        private int GetClosestWaypoint(Vector3 position, VehicleTypes type, List<PathFindingWaypoint> pathFindingWaypoints)
        {
#if GLEY_TRAFFIC_SYSTEM
            List<int> possibleWaypoints = gridManager.GetCell(position.x, position.z).waypointsInCell;
            if (possibleWaypoints.Count == 0)
            {
                List<Vector2Int> cells = gridManager.GetCellNeighbors(gridManager.GetCell(position.x, position.z), 1, true);
                foreach (Vector2Int cell in cells)
                {
                    possibleWaypoints.AddRange(gridManager.GetCell(cell).waypointsInCell);
                }
            }


            float distance = float.MaxValue;
            int waypointIndex = -1;
            foreach (int waypoint in possibleWaypoints)
            {
                if (pathFindingWaypoints[waypoint].allowedVehicles.Contains(type))
                {
                    float newDistance = Vector3.SqrMagnitude(waypointManager.GetWaypointPosition(waypoint) - position);
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        waypointIndex = waypoint;
                    }
                }
            }
            return waypointIndex;
#else
            return -1;
#endif

        }


        void OnDrawGizmos()
        {
            if (debugPathFinding)
            {
                if (path != null)
                {
                    foreach (int n in path)
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(CurrentSceneData.GetSceneInstance().GetComponent<PathFindingData>().allPathFindingWaypoints[n].worldPosition, Vector3.one);
                    }
                }
                if (startNode != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(startNode.worldPosition, Vector3.one * 5);
                }
                if (endNode != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(endNode.worldPosition, Vector3.one * 5);
                }
            }
        }
    }
}
