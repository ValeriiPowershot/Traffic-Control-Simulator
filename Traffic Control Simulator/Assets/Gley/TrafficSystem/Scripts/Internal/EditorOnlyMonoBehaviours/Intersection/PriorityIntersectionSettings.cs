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
    /// Stores priority intersection properties
    /// </summary>
    public class PriorityIntersectionSettings : GenericIntersectionSettings
    {
        public List<IntersectionStopWaypointsSettings> enterWaypoints;
        public List<WaypointSettings> exitWaypoints;


        public override GenericIntersectionSettings Initialize()
        {
            base.Initialize();
            enterWaypoints = new List<IntersectionStopWaypointsSettings>
            {
                new()
            };
            exitWaypoints = new List<WaypointSettings>();
            return this;
        }


        public override List<IntersectionStopWaypointsSettings> GetAssignedWaypoints()
        {
            return enterWaypoints;
        }


        public override List<WaypointSettings> GetStopWaypoints(int road)
        {
            return enterWaypoints[road].roadWaypoints;
        }


        public override List<WaypointSettings> GetExitWaypoints()
        {
            return exitWaypoints;
        }


        public override bool VerifyAssignments()
        {
            bool correct = true;
            if (enterWaypoints == null)
            {
                enterWaypoints = new List<IntersectionStopWaypointsSettings>();
            }

            if (!justCreated && enterWaypoints.Count < 2)
            {
                Debug.LogError($"Priority Intersection {name} has only {enterWaypoints.Count} roads. Please assign at least 2 or create a Priority Crossing", gameObject);
                correct = false;
            }

            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                if (enterWaypoints[i].roadWaypoints == null)
                {
                    enterWaypoints[i].roadWaypoints = new List<WaypointSettings>();
                }
                for (int j = enterWaypoints[i].roadWaypoints.Count - 1; j >= 0; j--)
                {
                    if (enterWaypoints[i].roadWaypoints[j] == null)
                    {
                        enterWaypoints[i].roadWaypoints.RemoveAt(j);
                    }
                }
#if GLEY_PEDESTRIAN_SYSTEM
                if (enterWaypoints[i].pedestrianWaypoints == null)
                {
                    enterWaypoints[i].pedestrianWaypoints = new List<PedestrianWaypointSettings>();
                }

                if (enterWaypoints[i].directionWaypoints == null)
                {
                    enterWaypoints[i].directionWaypoints = new List<PedestrianWaypointSettings>();
                }

                for (int j = enterWaypoints[i].directionWaypoints.Count - 1; j >= 0; j--)
                {
                    if (enterWaypoints[i].directionWaypoints[j] == null)
                    {
                        enterWaypoints[i].directionWaypoints.RemoveAt(j);
                    }
                    else
                    {
                        if (!enterWaypoints[i].directionWaypoints[j].neighbors.Intersect(enterWaypoints[i].pedestrianWaypoints).Any() && !enterWaypoints[i].directionWaypoints[j].prev.Intersect(enterWaypoints[i].pedestrianWaypoints).Any())
                        {
                            enterWaypoints[i].directionWaypoints.RemoveAt(j);
                        }
                    }
                }

                for (int j = 0; j < enterWaypoints[i].pedestrianWaypoints.Count; j++)
                {
                    if (enterWaypoints[i].pedestrianWaypoints[j] == null)
                    {
                        enterWaypoints[i].pedestrianWaypoints.RemoveAt(j);
                    }
                    else
                    {
                        if (!enterWaypoints[i].pedestrianWaypoints[j].neighbors.Intersect(enterWaypoints[i].directionWaypoints).Any() && !enterWaypoints[i].pedestrianWaypoints[j].prev.Intersect(enterWaypoints[i].directionWaypoints).Any())
                        {
                            Debug.LogError($"Pedestrian waypoint {enterWaypoints[i].pedestrianWaypoints[j].name} from intersection {name} road {i} has no direction assigned", gameObject);
                            correct = false;
                        }
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
            base.VerifyAssignments();
            return correct;
        }


#if GLEY_PEDESTRIAN_SYSTEM
        public override List<PedestrianWaypointSettings> GetPedestrianWaypoints()
        {
            List<PedestrianWaypointSettings> result = new List<PedestrianWaypointSettings>();

            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                result.AddRange(enterWaypoints[i].pedestrianWaypoints);
            }
            return result;
        }


        public override List<PedestrianWaypointSettings> GetPedestrianWaypoints(int road)
        {
            return enterWaypoints[road].pedestrianWaypoints;
        }


        public override List<PedestrianWaypointSettings> GetDirectionWaypoints()
        {
            List<PedestrianWaypointSettings> result = new List<PedestrianWaypointSettings>();

            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                result.AddRange(enterWaypoints[i].directionWaypoints);
            }
            return result;
        }
#endif
    }
}
#endif