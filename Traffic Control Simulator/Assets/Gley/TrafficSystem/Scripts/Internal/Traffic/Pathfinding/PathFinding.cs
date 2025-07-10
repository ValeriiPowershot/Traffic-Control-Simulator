using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    public class PathFinding : MonoBehaviour
    {
        List<PathFindingWaypoint> allWaypoints;


        public PathFinding Initialize(List<PathFindingWaypoint> allWaypoints)
        {
            this.allWaypoints = allWaypoints;
            return this;
        }


        public List<int> FindPath(PathFindingWaypoint startNode, PathFindingWaypoint targetNode, VehicleTypes vehicleType)
        {
            Heap<PathFindingWaypoint> openSet = new Heap<PathFindingWaypoint>(allWaypoints.Count);
            HashSet<PathFindingWaypoint> closedSet = new HashSet<PathFindingWaypoint>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                PathFindingWaypoint currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                for (int i = 0; i < currentNode.neighbours.Count; i++)
                {
                    PathFindingWaypoint neighbour = allWaypoints[currentNode.neighbours[i]];
                    if (!neighbour.allowedVehicles.Contains(vehicleType) || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + currentNode.movementPenalty[i];
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode.listIndex;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            return null;
        }


        List<int> RetracePath(PathFindingWaypoint startNode, PathFindingWaypoint endNode)
        {
            List<int> path = new List<int>();
            PathFindingWaypoint currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.listIndex);
                currentNode = allWaypoints[currentNode.parent];
            }
            path.Reverse();
            return path;
        }


        int GetDistance(PathFindingWaypoint nodeA, PathFindingWaypoint nodeB)
        {
            float dstX = Mathf.Abs(nodeA.worldPosition.x - nodeB.worldPosition.x);
            float dstY = Mathf.Abs(nodeA.worldPosition.z - nodeB.worldPosition.z);

            if (dstX > dstY)
                return (int)(14 * dstY + 10 * (dstX - dstY));
            return (int)(14 * dstX + 10 * (dstY - dstX));
        }
    }
}
