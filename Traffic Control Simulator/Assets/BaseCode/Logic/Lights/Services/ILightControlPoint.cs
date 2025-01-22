using BaseCode.Logic.Vehicles.Vehicles;

namespace BaseCode.Logic.Lights.Services
{
    public interface ILightControlPoint
    {
        void OnVehicleEnter(VehicleBase vehicle);
        void OnVehicleExit(VehicleBase vehicle);
        void Initialize(BasicLight parentLightControlPoint);
    }
}