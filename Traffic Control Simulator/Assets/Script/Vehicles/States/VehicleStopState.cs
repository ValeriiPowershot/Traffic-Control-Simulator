using DG.Tweening;
using Script.Vehicles.Controllers;

namespace Script.Vehicles.States
{
    public class VehicleStopState : IVehicleState
    {
        public VehicleController VehicleController { get; set; }
        public VehicleStopState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        public void MovementEnter()
        {
            VehicleController.MoveTween.Pause();

        }

        public void MovementUpdate()
        {
        }

        public void MovementExit()
        {
        }
  
         
    }
}