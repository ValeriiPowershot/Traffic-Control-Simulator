using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanAssets.Internal
{
    [System.Serializable]
    public class WaypointBase
    {
        public List<int> neighbors;
        public List<int> prev;

        public Vector3 position;
        public string name;
        public int listIndex;
        public bool temporaryDisabled;
        

        public WaypointBase()
        {

        }

        /// <summary>
        /// Constructor used to convert from editor waypoint to runtime waypoint 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="listIndex"></param>
        /// <param name="position"></param>
        /// <param name="allowedCars"></param>
        /// <param name="neighbors"></param>
        /// <param name="prev"></param>
        /// <param name="otherLanes"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="stop"></param>
        /// <param name="giveWay"></param>
        /// <param name="enter"></param>
        /// <param name="exit"></param>
        public WaypointBase(string name,
            int listIndex,
            Vector3 position,
            List<int> neighbors,
            List<int> prev)
        {
            this.name = name;
            this.listIndex = listIndex;
            this.position = position;
            this.neighbors = neighbors;
            this.prev = prev;
            temporaryDisabled = false;
        }

        /// <summary>
        /// Waypoint is no longer a target for the vehicle
        /// </summary>
        internal virtual void Passed(int agentIndex)
        {

        }
    }
}
