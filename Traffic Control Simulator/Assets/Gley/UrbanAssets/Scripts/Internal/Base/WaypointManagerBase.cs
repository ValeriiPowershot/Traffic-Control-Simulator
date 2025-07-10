using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanAssets.Internal
{
    public abstract class WaypointManagerBase : MonoBehaviour
    {
        //contains at index the waypoint index of the target waypoint of that agent. Agent at position 2 has the target waypoint index target[2]
        protected GridManager gridManager;
        private WaypointBase[] allWaypoints;
        protected List<WaypointBase> disabledWaypoints;

        internal virtual void Initialize(GridManager gridManager, WaypointBase[] allWaypoints, int nrOfAgents, bool debugWaypoints, bool debugDisabledWaypoints)
        {
            this.allWaypoints = allWaypoints;
            this.gridManager = gridManager;
            disabledWaypoints = new List<WaypointBase>();
        }


        /// <summary>
        /// Converts index to waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        internal T GetWaypoint<T>(int waypointIndex) where T : WaypointBase
        {
            if (waypointIndex == -1 || waypointIndex >= allWaypoints.Length)
            {
                return null;
            }
            return (T)allWaypoints[waypointIndex];
        }


        /// <summary>
        /// Checks if waypoint is temporary disabled
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        internal bool IsDisabled(int waypointIndex)
        {
            return GetWaypoint<WaypointBase>(waypointIndex).temporaryDisabled;
        }


        /// <summary>
        /// Get orientation of the waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        internal Quaternion GetNextOrientation(int waypointIndex)
        {
            if (GetWaypoint<WaypointBase>(waypointIndex).neighbors.Count == 0)
            {
                return Quaternion.identity;
            }
            return Quaternion.LookRotation(GetWaypoint<WaypointBase>(GetWaypoint<WaypointBase>(waypointIndex).neighbors[0]).position - GetWaypoint<WaypointBase>(waypointIndex).position);
        }


        /// <summary>
        /// Get orientation of the waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        internal Quaternion GetPrevOrientation(int waypointIndex)
        {
            if (GetWaypoint<WaypointBase>(waypointIndex).prev.Count == 0)
            {
                return Quaternion.identity;
            }
            return Quaternion.LookRotation(GetWaypoint<WaypointBase>(waypointIndex).position - GetWaypoint<WaypointBase>(GetWaypoint<WaypointBase>(waypointIndex).prev[0]).position);
        }


        /// <summary>
        /// Enables unavailable waypoints
        /// </summary>
        internal void EnableAllWaypoints()
        {
            for (int i = 0; i < disabledWaypoints.Count; i++)
            {
                disabledWaypoints[i].temporaryDisabled = false;
            }
            disabledWaypoints = new List<WaypointBase>();
        }


        /// <summary>
        /// Mark a waypoint as disabled
        /// </summary>
        /// <param name="waypoint"></param>
        internal void AddDisabledWaypoint(WaypointBase waypoint)
        {
            disabledWaypoints.Add(waypoint);
            waypoint.temporaryDisabled = true;
        }


    }
}
