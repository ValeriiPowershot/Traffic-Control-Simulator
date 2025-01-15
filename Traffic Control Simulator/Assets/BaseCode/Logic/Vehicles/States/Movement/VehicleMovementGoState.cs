using System;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public class VehicleMovementGoState : IVehicleMovementState
    {
        public VehicleScriptableObject CarData { get; }
        
        // controllers
        public Transform CarTransform => VehicleController.BasicCar.transform;
        public VehicleController VehicleController { get; set; }
        
        private readonly VehiclePathController _vehiclePathController;
        private readonly VehicleCollisionController _vehicleCollisionController;
        
        private float _speed;

        public VehicleMovementGoState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
            CarData = VehicleController.BasicCar.VehicleScriptableObject;
            _speed = CarData.DefaultSpeed;
            
            _vehiclePathController = new VehiclePathController(this);
            _vehicleCollisionController = new VehicleCollisionController(this);
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
            if (_vehicleCollisionController.CheckForCollision()) return;

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

                Vector3 arrowForward = (_vehiclePathController.GetEndPoint().position - VehicleController.BasicCar.ArrowIndicatorEndPoint.position).normalized;
                Quaternion arrowRotation = Quaternion.LookRotation(arrowForward);
        
                arrowRotation = Quaternion.Euler(arrowRotation.eulerAngles.x, arrowRotation.eulerAngles.y, 0);

                VehicleController.BasicCar.ArrowIndicatorEndPoint.rotation = arrowRotation;
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