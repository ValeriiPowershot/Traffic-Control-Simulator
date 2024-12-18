using DG.Tweening;
using Script.ScriptableObject;
using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles.States
{
    public class VehicleSlowDownState : IVehicleState
    {
        public VehicleController VehicleController { get; set; }

        public VehicleSlowDownState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }

        public void MovementEnter()
        {
            VehicleController.MoveTween.timeScale = VehicleScriptableObjects.slowDownSpeed; // Adjust tween speed dynamically
        }

        public void MovementUpdate()
        {
        }

        public void MovementExit()
        {
            // Restart the tween to full speed on exit 
        }
        private VehicleScriptableObjects VehicleScriptableObjects => VehicleController.Vehicle.VehicleScriptableObject;
    }
}