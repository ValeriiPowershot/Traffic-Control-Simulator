using Script.Vehicles.Controllers;

namespace Script.Vehicles.States
{
    public interface IVehicleState
    {
        void MovementStateHandler(VehicleController vehicle);
    }

}