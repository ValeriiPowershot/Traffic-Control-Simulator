using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Used to store intersection objects
    /// </summary>
    [System.Serializable]
    public class IntersectionStopWaypointsIndex
    {
        public List<int> roadWaypoints = new List<int>();
        public List<GameObject> redLightObjects;
        public List<GameObject> yellowLightObjects;
        public List<GameObject> greenLightObjects;
        public float greenLightTime;

        public List<int> pedestrianWaypoints = new List<int>();
        public List<int> directionWaypoints = new List<int>();

        public IntersectionStopWaypointsIndex(List<int> roadWaypoints, List<GameObject> redLightObjects, List<GameObject> yellowLightObjects, List<GameObject> greenLightObjects, float greenLightTime)
        {
            this.roadWaypoints = roadWaypoints;
            this.redLightObjects = redLightObjects;
            this.yellowLightObjects = yellowLightObjects;
            this.greenLightObjects = greenLightObjects;
            this.greenLightTime = greenLightTime;
        }
    }
}
