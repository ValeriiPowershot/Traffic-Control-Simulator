using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Vehicles.Boss
{
    public class Tank : AggressiveCar
    {
        public override void AssignCollisionController()
        {
            vehicleController.VehicleCollisionController = new TankVehicleCollisionController();
            vehicleController.VehicleCollisionController.Starter(this);
        }
    }
}