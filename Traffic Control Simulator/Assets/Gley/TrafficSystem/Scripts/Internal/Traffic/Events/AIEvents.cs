using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    public static class AIEvents
    {
        /// <summary>
        /// Triggered when the driving action of a vehicle changed
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="action">new action</param>
        /// <param name="actionValue">action time</param>
        public delegate void ChangeDrivingState(int vehicleIndex, TrafficSystem.DriveActions action, float actionValue);
        public static event ChangeDrivingState onChangeDrivingState;
        public static void TriggetChangeDrivingStateEvent(int vehicleIndex, TrafficSystem.DriveActions action, float actionValue)
        {
            if (onChangeDrivingState != null)
            {
                onChangeDrivingState(vehicleIndex, action, actionValue);
            }
        }


        /// <summary>
        /// Triggered when a vehicle changes his state to notify other vehicles about this 
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="collider">collider of the vehicle</param>
        /// <param name="action">new action</param>
        public delegate void NotifyVehicles(int vehicleIndex, Collider collider);
        public static NotifyVehicles onNotifyVehicles;
        public static void TriggerNotifyVehiclesEvent(int vehicleIndex, Collider collider)
        {
            if (onNotifyVehicles != null)
            {
                onNotifyVehicles(vehicleIndex, collider);
            }
        }


        /// <summary>
        /// Triggered when a vehicle changed waypoint
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="targetWaypointPosition">new waypoint position</param>
        /// <param name="maxSpeed">max possible speed</param>
        /// <param name="blinkType">blinking required</param>
        public delegate void ChangeDestination(int vehicleIndex);
        public static ChangeDestination onChangeDestination;
        public static void TriggerChangeDestinationEvent(int vehicleIndex)
        {
            if (onChangeDestination != null)
            {
                onChangeDestination(vehicleIndex);
            }
        }
    }
}