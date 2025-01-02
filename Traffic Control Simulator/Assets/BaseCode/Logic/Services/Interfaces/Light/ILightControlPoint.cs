using BaseCode.Logic.EntityHandler.Lights;
using BaseCode.Logic.EntityHandler.Vehicles;

namespace BaseCode.Logic.Services.Interfaces.Light
{
    public interface ILightControlPoint
    {
        void OnVehicleEnter(VehicleBase vehicle);
        void OnVehicleExit(VehicleBase vehicle);
        void Initialize(BasicLight parentLightControlPoint);
    }
}