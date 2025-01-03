using System;
using BaseCode.Infrastructure.ScriptableObject;
using BaseCode.Logic.EntityHandler.Vehicles.Controllers;
using UnityEngine;

namespace BaseCode.Logic.EntityHandler.Vehicles.States
{
    public class VehicleGoState : IVehicleMovementState
    {
        public VehicleScriptableObject CarData { get; private set; }
        private readonly VehiclePathController _vehiclePathController;
        private readonly VehicleCollisionController _vehicleCollisionController;

        private float _speed;
        
        public Transform CarTransform => VehicleController.BasicVehicle.transform;
        public VehicleController VehicleController { get; set; }

        public VehicleGoState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
            CarData = VehicleController.BasicVehicle.VehicleScriptableObject;
            _speed = CarData.NormalSpeed;

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
                RoadPointType.Slowdown => CarData.SlowDownSpeed,
                RoadPointType.Acceleration => CarData.AccelerationSpeed,
                _ => CarData.NormalSpeed
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

                Vector3 arrowForward = (_vehiclePathController.GetEndPoint().position -
                                        VehicleController.BasicVehicle.arrowIndicatorEndPoint.position).normalized;
                Quaternion arrowRotation = Quaternion.LookRotation(arrowForward);

                arrowRotation = Quaternion.Euler(arrowRotation.eulerAngles.x, arrowRotation.eulerAngles.y, 0);

                VehicleController.BasicVehicle.arrowIndicatorEndPoint.rotation = arrowRotation;
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