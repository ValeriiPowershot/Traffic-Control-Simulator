using BaseCode.Logic.Lights.Handler.Abstracts;
using BaseCode.Logic.Vehicles;

namespace BaseCode.Logic.Lights.Handler.Inheriteds.LightControlPointHandler
{
    public class ExitLightControlPoint : LightControlPointBase
    {
        public override void OnVehicleExit(VehicleBase vehicle)
        {
            ParentLightControlPoint?.RemoveVehicle(vehicle);
        }

        public override void OnVehicleEnter(VehicleBase vehicle)
        {
            vehicle.PassLightPlaceState(ParentLightControlPoint.Place);
        }
    }

}