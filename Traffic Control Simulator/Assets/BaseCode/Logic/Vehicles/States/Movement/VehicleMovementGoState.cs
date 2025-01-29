using System;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public class VehicleMovementGoState : IVehicleMovementState
    {
        public VehicleScriptableObject CarData { get; }
        
        public VehicleController VehicleController { get; set; }
        
        private readonly VehiclePathController _vehiclePathController;
        
        private float _speed;

        public VehicleMovementGoState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
            CarData = VehicleController.VehicleBase.VehicleScriptableObject;
            _speed = CarData.DefaultSpeed;
            
            _vehiclePathController = new VehiclePathController(this);
        }
        public void InitializePath()
        {
            _vehiclePathController.InitializePath();
        }

        public void MovementEnter()
        {
        }
        
        public void MovementUpdate() 
        {
            if (!_vehiclePathController.HasWaypoints()) return;
            if (_vehiclePathController.IsAtFinalWaypoint()) return;
            if (VehicleController.VehicleBase.VehicleCollisionController.CheckForCollision()) return;

            AdjustSpeed();

            MoveTowardsWaypoint();
            RotateTowardsWaypoint();

            if (_vehiclePathController.IsCloseToWaypoint())
                _vehiclePathController.ProceedToNextWaypoint();
        }

        public void MovementExit()
        {
            
        }
        
        private void AdjustSpeed()
        {
            RoadPoint roadPoint = _vehiclePathController.GetCurrentWaypoint();

            _speed = roadPoint.roadPointType switch
            {
                RoadPointType.Slowdown => CarData.SlowdownSpeed,
                RoadPointType.Acceleration => CarData.AccelerationSpeed,
                _ => CarData.DefaultSpeed
            };
        }

        private void MoveTowardsWaypoint()
        {
            Transform targetWaypoint = _vehiclePathController.GetCurrentWaypoint().point;
            CarTransform.position = Vector3.MoveTowards(
                CarTransform.position, 
                targetWaypoint.position, 
                _speed * Time.fixedDeltaTime
            );
        }
        private void RotateTowardsWaypoint()
        {
            Transform targetWaypoint = _vehiclePathController.GetCurrentWaypoint().point;

            Vector3 direction = (targetWaypoint.position - CarTransform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                CarTransform.rotation = Quaternion.RotateTowards(
                    CarTransform.rotation, 
                    targetRotation, 
                    CarData.RotationSpeed * Time.fixedDeltaTime
                );

                Vector3 arrowForward = (_vehiclePathController.GetEndPoint().position - VehicleController.VehicleBase.ArrowIndicatorEndPoint.position).normalized;
                Quaternion arrowRotation = Quaternion.LookRotation(arrowForward);
        
                arrowRotation = Quaternion.Euler(arrowRotation.eulerAngles.x, arrowRotation.eulerAngles.y, 0);

                VehicleController.VehicleBase.ArrowIndicatorEndPoint.rotation = arrowRotation;
            }
        }
        public Transform CarTransform => VehicleController.VehicleBase.transform;

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