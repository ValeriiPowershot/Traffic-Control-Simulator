using BaseCode.Logic.EntityHandler.Vehicles;

namespace BaseCode.Logic.EntityHandler.Lights.BasicLightHandler
{
    public interface ILightNotifier
    {
        void NotifyVehicles(LightState state);
        void NotifyVehicle(VehicleBase vehicle, LightState state);

    }
}