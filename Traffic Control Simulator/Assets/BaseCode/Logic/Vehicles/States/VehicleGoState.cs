using System;
using System.Collections.Generic;
using BaseCode.Logic.Lights;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Ways;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.States
{
    public class VehicleGoState : IVehicleState
    {
        private readonly VehicleScriptableObject _carData;
        private WaypointContainer _waypointContainer;
        private Transform _endPoint;
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
        private const float TURN_ANGLE = 0.5f;
        private const int CHECK_COUNT = 12;

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
            _endPoint = _waypoints[^1].point;
        }

        public void MovementEnter()
        {
        }
        
        public void MovementUpdate() 
        {
            // we can implement a real fixupdate also, but i am not sure
            if (!HasWaypoints()) return;
            if (IsAtFinalWaypoint()) return;
            if (CheckForCollision()) return;

            AdjustSpeed();
            
            MoveTowardsWaypoint();
            RotateTowardsWaypoint();
            if (IsCloseToWaypoint())
                ProceedToNextWaypoint();
 
        }

      

        public void MovementExit()
        {
            
        }

        private bool CheckForCollision()
        {
            Ray ray = new Ray(VehicleController.Vehicle.rayStartPoint.position, CarTransform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance, _stopLayer)) // hit to stop or car
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                
                if(hit.collider.TryGetComponent(out BasicCar hitVehicle))
                {
                    if (AreTheyInIntersection(hitVehicle) && AreTheyUsingDifferentPath(hitVehicle))
                    {
                        Debug.Log("Game Is Over");
                    }
                    VehicleController.SetState<VehicleStopState>();
                    return true;
                }

                if (IsItRedLight())
                {
                    VehicleController.SetState<VehicleStopState>();
                    return true;
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.green);
            }

            return false;
        }

        private bool IsItRedLight()
        {
            return VehicleController.Vehicle.CarLightState == LightState.Red;
        }
        private bool AreTheyUsingDifferentPath(BasicCar hitVehicle)
        {
            return hitVehicle.lightPlaceSave != VehicleController.Vehicle.lightPlaceSave;
        }

        private bool AreTheyInIntersection(BasicCar hitVehicle)
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

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                CarTransform.rotation = Quaternion.RotateTowards(
                    CarTransform.rotation, 
                    targetRotation, 
                    _carData.RotationSpeed * Time.fixedDeltaTime
                );

                Vector3 arrowForward = (_endPoint.position - VehicleController.Vehicle.arrowIndicatorEndPoint.position).normalized;
                Quaternion arrowRotation = Quaternion.LookRotation(arrowForward);
        
                arrowRotation = Quaternion.Euler(arrowRotation.eulerAngles.x, arrowRotation.eulerAngles.y, 0);

                VehicleController.Vehicle.arrowIndicatorEndPoint.rotation = arrowRotation;
            }
        }

        private bool IsCloseToWaypoint()
        {
            Transform targetWaypoint = _waypoints[_currentWaypointIndex].point;

            return Vector3.Distance(CarTransform.position, targetWaypoint.position) < 1f;
        }

        private void ProceedToNextWaypoint()
        {
            _currentWaypointIndex++;

            CheckForTurn();

            if (_currentWaypointIndex >= _waypoints.Count)
            {
                VehicleController.Vehicle.DestinationReached(); 
                _currentWaypointIndex = 0;
                CarTransform.position = _waypoints[_currentWaypointIndex].point.position;
            }
        }

        private void CheckForTurn()
        {
            int i = 0;
            if (_currentWaypointIndex + CHECK_COUNT < _waypoints.Count)
                i = _currentWaypointIndex + CHECK_COUNT;
            else
                i = _waypoints.Count - 1;

            Vector3 pointing = _waypoints[i].point.position - VehicleController.Vehicle.transform.position;
            pointing.Normalize();
            pointing -= VehicleController.Vehicle.transform.forward * 0.9f;
            pointing.Normalize();

            float dot = Vector3.Dot(pointing, VehicleController.Vehicle.transform.right);

            if (dot > TURN_ANGLE)
                VehicleController.Vehicle.ShowTurn(TurnType.Right);
            else if (dot < -TURN_ANGLE)
                VehicleController.Vehicle.ShowTurn(TurnType.Left);
            else
                VehicleController.Vehicle.ShowTurn(TurnType.None);
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