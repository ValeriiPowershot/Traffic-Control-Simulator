using System.Collections.Generic;
using BaseCode.Logic.EntityHandler.Vehicles;

namespace BaseCode.Logic.EntityHandler.Lights.BasicLightHandler
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