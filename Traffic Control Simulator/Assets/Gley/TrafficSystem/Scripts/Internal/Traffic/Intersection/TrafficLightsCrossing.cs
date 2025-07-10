#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem;
#endif
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    [System.Serializable]
    public class TrafficLightsCrossing : GenericIntersection
    {
        public List<IntersectionStopWaypointsIndex> stopWaypoints;
        public float greenLightTime;
        public float yellowLightTime;
        public float redLightTime;
        public bool hasPedestrians;

        private TrafficLightsBehaviour trafficLightsBehaviour;
        private TrafficLightsColor intersectionState;
        private float currentTime;
        private float currentTimer;
        private bool stopUpdate;


        /// <summary>
        /// Constructor used for conversion from editor intersection type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stopWaypoints"></param>
        /// <param name="greenLightTime"></param>
        /// <param name="yellowLightTime"></param>
        public TrafficLightsCrossing(string name, List<IntersectionStopWaypointsIndex> stopWaypoints, float greenLightTime, float yellowLightTime, float redLightTime, List<int> exitWaypoints)
        {
            this.name = name;
            this.stopWaypoints = stopWaypoints;
            this.greenLightTime = greenLightTime;
            this.yellowLightTime = yellowLightTime;
            this.exitWaypoints = exitWaypoints;
            this.redLightTime = redLightTime;
        }


        /// <summary>
        /// Assigns corresponding waypoints to work with this intersection and setup traffic lights
        /// </summary>
        /// <param name="waypointManager"></param>
        /// <param name="greenLightTime"></param>
        /// <param name="yellowLightTime"></param>
        internal override void Initialize(WaypointManagerBase waypointManager, float greenLightTime, float yellowLightTime)
        {
#if GLEY_TRAFFIC_SYSTEM
#if GLEY_PEDESTRIAN_SYSTEM
            GetPedestrianRoads();
#endif
            greenLightTime = stopWaypoints[0].greenLightTime;


            if (stopWaypoints.Count == 0)
            {
                Debug.LogWarning("Intersection " + name + " has some unassigned references");
                return;
            }

            base.Initialize(waypointManager, greenLightTime, yellowLightTime);

            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
                {
                    waypointManager.GetWaypoint<Waypoint>(stopWaypoints[i].roadWaypoints[j]).SetIntersection(this, false, true, true, false);
                }
            }

            intersectionState = TrafficLightsColor.Green;
            ApplyColorChanges();

            currentTime = Random.Range(0, 10);
#endif
        }


        /// <summary>
        /// Change traffic lights color
        /// </summary>
        internal override void UpdateIntersection(float realtimeSinceStartup)
        {
            if (stopUpdate)
                return;
            currentTimer = realtimeSinceStartup - currentTime;
            switch (intersectionState)
            {
                case TrafficLightsColor.Green:
                    if (currentTimer > greenLightTime)
                    {
                        intersectionState = TrafficLightsColor.YellowGreen;
                        ApplyColorChanges();
                        currentTime = realtimeSinceStartup;
                    }
                    break;

                case TrafficLightsColor.YellowGreen:
                    if (currentTimer > yellowLightTime)
                    {
                        intersectionState = TrafficLightsColor.Red;
                        ApplyColorChanges();
                        currentTime = realtimeSinceStartup;
                    }
                    break;

                case TrafficLightsColor.Red:
                    if (currentTimer > redLightTime)
                    {
                        intersectionState = TrafficLightsColor.YellowRed;
                        ApplyColorChanges();
                        currentTime = realtimeSinceStartup;
                    }
                    break;

                case TrafficLightsColor.YellowRed:
                    if (currentTimer > yellowLightTime)
                    {
                        intersectionState = TrafficLightsColor.Green;
                        ApplyColorChanges();
                        currentTime = realtimeSinceStartup;
                    }
                    break;
            }
        }


        /// <summary>
        /// Used for editor applications
        /// </summary>
        /// <returns></returns>
        internal override List<IntersectionStopWaypointsIndex> GetWaypoints()
        {
            return stopWaypoints;
        }


        /// <summary>
        /// Used to set up custom behavior for traffic lights
        /// </summary>
        /// <param name="trafficLightsBehaviour"></param>
        internal override void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
        {
            this.trafficLightsBehaviour = trafficLightsBehaviour;
        }

        internal override void SetGreenRoad(int roadIndex, bool doNotChangeAgain)
        {
            stopUpdate = doNotChangeAgain;
            intersectionState = TrafficLightsColor.Green;
            ApplyColorChanges();
        }


        /// <summary>
        /// After all intersection changes have been made this method apply them to the waypoint system and traffic lights 
        /// </summary>
        private void ApplyColorChanges()
        {
            //change waypoint color
            UpdateCurrentIntersectionWaypoints(0, intersectionState != TrafficLightsColor.Green);
#if GLEY_PEDESTRIAN_SYSTEM
            TriggerPedestrianWaypointsUpdate(intersectionState != TrafficLightsColor.Red);
#endif
            trafficLightsBehaviour?.Invoke(intersectionState, stopWaypoints[0].redLightObjects, stopWaypoints[0].yellowLightObjects, stopWaypoints[0].greenLightObjects, name);
        }


        /// <summary>
        /// Trigger state changes for specified waypoints
        /// </summary>
        /// <param name="road"></param>
        /// <param name="stop"></param>
        private void UpdateCurrentIntersectionWaypoints(int road, bool stop)
        {
#if GLEY_TRAFFIC_SYSTEM
            for (int j = 0; j < stopWaypoints[road].roadWaypoints.Count; j++)
            {
                WaypointEvents.TriggerTrafficLightChangedEvent(stopWaypoints[road].roadWaypoints[j], stop);
            }
#endif
        }


        public override bool IsPathFree(int waypointIndex)
        {
            return false;
        }

        internal TrafficLightsColor GetCrossingState()
        {
            return intersectionState;
        }


