using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    [System.Serializable]
    public class PathFindingWaypoint: IHeapItem<PathFindingWaypoint>
    {
        public string name;
        public int listIndex;
        public Vector3 worldPosition;
        public List<VehicleTypes> allowedVehicles;
        public int gCost;
        public int hCost;
        public int parent;
        public List<int> neighbours;
        public List<int> movementPenalty;

        public PathFindingWaypoint(string name, int listIndex, Vector3 worldPosition, int gCost, int hCost, int parent, List<int> neighbours, List<int> movementPenalty,List<VehicleTypes> allowedVehicles)
        {
            this.name = name;
            this.listIndex = listIndex;
            this.worldPosition = worldPosition;
            this.gCost = gCost;
            this.hCost = hCost;
            this.parent = parent;
            this.neighbours = neighbours;
            this.movementPenalty = movementPenalty;
            this.allowedVehicles = allowedVehicles;
        }


        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }


        public int HeapIndex { get; set; }


        public int CompareTo(PathFindingWaypoint nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }
    }
}
