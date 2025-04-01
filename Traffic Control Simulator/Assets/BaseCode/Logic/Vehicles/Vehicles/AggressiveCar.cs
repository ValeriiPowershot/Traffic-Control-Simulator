using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class AggressiveCar : BasicCar
    {
        public override void AssignCollisionController()
        {
            VehicleController.VehicleCollisionController = new AggressiveVehicleCollisionController();
            VehicleController.VehicleCollisionController.Starter(this);
        }
    }
}