using System.Collections.Generic;
using UnityEngine;
namespace Gley.TrafficSystem.Internal
{
    public class IntersectionEvents : MonoBehaviour
    {
        /// <summary>
        /// Triggered when active intersections modify
        /// </summary>
        /// <param name="activeIntersections"></param>
        public delegate void ActiveIntersectionsChanged(List<GenericIntersection> activeIntersections);
        public static event ActiveIntersectionsChanged onActiveIntersectionsChanged;
        public static void TriggetActiveIntersectionsChangedEvent(List<GenericIntersection> activeIntersections)
        {
            if (onActiveIntersectionsChanged != null)
            {
                onActiveIntersectionsChanged(activeIntersections);
            }
        }
    }
}
