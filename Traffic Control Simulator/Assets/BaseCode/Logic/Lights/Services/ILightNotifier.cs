using BaseCode.Logic.Vehicles;

namespace BaseCode.Logic.Lights.Services
{
    public interface ILightNotifier
    {
        void NotifyVehicles(LightState state);
        void NotifyVehicle(VehicleBase vehicle, LightState state);

    }
}