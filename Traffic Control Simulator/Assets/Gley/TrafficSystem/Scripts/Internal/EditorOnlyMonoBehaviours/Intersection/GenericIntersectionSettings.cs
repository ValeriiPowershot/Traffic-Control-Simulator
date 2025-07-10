#if UNITY_EDITOR
#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Internal;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    public abstract class GenericIntersectionSettings : MonoBehaviour
    {
        public Vector3 position;
        public bool inView;
        public bool justCreated;
        public virtual GenericIntersectionSettings Initialize()
        {
            justCreated = true;
            return this;
        }
        public abstract List<IntersectionStopWaypointsSettings> GetAssignedWaypoints();
        public abstract List<WaypointSettings> GetStopWaypoints(int road);
        public abstract List<WaypointSettings> GetExitWaypoints();
        public virtual bool VerifyAssignments()
        {
            justCreated = false;
            return false;
        }
#if GLEY_PEDESTRIAN_SYSTEM
        public abstract List<PedestrianWaypointSettings> GetPedestrianWaypoints();
        public abstract List<PedestrianWaypointSettings> GetPedestrianWaypoints(int road);
        public abstract List<PedestrianWaypointSettings> GetDirectionWaypoints();
#endif
    }
}
#endif