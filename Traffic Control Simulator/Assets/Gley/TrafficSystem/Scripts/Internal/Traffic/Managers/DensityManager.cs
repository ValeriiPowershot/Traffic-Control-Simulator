using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
#if GLEY_TRAFFIC_SYSTEM
using Unity.Mathematics;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Controls the number of active vehicles
    /// </summary>
    public class DensityManager : MonoBehaviour
    {
        private PositionValidator positionValidator;
        private GridManager gridManager;
#if GLEY_TRAFFIC_SYSTEM
        private TrafficVehicles trafficVehicles;
        private WaypointManager waypointManager;
        private Queue<RequestVehicle> requestedVehicles;
        private int maxNrOfVehicles;
        private int currentnrOfVehicles;
        private int activeSquaresLevel;
        private bool useWaypointPriority;

        enum Category
        {
            Idle,
            Ignored,
        }

        class RequestVehicle
        {
            public int waypoint;
            public VehicleTypes type;
            public VehicleComponent vehicle;
            public Category category;
            public UnityAction<VehicleComponent, int> completeMethod;
            public List<int> path;

            public RequestVehicle(int waypoint, VehicleTypes type, Category category, VehicleComponent vehicle, UnityAction<VehicleComponent, int> completeMethod, List<int> path)
            {
                this.waypoint = waypoint;
                this.type = type;
                this.category = category;
                this.vehicle = vehicle;
                this.completeMethod = completeMethod;
                this.path = path;
            }
        }

        public DensityManager Initialize(TrafficVehicles trafficVehicles, WaypointManager waypointManager, GridManager gridManager, PositionValidator positionValidator, NativeArray<float3> activeCameraPositions, int maxNrOfVehicles, Vector3 playerPosition, Vector3 playerDirection, int activeSquaresLevel, bool useWaypointPriority, int initialDensity, Area disableWaypointsArea)
        {
            this.positionValidator = positionValidator;
            this.trafficVehicles = trafficVehicles;
            this.waypointManager = waypointManager;
            this.activeSquaresLevel = activeSquaresLevel;
            this.gridManager = gridManager;
            this.maxNrOfVehicles = maxNrOfVehicles;
            this.useWaypointPriority = useWaypointPriority;

            requestedVehicles = new Queue<RequestVehicle>();

            List<GridCell> gridCells = new List<GridCell>();
            for (int i = 0; i < activeCameraPositions.Length; i++)
            {
                gridCells.Add(gridManager.GetCell(activeCameraPositions[i].x, activeCameraPositions[i].z));
            }
            if (initialDensity >= 0)
            {
                SetTrafficDensity(initialDensity);
            }

            if (disableWaypointsArea.radius > 0)
            {
                DisableAreaWaypoints(disableWaypointsArea);
            }

            LoadInitialVehicles(gridCells, playerPosition, playerDirection);

            return this;
        }


        /// <summary>
        /// Change vehicle density
        /// </summary>
        /// <param name="nrOfVehicles">cannot be greater than max vehicle number set on initialize</param>
        public void SetTrafficDensity(int nrOfVehicles)
        {
            maxNrOfVehicles = nrOfVehicles;
        }


        internal void UpdateActiveSquares(int newLevel)
        {
            activeSquaresLevel = newLevel;
        }


        /// <summary>
        /// Add all vehicles around the player even if they are inside players view
        /// </summary>
        /// <param name="currentGridRow"></param>
        /// <param name="currentGridColumn"></param>
        private void LoadInitialVehicles(List<GridCell> gridCells, Vector3 playerPosition, Vector3 playerDirection)
        {
            for (int i = 0; i < maxNrOfVehicles; i++)
            {
                int cellIndex = UnityEngine.Random.Range(0, gridCells.Count);
                BeginAddVehicleProcess(playerPosition, playerDirection, gridCells[cellIndex], true);
            }
        }


        /// <summary>
        /// Ads new vehicles if required
        /// </summary>
        public void UpdateVehicleDensity(Vector3 playerPosition, Vector3 playerDirection, int activeCameraIndex)
        {
            if (currentnrOfVehicles < maxNrOfVehicles)
            {
                GridCell gridCell = gridManager.GetCell(activeCameraIndex);
                BeginAddVehicleProcess(playerPosition, playerDirection, gridCell, false);
            }
        }


        void BeginAddVehicleProcess(Vector3 playerPosition, Vector3 playerDirection, GridCell gridCell, bool ignorLOS)
        {
            if (requestedVehicles.Count == 0)
            {
                AddVehicleOnArea(playerPosition, playerDirection, gridCell, ignorLOS);
            }
            else
            {
                //add specific vehicle on position
                var requested = requestedVehicles.Peek();
                switch (requested.category)
                {
                    case Category.Idle:
                        if (requested.vehicle == null)
                        {
                            int idleVehicleIndex = trafficVehicles.GetIdleVehicleIndex(requested.type);
                            //if an idle vehicle does not exists
                            if (idleVehicleIndex == -1)
                            {
                                AddVehicleOnArea(playerPosition, playerDirection, gridCell, ignorLOS);
                                return;
                            }
                            requested.vehicle = trafficVehicles.GetIdleVehicle(idleVehicleIndex);
                            if (requested.vehicle == null)
                            {
                                return;
                            }
                        }
                        break;

                    case Category.Ignored:
                       
                        if (requested.vehicle.gameObject.activeSelf)
                        {
                            AddVehicleOnArea(playerPosition, playerDirection, gridCell, ignorLOS);
                            return;
                        }
                        break;
                }


                if (AddVehicle(true, requested.waypoint, requested.vehicle))
                {
                    var request = requestedVehicles.Dequeue();
                    request.completeMethod?.Invoke(request.vehicle, request.waypoint);
                    if (request.path != null)
                    {
                        waypointManager.SetAgentPath(request.vehicle.GetIndex(), new Queue<int>(request.path));
                    }
                }
                else
                {
                    AddVehicleOnArea(playerPosition, playerDirection, gridCell, ignorLOS);
                }
            }
        }


        void AddVehicleOnArea(Vector3 playerPosition, Vector3 playerDirection, GridCell gridCell, bool ignorLOS)
        {
            //add any vehicle on area
            int idleVehicleIndex = trafficVehicles.GetIdleVehicleIndex();

            //if an idle vehicle does not exists
            if (idleVehicleIndex == -1)
            {
                return;
            }

            int freeWaypointIndex = waypointManager.GetNeighborCellWaypoint(gridCell.row, gridCell.column, activeSquaresLevel, trafficVehicles.GetIdleVehicleType(idleVehicleIndex), playerPosition, playerDirection, useWaypointPriority);
            //Debug.Log(freeWaypointIndex);
            //freeWaypointIndex = 3;

            if (freeWaypointIndex == -1)
            {
                return;
            }

            AddVehicle(ignorLOS, freeWaypointIndex, trafficVehicles.PeakIdleVehicle(idleVehicleIndex));
        }


        internal void AddExcludedVehicle(int vehicleIndex, Vector3 position, UnityAction<VehicleComponent, int> completeMethod)
        {
            if (position == Vector3.zero)
            {
                return;
            }

            if (!trafficVehicles.VehicleIsExcluded(vehicleIndex))
            {
                Debug.LogWarning($"vehicleIndex {vehicleIndex} is not marked as ignored, it will not be instantiated");
                return;
            }
            VehicleComponent vehicle = trafficVehicles.GetExcludedVehicle(vehicleIndex);
            VehicleTypes type = vehicle.GetVehicleType();
            int waypointIndex = GetClosestSpawnWaypoint(position, type);
            if (waypointIndex >= 0)
            {
                requestedVehicles.Enqueue(new RequestVehicle(waypointIndex, type, Category.Ignored, trafficVehicles.GetExcludedVehicle(vehicleIndex), completeMethod, null));
            }
            else
            {
                Debug.LogWarning("No waypoint found!");
            }
        }


        public void AddVehicleAtPosition(Vector3 position, VehicleTypes type, UnityAction<VehicleComponent, int> completeMethod, List<int> path)
        {
            int waypointIndex = GetClosestSpawnWaypoint(position, type);

            if(waypointIndex<0)
            {
                Debug.LogWarning("There are no free waypoints in the current cell");
                return;
            }

            requestedVehicles.Enqueue(new RequestVehicle(waypointIndex, type, Category.Idle, null, completeMethod, path));
        }


        /// <summary>
        /// Remove a vehicle if required
        /// </summary>
        /// <param name="index">vehicle to remove</param>
        /// <param name="force">remove the vehicle even if not all conditions for removing are met</param>
        /// <returns>true if a vehicle was really removed</returns>
        public void RemoveVehicle(int index)
        {
            currentnrOfVehicles--;
        }


        /// <summary>
        /// Update the active camera used to determine if a vehicle is in view
        /// </summary>
        /// <param name="activeCamerasPosition"></param>
        public void UpdateCameraPositions(Transform[] activeCameras)
        {
            positionValidator.UpdateCamera(activeCameras);
        }


        /// <summary>
        /// Trying to load an idle vehicle if exists
        /// </summary>
        /// <param name="firstTime">initial load</param>
        /// <param name="currentGridRow"></param>
        /// <param name="currentGridColumn"></param>
        private bool AddVehicle(bool firstTime, int freeWaypointIndex, VehicleComponent vehicle)
        {
            //Debug.Log(freeWaypointIndex);
            //if a valid waypoint was found, check if it was not manually disabled
            if (waypointManager.IsDisabled(freeWaypointIndex))
            {
                return false;
            }

            //check if the car type can be instantiated on selected waypoint
            if (!positionValidator.IsValid(waypointManager.GetWaypointPosition(freeWaypointIndex), vehicle.length * 2, vehicle.coliderHeight, vehicle.colliderWidth, firstTime, vehicle.frontTrigger.localPosition.z, waypointManager.GetNextOrientation(freeWaypointIndex)))
            {
                return false;
            }

            Quaternion trailerRotaion = Quaternion.identity;
            if (vehicle.trailer != null)
            {
                trailerRotaion = waypointManager.GetPrevOrientation(freeWaypointIndex);
                if (trailerRotaion == Quaternion.identity)
                {
                    trailerRotaion = waypointManager.GetNextOrientation(freeWaypointIndex);
                }

                if (!positionValidator.CheckTrailerPosition(waypointManager.GetWaypointPosition(freeWaypointIndex), waypointManager.GetNextOrientation(freeWaypointIndex), trailerRotaion, vehicle))
                {
                    return false;
                }
            }

            currentnrOfVehicles++;
            int index = vehicle.GetIndex();
            waypointManager.SetTargetWaypoint(index, freeWaypointIndex);
            trafficVehicles.ActivateVehicle(vehicle, waypointManager.GetTargetWaypointPosition(index), waypointManager.GetTargetWaypointRotation(index), trailerRotaion);
            Events.TriggerVehicleAddedEvent(index);
            return true;
        }


        /// <summary>
        /// Makes waypoints on a given radius unavailable
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void DisableAreaWaypoints(Area area)
        {
            GridCell cell = gridManager.GetCell(area.center);
            List<Vector2Int> neighbors = gridManager.GetCellNeighbors(cell.row, cell.column, Mathf.CeilToInt(area.radius * 2 / cell.size.x), false);
            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                cell = gridManager.GetCell(neighbors[i]);
                for (int j = 0; j < cell.waypointsInCell.Count; j++)
                {
                    Waypoint waypoint = waypointManager.GetWaypoint<Waypoint>(cell.waypointsInCell[j]);
                    if (Vector3.SqrMagnitude(area.center - waypoint.position) < area.sqrRadius)
                    {
                        waypointManager.AddDisabledWaypoint(waypoint);
                    }
                }
            }
        }


        internal int GetClosestSpawnWaypoint(Vector3 position, VehicleTypes type)
        {
            List<SpawnWaypoint> possibleWaypoints = gridManager.GetCell(position.x, position.z).spawnWaypoints.Where(cond1 => cond1.allowedAgent.Contains((int)type)).ToList();

            if (possibleWaypoints.Count == 0)
                return -1;

            float distance = float.MaxValue;
            int waypointIndex = -1;
            for (int i = 0; i < possibleWaypoints.Count; i++)
            {
                float newDistance = Vector3.SqrMagnitude(waypointManager.GetWaypointPosition(possibleWaypoints[i].waypointIndex) - position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    waypointIndex = possibleWaypoints[i].waypointIndex;
                }
            }
            return waypointIndex;
        }


        internal int GetClosestWayoint(Vector3 position, VehicleTypes type)
        {
            List<SpawnWaypoint> possibleWaypoints = gridManager.GetCell(position.x, position.z).spawnWaypoints.Where(cond1 => cond1.allowedAgent.Contains((int)type)).ToList();

            if (possibleWaypoints.Count == 0)
                return -1;

            float distance = float.MaxValue;
            int waypointIndex = -1;
            for (int i = 0; i < possibleWaypoints.Count; i++)
            {
                float newDistance = Vector3.SqrMagnitude(waypointManager.GetWaypointPosition(possibleWaypoints[i].waypointIndex) - position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    waypointIndex = possibleWaypoints[i].waypointIndex;
                }
            }
            return waypointIndex;
        }
#endif
    }
}