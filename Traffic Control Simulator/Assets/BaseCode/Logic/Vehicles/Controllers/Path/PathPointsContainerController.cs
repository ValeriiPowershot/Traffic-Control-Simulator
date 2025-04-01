using System.Collections.Generic;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Path
{
    public class PathPointsContainerController
    {
        private readonly VehiclePathController _vehiclePathController;
        private readonly PathContainer _pathContainer;
        
        private readonly List<RoadPoint> _waypoints = new List<RoadPoint>();
        private Transform _endPoint;
        private int _currentWaypointIndex;

        public PathPointsContainerController(VehiclePathController vehiclePathController)
        {
            _vehiclePathController = vehiclePathController;
            _pathContainer= new PathContainer(VehicleBase);
        }
        public void AssignFoundPath()
        {
            _waypoints.Clear();
            _waypoints.AddRange(_pathContainer.GetPathContainer().roadPoints);
            
            _endPoint = _waypoints[^1].point;
            _currentWaypointIndex = 0;
        }
        public bool CanProceedToEndCar()
        {
            _currentWaypointIndex++;

            if (IsEndPoint())
            {
                OnReachDestination();
                return true;
            }

            return false;
        }
        public void OnReachDestination()
        {
            _currentWaypointIndex = 0;
            _waypoints.Clear();
        }
        public bool IsCloseToWaypoint(Vector3 position)
        {
            Transform targetWaypoint = GetCurrentWaypoint().point;
            return Vector3.Distance(position, targetWaypoint.position) < 1f;
        }
        
        public bool HasWaypoints() =>
            _waypoints.Count > 0;
        public void SetCurrentToEndPosition()
        {
            _currentWaypointIndex = _waypoints.Count-1;
        }
        public bool IsAtFinalWaypoint() =>
            _currentWaypointIndex == _waypoints.Count;
        
        public RoadPoint GetCurrentWaypoint() =>
            _waypoints[_currentWaypointIndex];
        public Vector3 GetCurrentWaypointPosition() =>
            GetCurrentWaypoint().point.position;
        public Transform GetEndPoint() =>
            _endPoint;
        private bool IsEndPoint() => 
            _currentWaypointIndex >= _waypoints.Count;

        public PathContainer PathContainer => _pathContainer;
        public VehicleBase VehicleBase => _vehiclePathController.VehicleController.VehicleBase;
    }
}