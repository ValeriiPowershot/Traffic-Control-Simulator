using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers;

namespace BaseCode.Logic.Vehicles.States
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
          //  VehicleController.MoveTween.timeScale = VehicleScriptableObject.slowDownSpeed; // Adjust tween speed dynamically
        }

        public void MovementUpdate()
        {
        }

        public void MovementExit()
        {
            // Restart the tween to full speed on exit 
        }
        private VehicleScriptableObject VehicleScriptableObject => VehicleController.BasicCar.VehicleScriptableObject;
    }
}