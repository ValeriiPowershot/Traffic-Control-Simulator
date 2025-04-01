using BaseCode.Logic.Entity.Npcs.Npc;
using BaseCode.Logic.Lights.Services;
using LightState = BaseCode.Logic.Vehicles.Vehicles.LightState;
using VehicleBase = BaseCode.Logic.Vehicles.Vehicles.VehicleBase;

namespace BaseCode.Logic.Lights.Handler.Abstracts
{
    public class LightNotifier : ILightNotifier
    {
        private readonly LightBase _lightBase;

        public LightNotifier(LightBase lightBase)
        {
            _lightBase = lightBase;
        }

        public void NotifyNpc(NpcBase npcBase, LightState state)
        {
            npcBase.PassLightState(state); 
        }
        public void NotifyNpcs(LightState state)
        {
            if (_lightBase.ControlledNpc == null) return;
            NotifyNpc(_lightBase.ControlledNpc, state);
        }

        public void NotifyVehicles(LightState state)
        {
            foreach (var vehicle in _lightBase.ControlledVehicles)
            {
                NotifyVehicle(vehicle, state);
            }
        }
        
        public void NotifyVehicle(VehicleBase vehicle, LightState state)
        {
            vehicle.VehicleController.VehicleLightController.PassLightState(state);
        }
        
    }
}