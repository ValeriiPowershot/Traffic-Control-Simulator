using BaseCode.Logic.Entity.Lights.Services;
using BaseCode.Logic.Lights.Handler.Abstracts;
using LightState = BaseCode.Logic.Vehicles.Vehicles.LightState;
using VehicleBase = BaseCode.Logic.Vehicles.Vehicles.VehicleBase;

namespace BaseCode.Logic.Entity.Lights.Handler.Abstracts
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
            vehicle.CarLightService.PassLightState(state);
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