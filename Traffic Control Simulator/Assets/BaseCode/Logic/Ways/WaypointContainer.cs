using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseCode.Logic.Ways
{
    public class WaypointContainer : MonoBehaviour
    {
        [SerializeField] private Transform _slowdownPoint;
        [SerializeField] private Transform _accelerationPoint;

        public List<Transform> Waypoints = new List<Transform>();
        public int SlowdownPointIndex { get; set; }
        public int AccelerationPointIndex { get; set; }

        /*private void OnValidate()
        {
            List<Transform> childWaypoints = GetComponentsInChildren<Transform>()
                .Where(t => t != transform)
                .ToList();
        
            Waypoints = childWaypoints;
        
            SlowdownPointIndex = GetWaypointIndex(_slowdownPoint);
            AccelerationPointIndex = GetWaypointIndex(_accelerationPoint);
        }
    
        private int GetWaypointIndex(Transform target)
        {
            return Waypoints.IndexOf(target);
        }*/

        public void OnDrawGizmos()
        {
            foreach (var waypoint in Waypoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(waypoint.position, 0.5f);
            }
        }
    }
}
