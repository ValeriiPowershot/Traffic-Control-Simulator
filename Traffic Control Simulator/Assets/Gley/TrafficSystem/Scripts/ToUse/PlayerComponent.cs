using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem
{
    public class PlayerComponent : MonoBehaviour, ITrafficParticipant
    {
        private Rigidbody rb;
        #if GLEY_TRAFFIC_SYSTEM
        private Transform myTransform;
        private GridManager trafficGridManager;
        private GridCell currentCell;
        private WaypointManager waypointManager;
        private List<Waypoint> allWaypoints = new List<Waypoint>();
        private List<Vector2Int> cellNeighbors = new List<Vector2Int>();

        private Waypoint proposedTarget;
        private Waypoint currentTarget;

        bool initialized;
        bool targetChanged;

        public delegate void LaneChange();
        public event LaneChange onLaneChange;

        void TriggetChangeDrivingStateEvent(Waypoint fromWaypoint, Waypoint toWaypoint)
        {
            if (onLaneChange != null)
            {
                if (!waypointManager.HaveCommonNeighbors(fromWaypoint, toWaypoint))
                {
                    if (!fromWaypoint.name.Contains(Gley.UrbanAssets.Internal.Constants.connect) && !toWaypoint.name.Contains(Gley.UrbanAssets.Internal.Constants.connect))
                    {
                        onLaneChange();
                    }
                }
            }
        }


        private void Start()
        {
            StartCoroutine(Initialize());
        }


        IEnumerator Initialize()
        {
            while (!TrafficManager.Instance.IsInitialized())
            {
                yield return null;
            }
            rb = GetComponent<Rigidbody>();
            myTransform = transform;
            trafficGridManager = TrafficManager.Instance.GridManager;
            waypointManager = TrafficManager.Instance.WaypointManager;
            waypointManager.RegisterPlayer(GetInstanceID(), -1);
            initialized = true;
        }


        // Update is called once per frame
        void Update()
        {
            if (initialized)
            {
                GridCell cell = trafficGridManager.GetCell(myTransform.position);
                if (cell != currentCell)
                {
                    currentCell = cell;
                    cellNeighbors = trafficGridManager.GetCellNeighbors(cell.row, cell.column, 1, false);
                    allWaypoints = new List<Waypoint>();
                    for (int i = 0; i < cellNeighbors.Count; i++)
                    {
                        List<int> cellWaypoints = trafficGridManager.GetAllWaypoints(cellNeighbors[i]);
                        for (int j = 0; j < cellWaypoints.Count; j++)
                        {
                            allWaypoints.Add(waypointManager.GetWaypoint<Waypoint>(cellWaypoints[j]));
                        }
                    }
                }

                float oldDistance = Mathf.Infinity;
                for (int i = 0; i < allWaypoints.Count; i++)
                {
                    float newDistance = Vector3.SqrMagnitude(myTransform.position - allWaypoints[i].position);
                    if (newDistance < oldDistance)
                    {
                        if (CheckOrientation(allWaypoints[i]))
                        {
                            oldDistance = newDistance;
                        }
                    }
                }
                if (currentTarget != proposedTarget)
                {
                    targetChanged = false;
                    if (currentTarget != null)
                    {
                        if (currentTarget.neighbors.Contains(proposedTarget.listIndex))
                        {
                            targetChanged = true;
                        }
                        else
                        {
                            float angle1 = Vector3.SignedAngle(myTransform.forward, proposedTarget.position - myTransform.position, Vector3.up);
                            float angle2 = Vector3.SignedAngle(myTransform.forward, currentTarget.position - myTransform.position, Vector3.up);                          
                            if (Mathf.Abs(angle1) < Mathf.Abs(angle2))
                            {
                                targetChanged = true;
                                TriggetChangeDrivingStateEvent(currentTarget, proposedTarget);
                            }
                            else
                            {
                                float distance1 = Vector3.SqrMagnitude(myTransform.position - proposedTarget.position);
                                float distance2 = Vector3.SqrMagnitude(myTransform.position - currentTarget.position);
                                if (distance1 * distance1 < distance2)
                                {
                                    targetChanged = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        targetChanged = true;
                    }

                    if (targetChanged)
                    {
                        currentTarget = proposedTarget;
                        waypointManager.UpdatePlayerWaypoint(GetInstanceID(), proposedTarget.listIndex);
                    }
                }
            }
        }


        private bool CheckOrientation(Waypoint waypoint)
        {
            if (waypoint.neighbors.Count < 1)
                return false;

            Waypoint neighbor = waypointManager.GetWaypoint<Waypoint>(waypoint.neighbors[0]);
            float angle = Vector3.SignedAngle(myTransform.forward, neighbor.position - waypoint.position, Vector3.up);
            if (Math.Abs(angle) < 90)
            {
                proposedTarget = neighbor;
                return true;
            }
            return false;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                if (initialized)
                {
                    if (TrafficManager.Instance.IsDebugWaypointsEnabled())
                    {
                        if (currentTarget != null)
                        {
                            Gizmos.color = Color.green;
                            Vector3 position = currentTarget.position;
                            Gizmos.DrawSphere(position, 1);
                        }
                    }
                }
            }
        }
#endif
#endif

        public float GetCurrentSpeedMS()
        {
            return rb.velocity.magnitude;
        }
    }

}
