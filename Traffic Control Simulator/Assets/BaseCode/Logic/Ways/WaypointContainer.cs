using System;
using System.Linq;
using UnityEngine;

namespace BaseCode.Logic.Ways
{
    public class WaypointContainer : MonoBehaviour
    {
        [SerializeField] private Transform _slowdownPoint;
        [SerializeField] private Transform _accelerationPoint;
    
        public Transform[] Waypoints;

        public int SlowdownPointIndex { get; set; }
        public int AccelerationPointIndex { get; set; }

        private void OnValidate()
        {
            Transform[] childWaypoints = GetComponentsInChildren<Transform>()
                .Where(t => t != transform)
                .ToArray();
        
            Waypoints = childWaypoints;
        
            SlowdownPointIndex = GetWaypointIndex(_slowdownPoint);
            AccelerationPointIndex = GetWaypointIndex(_accelerationPoint);
        }
    
        private int GetWaypointIndex(Transform target)
        {
            return Array.IndexOf(Waypoints, target);
        }
    }
}
