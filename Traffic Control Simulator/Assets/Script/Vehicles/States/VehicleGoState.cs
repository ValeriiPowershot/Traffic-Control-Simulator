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
        
        public AllWaysContainer _allWaysContainer;
        private WaypointContainer _waypointContainer;
        
        // points
        private readonly List<RoadPoint> _waypoints = new List<RoadPoint>();
        private int _currentWaypointIndex;
        
        // parameters from data
        private float _speed;
        private float _rayDistance;
        private LayerMask _carLayer = LayerMask.GetMask("Car"); // Ensure cars are on a "Car" layer

        // controllers
        private Transform transform => VehicleController.Vehicle.transform;
        public VehicleController VehicleController { get; set; }

        public VehicleGoState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
            _allWaysContainer = VehicleController.Vehicle.AllWaysContainer; 
            _carData = VehicleController.Vehicle.VehicleScriptableObject;

            _rayDistance = _carData.rayDistance;
        }
        
        public void InitializePath()
        {
            if (_allWaysContainer.AllWays.Length == 0)
            {
                Debug.LogError("No waypoints found, Add A Path Container");
            }
            _waypointContainer = VehicleController.Vehicle.WaypointContainer;
//            _waypointContainer = _allWaysContainer.AllWays[_carData.indexPath];
//            _waypointContainer = _allWaysContainer.AllWays[Random.Range(0, _allWaysContainer.AllWays.Length)];

            List<Transform> slowdownPoints = _waypointContainer.SlowdownPoints();
            List<Transform> accelerationPoints = _waypointContainer.AccelerationPoints();
            
            foreach (var waypoint in _waypointContainer.waypoints)
            {
                if (slowdownPoints.Contains(waypoint))
                {
                    _waypoints.Add(
                        new RoadPoint() { point = waypoint, roadPointType = RoadPointType.Slowdown}
                    );    
                }
                else if (accelerationPoints.Contains(waypoint))
                {
                    _waypoints.Add(
                        new RoadPoint() { point = waypoint, roadPointType = RoadPointType.Acceleration}
                    );    
                }
                else
                {
                    _waypoints.Add(
                        new RoadPoint() { point = waypoint, roadPointType = RoadPointType.Normal}
                    );
                }
            }
        }

        public void MovementEnter()
        {
        }
        
        public void MovementUpdate()
        {
            Ray ray = new Ray(VehicleController.Vehicle.RayStartPoint.position, transform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance,_carLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                Vehicle hitVehicle = hit.collider.gameObject.GetComponent<Vehicle>();
                
                if (hitVehicle.lightPlaceSave != LightPlace.None 
                    && VehicleController.Vehicle.lightPlaceSave != LightPlace.None 
                        && hitVehicle.lightPlaceSave != VehicleController.Vehicle.lightPlaceSave)
                {
                    Debug.Log("Game Is Over");
                }
                
                VehicleController.SetState<VehicleStopState>();
                
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.green);
            }

            FixedUpdate(); // we can implement a real fixupdate also, but i am not sure
        }

        public void MovementExit()
        {
            
        }
        
        private void FixedUpdate()
        {
            if (!HasWaypoints()) return;
            if (IsAtFinalWaypoint()) return;
            
            AdjustSpeed();
            
            MoveTowardsWaypoint();
            RotateTowardsWaypoint();
            if (IsCloseToWaypoint())
                ProceedToNextWaypoint();
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
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetWaypoint.position, 
                _speed * Time.fixedDeltaTime
            );
        }
        private void RotateTowardsWaypoint()
        {
            Transform targetWaypoint = _waypoints[_currentWaypointIndex].point;

            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                _carData.RotationSpeed * Time.fixedDeltaTime
            );
        }
        private bool IsCloseToWaypoint()
        {
            Transform targetWaypoint = _waypoints[_currentWaypointIndex].point;

            return Vector3.Distance(transform.position, targetWaypoint.position) < 1f;
        }
        private void ProceedToNextWaypoint()
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _waypoints.Count)
            {
                VehicleController.Vehicle.DestinationReached(); 
                _currentWaypointIndex = 0;
                transform.position = _waypoints[_currentWaypointIndex].point.position;
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