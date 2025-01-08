using BaseCode.Logic.Vehicles.Controllers;

namespace BaseCode.Logic.Vehicles.States
{
    public interface IVehicleState
    {
        public VehicleController VehicleController { get; set; }

        void MovementEnter();
        void MovementUpdate();
        void MovementExit();
    }

}