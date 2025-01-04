using Script.Vehicles.Controllers;

namespace Script.Vehicles.States
{
    public interface IVehicleState
    {
        public VehicleController VehicleController { get; set; }

        void MovementEnter();
        void MovementUpdate();
        void MovementExit();
    }

}