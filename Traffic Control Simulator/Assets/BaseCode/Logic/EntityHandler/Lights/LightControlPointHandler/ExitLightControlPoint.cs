using BaseCode.Logic.EntityHandler.Vehicles;

namespace BaseCode.Logic.EntityHandler.Lights.LightControlPointHandler
{
    public class ExitLightControlPoint : LightControlPointBase
    {
        public override void OnVehicleExit(VehicleBase vehicle)
        {
            ParentLightControlPoint?.RemoveVehicle(vehicle);
        }

        public override void OnVehicleEnter(VehicleBase vehicle)
        {
            vehicle.CarLightService.PassLightPlaceState(ParentLightControlPoint.Place);
        }
    }

}