using Gley.TrafficSystem.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem
{
    public delegate void TrafficLightsBehaviour(TrafficLightsColor currentRoadColor, List<GameObject> redLightObjects, List<GameObject> yellowLightObjects, List<GameObject> greenLightObjects, string name);

    public delegate int SpawnWaypointSelector(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes vehicleType, bool useWaypointPriority);

    public delegate void ModifyTriggerSize(float currentSpeed, BoxCollider frontCollider, float maxSpeed, float minTriggerLength, float maxTriggerLength);

    public delegate void PlayerInTrigger(int vehicleIndex, Collider player);

    public delegate void DynamicObstacleInTrigger(int vehicleIndex, Collider obstacle);

    public delegate void BuildingInTrigger(int vehicleIndex, Collider building);

    public delegate void VehicleCrash(int vehicleIndex, ObstacleTypes obstacleType, Collider other);


    public class Delegates
    {
        /// <summary>
        /// Controls the behavior of the lights inside a traffic light intersection.
        /// </summary>
        /// <param name="trafficLightsBehaviourDelegate">new delegate method</param>
        public static void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviourDelegate)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.IntersectionManager?.SetTrafficLightsBehaviour(trafficLightsBehaviourDelegate);
#endif
        }


        /// <summary>
        /// Controls the selection of a free waypoint to instantiate a new vehicle on.
        /// </summary>
        /// <param name="spawnWaypointSelectorDelegate">new delegate method</param>
        public static void SetSpawnWaypointSelector(SpawnWaypointSelector spawnWaypointSelectorDelegate)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.WaypointManager?.SetSpawnWaypointSelector(spawnWaypointSelectorDelegate);
#endif
        }


        /// <summary>
        /// Controls the dimension of the front trigger based on the vehicle's speed.
        /// </summary>
        /// <param name="modifyTriggerSizeDelegate">new delegate method</param>
        /// <param name="vehicleIndex">vehicle index to apply (-1 apply to all)</param>
        public static void SetModifyTriggerSize(ModifyTriggerSize modifyTriggerSizeDelegate, int vehicleIndex = -1)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.TrafficVehicles?.ModifyTriggerSize(vehicleIndex, modifyTriggerSizeDelegate);
#endif
        }


        /// <summary>
        /// Controls how the traffic vehicle reacts when a player car is in trigger.
        /// </summary>
        /// <param name="playerInTriggerDelegate">new delegate method</param>
        public static void SetPlayerInTrigger(PlayerInTrigger playerInTriggerDelegate)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DrivingAI?.SetPlayerInTriggerDelegate(playerInTriggerDelegate);
#endif
        }


        /// <summary>
        /// Controls how the traffic vehicle reacts when a dynamic obstacle is in the trigger.
        /// </summary>
        /// <param name="dynamicObstacleInTriggerDelegate">new delegate method</param>
        public static void SetDynamicObstacleInTrigger(DynamicObstacleInTrigger dynamicObstacleInTriggerDelegate)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DrivingAI?.SetDynamicObstacleInTriggerDelegate(dynamicObstacleInTriggerDelegate);
#endif
        }


        /// <summary>
        /// Controls how the traffic vehicle reacts when a building is in the trigger.
        /// </summary>
        /// <param name="buildingInTriggerDelegate">new delegate method</param>
        public static void SetBuildingInTrigger(BuildingInTrigger buildingInTriggerDelegate)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DrivingAI?.SetBuildingInTriggerDelegate(buildingInTriggerDelegate);
#endif
        }


        /// <summary>
        /// Controls how the traffic vehicle reacts when it crashes into something.
        /// </summary>
        /// <param name="vehicleCrashDelegate">new delegate method</param>
        public static void SetVehicleCrash(VehicleCrash vehicleCrashDelegate)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DrivingAI?.SetVehicleCrashDelegate(vehicleCrashDelegate);
#endif
        }

    }
}