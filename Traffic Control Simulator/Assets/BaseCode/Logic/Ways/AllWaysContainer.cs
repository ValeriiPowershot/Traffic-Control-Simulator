using System;
using System.Collections.Generic;
using BaseCode.Logic.Roads.RoadTool;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.Ways
{
    public class AllWaysContainer : MonoBehaviour
    { 
        public List<WaypointGroup> levelAllWays = new();
        public CarDetector carDetectorPrefab;
        
        public List<WaypointContainer> GetWayGroupByIndex(int index)
        {
            return index >= 0 && index < levelAllWays.Count ? levelAllWays[index].waypoints : null;
        }
    }
    [Serializable]
    public class WaypointGroup
    {
        public List<WaypointContainer> waypoints = new();
    }
}
