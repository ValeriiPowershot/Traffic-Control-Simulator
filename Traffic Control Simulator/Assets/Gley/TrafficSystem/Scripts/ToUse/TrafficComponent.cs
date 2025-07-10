using UnityEngine;

namespace Gley.TrafficSystem
{
    /// <summary>
    /// Add this component on a GameObject inside your scene to enable Traffic System
    /// </summary>
    [HelpURL("https://gley.gitbook.io/mobile-traffic-system/setup-guide/initializing-asset")]
    public class TrafficComponent : MonoBehaviour
    {
        [Header("Required Settings")]
        [Tooltip("Player is used to instantiate vehicles out of view")]
        public Transform player;
        [Tooltip("Max number of possible vehicles. Cannot be increased during game-play")]
        public int nrOfVehicles = 1;
        [Tooltip("List of different vehicles (Right Click->Create->Traffic System->Vehicle Pool)")]
        public VehiclePool vehiclePool;

        [Header("Optional Settings")]

        [Header("Spawning")]
        [Tooltip("Square located at this distance from the player are actively update. Ex: if set is to 2 -> intersections will update on a 2 square distance from the player")]
        public int activeSquareLevels = 1;
        [Tooltip("Minimum distance from the player where a vehicle can be instantiated. (If -1 the system will automatically determine this value)")]
        public float minDistanceToAdd = -1;
        [Tooltip("Distance from the player where a vehicle can be removed. (If -1 the system will automatically determine this value)")]
        public float distanceToRemove = -1;

        [Header("Intersection")]
        [Tooltip("How long yellow light is on. (If -1 the value from the intersection component will be used)")]
        public float yellowLightTime = -1;
        [Tooltip("How long green light is on. (If -1 the value from the intersection component will be used)")]
        public float greenLightTime = -1;

        [Header("Density")]
        [Tooltip("Nr of vehicles instantiated around the player from the start. Set it to something < nrOfVehicles for low density right at the start. (If -1 all vehicles will be instantiated from the beginning)")]
        public int initialActiveVehicles = -1;
        [Tooltip("Set high priority on roads for higher traffic density(ex highways). See priority setup")]
        public bool useWaypointPriority = false;


        [Header("Lights")]
        [Tooltip("Set the initial state of the car lights")]
        public bool lightsOn = false;

        [Header("Waypoints")]
        [Tooltip("Area to disable from the start if cars are not allowed to spawn there")]
        public Area disableWaypointsArea = default;

        void Start()
        {
            TrafficOptions options = new TrafficOptions()
            {
                activeSquaresLevel = activeSquareLevels,
                disableWaypointsArea = disableWaypointsArea,
                distanceToRemove = distanceToRemove,
                greenLightTime = greenLightTime,
                initialDensity = initialActiveVehicles,
                lightsOn = lightsOn,
                minDistanceToAdd = minDistanceToAdd,
                useWaypointPriority = useWaypointPriority,
                yellowLightTime = yellowLightTime,
            };

           
            API.Initialize(player, nrOfVehicles, vehiclePool, options);
        }
    }
}
