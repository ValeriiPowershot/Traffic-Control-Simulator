using System;
using System.Collections;
using System.Collections.Generic;
using BaseCode.Extensions.UI;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using BaseCode.Logic.Vehicles.Controllers.Path;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public class VehicleMovementGoState : IVehicleMovementState
    {
        public VehicleController VehicleController { get; set; }
        
        private float _currentSpeed;
        private float _defaultSpeed;
        private float _slowDownSpeed;
        private float _accelerationSpeed;

        private bool _isSlowingDown;

        private float _startSpeed;
        private float _preSlowdownSpeed;
        private float _targetSpeed;
        private float _transitionDuration = 1.0f;
        private float _transitionTimer = 0f;

        
        public VehicleMovementGoState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
            AssignNewSpeedValues();
        }

        public void MovementEnter()
        {
        }
        
        public void MovementUpdate() 
        {
            if (!PathPointController.HasWaypoints()) return;
            if (PathPointController.IsAtFinalWaypoint()) return;
            if (VehicleCollisionController.CheckForCollision()) return;

            Debug.Log("Movement Enter");
            
            AdjustSpeed();
            MoveTowardsWaypoint();
            RotateTowardsWaypoint();

            SetToNextPoint();
        }

        public void MovementExit()
        {
            
        }
        private void SetToNextPoint()
        {
            if (PathPointController.IsCloseToWaypoint(VehicleController.VehicleBase.transform.position))
                VehiclePathController.ProceedToNextWaypoint();
        }
        
        public void SmoothSlowdownByAmount(float amount, float duration)
        {
            _isSlowingDown = true;
            
            _currentSpeed = Mathf.Lerp(_currentSpeed, _currentSpeed - amount, duration);
        }
        
        public void SmoothRestoreSpeed()
        {
            _isSlowingDown = false;
        }
        
        private void AdjustSpeed()
        {
            if (!_isSlowingDown)
            {
                RoadPoint roadPoint = PathPointController.GetCurrentWaypoint();
                _currentSpeed = roadPoint.roadPointType switch
                {
                    RoadPointType.Slowdown => _slowDownSpeed,
                    RoadPointType.Acceleration => _accelerationSpeed,
                    _ => _defaultSpeed
                };
            }
            
        }

        private void MoveTowardsWaypoint()
        {
            Transform targetWaypoint = PathPointController.GetCurrentWaypoint().point;
            CarTransform.position = Vector3.MoveTowards(
                CarTransform.position, 
                targetWaypoint.position, 
                _currentSpeed * Time.deltaTime
            );
        }
        
        private void RotateTowardsWaypoint()
        {
            Transform targetWaypoint = PathPointController.GetCurrentWaypoint().point;

            Vector3 direction = (targetWaypoint.position - CarTransform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                CarTransform.rotation = Quaternion.RotateTowards(
                    CarTransform.rotation, 
                    targetRotation, 
                    CarData.rotationSpeed * Time.deltaTime
                );

                Vector3 arrowForward = (PathPointController.GetEndPoint().position - 
                                        ReferenceController.arrowIndicatorEndPoint.position).normalized;
                Quaternion arrowRotation = Quaternion.LookRotation(arrowForward);
        
                arrowRotation = Quaternion.Euler(arrowRotation.eulerAngles.x, arrowRotation.eulerAngles.y, 0);

                ReferenceController.arrowIndicatorEndPoint.rotation = arrowRotation;
            }
        }
        public void AssignNewSpeedValues()
        {
            _defaultSpeed = CarData.DefaultSpeed;
            _slowDownSpeed = CarData.SlowdownSpeed;
            _accelerationSpeed = CarData.AccelerationSpeed;

            _currentSpeed = _defaultSpeed;
            
            Debug.Log("Assigned New Values");
        }
        
        public Transform CarTransform => VehicleController.VehicleBase.transform;
        public VehiclePathController VehiclePathController
        {
            get => VehicleController.VehiclePathController;
            set => VehicleController.VehiclePathController = value;
        }
        private VehicleScriptableObject CarData => VehicleController.VehicleBase.VehicleScriptableObject;
        public PathPointsContainerController PathPointController => VehiclePathController.PathPointController;
        protected VehicleReferenceController ReferenceController =>VehicleController.vehicleReferenceController;
        private VehicleCollisionControllerBase VehicleCollisionController => VehicleController.VehicleCollisionController;
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