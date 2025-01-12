using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public class VehicleMovementSlowDownState : IVehicleMovementState
    {
        public VehicleController VehicleController { get; set; }

        public VehicleMovementSlowDownState(VehicleController vehicleController)
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
        private VehicleScriptableObject VehicleScriptableObject => VehicleController.BasicCar.VehicleScriptableObject;
    }
}