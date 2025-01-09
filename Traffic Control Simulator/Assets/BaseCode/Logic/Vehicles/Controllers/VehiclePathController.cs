
using System.Collections.Generic;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Ways;
using Unity.VisualScripting;
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

        private const float TURN_ANGLE = 0.5f;
        private const int CHECK_COUNT = 12;

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
            CheckForTurn();

            if (_currentWaypointIndex >= _waypoints.Count)
            {
                VehicleController.BasicCar.DestinationReached(); 
                _currentWaypointIndex = 0;
                CarTransform.position = GetCurrentWaypoint().point.position;
            }
        }
        
        private void CheckForTurn()
        {
            int i = 0;
            if (_currentWaypointIndex + CHECK_COUNT < _waypoints.Count)
                i = _currentWaypointIndex + CHECK_COUNT;
            else
                i = _waypoints.Count - 1;

            Vector3 pointing = _waypoints[i].point.position - VehicleController.BasicCar.transform.position;
            pointing.Normalize();
            pointing -= VehicleController.BasicCar.transform.forward * 0.9f;
            pointing.Normalize();

            float dot = Vector3.Dot(pointing, VehicleController.BasicCar.transform.right);

            if (dot > TURN_ANGLE)
                VehicleController.BasicCar.ShowTurn(Vehicles.TurnType.Right);
            else if (dot < -TURN_ANGLE)
                VehicleController.BasicCar.ShowTurn(Vehicles.TurnType.Left);
            else
                VehicleController.BasicCar.ShowTurn(Vehicles.TurnType.None);
        }
        
        public bool IsCloseToWaypoint()
        {
            Transform targetWaypoint = GetCurrentWaypoint().point;
            return Vector3.Distance(CarTransform.position, targetWaypoint.position) < 1f;
        }
        
        private Transform CarTransform => _vehicleGoState.CarTransform;

        private IPathFindingService PathFindingService => 
            VehicleController.BasicCar.PathContainerService;
        
        private VehicleController VehicleController => _vehicleGoState.VehicleController;
    }
}