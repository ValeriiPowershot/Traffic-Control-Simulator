using System.Collections.Generic;
using BaseCode.Logic.Vehicles.States.Movement;
using UnityEngine;

namespace BaseCode.Logic.Ways
{
    public class WaypointContainer : MonoBehaviour
    {
        public List<RoadPoint> roadPoints = new();

        public void SetRoadPoints(List<Transform> waypoints,List<Transform> decelerationPoints,List<Transform> accelerationPoints)
        {
            foreach (Transform waypoint in waypoints)
            {
                if (decelerationPoints.Contains(waypoint))
                {
                    roadPoints.Add(new RoadPoint
                    {
                        point = waypoint, roadPointType = RoadPointType.Slowdown
                    });    
                }
                else if (accelerationPoints.Contains(waypoint))
                {
                    roadPoints.Add(
                        new RoadPoint
                        {
                            point = waypoint, roadPointType = RoadPointType.Acceleration
                        });    
                }
                else
                {
                    roadPoints.Add(new RoadPoint
                    {
                        point = waypoint, roadPointType = RoadPointType.Normal
                    });
                }
            }
        }
        
        public void OnDrawGizmosSelected()
        {
            if(roadPoints.Count == 0)
                return;
            
            foreach (RoadPoint waypoint in roadPoints)
            {
                switch (waypoint.roadPointType)
                {
                    case RoadPointType.Normal:
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(waypoint.point.transform.position, 0.5f);
                        break;
                    case RoadPointType.Slowdown:
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(waypoint.point.transform.position, 0.5f);
                        break;
                    case RoadPointType.Acceleration:
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(waypoint.point.transform.position, 0.5f);
                        break;
                }
            }
         
        }
    }
}
