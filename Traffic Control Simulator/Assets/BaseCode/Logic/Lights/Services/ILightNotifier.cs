using BaseCode.Logic.Npcs.Npc;
using LightState = BaseCode.Logic.Vehicles.Vehicles.LightState;
using VehicleBase = BaseCode.Logic.Vehicles.Vehicles.VehicleBase;

namespace BaseCode.Logic.Lights.Services
{
    public interface ILightNotifier
    {
        void NotifyNpc(NpcBase npcBase, LightState state);
        void NotifyNpcs(LightState state);

        
        void NotifyVehicle(VehicleBase vehicle, LightState state);
        void NotifyVehicles(LightState state);

    }
}