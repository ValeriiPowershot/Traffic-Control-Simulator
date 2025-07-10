using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    public static class VehicleEvents
    {
        public delegate void ObjectInTrigger(int vehicleIndex, ObstacleTypes obstacleType, Collider other);
        public static event ObjectInTrigger onObjectInTrigger;
        public static void TriggeerObjectInTriggerEvent(int vehicleIndex, ObstacleTypes obstacleType, Collider other)
        {
            if (onObjectInTrigger != null)
            {
                onObjectInTrigger(vehicleIndex, obstacleType, other);
            }
        }


        public delegate void TriggerCleared(int vehicleIndex);
        public static event TriggerCleared onTriggerCleared;
        public static void TriggerTriggerClearedEvent(int vehicleIndex)
        {
            if (onTriggerCleared != null)
            {
                onTriggerCleared(vehicleIndex);
            }
        }
    }
}
