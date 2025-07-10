#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem;
#endif
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    [System.Serializable]
    public class TrafficLightsIntersection : GenericIntersection
    {
        public List<IntersectionStopWaypointsIndex> stopWaypoints;
        public float greenLightTime;
        public float yellowLightTime;

        //for every road color is set here
        TrafficLightsColor[] intersectionState;
        private float[] roadGreenLightTime;
        private float currentTime;
        private int nrOfRoads;
        private int currentRoad;
        private bool yellowLight;
        private bool stopUpdate;
        public bool hasPedestrians;
        private TrafficLightsBehaviour trafficLightsBehaviour;

        /// <summary>
        /// Constructor used for conversion from editor intersection type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stopWaypoints"></param>
        /// <param name="greenLightTime"></param>
        /// <param name="yellowLightTime"></param>
        public TrafficLightsIntersection(string name, List<IntersectionStopWaypointsIndex> stopWaypoints, float greenLightTime, float yellowLightTime, List<int> exitWaypoints)
        {
            this.name = name;
            this.stopWaypoints = stopWaypoints;
            this.greenLightTime = greenLightTime;
            this.yellowLightTime = yellowLightTime;
            this.exitWaypoints = exitWaypoints;

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
            nrOfRoads = stopWaypoints.Count;
#if GLEY_PEDESTRIAN_SYSTEM
            GetPedestrianRoads();
#endif
            roadGreenLightTime = new float[nrOfRoads];
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                roadGreenLightTime[i] = stopWaypoints[i].greenLightTime;
            }
#if GLEY_PEDESTRIAN_SYSTEM
            SetPedestrianGreenLightTime();
#endif

            if (nrOfRoads == 0)
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



            intersectionState = new TrafficLightsColor[nrOfRoads];

            currentRoad = Random.Range(0, nrOfRoads);
            ChangeCurrentRoadColors(currentRoad, TrafficLightsColor.Green);
            ChangeAllRoadsExceptSelectd(currentRoad, TrafficLightsColor.Red);
            ApplyColorChanges();

            currentTime = 0;
            if (greenLightTime >= 0)
            {
                for (int i = 0; i < roadGreenLightTime.Length; i++)
                {
                    roadGreenLightTime[i] = greenLightTime;
                }
            }
            if (yellowLightTime >= 0)
            {
                this.yellowLightTime = yellowLightTime;
            }

            for (int i = 0; i < roadGreenLightTime.Length; i++)
            {
                if (roadGreenLightTime[i] == 0)
                {
                    roadGreenLightTime[i] = this.greenLightTime;
                }
            }
#endif
        }


        /// <summary>
        /// Change traffic lights color
        /// </summary>
        internal override void UpdateIntersection(float realtimeSinceStartup)
        {
            if (stopUpdate)
                return;
            if (yellowLight == false)
            {
                if (realtimeSinceStartup - currentTime > roadGreenLightTime[currentRoad])
                {
                    ChangeCurrentRoadColors(currentRoad, TrafficLightsColor.YellowGreen);
                    ChangeCurrentRoadColors(GetValidValue(currentRoad + 1), TrafficLightsColor.YellowRed);
                    ApplyColorChanges();
                    yellowLight = true;
                    currentTime = realtimeSinceStartup;
                }
            }
            else
            {
                if (realtimeSinceStartup - currentTime > yellowLightTime)
                {
                    if (carsInIntersection.Count == 0 || exitWaypoints.Count == 0)
                    {
                        ChangeCurrentRoadColors(currentRoad, TrafficLightsColor.Red);
                        currentRoad++;
                        currentRoad = GetValidValue(currentRoad);
                        ChangeCurrentRoadColors(currentRoad, TrafficLightsColor.Green);
                        yellowLight = false;
                        currentTime = realtimeSinceStartup;
                        ApplyColorChanges();
                    }
                }
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
            ChangeCurrentRoadColors(roadIndex, TrafficLightsColor.Green);
            ChangeAllRoadsExceptSelectd(roadIndex, TrafficLightsColor.Red);
            ApplyColorChanges();
        }


        /// <summary>
        /// After all intersection changes have been made this method apply them to the waypoint system and traffic lights 
        /// </summary>
        private void ApplyColorChanges()
        {
            for (int i = 0; i < intersectionState.Length; i++)
            {
                //change waypoint color
                UpdateCurrentIntersectionWaypoints(i, intersectionState[i] != TrafficLightsColor.Green);

                if (i < stopWaypoints.Count)
                {
                    //change traffic lights color
                    trafficLightsBehaviour?.Invoke(intersectionState[i], stopWaypoints[i].redLightObjects, stopWaypoints[i].yellowLightObjects, stopWaypoints[i].greenLightObjects, name);
                }
            }
        }


        /// <summary>
        /// Trigger state changes for specified waypoints
        /// </summary>
        /// <param name="road"></param>
        /// <param name="stop"></param>
        private void UpdateCurrentIntersectionWaypoints(int road, bool stop)
        {
#if GLEY_TRAFFIC_SYSTEM
#if GLEY_PEDESTRIAN_SYSTEM
            if (hasPedestrians && road >= stopWaypoints.Count)
            {
                TriggerPedestrianWaypointsUpdate(stop);
                return;
            }
#endif
            for (int j = 0; j < stopWaypoints[road].roadWaypoints.Count; j++)
            {
                WaypointEvents.TriggerTrafficLightChangedEvent(stopWaypoints[road].roadWaypoints[j], stop);
            }
#endif
        }


        /// <summary>
        /// Change color for specified road
        /// </summary>
        /// <param name="currentRoad"></param>
        /// <param name="newColor"></param>
        private void ChangeCurrentRoadColors(int currentRoad, TrafficLightsColor newColor)
        {
            if (currentRoad < intersectionState.Length)
            {
                intersectionState[currentRoad] = newColor;
            }
            else
            {
                Debug.LogError(currentRoad + "is grated than the max number of roads for intersection " + name);
            }
        }


        /// <summary>
        /// Change color for all roads except the specified one
        /// </summary>
        /// <param name="currentRoad"></param>
        /// <param name="newColor"></param>
        private void ChangeAllRoadsExceptSelectd(int currentRoad, TrafficLightsColor newColor)
        {
            for (int i = 0; i < intersectionState.Length; i++)
            {
                if (i != currentRoad)
                {
                    intersectionState[i] = newColor;
                }
            }
        }


        /// <summary>
        /// Correctly increment the road number
        /// </summary>
        /// <param name="roadNumber"></param>
        /// <returns></returns>
        private int GetValidValue(int roadNumber)
        {
            if (roadNumber >= nrOfRoads)
            {
                roadNumber = roadNumber % nrOfRoads;
            }
            if (roadNumber < 0)
            {
                roadNumber = nrOfRoads + roadNumber;
            }
            return roadNumber;
        }


        public override bool IsPathFree(int waypointIndex)
        {
            return false;
        }

#if GLEY_TRAFFIC_SYSTEM
#if GLEY_PEDESTRIAN_SYSTEM
        public List<int> pedestrianWaypoints;
        public List<int> directionWaypoints;
        public List<GameObject> redLightObjects;
        public List<GameObject> greenLightObjects;
        public float pedestrianGreenLightTime;
        private bool debugIntersections;

        public void AddPedestrianWaypoints(List<int> pedestrianWaypoints, List<int> directionWaypoints, List<GameObject> redLightObjects, List<GameObject> greenLightObjects, float pedestrianGreenLightTime)
        {
            this.pedestrianWaypoints = pedestrianWaypoints;
            this.directionWaypoints = directionWaypoints;
            this.redLightObjects = redLightObjects;
            this.greenLightObjects = greenLightObjects;
            this.pedestrianGreenLightTime = pedestrianGreenLightTime;
        }

        void GetPedestrianRoads()
        {
            if (pedestrianWaypoints.Count > 0)
            {
                hasPedestrians = true;
                nrOfRoads += 1;
                CurrentSceneData currentSceneData = CurrentSceneData.GetSceneInstance();
                currentSceneData.AssignIntersections(pedestrianWaypoints, directionWaypoints, this);
            }
        }

        void SetPedestrianGreenLightTime()
        {
            if (hasPedestrians)
            {
                roadGreenLightTime[roadGreenLightTime.Length - 1] = pedestrianGreenLightTime;
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
