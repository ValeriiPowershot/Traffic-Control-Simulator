using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles.States
{
    public class VehicleSlowDownState : IVehicleState
    {
        public void MovementStateHandler(VehicleController vehicle)
        {
            Move(vehicle);
        }
        
        private void Move(VehicleController vehicleController)
        {
            var vehicle = vehicleController.Vehicle;
            var moveSpeed = vehicle.vehicleSo.SlowDownSpeed;
            vehicle.transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
        }

    }
}