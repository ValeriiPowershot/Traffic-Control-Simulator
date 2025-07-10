using System.Collections.Generic;

namespace Gley.UrbanAssets.Internal
{
    /// <summary>
    /// This type of waypoint can spawn a vehicle, 
    /// used to store waypoint properties 
    /// </summary>
    [System.Serializable]
    public struct SpawnWaypoint
    {
        public int waypointIndex;
        public List<int> allowedAgent;
        public int priority;
        public SpawnWaypoint(int waypointIndex, List<int> allowedVehicles, int priority)
        {
            this.waypointIndex = waypointIndex;
            this.allowedAgent = allowedVehicles;
            this.priority = priority;
        }
    }
}
