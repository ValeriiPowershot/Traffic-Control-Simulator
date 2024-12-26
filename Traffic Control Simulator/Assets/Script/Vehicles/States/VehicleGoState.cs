using System;
using System.Collections.Generic;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Ways;
using Script.ScriptableObject;
using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles.States
{
    public class VehicleGoState : IVehicleState
    {
        private readonly VehicleScriptableObject _carData;
        private WaypointContainer _waypointContainer;
        
        // points
        private readonly List<RoadPoint> _waypoints = new List<RoadPoint>();
        private int _currentWaypointIndex;
        
        // parameters from data
        private float _speed;
        private float _rayDistance;
        //private LayerMask _carLayer = LayerMask.GetMask("Car"); // Ensure cars are on a "Car" layer
        private LayerMask _stopLayer = 0;
        // controllers
        private Transform CarTransform => VehicleController.Vehicle.transform;
        public VehicleController VehicleController { get; set; }

        public VehicleGoState(VehicleController vehicleController)
        {
            _stopLayer += 1 << 7; //add car layer
            _stopLayer += 1 << 10; //add stop line layer
            VehicleController = vehicleController;
            _carData = VehicleController.Vehicle.VehicleScriptableObject;
            _rayDistance = _carData.rayDistance;
            _speed = _carData.NormalSpeed;
        }
        public void InitializePath()
        {
            _waypoints.Clear();
            _waypointContainer = VehicleController.Vehicle.WaypointContainer;
            _waypoints.AddRange(_waypointContainer.roadPoints);
        }

        public void MovementEnter()
        {
        }
        
        public void MovementUpdate() 
        {
            // we can implement a real fixupdate also, but i am not sure
            if (!HasWaypoints()) return;
            if (IsAtFinalWaypoint()) return;
            
            AdjustSpeed();
            
            MoveTowardsWaypoint();
            RotateTowardsWaypoint();
            if (IsCloseToWaypoint())
                ProceedToNextWaypoint();
            
            CheckForCollision();

        }

        public void MovementExit()
        {
            
        }

        private void CheckForCollision()
        {
            Ray ray = new Ray(VehicleController.Vehicle.rayStartPoint.position, CarTransform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance, _stopLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                
                if(hit.collider.TryGetComponent<Vehicle>(out Vehicle hitVehicle))
                {
                    if (AreTheyInIntersection(hitVehicle) && AreTheyUsingDifferentPath(hitVehicle))
                        Debug.Log("Game Is Over");
                    else if (VehicleController.Vehicle.CarLightState == LightState.Red)
                        VehicleController.SetState<VehicleStopState>();
                }
                else if (VehicleController.Vehicle.CarLightState == LightState.Red)
                    VehicleController.SetState<VehicleStopState>();
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.green);
            }
        }

        private bool AreTheyUsingDifferentPath(Vehicle hitVehicle)
        {
            return hitVehicle.lightPlaceSave != VehicleController.Vehicle.lightPlaceSave;
        }

        private bool AreTheyInIntersection(Vehicle hitVehicle)
        {
            return hitVehicle.lightPlaceSave != LightPlace.None && VehicleController.Vehicle.lightPlaceSave != LightPlace.None;
        }

        private void AdjustSpeed()
        {
            RoadPoint roadPoint = _waypoints[_currentWaypointIndex];

            _speed = roadPoint.roadPointType switch
            {
                RoadPointType.Slowdown => _carData.SlowDownSpeed,
                RoadPointType.Acceleration => _carData.AccelerationSpeed,
                _ => _carData.NormalSpeed
            };
        }
        private bool HasWaypoints()
        {
            return _waypoints.Count > 0;
        }
        private bool IsAtFinalWaypoint()
        {
            return _currentWaypointIndex == _waypoints.Count;
        }
        private void MoveTowardsWaypoint()
        {
            Transform targetWaypoint = _waypoints[_currentWaypointIndex].point;
            CarTransform.position = Vector3.MoveTowards(
                CarTransform.position, 
                targetWaypoint.position, 
                _speed * Time.fixedDeltaTime
            );
        }
        private void RotateTowardsWaypoint()
        {
            Transform targetWaypoint = _waypoints[_currentWaypointIndex].point;

            Vector3 direction = (targetWaypoint.position - CarTransform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            CarTransform.rotation = Quaternion.RotateTowards(
                CarTransform.rotation, 
                targetRotation, 
                _carData.RotationSpeed * Time.fixedDeltaTime
            );
        }
        private bool IsCloseToWaypoint()
        {
            Transform targetWaypoint = _waypoints[_currentWaypointIndex].point;

            return Vector3.Distance(CarTransform.position, targetWaypoint.position) < 1f;
        }
        private void ProceedToNextWaypoint()
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _waypoints.Count)
            {
                VehicleController.Vehicle.DestinationReached(); 
                _currentWaypointIndex = 0;
                CarTransform.position = _waypoints[_currentWaypointIndex].point.position;
            }
        }
    }
    
    [Serializable]
    public class RoadPoint
    {
        public Transform point;
        public RoadPointType roadPointType = RoadPointType.Normal;
    }
    public enum RoadPointType
    {
        Slowdown,
        Normal,
        Acceleration
    }
}