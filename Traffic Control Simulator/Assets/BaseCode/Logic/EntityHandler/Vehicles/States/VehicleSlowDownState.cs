using BaseCode.Infrastructure.ScriptableObject;
using BaseCode.Logic.EntityHandler.Vehicles.Controllers;

namespace BaseCode.Logic.EntityHandler.Vehicles.States
{
    public class VehicleSlowDownState : IVehicleMovementState
    {
        public VehicleController VehicleController { get; set; }

        public VehicleSlowDownState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }

        public void MovementEnter()
        {
        }

        public void MovementUpdate()
        {
        }

        public void MovementExit()
        {
        }
        private VehicleScriptableObject VehicleScriptableObject => VehicleController.BasicVehicle.VehicleScriptableObject;
    }
}