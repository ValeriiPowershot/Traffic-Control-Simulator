using System;
using System.Collections.Generic;
using BaseCode.Logic.EntityHandler.Vehicles.States;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Ways;
using UnityEngine;

namespace BaseCode.Logic.EntityHandler.Vehicles.Controllers
{
    public class VehiclePathController
    {
        private VehicleGoState _vehicleGoState;
        private WaypointContainer _waypointContainer;
        private Transform _endPoint;

        // points
        private readonly List<RoadPoint> _waypoints = new List<RoadPoint>();
        private int _currentWaypointIndex;

        public VehiclePathController(VehicleGoState vehicleGoState)
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
        
        public bool HasWaypoints()
        {
            return _waypoints.Count > 0;
        }
        public bool IsAtFinalWaypoint()
        {
            return _currentWaypointIndex == _waypoints.Count;
        }
        public RoadPoint GetCurrentWaypoint()
        {
            return _waypoints[_currentWaypointIndex];
        }
        public Transform GetEndPoint()
        {
            return _endPoint;
        }
        public void ProceedToNextWaypoint()
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _waypoints.Count)
            {
                _vehicleGoState.VehicleController.BasicVehicle.DestinationReached(); 
                _currentWaypointIndex = 0;
                CarTransform.position = GetCurrentWaypoint().point.position;
            }
        }
        
        public bool IsCloseToWaypoint()
        {
            Transform targetWaypoint = GetCurrentWaypoint().point;
            return Vector3.Distance(CarTransform.position, targetWaypoint.position) < 1f;
        }
        
        private Transform CarTransform => _vehicleGoState.CarTransform;

        private IPathFindingService PathFindingService => 
            _vehicleGoState.VehicleController.BasicVehicle.PathContainerService;
        
    }
}