#if UNITY_EDITOR
using System.Collections.Generic;
namespace Gley.TrafficSystem.Internal
{
    public static class IntersectionExtensionMethods
    {
        /// <summary>
        /// Converts editor priority intersection to runtime priority intersection
        /// </summary>
        /// <param name="priorityIntersection"></param>
        /// <param name="allWaypoints"></param>
        /// <returns></returns>
        public static PriorityIntersection ToPlayModeIntersection(this PriorityIntersectionSettings priorityIntersection, List<WaypointSettings> allWaypoints)
        {
            return new PriorityIntersection(priorityIntersection.name, priorityIntersection.enterWaypoints.ToPlayIndex(allWaypoints), priorityIntersection.exitWaypoints.ToListIndex(allWaypoints));
        }


        public static PriorityCrossing ToPlayModeIntersection(this PriorityCrossingSettings priorityIntersection, List<WaypointSettings> allWaypoints)
        {
            return new PriorityCrossing(priorityIntersection.name, priorityIntersection.enterWaypoints.ToPlayIndex(allWaypoints), priorityIntersection.exitWaypoints.ToListIndex(allWaypoints));
        }

        /// <summary>
        /// Converts editor traffic lights intersection to runtime traffic lights intersection
        /// </summary>
        /// <param name="trafficLightsIntersection"></param>
        /// <param name="allWaypoints"></param>
        /// <returns></returns>
        public static TrafficLightsIntersection ToPlayModeIntersection(this TrafficLightsIntersectionSettings trafficLightsIntersection, List<WaypointSettings> allWaypoints)
        {
            return new TrafficLightsIntersection(trafficLightsIntersection.name, trafficLightsIntersection.stopWaypoints.ToPlayIndex(allWaypoints), trafficLightsIntersection.greenLightTime, trafficLightsIntersection.yellowLightTime, trafficLightsIntersection.exitWaypoints.ToListIndex(allWaypoints));
        }

        public static TrafficLightsCrossing ToPlayModeIntersection(this TrafficLightsCrossingSettings trafficLightsIntersection, List<WaypointSettings> allWaypoints)
        {
            return new TrafficLightsCrossing(trafficLightsIntersection.name, trafficLightsIntersection.stopWaypoints.ToPlayIndex(allWaypoints), trafficLightsIntersection.greenLightTime, trafficLightsIntersection.yellowLightTime, trafficLightsIntersection.redLightTime, trafficLightsIntersection.exitWaypoints.ToListIndex(allWaypoints));
        }
    }
}
#endif
