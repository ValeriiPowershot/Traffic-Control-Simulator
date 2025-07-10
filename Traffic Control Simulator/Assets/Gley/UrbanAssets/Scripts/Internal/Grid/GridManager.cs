#if GLEY_PEDESTRIAN_SYSTEM || GLEY_TRAFFIC_SYSTEM
using Unity.Mathematics;
#endif

#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem;
#endif

#if GLEY_TRAFFIC_SYSTEM
using Gley.TrafficSystem;
using Gley.TrafficSystem.Internal;
#endif

using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Gley.UrbanAssets.Internal
{
    public class GridManager : MonoBehaviour
    {
        #region Traffic+Pedestrian
#if GLEY_PEDESTRIAN_SYSTEM || GLEY_TRAFFIC_SYSTEM
        protected CurrentSceneData currentSceneData;
        protected List<Vector2Int> activeCells;
        private List<Vector2Int> currentCells;
        private NativeArray<float3> activeCameraPositions;

        /// <summary>
        /// Initialize grid
        /// </summary>
        /// <typeparam name="T">Type of class that extends the grid manager</typeparam>
        /// <param name="currentSceneData">all waypoint information</param>
        /// <param name="activeCameraPositions">all active cameras</param>
        /// <returns></returns>
        internal GridManager Initialize(CurrentSceneData currentSceneData, NativeArray<float3> activeCameraPositions)
        {
            this.currentSceneData = currentSceneData;
            this.activeCameraPositions = activeCameraPositions;
            currentCells = new List<Vector2Int>();
            for (int i = 0; i < activeCameraPositions.Length; i++)
            {
                currentCells.Add(new Vector2Int());
            }
#if GLEY_TRAFFIC_SYSTEM
            UpdateActiveCells(activeCameraPositions, 1);
#endif
            return this;
        }


        internal CurrentSceneData GetCurrentSceneData()
        {
            return currentSceneData;
        }


        /// <summary>
        /// Get all specified neighbors for the specified depth
        /// </summary>
        /// <param name="row">current row</param>
        /// <param name="column">current column</param>
        /// <param name="depth">how far the cells should be</param>
        /// <param name="justEdgeCells">ignore middle cells</param>
        /// <returns>Returns the neighbors of the given cells</returns>
        internal List<Vector2Int> GetCellNeighbors(int row, int column, int depth, bool justEdgeCells)
        {
            List<Vector2Int> result = new List<Vector2Int>();

            int rowMinimum = row - depth;
            if (rowMinimum < 0)
            {
                rowMinimum = 0;
            }

            int rowMaximum = row + depth;
            if (rowMaximum >= currentSceneData.grid.Length)
            {
                rowMaximum = currentSceneData.grid.Length - 1;
            }


            int columnMinimum = column - depth;
            if (columnMinimum < 0)
            {
                columnMinimum = 0;
            }

            int columnMaximum = column + depth;
            if (columnMaximum >= currentSceneData.grid[row].row.Length)
            {
                columnMaximum = currentSceneData.grid[row].row.Length - 1;
            }

            for (int i = rowMinimum; i <= rowMaximum; i++)
            {
                for (int j = columnMinimum; j <= columnMaximum; j++)
                {
                    if (justEdgeCells)
                    {
                        if (i == row + depth || i == row - depth || j == column + depth || j == column - depth)
                        {
                            result.Add(new Vector2Int(i, j));
                        }
                    }
                    else
                    {
                        result.Add(new Vector2Int(i, j));
                    }
                }
            }
            return result;
        }


        internal List<Vector2Int> GetCellNeighbors(GridCell cell, int depth, bool justEdgeCells)
        {
            return GetCellNeighbors(cell.row, cell.column, depth, justEdgeCells);
        }


        /// <summary>
        /// Convert indexes to Grid cell
        /// </summary>
        /// <param name="xPoz"></param>
        /// <param name="zPoz"></param>
        /// <returns></returns>
        internal GridCell GetCell(float xPoz, float zPoz)
        {
            return currentSceneData.GetCell(xPoz, zPoz);
        }


        /// <summary>
        /// Convert position to Grid cell
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal GridCell GetCell(Vector3 position)
        {
            return currentSceneData.GetCell(position);
        }


        /// <summary>
        /// Convert cell index to Grid cell
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        internal GridCell GetCell(Vector2Int cellIndex)
        {
            return currentSceneData.grid[cellIndex.x].row[cellIndex.y];
        }


        internal float GetCellSize()
        {
            return GetCell(0, 0).size.x;
        }


        /// <summary>
        /// Get active cell for the active camera position
        /// </summary>
        /// <param name="activeCameraIndex"></param>
        /// <returns></returns>
        internal GridCell GetCell(int activeCameraIndex)
        {
            return GetCell(activeCameraPositions[activeCameraIndex].x, activeCameraPositions[activeCameraIndex].z);
        }


        /// <summary>
        /// Get position of the cell at index
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        internal Vector3 GetCellPosition(Vector2Int cellIndex)
        {
            return GetCell(cellIndex).center;
        }


        /// <summary>
        /// Convert position to cell index
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal Vector2Int GetCellIndex(Vector3 position)
        {
            int rowIndex = Mathf.FloorToInt(Mathf.Abs((currentSceneData.gridCorner.z - position.z) / currentSceneData.gridCellSize));
            int columnIndex = Mathf.FloorToInt(Mathf.Abs((currentSceneData.gridCorner.x - position.x) / currentSceneData.gridCellSize));
            return new Vector2Int(currentSceneData.grid[rowIndex].row[columnIndex].row, currentSceneData.grid[rowIndex].row[columnIndex].column);
        }



#endif
        #endregion


        #region Pedestrian
#if GLEY_PEDESTRIAN_SYSTEM
        internal List<SpawnWaypoint> GetPedestrianSpawnWaypointsForCell(Vector2Int cellIndex, PedestrianTypes agentType)
        {
            return currentSceneData.grid[cellIndex.x].row[cellIndex.y].pedestrianSpawnWaypoints.Where(cond1 => cond1.allowedAgent.Contains((int)agentType)).ToList();
        }

#endif
        #endregion


        #region Traffic
#if GLEY_TRAFFIC_SYSTEM

        private List<GenericIntersection> activeIntersections = new List<GenericIntersection>();

        /// <summary>
        /// Update active grid cells. Only active grid cells are allowed to perform additional operations like updating traffic lights  
        /// </summary>
        internal void UpdateGrid(int level, NativeArray<float3> activeCameraPositions)
        {
            UpdateActiveCells(activeCameraPositions, level);
        }

        /// <summary>
        /// Get active all intersections 
        /// </summary>
        /// <returns></returns>
        internal List<GenericIntersection> GetActiveIntersections()
        {
            return activeIntersections;
        }


        /// <summary>
        /// Create a list of active intersections
        /// </summary>
        void UpdateActiveIntersections()
        {
            List<int> intersectionIndexes = new List<int>();
            for (int i = 0; i < activeCells.Count; i++)
            {
                intersectionIndexes.AddRange(GetCell(activeCells[i]).intersectionsInCell.Except(intersectionIndexes));
            }

            List<GenericIntersection> result = new List<GenericIntersection>();
            for (int i = 0; i < intersectionIndexes.Count; i++)
            {
                switch (currentSceneData.allIntersections[intersectionIndexes[i]].type)
                {
                    case IntersectionType.TrafficLights:
                        result.Add(currentSceneData.allLightsIntersections[currentSceneData.allIntersections[intersectionIndexes[i]].index]);
                        break;
                    case IntersectionType.Priority:
                        result.Add(currentSceneData.allPriorityIntersections[currentSceneData.allIntersections[intersectionIndexes[i]].index]);
                        break;
                    case IntersectionType.LightsCrossing:
                        result.Add(currentSceneData.allLightsCrossings[currentSceneData.allIntersections[intersectionIndexes[i]].index]);
                        break;
                    case IntersectionType.PriorityCrossing:
                        result.Add(currentSceneData.allPriorityCrossings[currentSceneData.allIntersections[intersectionIndexes[i]].index]);
                        break;
                }
            }

            if (activeIntersections.Count == result.Count && activeIntersections.All(result.Contains))
            {

            }
            else
            {
                activeIntersections = result;
                IntersectionEvents.TriggetActiveIntersectionsChangedEvent(activeIntersections);
            }
        }


        /// <summary>
        /// Return all intersections
        /// </summary>
        /// <returns></returns>
        internal GenericIntersection[] GetAllIntersections()
        {
            GenericIntersection[] result = new GenericIntersection[currentSceneData.allIntersections.Length];
            for (int i = 0; i < currentSceneData.allIntersections.Length; i++)
            {
                switch (currentSceneData.allIntersections[i].type)
                {
                    case IntersectionType.TrafficLights:
                        result[i] = currentSceneData.allLightsIntersections[currentSceneData.allIntersections[i].index];
                        break;
                    case IntersectionType.Priority:
                        result[i] = currentSceneData.allPriorityIntersections[currentSceneData.allIntersections[i].index];
                        break;
                    case IntersectionType.LightsCrossing:
                        result[i] = currentSceneData.allLightsCrossings[currentSceneData.allIntersections[i].index];
                        break;
                    case IntersectionType.PriorityCrossing:
                        result[i] = currentSceneData.allPriorityCrossings[currentSceneData.allIntersections[i].index];
                        break;
                }
            }
            return result;
        }

        internal List<SpawnWaypoint> GetTrafficSpawnWaypointsForCell(Vector2Int cellIndex, VehicleTypes agentType)
        {
            List<SpawnWaypoint> spawnWaypoints = currentSceneData.grid[cellIndex.x].row[cellIndex.y].spawnWaypoints;

            return currentSceneData.grid[cellIndex.x].row[cellIndex.y].spawnWaypoints.Where(cond1 => cond1.allowedAgent.Contains((int)agentType)).ToList();
        }


        internal List<int> GetAllWaypoints(Vector2Int cellIndex)
        {
            return currentSceneData.grid[cellIndex.x].row[cellIndex.y].waypointsInCell;
        }

        /// <summary>
        /// Update active cells based on player position
        /// </summary>
        /// <param name="activeCameraPositions">position to check</param>
        private void UpdateActiveCells(NativeArray<float3> activeCameraPositions, int level)
        {
            this.activeCameraPositions = activeCameraPositions;

            if (currentCells.Count != activeCameraPositions.Length)
            {
                currentCells = new List<Vector2Int>();
                for (int i = 0; i < activeCameraPositions.Length; i++)
                {
                    currentCells.Add(new Vector2Int());
                }
            }

            bool changed = false;
            for (int i = 0; i < activeCameraPositions.Length; i++)
            {
                Vector2Int temp = GetCellIndex(activeCameraPositions[i]);
                if (currentCells[i] != temp)
                {
                    currentCells[i] = temp;
                    changed = true;
                }
            }

            if (changed)
            {
                activeCells = new List<Vector2Int>();
                for (int i = 0; i < activeCameraPositions.Length; i++)
                {
                    activeCells.AddRange(GetCellNeighbors(currentCells[i].x, currentCells[i].y, level, false));
                }
#if GLEY_TRAFFIC_SYSTEM
                UpdateActiveIntersections();
#endif
            }
        }
#endif
        #endregion
    }
}
