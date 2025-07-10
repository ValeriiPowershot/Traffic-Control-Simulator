using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Performs waypoint operations
    /// </summary>
    public class WaypointManager : WaypointManagerBase
    {
        protected Dictionary<int, int> playerTarget;
        protected int[] target;
        private bool debugDisabledWaypoints;
        protected bool[] hasPath;
        protected Dictionary<int, Queue<int>> pathToDestination;

        private SpawnWaypointSelector spawnWaypointSelector;
        private bool debugWaypoints;

        internal WaypointManager Initialize(GridManager gridManager, Waypoint[] allWaypoints, int nrOfVehicles, bool debugWaypoints, bool debugDisabledWaypoints)
        {
            this.debugWaypoints = debugWaypoints;
            WaypointEvents.onTrafficLightChanged += TrafficLightChanged;
            playerTarget = new Dictionary<int, int>();
            target = new int[nrOfVehicles];
            for (int i = 0; i < target.Length; i++)
            {
                target[i] = -1;
            }
            this.debugDisabledWaypoints = debugDisabledWaypoints;
            pathToDestination = new Dictionary<int, Queue<int>>();

            hasPath = new bool[nrOfVehicles];
            base.Initialize(gridManager, allWaypoints, nrOfVehicles, debugWaypoints, debugDisabledWaypoints);           
            return this;
        }

        /// <summary>
        /// Get Target waypoint index of agent
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <returns></returns>
        internal int GetTargetWaypointIndex(int agentIndex)
        {
            return target[agentIndex];
        }

        /// <summary>
        /// Get a free waypoint connected to the current one
        /// </summary>
        /// <param name="agentIndex">agent that requested the waypoint</param>
        /// <param name="agentType">type of the agent that requested the waypoint</param>
        /// <returns></returns>
        internal virtual int GetCurrentLaneWaypointIndex(int agentIndex, int agentType)
        {
            int waypointIndex = PeekPoint(agentIndex);
            if (waypointIndex != -1)
            {
                return waypointIndex;
            }
            
            Waypoint oldWaypoint = GetTargetWaypointOfAgent<Waypoint>(agentIndex);

            //check direct neighbors
            if (oldWaypoint.neighbors.Count > 0)
            {
                Waypoint[] possibleWaypoints = GetAllWaypoints(oldWaypoint.neighbors).Where(cond => cond.allowedAgents.Contains(agentType) && cond.temporaryDisabled == false).ToArray();
                if (possibleWaypoints.Length > 0)
                {
                    waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                }
            }

            //check other lanes
            if (waypointIndex == -1)
            {
                if (oldWaypoint.otherLanes.Count > 0)
                {
                    Waypoint[] possibleWaypoints = GetAllWaypoints(oldWaypoint.otherLanes).Where(cond => cond.allowedAgents.Contains(agentType) && cond.temporaryDisabled == false).ToArray();
                    if (possibleWaypoints.Length > 0)
                    {
                        waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                    }
                }
            }

            //check neighbors that are not allowed
            if (waypointIndex == -1)
            {
                if (oldWaypoint.neighbors.Count > 0)
                {
                    Waypoint[] possibleWaypoints = GetAllWaypoints(oldWaypoint.neighbors).Where(cond => cond.temporaryDisabled == false).ToArray();
                    if (possibleWaypoints.Length > 0)
                    {
                        waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                    }
                }
            }

            //check other lanes that are not allowed
            if (waypointIndex == -1)
            {
                if (oldWaypoint.otherLanes.Count > 0)
                {
                    Waypoint[] possibleWaypoints = GetAllWaypoints(oldWaypoint.otherLanes).Where(cond => cond.temporaryDisabled == false).ToArray();
                    if (possibleWaypoints.Length > 0)
                    {
                        waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                    }
                }
            }
            return waypointIndex;
        }

        public int GetAgentIndexAtTarget(int waypointIndex)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] == waypointIndex)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the target waypoint of the agent
        /// </summary>
        /// <typeparam name="T">type of waypoint</typeparam>
        /// <param name="agentIndex">agent index</param>
        /// <returns></returns>
        internal T GetTargetWaypointOfAgent<T>(int agentIndex) where T : WaypointBase
        {
            return GetWaypoint<T>(GetTargetWaypointIndex(agentIndex));
        }

        internal void RegisterPlayer(int id, int waypointIndex)
        {
            if (!playerTarget.ContainsKey(id))
            {
                playerTarget.Add(id, waypointIndex);
            }
        }


        internal void UpdatePlayerWaypoint(int id, int waypointIndex)
        {
            playerTarget[id] = waypointIndex;
        }


        /// <summary>
        /// Check if waypoint is a target for another agent
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        public bool IsThisWaypointATarget(int waypointIndex)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] == waypointIndex)
                {
                    return true;
                }
            }

            return playerTarget.ContainsValue(waypointIndex);
        }


        internal float GetLaneWidth(int vehicleIndex)
        {
            try
            {
                return GetTargetWaypointOfAgent<Waypoint>(vehicleIndex).laneWidth;
            }
            catch
            {
                return 0;
            }
        }


        internal bool AreTheseWaypointsATarget(List<int> waypointsToCheck)
        {
            return target.Intersect(waypointsToCheck).Any() || playerTarget.Values.Any(v => waypointsToCheck.Contains(v));
        }


        void TrafficLightChanged(int waypointIndex, bool newValue)
        {
            if (GetWaypoint<Waypoint>(waypointIndex).stop != newValue)
            {
                GetWaypoint<Waypoint>(waypointIndex).stop = newValue;
                for (int i = 0; i < target.Length; i++)
                {
                    if (target[i] == waypointIndex)
                    {
                        WaypointEvents.TriggerStopStateChangedEvent(i, GetWaypoint<Waypoint>(waypointIndex).stop);
                    }
                }
            }

        }


        internal bool HaveCommonNeighbors(Waypoint fromWaypoint, Waypoint toWaypoint, int level = 0)
        {
            if (level == 0)
            {
                if (fromWaypoint.neighbors.Intersect(toWaypoint.neighbors).Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }


        /// <summary>
        /// Set next waypoint and trigger the required events
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <param name="waypointIndex"></param>
        internal void SetNextWaypoint(int vehicleIndex, int waypointIndex)
        {
            bool stop = false;
            if (hasPath[vehicleIndex])
            {
                Queue<int> queue;
                if (pathToDestination.TryGetValue(vehicleIndex, out queue))
                {
                    queue.Dequeue();
                    if (queue.Count == 0)
                    {
                        stop = true;
                    }
                }
            }

            SetTargetWaypoint(vehicleIndex, waypointIndex);
            Waypoint targetWaypoint = GetTargetWaypointOfAgent<Waypoint>(vehicleIndex);
            if (stop == true)
            {
                WaypointEvents.TriggerStopStateChangedEvent(vehicleIndex, true);
            }
            if (targetWaypoint.stop == true)
            {
                WaypointEvents.TriggerStopStateChangedEvent(vehicleIndex, targetWaypoint.stop);
            }
            if (targetWaypoint.giveWay == true)
            {
                WaypointEvents.TriggerGiveWayStateChangedEvent(vehicleIndex, targetWaypoint.giveWay);
            }

            if (targetWaypoint.complexGiveWay == true)
            {
                WaypointEvents.TriggerGiveWayStateChangedEvent(vehicleIndex, targetWaypoint.complexGiveWay);
            }

        }

        /// <summary>
        /// Remove target waypoint for the agent at index
        /// </summary>
        /// <param name="agentIndex"></param>
        internal void RemoveAgent(int agentIndex)
        {
            //MarkWaypointAsPassed(agentIndex);
            SetTargetWaypointIndex(agentIndex, -1);
        }

        /// <summary>
        /// Directly set the target waypoint for the vehicle at index.
        /// Used to set first waypoint after vehicle initialization
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <param name="waypointIndex"></param>
        internal void SetTargetWaypoint(int agentIndex, int waypointIndex)
        {
            MarkWaypointAsPassed(agentIndex);
            SetTargetWaypointIndex(agentIndex, waypointIndex);
        }

        protected void SetTargetWaypointIndex(int agentIndex, int waypointIndex)
        {
            //set current target
            target[agentIndex] = waypointIndex;
        }

        /// <summary>
        /// called when a waypoint was passed
        /// </summary>
        /// <param name="agentIndex"></param>
        private void MarkWaypointAsPassed(int agentIndex)
        {
            if (GetTargetWaypointIndex(agentIndex) != -1)
            {
                GetWaypoint<WaypointBase>(GetTargetWaypointIndex(agentIndex)).Passed(agentIndex);
            }
        }


        public bool CanContinueStraight(int vehicleIndex, int carType)
        {
            Waypoint targetWaypoint = GetTargetWaypointOfAgent<Waypoint>(vehicleIndex);
            if (targetWaypoint.neighbors.Count > 0)
            {
                if (hasPath[vehicleIndex])
                {
                    Queue<int> queue;
                    if (pathToDestination.TryGetValue(vehicleIndex, out queue))
                    {
                        if (queue.Count > 0)
                        {
                            int nextWaypoint = queue.Peek();
                            if (!targetWaypoint.neighbors.Contains(nextWaypoint))
                            {
                                return false;
                            }
                        }
                    }
                }
                for (int i = 0; i < targetWaypoint.neighbors.Count; i++)
                {
                    if (GetWaypoint<Waypoint>(targetWaypoint.neighbors[i]).allowedAgents.Contains(carType) && !IsDisabled(targetWaypoint.neighbors[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Check if target waypoint of the vehicle is in intersection
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        internal bool IsInIntersection(int vehicleIndex)
        {
            return GetTargetWaypointOfAgent<Waypoint>(vehicleIndex).IsInIntersection();
        }


        /// <summary>
        /// Check if can switch to target waypoint
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        internal bool CanEnterIntersection(int vehicleIndex)
        {
            return GetTargetWaypointOfAgent<Waypoint>(vehicleIndex).CanChange();
        }


        /// <summary>
        /// Check if the previous waypoints are free
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <param name="freeWaypointsNeeded"></param>
        /// <param name="possibleWaypoints"></param>
        /// <returns></returns>
        internal bool AllPreviousWaypointsAreFree(int vehicleIndex, float distance, int waypointToCheck, ref int incomingCarIndex)
        {
            return IsTargetFree(GetWaypoint<WaypointBase>(waypointToCheck), distance, GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex), vehicleIndex, ref incomingCarIndex);
        }


        /// <summary>
        /// Check what vehicle is in front
        /// </summary>
        /// <param name="vehicleIndex1"></param>
        /// <param name="vehicleIndex2"></param>
        /// <returns>
        /// 1-> if 1 is in front of 2
        /// 2-> if 2 is in front of 1
        /// 0-> if it is not possible to determine
        /// </returns>
        internal int IsInFront(int vehicleIndex1, int vehicleIndex2)
        {
            //compares waypoints to determine which vehicle is in front 
            int distance = 0;
            //if no neighbors are available -> not possible to determine
            if (GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex1).neighbors.Count == 0)
            {
                return 0;
            }

            //check next 10 waypoints to find waypoint 2
            int startWaypointIndex = GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex1).neighbors[0];
            while (startWaypointIndex != GetTargetWaypointIndex(vehicleIndex2) && distance < 10)
            {
                distance++;
                if (GetWaypoint<WaypointBase>(startWaypointIndex).neighbors.Count == 0)
                {
                    //if not found -> not possible to determine
                    return 0;
                }
                startWaypointIndex = GetWaypoint<WaypointBase>(startWaypointIndex).neighbors[0];
            }


            int distance2 = 0;
            if (GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex2).neighbors.Count == 0)
            {
                return 0;
            }

            startWaypointIndex = GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex2).neighbors[0];
            while (startWaypointIndex != GetTargetWaypointIndex(vehicleIndex1) && distance2 < 10)
            {
                distance2++;
                if (GetWaypoint<WaypointBase>(startWaypointIndex).neighbors.Count == 0)
                {
                    //if not found -> not possible to determine
                    return 0;
                }
                startWaypointIndex = GetWaypoint<WaypointBase>(startWaypointIndex).neighbors[0];
            }

            //if no waypoints found -> not possible to determine
            if (distance == 10 && distance2 == 10)
            {
                return 0;
            }

            if (distance2 > distance)
            {
                return 2;
            }

            return 1;
        }


        /// <summary>
        /// Check if 2 vehicles have the same target
        /// </summary>
        /// <param name="vehicleIndex1"></param>
        /// <param name="VehicleIndex2"></param>
        /// <returns></returns>
        internal bool IsSameTarget(int vehicleIndex1, int VehicleIndex2)
        {
            return GetTargetWaypointIndex(vehicleIndex1) == GetTargetWaypointIndex(VehicleIndex2);
        }


        /// <summary>
        /// Get waypoint speed
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        internal float GetMaxSpeed(int vehicleIndex)
        {
            return GetTargetWaypointOfAgent<Waypoint>(vehicleIndex).maxSpeed;
        }


        /// <summary>
        /// Check if previous waypoints are free
        /// </summary>
        /// <param name="waypoint"></param>
        /// <param name="level"></param>
        /// <param name="initialWaypoint"></param>
        /// <returns></returns>
        private bool IsTargetFree(WaypointBase waypoint, float distance, WaypointBase initialWaypoint, int currentCarIndex, ref int incomingCarIndex)
        {
#if UNITY_EDITOR
            if (debugWaypoints)
            {
                Debug.DrawLine(waypoint.position, initialWaypoint.position, Color.green, 1);
            }
#endif
            if (distance <= 0)
            {
#if UNITY_EDITOR
                if (debugWaypoints)
                {
                    Debug.DrawLine(waypoint.position, initialWaypoint.position, Color.green, 1);
                }
#endif
                return true;
            }
            if (waypoint == initialWaypoint)
            {
#if UNITY_EDITOR
                if (debugWaypoints)
                {
                    Debug.DrawLine(waypoint.position, initialWaypoint.position, Color.white, 1);
                }
#endif
                return true;
            }
            if (IsThisWaypointATarget(waypoint.listIndex))
            {
                incomingCarIndex = GetAgentIndexAtTarget(waypoint.listIndex);
                if (GetTargetWaypointIndex(currentCarIndex) == waypoint.listIndex)
                {
#if UNITY_EDITOR
                    if (debugWaypoints)
                    {
                        Debug.DrawLine(waypoint.position, initialWaypoint.position, Color.blue, 1);
                    }
#endif
                    return true;
                }
                else
                {
#if UNITY_EDITOR
                    if (debugWaypoints)
                    {
                        Debug.DrawLine(waypoint.position, initialWaypoint.position, Color.red, 1);
                    }
#endif
                    return false;
                }
            }
            else
            {
                if (waypoint.prev.Count <= 0)
                {
                    return true;
                }
                distance -= Vector3.Distance(waypoint.position, initialWaypoint.position);
                for (int i = 0; i < waypoint.prev.Count; i++)
                {
                    if (!IsTargetFree(GetWaypoint<WaypointBase>(waypoint.prev[i]), distance, initialWaypoint, currentCarIndex, ref incomingCarIndex))
                    {
                        if (debugWaypoints)
                        {
                            Debug.DrawLine(waypoint.position, initialWaypoint.position, Color.magenta, 1);
                        }
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Get rotation of the target waypoint
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <returns></returns>
        internal Quaternion GetTargetWaypointRotation(int agentIndex)
        {
            if (GetTargetWaypointOfAgent<WaypointBase>(agentIndex).neighbors.Count == 0)
            {
                return Quaternion.identity;
            }
            return Quaternion.LookRotation(GetWaypoint<WaypointBase>(GetTargetWaypointOfAgent<WaypointBase>(agentIndex).neighbors[0]).position - GetTargetWaypointOfAgent<WaypointBase>(agentIndex).position);
        }


        /// <summary>
        /// Check if a change of lane is possible
        /// Used to overtake and give way
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <param name="agentType"></param>
        /// <returns></returns>
        internal int GetOtherLaneWaypointIndex(int agentIndex, int agentType, RoadSide side = RoadSide.Any, Vector3 forwardVector = default)
        {
            int waypointIndex = PeekPoint(agentIndex);
            if (waypointIndex != -1)
            {
                return waypointIndex;
            }

            Waypoint currentWaypoint = GetTargetWaypointOfAgent<Waypoint>(agentIndex);

            if (currentWaypoint.otherLanes.Count > 0)
            {
                WaypointBase[] possibleWaypoints = GetAllWaypoints(currentWaypoint.otherLanes).Where(cond => cond.allowedAgents.Contains(agentType)).ToArray();
                if (possibleWaypoints.Length > 0)
                {
                    return GetSideWaypoint(possibleWaypoints, currentWaypoint, side, forwardVector);
                }
            }

            return -1;
        }


        private int PeekPoint(int agentIndex)
        {
            Queue<int> queue;
            if (pathToDestination.TryGetValue(agentIndex, out queue))
            {
                if (queue.Count > 0)
                {
                    return queue.Peek();
                }
                return -2;
            }
            return -1;
        }


        private int GetSideWaypoint(WaypointBase[] waypoints, WaypointBase currentWaypoint, RoadSide side, Vector3 forwardVector)
        {
            switch (side)
            {
                case RoadSide.Any:
                    return waypoints[Random.Range(0, waypoints.Length)].listIndex;
                case RoadSide.Left:
                    for (int i = 0; i < waypoints.Length; i++)
                    {
                        if (Vector3.SignedAngle(waypoints[i].position - currentWaypoint.position, forwardVector, Vector3.up) > 5)
                        {
                            return waypoints[i].listIndex;
                        }
                    }
                    break;
                case RoadSide.Right:
                    for (int i = 0; i < waypoints.Length; i++)
                    {
                        if (Vector3.SignedAngle(waypoints[i].position - currentWaypoint.position, forwardVector, Vector3.up) < -5)
                        {
                            return waypoints[i].listIndex;
                        }
                    }
                    break;
            }

            return -1;
        }


        /// <summary>
        /// Cleanup
        /// </summary>
        private void OnDestroy()
        {
            WaypointEvents.onTrafficLightChanged -= TrafficLightChanged;
        }


        internal void AddWaypointEvent(int waypointIndex, string data)
        {
            Waypoint waypoint = GetWaypoint<Waypoint>(waypointIndex);
            if (waypoint != null)
            {
                waypoint.triggerEvent = true;
                waypoint.eventData = data;
            }
        }


        internal void RemoveWaypointEvent(int waypointIndex)
        {
            Waypoint waypoint = GetWaypoint<Waypoint>(waypointIndex);
            if (waypoint != null)
            {
                waypoint.triggerEvent = false;
                waypoint.eventData = string.Empty;
            }
        }

        internal int GetNeighborCellWaypoint(int row, int column, int depth, VehicleTypes carType, Vector3 playerPosition, Vector3 playerDirection, bool useWaypointPriority)
        {
#if GLEY_TRAFFIC_SYSTEM
            //get all cell neighbors for the specified depth
            List<Vector2Int> neighbors = gridManager.GetCellNeighbors(row, column, depth, false);

            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                if (gridManager.GetCurrentSceneData().grid[neighbors[i].x].row[neighbors[i].y].spawnWaypoints.Count == 0)
                {
                    neighbors.RemoveAt(i);
                }
            }

            //if neighbors exists
            if (neighbors.Count > 0)
            {
                return ApplyNeighborSelectorMethod(neighbors, playerPosition, playerDirection, carType, useWaypointPriority);
            }
#endif
            return -1;
        }

        /// <summary>
        /// Should be overriden in derived class
        /// </summary>
        /// <param name="neighbors">cell neighbors</param>
        /// <param name="playerPosition">position of the player</param>
        /// <param name="playerDirection">heading of the player</param>
        /// <param name="carType">type of car to instantiate</param>
        /// <returns></returns>
        protected int ApplyNeighborSelectorMethod(List<Vector2Int> neighbors, Vector3 playerPosition, Vector3 playerDirection, VehicleTypes carType, bool useWaypointPriority)
        {
            try
            {
                return spawnWaypointSelector(neighbors, playerPosition, playerDirection, carType, useWaypointPriority);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Your neighbor selector method has the following error: " + e.Message);
                return DefaultDelegates.GetRandomSpawnWaypoint(neighbors, playerPosition, playerDirection, carType, useWaypointPriority);
            }
        }


        /// <summary>
        /// Set the default waypoint generating method
        /// </summary>
        /// <param name="spawnWaypointSelector"></param>
        internal void SetSpawnWaypointSelector(SpawnWaypointSelector spawnWaypointSelector)
        {
            this.spawnWaypointSelector = spawnWaypointSelector;
        }


        /// <summary>
        /// Get position of the waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        internal Vector3 GetWaypointPosition(int waypointIndex)
        {
            return GetWaypoint<WaypointBase>(waypointIndex).position;
        }

        /// <summary>
        /// Get position of the target waypoint
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <returns></returns>
        internal Vector3 GetTargetWaypointPosition(int agentIndex)
        {
            try
            {
                return GetTargetWaypointOfAgent<WaypointBase>(agentIndex).position;
            }
            catch
            {
                return Vector3.zero;
            }
        }

        internal void SetAgentPath(int agentIndex, Queue<int> pathWaypoints)
        {
            if (!pathToDestination.ContainsKey(agentIndex))
            {
                pathToDestination.Add(agentIndex, pathWaypoints);
                hasPath[agentIndex] = true;
            }
            pathToDestination[agentIndex] = pathWaypoints;
        }

        internal void RemoveAgentPath(int agentIndex)
        {
            pathToDestination.Remove(agentIndex);
            hasPath[agentIndex] = false;
        }


        internal bool HasPath(int agentIndex)
        {
            return hasPath[agentIndex];
        }

        internal Queue<int> GetPath(int agentIndex)
        {
            if (pathToDestination.ContainsKey(agentIndex))
            {
                return pathToDestination[agentIndex];
            }
            return new Queue<int>();
        }


        /// <summary>
        /// Convert list of indexes to list of waypoints
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        protected Waypoint[] GetAllWaypoints(List<int> indexes)
        {
            Waypoint[] result = new Waypoint[indexes.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetWaypoint<Waypoint>(indexes[i]);
            }
            return result;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (debugWaypoints)
            {
                for (int i = 0; i < target.Length; i++)
                {
                    if (target[i] != -1)
                    {
                        Gizmos.color = Color.green;
                        Vector3 position = GetWaypoint<WaypointBase>(target[i]).position;
                        Gizmos.DrawSphere(position, 1);
                        position.y += 1.5f;
                        UnityEditor.Handles.Label(position, i.ToString());
                    }
                }
            }
            if (debugDisabledWaypoints)
            {
                for (int i = 0; i < disabledWaypoints.Count; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(disabledWaypoints[i].position, 1);
                }
            }
        }
#endif
    }
}