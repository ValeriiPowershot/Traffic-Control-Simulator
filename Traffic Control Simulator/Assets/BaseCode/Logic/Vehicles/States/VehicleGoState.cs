using System;
using System.Collections.Generic;
using BaseCode.Infrastructure;
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
        private Transform CarTransform => VehicleController.BasicCar.transform;
        public VehicleController VehicleController { get; set; }
        private const float TURN_ANGLE = 0.5f;
        private const int CHECK_COUNT = 12;

        public VehicleGoState(VehicleController vehicleController)
        {
            _stopLayer += 1 << 7;
            _stopLayer += 1 << 10;
            VehicleController = vehicleController;
            _carData = VehicleController.BasicCar.VehicleScriptableObject;
            _rayDistance = _carData.rayDistance;
            _speed = _carData.NormalSpeed;
        }
        public void InitializePath()
        {
            _waypoints.Clear();
            _waypointContainer = VehicleController.BasicCar.WaypointContainer;
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
            Ray ray = new Ray(VehicleController.BasicCar.RayStartPoint.position, CarTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, _stopLayer)) // hit to stop or car
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                
                if(hit.collider.TryGetComponent(out VehicleBase hitVehicle))
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
            return VehicleController.BasicCar.CarLightState == LightState.Red;
        }
        private bool AreTheyUsingDifferentPath(VehicleBase hitVehicle)
        {
            return hitVehicle.lightPlaceSave != VehicleController.BasicCar.lightPlaceSave;
        }

        private bool AreTheyInIntersection(VehicleBase hitVehicle)
        {
            return hitVehicle.lightPlaceSave != LightPlace.None && VehicleController.BasicCar.lightPlaceSave != LightPlace.None;
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

                Vector3 arrowForward = (_endPoint.position - VehicleController.BasicCar.ArrowIndicatorEndPoint.position).normalized;
                Quaternion arrowRotation = Quaternion.LookRotation(arrowForward);
        
                arrowRotation = Quaternion.Euler(arrowRotation.eulerAngles.x, arrowRotation.eulerAngles.y, 0);

                VehicleController.BasicCar.ArrowIndicatorEndPoint.rotation = arrowRotation;
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
                VehicleController.BasicCar.DestinationReached(); 
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

            Vector3 pointing = _waypoints[i].point.position - VehicleController.BasicCar.transform.position;
            pointing.Normalize();
            pointing -= VehicleController.BasicCar.transform.forward * 0.9f;
            pointing.Normalize();

            float dot = Vector3.Dot(pointing, VehicleController.BasicCar.transform.right);

            if (dot > TURN_ANGLE)
                VehicleController.BasicCar.ShowTurn(TurnType.Right);
            else if (dot < -TURN_ANGLE)
                VehicleController.BasicCar.ShowTurn(TurnType.Left);
            else
                VehicleController.BasicCar.ShowTurn(TurnType.None);
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