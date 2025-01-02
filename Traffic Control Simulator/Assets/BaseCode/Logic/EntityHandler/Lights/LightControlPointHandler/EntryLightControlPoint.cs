using BaseCode.Logic.EntityHandler.Vehicles;

namespace BaseCode.Logic.EntityHandler.Lights.LightControlPointHandler
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