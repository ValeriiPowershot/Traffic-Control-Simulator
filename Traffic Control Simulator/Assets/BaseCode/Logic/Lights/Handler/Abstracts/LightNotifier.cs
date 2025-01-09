using BaseCode.Logic.Lights.Services;
using BaseCode.Logic.Vehicles;

namespace BaseCode.Logic.Lights.Handler.Abstracts
{
    public class LightNotifier : ILightNotifier
    {
        private readonly LightBase _lightBase;

        public LightNotifier(LightBase lightBase)
        {
            _lightBase = lightBase;
        }

        public void NotifyVehicle(VehicleBase vehicle, LightState state)
        {
            vehicle.PassLightState(state);
        }
        public void NotifyVehicles(LightState state)
        {
            foreach (var vehicle in _lightBase.ControlledVehicles)
            {
                NotifyVehicle(vehicle, state);
            }
        }
    }
}