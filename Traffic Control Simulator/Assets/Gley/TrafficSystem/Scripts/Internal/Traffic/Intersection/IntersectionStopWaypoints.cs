using System.Collections.Generic;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// This type of waypoint is used for intersection to stop before entering the intersection
    /// </summary>
    [System.Serializable]
    public class IntersectionStopWaypoints
    {
        public List<Waypoint> roadWaypoints = new List<Waypoint>();

        public IntersectionStopWaypoints(List<Waypoint> roadWaypoints)
        {
            this.roadWaypoints = roadWaypoints;
        }
    }
}