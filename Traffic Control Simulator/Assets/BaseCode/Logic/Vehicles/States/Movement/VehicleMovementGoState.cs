using System;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public class VehicleMovementGoState : IVehicleMovementState
    {
        public VehicleScriptableObject CarData { get; }
        
        public VehicleController VehicleController { get; set; }
        
        public VehiclePathController VehiclePathController { get; set; }
        
        private float _speed;

        public VehicleMovementGoState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
            CarData = VehicleController.VehicleBase.VehicleScriptableObject;
            _speed = CarData.DefaultSpeed;
            
            VehiclePathController = new VehiclePathController(this);
        }
        public void InitializePath()
        {
            VehiclePathController.InitializePath();
        }

        public void MovementEnter()
        {
        }
        
        public void MovementUpdate() 
        {
            if (!VehiclePathController.HasWaypoints()) return;
            if (VehiclePathController.IsAtFinalWaypoint()) return;
            if (VehicleController.VehicleBase.VehicleCollisionController.CheckForCollision()) return;

            AdjustSpeed();

            MoveTowardsWaypoint();
            RotateTowardsWaypoint();

            if (VehiclePathController.IsCloseToWaypoint())
                VehiclePathController.ProceedToNextWaypoint();
        }

        public void MovementExit()
        {
            
        }
        
        private void AdjustSpeed()
        {
            RoadPoint roadPoint = VehiclePathController.GetCurrentWaypoint();

            _speed = roadPoint.roadPointType switch
            {
                RoadPointType.Slowdown => CarData.SlowdownSpeed,
                RoadPointType.Acceleration => CarData.AccelerationSpeed,
                _ => CarData.DefaultSpeed
            };
        }

        private void MoveTowardsWaypoint()
        {
            Transform targetWaypoint = VehiclePathController.GetCurrentWaypoint().point;
            CarTransform.position = Vector3.MoveTowards(
                CarTransform.position, 
                targetWaypoint.position, 
                _speed * Time.deltaTime
            );
        }
        private void RotateTowardsWaypoint()
        {
            Transform targetWaypoint = VehiclePathController.GetCurrentWaypoint().point;

            Vector3 direction = (targetWaypoint.position - CarTransform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                CarTransform.rotation = Quaternion.RotateTowards(
                    CarTransform.rotation, 
                    targetRotation, 
                    CarData.RotationSpeed * Time.deltaTime
                );

                Vector3 arrowForward = (VehiclePathController.GetEndPoint().position - VehicleController.VehicleBase.ArrowIndicatorEndPoint.position).normalized;
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