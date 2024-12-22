using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.Ways
{
    public class WaypointContainer : MonoBehaviour
    {
        [SerializeField] private List<Transform> _slowdownPoints;
        [SerializeField] private List<Transform> _accelerationPoints;

        public List<Transform> waypoints = new List<Transform>();
        public List<Transform> SlowdownPoints() => _slowdownPoints;
        public List<Transform> AccelerationPoints() => _accelerationPoints;
        
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
            Gizmos.color = Color.green;
            
            foreach (var waypoint in waypoints)
            {
                if(waypoint == null)
                    continue;
                Gizmos.DrawSphere(waypoint.position, 0.5f);
            }
        }
    }
}
