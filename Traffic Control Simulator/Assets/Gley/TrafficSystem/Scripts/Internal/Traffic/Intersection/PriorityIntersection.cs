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
    public class PriorityIntersection : GenericIntersection
    {
        public List<IntersectionStopWaypointsIndex> enterWaypoints;

        private float currentTime;
        private float requiredTime;
        private int currentRoadIndex;
        private int tempRoadIndex;
        private bool changeRequested;
        private Vector3 position;
        List<int> waypointsToCkeck = new List<int>();
        List<Color> waypointColor = new List<Color>();

        /// <summary>
        /// Constructor used for conversion from editor intersection type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enterWaypoints"></param>
        /// <param name="exitWaypoints"></param>
        public PriorityIntersection(string name, List<IntersectionStopWaypointsIndex> enterWaypoints, List<int> exitWaypoints)
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
            requiredTime = 3;
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
            waypointsToCkeck = new List<int>();
            waypointColor = new List<Color>();

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
            return waypointsToCkeck;
        }

        internal List<Color> GetWaypointColors()
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
            int road = 0;
            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                //if the waypoint is in the enter waypoints list needs to be verified if is free
                if (enterWaypoints[i].roadWaypoints.Contains(waypointIndex))
                {
                    if (!waypointsToCkeck.Contains(waypointIndex))
                    {
                        waypointsToCkeck.Add(waypointIndex);
                        waypointColor.Add(Color.green);
                    }
                    road = i;
#if GLEY_PEDESTRIAN_SYSTEM
#if GLEY_TRAFFIC_SYSTEM
                    if (IsPedestrianCrossing(road))
                    {
                        waypointColor[waypointsToCkeck.IndexOf(waypointIndex)] = Color.red;
                        return false;
                    }
#endif
#endif
                    bool stopChange = false;
                    //if vehicle is on current road, wait to pass before changing the road priority
                    if (i == currentRoadIndex)
                    {
                        changeRequested = false;
                        stopChange = true;
                    }

                    //construct priority if vehicle is not on the priority road
                    if (stopChange == false)
                    {
                        if (tempRoadIndex == currentRoadIndex)
                        {
                            tempRoadIndex = road;
                            changeRequested = true;
                            currentTime = Time.timeSinceLevelLoad;
                        }
                    }
                    break;
                }
            }

            int index = waypointsToCkeck.IndexOf(waypointIndex);

            //if a new vehicle is requesting access to intersection but there are vehicles on intersection -> wait
            if (changeRequested == true)
            {
                if (carsInIntersection.Count >= 1)
                {
                    waypointColor[index] = Color.red;
                    return false;
                }
                changeRequested = false;
                currentRoadIndex = tempRoadIndex;
            }

            //if the number of vehicles in intersection is <3 -> permit access
            if (carsInIntersection.Count <= 3)
            {
                if (enterWaypoints[currentRoadIndex].roadWaypoints.Contains(waypointIndex))
                {
                    waypointColor[index] = Color.green;
                    return true;
                }
            }

            //after some time change the priority road
            if (Time.timeSinceLevelLoad - currentTime > requiredTime)
            {
                tempRoadIndex = road;
                changeRequested = true;
                currentTime = Time.timeSinceLevelLoad;
            }
            waypointColor[index] = Color.red;
            return false;
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
                }
            }
        }

        internal override List<int> GetPedStopWaypoint()
        {
            return new List<int>();
        }
#endif
#endif

        void OnDrawGizmos()
        {

        }
    }
}