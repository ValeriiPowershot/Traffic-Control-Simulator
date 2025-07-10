using Gley.UrbanAssets.Internal;
using System.Collections.Generic;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Base class for all intersections
    /// </summary>
    [System.Serializable]
    public abstract class GenericIntersection : IIntersection
    {
        public string name;
        protected List<int> carsInIntersection;
        public List<int> exitWaypoints;


        #region InterfactImplementation
        public abstract bool IsPathFree(int waypointIndex);

        public void VehicleEnter(int vehicleIndex)
        {
            carsInIntersection.Add(vehicleIndex);
        }


        public void VehicleLeft(int vehicleIndex)
        {
            carsInIntersection.Remove(vehicleIndex);
        }
#if GLEY_PEDESTRIAN_SYSTEM
#if GLEY_TRAFFIC_SYSTEM
         public abstract void PedestrianPassed(int agentIndex);
#endif
#endif

#endregion


        internal abstract void UpdateIntersection(float realtimeSinceStartup);
        internal abstract List<IntersectionStopWaypointsIndex> GetWaypoints();

        internal abstract void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour);
        internal abstract void SetGreenRoad(int roadIndex, bool doNotChangeAgain);


#if GLEY_PEDESTRIAN_SYSTEM
#if GLEY_TRAFFIC_SYSTEM
        internal abstract List<int> GetPedStopWaypoint();
        internal abstract void DestroyIntersection();
#endif
#endif

        internal virtual void Initialize(WaypointManagerBase waypointManager, float greenLightTime, float yellowLightTime)
        {
            carsInIntersection = new List<int>();

            for (int i = 0; i < exitWaypoints.Count; i++)
            {
                waypointManager.GetWaypoint<Waypoint>(exitWaypoints[i]).SetIntersection(this, false, false, false, true);
            }
        }


        internal void RemoveCar(int index)
        {
            VehicleLeft(index);
        }


        internal void ResetIntersection()
        {
            carsInIntersection = new List<int>();
        }
    }
}