#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem;
#endif
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    [System.Serializable]
    public class PriorityCrossing : GenericIntersection
    {
        public List<IntersectionStopWaypointsIndex> enterWaypoints;

        private Vector3 position;
        private Color waypointColor;
        private bool stopCars;
        private bool stopUpdate;


        /// <summary>
        /// Constructor used for conversion from editor intersection type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enterWaypoints"></param>
        /// <param name="exitWaypoints"></param>
        public PriorityCrossing(string name, List<IntersectionStopWaypointsIndex> enterWaypoints, List<int> exitWaypoints)
        {
            this.name = name;
            this.enterWaypoints = enterWaypoints;
            this.exitWaypoints = exitWaypoints;
        }


        /// <summary>
        /// Assigns corresponding waypoints to work with this intersection
        /// </summary>
        /// <param name="waypointManager"></param>
        /// <param name="greenLightTime"></param>
        /// <param name="yellowLightTime"></param>
        internal override void Initialize(WaypointManagerBase waypointManager, float greenLightTime, float yellowLightTime)
        {
            base.Initialize(waypointManager, greenLightTime, yellowLightTime);
            int nr = 0;
            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                for (int j = 0; j < enterWaypoints[i].roadWaypoints.Count; j++)
                {
                    Waypoint waypoint = waypointManager.GetWaypoint<Waypoint>(enterWaypoints[i].roadWaypoints[j]);
                    waypoint.SetIntersection(this, true, false, true, false);
                    position += waypoint.position;
                    nr++;
                }
            }
            position = position / nr;
            waypointColor = Color.green;

#if GLEY_PEDESTRIAN_SYSTEM
#if GLEY_TRAFFIC_SYSTEM
            InitializePedestrianWaypoints();
#endif
#endif
        }


        internal Vector3 GetPosition()
        {
            return position;
        }


        internal List<int> GetWaypointsToCkeck()
        {
            return enterWaypoints[0].roadWaypoints;
        }


        internal Color GetWaypointColors()
        {
            return waypointColor;
        }


        internal override List<IntersectionStopWaypointsIndex> GetWaypoints()
        {
            return enterWaypoints;
        }


        /// <summary>
        /// Check if the intersection road is free and update intersection priority
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        public override bool IsPathFree(int waypointIndex)
        {
#if GLEY_PEDESTRIAN_SYSTEM
#if GLEY_TRAFFIC_SYSTEM
            if (!stopUpdate)
            {
                stopCars = IsPedestrianCrossing(0);
            }
#endif
#endif
            if (stopCars)
            {
                if(waypointColor != Color.red)
                {
                    waypointColor = Color.red;
#if GLEY_PEDESTRIAN_SYSTEM
                    CheckColor();
#endif
                }
                return false;
            }
            if (waypointColor != Color.green)
            {
                waypointColor = Color.green;
            }
       
            return true;
        }


        internal void SetPriorityCrossingState(string crossingName, bool stop, bool stopUpdate)
        {
            if (crossingName == name)
            {
                stopCars = stop;
                this.stopUpdate = stopUpdate;
                IsPathFree(0);
            }
        }


        internal bool GetPriorityCrossingState()
        {
            return waypointColor == Color.red;
        }


        internal override void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
        {

        }


        internal override void SetGreenRoad(int roadIndex, bool doNotChangeAgain)
        {

        }


        internal override void UpdateIntersection(float realtimeSinceStartup)
        {

        }


        internal int GetCarsInIntersection()
        {
            return carsInIntersection.Count;
        }

#if GLEY_TRAFFIC_SYSTEM
#if GLEY_PEDESTRIAN_SYSTEM
        internal class PedestrianCrossing
        {
            public int pedestrianIndex;
            public bool crossing;
            public int road;

            public PedestrianCrossing(int pedestrianIndex, int road)
            {
                this.pedestrianIndex = pedestrianIndex;
                crossing = false;
                this.road = road;
            }
        }

        private List<PedestrianCrossing> pedestriansCrossing;


        internal List<PedestrianCrossing> GetPedestriansCrossing()
        {
            return pedestriansCrossing;
        }


        private void InitializePedestrianWaypoints()
        {
            CurrentSceneData currentSceneData = CurrentSceneData.GetSceneInstance();
            currentSceneData.AssignIntersections(enterWaypoints, this);
            pedestriansCrossing = new List<PedestrianCrossing>();
            PedestrianEvents.onStreetCrossing += PedestrianWantsToCross;
        }

        private void MakePedestriansCross(int road)
        {
            for (int i = 0; i < enterWaypoints[road].pedestrianWaypoints.Count; i++)
            {
                PedestrianEvents.TriggerStopStateChangedEvent(enterWaypoints[road].pedestrianWaypoints[i], false);
            }
        }

        public void AddPedestrianWaypoints(int road, List<int> pedestrianWaypoints)
        {
            enterWaypoints[road].pedestrianWaypoints = pedestrianWaypoints;
        }


        public void AddDirectionWaypoints(int road, List<int> directionWaypoints)
        {
            enterWaypoints[road].directionWaypoints = directionWaypoints;
        }


        private void PedestrianWantsToCross(int pedestrianIndex, IIntersection intersection, int waypointIndex)
        {
            if (intersection == this)
            {
                int road = GetRoadToCross(waypointIndex);
                pedestriansCrossing.Add(new PedestrianCrossing(pedestrianIndex, road));
                CheckColor();
            }
        }


        private void CheckColor()
        { 
            if (pedestriansCrossing.Count > 0)
            {
                if (waypointColor == Color.red)
                {
                    MakePedestriansCross(0);
                }
                else
                {
                    IsPathFree(0);
                }
            }
        }


        private int GetRoadToCross(int waypoint)
        {
            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                for (int j = 0; j < enterWaypoints[i].pedestrianWaypoints.Count; j++)
                {
                    if (enterWaypoints[i].pedestrianWaypoints[j] == waypoint)
                    {
                        return i;
                    }
                }
            }
            Debug.LogError("Not Good - verify pedestrians assignments in priority intersection");
            return -1;
        }


        private bool IsPedestrianCrossing(int road)
        {
            return pedestriansCrossing.FirstOrDefault(cond => cond.road == road) != null;
        }


        internal override void DestroyIntersection()
        {
            PedestrianEvents.onStreetCrossing -= PedestrianWantsToCross;
        }


        public override void PedestrianPassed(int pedestrianIndex)
        {
            PedestrianCrossing ped = pedestriansCrossing.FirstOrDefault(cond => cond.pedestrianIndex == pedestrianIndex);
            if (ped != null)
            {
                if (ped.crossing == false)
                {
                    ped.crossing = true;
                }
                else
                {
                    pedestriansCrossing.Remove(ped);
                    //reset stop
                    for (int i = 0; i < enterWaypoints[ped.road].pedestrianWaypoints.Count; i++)
                    {
                        PedestrianEvents.TriggerStopStateChangedEvent(enterWaypoints[ped.road].pedestrianWaypoints[i], true);
                    }
                }
            }
        }


        internal override List<int> GetPedStopWaypoint()
        {
            return new List<int>();
        }
#endif
#endif
    }
}