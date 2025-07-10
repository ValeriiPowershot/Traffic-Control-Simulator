namespace Gley.TrafficSystem.Internal
{
    public static class WaypointEvents
    {
        /// <summary>
        /// Triggered to change the stop value of the waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        public delegate void TrafficLightChanged(int waypointIndex, bool stop);
        public static event TrafficLightChanged onTrafficLightChanged;
        public static void TriggerTrafficLightChangedEvent(int waypointIndex, bool stop)
        {
            if (onTrafficLightChanged != null)
            {
                onTrafficLightChanged(waypointIndex, stop);
            }
        }


        /// <summary>
        /// Triggered to notify vehicle about stop state and give way state of the waypoint
        /// </summary>
        /// <param name="vehicleIndex">vehicle index</param>
        /// <param name="stopState">stop in point needed</param>
        /// <param name="giveWayState">give way needed</param>
        public delegate void StopStateChanged(int vehicleIndex, bool stopState);
        public static event StopStateChanged onStopStateChanged;
        public static void TriggerStopStateChangedEvent(int vehicleIndex, bool stopState)
        {
            if (onStopStateChanged != null)
            {
                onStopStateChanged(vehicleIndex, stopState);
            }
        }

        public delegate void GiveWayStateChanged(int vehicleIndex,bool giveWayState);
        public static event GiveWayStateChanged onGiveWayStateChanged;
        public static void TriggerGiveWayStateChangedEvent(int vehicleIndex, bool giveWayState)
        {
            if (onGiveWayStateChanged != null)
            {
                onGiveWayStateChanged(vehicleIndex, giveWayState);
            }
        }
    }
}
