using LightState = BaseCode.Logic.Vehicles.Vehicles.LightState;
using VehicleBase = BaseCode.Logic.Vehicles.Vehicles.VehicleBase;

namespace BaseCode.Logic.Entity.Lights.Services
{
    public interface ILightNotifier
    {
        void NotifyVehicles(LightState state);
        void NotifyVehicle(VehicleBase vehicle, LightState state);

    }
}