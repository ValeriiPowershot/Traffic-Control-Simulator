#if UNITY_EDITOR
#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Internal;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Stores stop waypoints properties
    /// </summary>
    [System.Serializable]
    public class IntersectionStopWaypointsSettings
    {
        public List<WaypointSettings> roadWaypoints = new List<WaypointSettings>();
        public List<GameObject> redLightObjects = new List<GameObject>();
        public List<GameObject> yellowLightObjects = new List<GameObject>();
        public List<GameObject> greenLightObjects = new List<GameObject>();
        public float greenLightTime;
        public bool draw = true;

#if GLEY_PEDESTRIAN_SYSTEM
        public List<PedestrianWaypointSettings> pedestrianWaypoints = new List<PedestrianWaypointSettings>();
        public List<PedestrianWaypointSettings> directionWaypoints = new List<PedestrianWaypointSettings>();
#endif
    }
}
#endif