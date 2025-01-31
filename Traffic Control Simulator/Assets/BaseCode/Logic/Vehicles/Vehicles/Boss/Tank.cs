using BaseCode.Logic.Vehicles.Controllers;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Vehicles.Boss
{
    public class Tank : AggressiveCar
    {
        public override void AssignCollisionController()
        {
            VehicleCollisionController = new TankVehicleCollisionController();
            VehicleCollisionController.Starter(this);
        }
    }
}