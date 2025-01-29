using BaseCode.Logic.Vehicles.Controllers;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class AggressiveCar : BasicCar
    {
        public override void AssignCollisionController()
        {
            VehicleCollisionController = new AggressiveVehicleCollisionController();
            VehicleCollisionController.Starter(this);
        }
    }
}