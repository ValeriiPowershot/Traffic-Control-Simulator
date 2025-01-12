using BaseCode.Logic.Lights.Handler.Abstracts;
using VehicleBase = BaseCode.Logic.Vehicles.Vehicles.VehicleBase;

namespace BaseCode.Logic.Lights.Handler.Inheriteds.LightControlPointHandler
{
    public class EntryLightControlPoint : LightControlPointBase
    {
        public override void OnVehicleExit(VehicleBase vehicle)
        {
            ParentLightControlPoint?.AddVehicle(vehicle);
        }

        public override void OnVehicleEnter(VehicleBase vehicle)
        {
            
        }
    }

}