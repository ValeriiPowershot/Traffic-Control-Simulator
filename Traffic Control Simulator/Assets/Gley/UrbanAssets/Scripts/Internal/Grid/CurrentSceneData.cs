#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Internal;
#endif

#if GLEY_TRAFFIC_SYSTEM
using Gley.TrafficSystem.Internal;
#endif

using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanAssets.Internal
{
    public class CurrentSceneData : MonoBehaviour
    {
        public int gridCellSize = 50;
        public Vector3 gridCorner;
        public GridRow[] grid;
#if GLEY_TRAFFIC_SYSTEM
        public Waypoint[] allWaypoints;
        public IntersectionData[] allIntersections;
        public PriorityIntersection[] allPriorityIntersections;
        public TrafficLightsIntersection[] allLightsIntersections;
        public TrafficLightsCrossing[] allLightsCrossings;
        public PriorityCrossing[] allPriorityCrossings;
#endif

#if GLEY_PEDESTRIAN_SYSTEM
        public PedestrianWaypoint[] allPedestrianWaypoints;

        public void AssignIntersections(List<int> pedestrianWaypoints, IIntersection associatedIntersection)
        {
            for (int j = 0; j < pedestrianWaypoints.Count; j++)
            {
                allPedestrianWaypoints[pedestrianWaypoints[j]].SetIntersection(associatedIntersection);
            }
        }
#endif

#if GLEY_TRAFFIC_SYSTEM
#if GLEY_PEDESTRIAN_SYSTEM
        public void AssignIntersections(List<IntersectionStopWaypointsIndex> enterWaypoints, IIntersection associatedIntersection)
        {
            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                for (int j = 0; j < enterWaypoints[i].pedestrianWaypoints.Count; j++)
                {
                    allPedestrianWaypoints[enterWaypoints[i].pedestrianWaypoints[j]].SetIntersection(associatedIntersection);
                }
                for (int j = 0; j < enterWaypoints[i].directionWaypoints.Count; j++)
                {
                    allPedestrianWaypoints[enterWaypoints[i].directionWaypoints[j]].crossing = true;
                }
            }
        }

        public void AssignIntersections(List<int> pedestrianWaypoints, List<int> directionWaypoints, IIntersection associatedIntersection)
        {
            for (int j = 0; j < pedestrianWaypoints.Count; j++)
            {
                allPedestrianWaypoints[pedestrianWaypoints[j]].SetIntersection(associatedIntersection);
            }

            for (int j = 0; j < directionWaypoints.Count; j++)
            {
                allPedestrianWaypoints[directionWaypoints[j]].crossing = true;
            }
        }
#endif
#endif

        /// <summary>
        /// Get scene data object from active scene 
        /// </summary>
        /// <returns></returns>
        public static CurrentSceneData GetSceneInstance()
        {
            CurrentSceneData[] allSceneGrids = FindObjectsByType<CurrentSceneData>(FindObjectsSortMode.None);
            if (allSceneGrids.Length > 1)
            {
                Debug.LogError("Multiple Grid components exists in scene. Just one is required, delete extra components before continuing.");
                for (int i = 0; i < allSceneGrids.Length; i++)
                {
                    Debug.LogWarning("Grid component exists on: " + allSceneGrids[i].name, allSceneGrids[i]);
                }
            }

            if (allSceneGrids.Length == 0)
            {
                GameObject go = new GameObject(Constants.gleyTrafficHolderName);
                CurrentSceneData grid = go.AddComponent<CurrentSceneData>();
                return grid;
            }
            return allSceneGrids[0];
        }


        /// <summary>
        /// Convert position to Grid cell
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public GridCell GetCell(Vector3 position)
        {
            return GetCell(position.x, position.z);
        }


        /// <summary>
        /// Convert indexes to Grid cell
        /// </summary>
        /// <param name="xPoz"></param>
        /// <param name="zPoz"></param>
        /// <returns></returns>
        public GridCell GetCell(float xPoz, float zPoz)
        {
            int rowIndex = Mathf.FloorToInt(Mathf.Abs((gridCorner.z - zPoz) / gridCellSize));
            int columnIndex = Mathf.FloorToInt(Mathf.Abs((gridCorner.x - xPoz) / gridCellSize));
            return grid[rowIndex].row[columnIndex];
        }
    }
}