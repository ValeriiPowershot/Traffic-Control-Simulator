using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles.States
{
    public class VehicleGoState : IVehicleState
    {
        public void MovementStateHandler(VehicleController vehicleController)
        {
            Move(vehicleController);
        }

        private void Move(VehicleController vehicleController)
        {
            var vehicle = vehicleController.Vehicle;
            var moveSpeed = vehicle.vehicleSo.Speed;
            vehicle.transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
        }

    }
}