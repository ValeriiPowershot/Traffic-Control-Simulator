using System.Collections.Generic;
using BaseCode.Logic.Vehicles.States.Movement;
using UnityEngine;

namespace BaseCode.Logic.Ways
{
    public class WaypointContainer : MonoBehaviour
    {
        public List<RoadPoint> roadPoints = new();

        /// <summary>
        /// Установка точек с ручным списком (старый способ)
        /// </summary>
        public void SetRoadPoints(List<Transform> waypoints, List<Transform> decelerationPoints, List<Transform> accelerationPoints)
        {
            roadPoints.Clear();

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
                    roadPoints.Add(new RoadPoint
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

        /// <summary>
        /// Установка точек автоматически из родительского объекта
        /// </summary>
        public void SetRoadPointsFromParent(Transform parent, List<Transform> decelerationPoints = null, List<Transform> accelerationPoints = null)
        {
            roadPoints.Clear();

            decelerationPoints ??= new List<Transform>();
            accelerationPoints ??= new List<Transform>();

            foreach (Transform child in parent)
            {
                RoadPointType type = RoadPointType.Normal;

                if (decelerationPoints.Contains(child))
                    type = RoadPointType.Slowdown;
                else if (accelerationPoints.Contains(child))
                    type = RoadPointType.Acceleration;

                roadPoints.Add(new RoadPoint
                {
                    point = child,
                    roadPointType = type
                });
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (roadPoints.Count == 0)
                return;

            foreach (RoadPoint waypoint in roadPoints)
            {
                switch (waypoint.roadPointType)
                {
                    case RoadPointType.Normal:
                        Gizmos.color = Color.green;
                        break;
                    case RoadPointType.Slowdown:
                        Gizmos.color = Color.red;
                        break;
                    case RoadPointType.Acceleration:
                        Gizmos.color = Color.yellow;
                        break;
                }

                if (waypoint.point != null)
                    Gizmos.DrawSphere(waypoint.point.position, 0.5f);
            }
        }
    }
}
