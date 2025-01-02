using BaseCode.Logic.EntityHandler.Lights;
using BaseCode.Logic.EntityHandler.Vehicles;

namespace BaseCode.Domain.Entity
{
    public interface IBasicLightControlPoint : IEntity
    {
        void AddNewCar(VehicleBase car);
        void RemoveCar(VehicleBase car);
        LightPlace LightPlace { get; }
    }
}