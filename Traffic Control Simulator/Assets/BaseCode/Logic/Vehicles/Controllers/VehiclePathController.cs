using System.Collections.Generic;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Ways;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers
{
    public class VehiclePathController
    {
        private VehicleMovementGoState _vehicleGoState;
        private WaypointContainer _waypointContainer;

        // points
        private Transform _endPoint;
        private readonly List<RoadPoint> _waypoints = new List<RoadPoint>();
        private int _currentWaypointIndex;
        
        public VehiclePathController(VehicleMovementGoState vehicleGoState)
        {
            _vehicleGoState = vehicleGoState;
        }
        
        public void InitializePath()
        {
            _waypoints.Clear();
            _waypointContainer = PathFindingService.GetPathContainer();
            _waypoints.AddRange(_waypointContainer.roadPoints);
            _endPoint = _waypoints[^1].point;
        }
        
        public bool HasWaypoints() =>
            _waypoints.Count > 0;

        public bool IsAtFinalWaypoint() =>
            _currentWaypointIndex == _waypoints.Count;
        
        public RoadPoint GetCurrentWaypoint() =>
            _waypoints[_currentWaypointIndex];

        public RoadPoint GetPointIndex(int i) =>_waypoints[i];
        public RoadPoint GetPointIndexReverse(int i) => _waypoints[_waypoints.Count - 1 - i];
        
        public Transform GetEndPoint() =>
            _endPoint;
        
        public void ProceedToNextWaypoint()
        {
            _currentWaypointIndex++;

            if (!IsEndPoint()) return;
            
            VehicleController.VehicleBase.DestinationReached(); 
            _currentWaypointIndex = 0;
            CarTransform.position = GetCurrentWaypoint().point.position;
        }
        private bool IsEndPoint() => _currentWaypointIndex >= _waypoints.Count;

        public void SetPathToEndPosition(Vector3 originalY)
        {
            SetPathToEndPosition();
            CarTransform.localScale= originalY;
        }

        public void SetPathToEndPosition()
        {
            _currentWaypointIndex = _waypoints.Count-1;
            CarTransform.position = GetCurrentWaypoint().point.position;            
        }        
        
        public bool IsCloseToWaypoint()
        {
            Transform targetWaypoint = GetCurrentWaypoint().point;
            return Vector3.Distance(CarTransform.position, targetWaypoint.position) < 1f;
        }
        
        private Transform CarTransform => _vehicleGoState.CarTransform;

        private IPathFindingService PathFindingService => 
            VehicleController.VehicleBase.PathContainerService;
        
        private VehicleController VehicleController => _vehicleGoState.VehicleController;


    }
}