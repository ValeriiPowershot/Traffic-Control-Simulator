using DG.Tweening;
using Script.So;
using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles.States
{
    public class VehicleGoState : IVehicleState
    {
        public VehicleController VehicleController { get; set; }
        public VehicleGoState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        
        public void MovementEnter()
        {
            VehicleController.MoveTween.Play();
            VehicleController.MoveTween.timeScale = VehicleSo.speed; // Adjust tween speed dynamically
        }

        public void MovementUpdate()
        {
        }

        public void MovementExit()
        {
        }

       
        private VehicleSo VehicleSo => VehicleController.Vehicle.vehicleSo;
    }
}