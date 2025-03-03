using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class AggressiveCar : BasicCar
    {
        protected override void AssignCollisionController()
        {
            VehicleCollisionController = new AggressiveVehicleCollisionController();
            VehicleCollisionController.Starter(this);
        }
    }
}