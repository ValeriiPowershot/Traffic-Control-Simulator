#if UNITY_EDITOR
#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Internal;
#endif
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Stores traffic lights crossing properties
    /// </summary>
    public class TrafficLightsCrossingSettings : GenericIntersectionSettings
    {
#if GLEY_PEDESTRIAN_SYSTEM
        public List<PedestrianWaypointSettings> pedestrianWaypoints;
        public List<PedestrianWaypointSettings> directionWaypoints;
        public List<GameObject> pedestrianRedLightObjects;
        public List<GameObject> pedestrianGreenLightObjects;
        public float pedestrianGreenLightTime;
#endif
        public List<IntersectionStopWaypointsSettings> stopWaypoints;
        public List<WaypointSettings> exitWaypoints;

        public float greenLightTime = 10;
        public float yellowLightTime = 2;
        public float redLightTime = 5;



        public override GenericIntersectionSettings Initialize()
        {
            base.Initialize();
            stopWaypoints = new List<IntersectionStopWaypointsSettings>
            {
                new()
            };
            exitWaypoints = new List<WaypointSettings>();
#if GLEY_PEDESTRIAN_SYSTEM
            pedestrianWaypoints = new List<PedestrianWaypointSettings>();
            directionWaypoints = new List<PedestrianWaypointSettings>();
            pedestrianRedLightObjects = new List<GameObject>();
            pedestrianGreenLightObjects = new List<GameObject>();
#endif
            return this;
        }


        public override List<IntersectionStopWaypointsSettings> GetAssignedWaypoints()
        {
            return stopWaypoints;
        }


        public override List<WaypointSettings> GetStopWaypoints(int road)
        {
            return stopWaypoints[road].roadWaypoints;
        }


        public override List<WaypointSettings> GetExitWaypoints()
        {
            return exitWaypoints;
        }


        public override bool VerifyAssignments()
        {
            bool correct = true;
            if (stopWaypoints == null)
            {
                stopWaypoints = new List<IntersectionStopWaypointsSettings>();
            }

            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                if (stopWaypoints[i].roadWaypoints == null)
                {
                    stopWaypoints[i].roadWaypoints = new List<WaypointSettings>();
                }
                for (int j = stopWaypoints[i].roadWaypoints.Count - 1; j >= 0; j--)
                {
                    if (stopWaypoints[i].roadWaypoints[j] == null)
                    {
                        stopWaypoints[i].roadWaypoints.RemoveAt(j);
                    }
                }

                if (stopWaypoints[i].redLightObjects == null)
                {
                    stopWaypoints[i].redLightObjects = new List<GameObject>();
                }
                for (int j = stopWaypoints[i].redLightObjects.Count - 1; j >= 0; j--)
                {
                    if (stopWaypoints[i].redLightObjects[j] == null)
                    {
                        stopWaypoints[i].redLightObjects.RemoveAt(j);
                    }
                }

                if (stopWaypoints[i].yellowLightObjects == null)
                {
                    stopWaypoints[i].yellowLightObjects = new List<GameObject>();
                }
                for (int j = stopWaypoints[i].yellowLightObjects.Count - 1; j >= 0; j--)
                {
                    if (stopWaypoints[i].yellowLightObjects[j] == null)
                    {
                        stopWaypoints[i].yellowLightObjects.RemoveAt(j);
                    }
                }

                if (stopWaypoints[i].greenLightObjects == null)
                {
                    stopWaypoints[i].greenLightObjects = new List<GameObject>();
                }
                for (int j = stopWaypoints[i].greenLightObjects.Count - 1; j >= 0; j--)
                {
                    if (stopWaypoints[i].greenLightObjects[j] == null)
                    {
                        stopWaypoints[i].greenLightObjects.RemoveAt(j);
                    }
                }

#if GLEY_PEDESTRIAN_SYSTEM
                if (stopWaypoints[i].pedestrianWaypoints == null)
                {
                    stopWaypoints[i].pedestrianWaypoints = new List<PedestrianWaypointSettings>();
                }
                for (int j = stopWaypoints[i].pedestrianWaypoints.Count - 1; j >= 0; j--)
                {
                    if (stopWaypoints[i].pedestrianWaypoints[j] == null)
                    {
                        stopWaypoints[i].pedestrianWaypoints.RemoveAt(j);
                    }
                }

                if (stopWaypoints[i].directionWaypoints == null)
                {
                    stopWaypoints[i].directionWaypoints = new List<PedestrianWaypointSettings>();
                }
                for (int j = stopWaypoints[i].directionWaypoints.Count - 1; j >= 0; j--)
                {
                    if (stopWaypoints[i].directionWaypoints[j] == null)
                    {
                        stopWaypoints[i].directionWaypoints.RemoveAt(j);
                    }
                }
#endif
            }
            if (exitWaypoints == null)
            {
                exitWaypoints = new List<WaypointSettings>();
            }
            for (int i = exitWaypoints.Count - 1; i >= 0; i--)
            {
                if (exitWaypoints[i] == null)
                {
                    exitWaypoints.RemoveAt(i);
                }
            }
#if GLEY_PEDESTRIAN_SYSTEM
            if (directionWaypoints == null)
            {
                directionWaypoints = new List<PedestrianWaypointSettings>();
            }

            if (pedestrianWaypoints == null)
            {
                pedestrianWaypoints = new List<PedestrianWaypointSettings>();
            }

            for (int i = directionWaypoints.Count - 1; i >= 0; i--)
            {
                if (directionWaypoints[i] == null)
                {
                    directionWaypoints.RemoveAt(i);
                }
                else
                {
                    if (!directionWaypoints[i].neighbors.Intersect(pedestrianWaypoints).Any() && !directionWaypoints[i].prev.Intersect(pedestrianWaypoints).Any())
                    {
                        directionWaypoints.RemoveAt(i);
                    }
                }
            }

            for (int i = pedestrianWaypoints.Count - 1; i >= 0; i--)
            {
                if (pedestrianWaypoints[i] == null)
                {
                    pedestrianWaypoints.RemoveAt(i);
                }
                else
                {
                    if (!pedestrianWaypoints[i].neighbors.Intersect(directionWaypoints).Any() && !pedestrianWaypoints[i].prev.Intersect(directionWaypoints).Any())
                    {
                        Debug.LogError($"Pedestrian waypoint {pedestrianWaypoints[i].name} from intersection {name} has no direction assigned", gameObject);
                       correct=false; 
                    }
                }
            }

            if (pedestrianRedLightObjects == null)
            {
                pedestrianRedLightObjects = new List<GameObject>();
            }
            for (int i = pedestrianRedLightObjects.Count - 1; i >= 0; i--)
            {
                if (pedestrianRedLightObjects[i] == null)
                {
                    pedestrianRedLightObjects.RemoveAt(i);
                }
            }

            if (pedestrianGreenLightObjects == null)
            {
                pedestrianGreenLightObjects = new List<GameObject>();
            }
            for (int i = pedestrianGreenLightObjects.Count - 1; i >= 0; i--)
            {
                if (pedestrianGreenLightObjects[i] == null)
                {
                    pedestrianGreenLightObjects.RemoveAt(i);
                }
            }
#endif
            base.VerifyAssignments();
            return correct;
        }


#if GLEY_PEDESTRIAN_SYSTEM
        public override List<PedestrianWaypointSettings> GetPedestrianWaypoints()
        {
            return pedestrianWaypoints;
        }


        public override List<PedestrianWaypointSettings> GetPedestrianWaypoints(int road)
        {
            return pedestrianWaypoints;
        }


        public override List<PedestrianWaypointSettings> GetDirectionWaypoints()
        {
            return directionWaypoints;
        }
#endif
    }
}
#endif