#if GLEY_TRAFFIC_SYSTEM
#if GLEY_PEDESTRIAN_SYSTEM
        public List<int> pedestrianWaypoints;
        public List<int> directionWaypoints;
        public List<GameObject> redLightObjects;
        public List<GameObject> greenLightObjects;

        public void AddPedestrianWaypoints(List<int> pedestrianWaypoints, List<int> directionWaypoints, List<GameObject> redLightObjects, List<GameObject> greenLightObjects)
        {
            this.pedestrianWaypoints = pedestrianWaypoints;
            this.directionWaypoints = directionWaypoints;
            this.redLightObjects = redLightObjects;
            this.greenLightObjects = greenLightObjects;
        }


        void GetPedestrianRoads()
        {
            if (pedestrianWaypoints.Count > 0)
            {
                hasPedestrians = true;
                CurrentSceneData currentSceneData = CurrentSceneData.GetSceneInstance();
                currentSceneData.AssignIntersections(pedestrianWaypoints, directionWaypoints, this);
            }
        }


        void TriggerPedestrianWaypointsUpdate(bool stop)
        {
            for (int i = 0; i < redLightObjects.Count; i++)
            {
                if (redLightObjects[i].activeSelf != stop)
                {
                    redLightObjects[i].SetActive(stop);
                }
            }

            for (int i = 0; i < greenLightObjects.Count; i++)
            {
                if (greenLightObjects[i].activeSelf != !stop)
                {
                    greenLightObjects[i].SetActive(!stop);
                }
            }
            for (int i = 0; i < pedestrianWaypoints.Count; i++)
            {
                PedestrianEvents.TriggerStopStateChangedEvent(pedestrianWaypoints[i], stop);
            }
        }


        internal override List<int> GetPedStopWaypoint()
        {
            return pedestrianWaypoints;
        }


        public override void PedestrianPassed(int pedestrianIndex)
        {

        }


        internal override void DestroyIntersection()
        {

        }
#endif
#endif
    }
}
