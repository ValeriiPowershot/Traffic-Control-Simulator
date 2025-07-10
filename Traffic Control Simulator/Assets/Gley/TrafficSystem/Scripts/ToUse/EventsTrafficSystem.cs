using UnityEngine;

namespace Gley.TrafficSystem
{
    public class Events
    {
        #region Density
        /// <summary>
        /// Triggered every time a vehicle is activated inside the scene.
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        public delegate void VehicleAdded(int vehicleIndex);
        public static VehicleAdded onVehicleAdded;
        public static void TriggerVehicleAddedEvent(int vehicleIndex)
        {
            if (onVehicleAdded != null)
            {
                onVehicleAdded(vehicleIndex);
            }
        }


        /// <summary>
        /// Triggered every time a vehicle is deactivated inside the scene.
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        public delegate void VehicleRemoved(int vehicleIndex);
        public static VehicleRemoved onVehicleRemoved;
        public static void TriggerVehicleRemovedEvent(int vehicleIndex)
        {
            if (onVehicleRemoved != null)
            {
                onVehicleRemoved(vehicleIndex);
            }
        }
        #endregion


        /// <summary>
        /// Triggered when a vehicle crashes into another object. 
        /// </summary>
        public static event VehicleCrash onVehicleCrashed;
        public static void TriggerVehicleCrashEvent(int vehicleIndex, ObstacleTypes obstacleType, Collider other)
        {
            if (onVehicleCrashed != null)
            {
                onVehicleCrashed(vehicleIndex, obstacleType, other);
            }
        }


        /// <summary>
        /// Triggered every time a vehicle reaches the last point of its path.
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        public delegate void DestinationReached(int vehicleIndex);
        public static DestinationReached onDestinationReached;
        public static void TriggerDestinationReachedEvent(int vehicleIndex)
        {
            if (onDestinationReached != null)
            {
                onDestinationReached(vehicleIndex);
            }
        }


        /// <summary>
        /// Triggered every time a waypoint that has the Trigger Event option enabled is reached by a vehicle.
        /// </summary>
        /// <param name="vehicleIndex">The index of the vehicle that reached the waypoint.</param>
        /// <param name="waypointIndex">The waypoint index that triggered the event.</param>
        /// <param name="data">The data set on that waypoint by Trigger Event option.</param>
        public delegate void WaypointReached(int vehicleIndex, int waypointIndex, string data);
        public static WaypointReached onWaypointReached;
        public static void TriggerWaypointReachedEvent(int vehicleIndex, int waypointIndex, string data)
        {
            if (onWaypointReached != null)
            {
                onWaypointReached(vehicleIndex, waypointIndex, data);
            }
        }
    }
}
