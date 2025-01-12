using BaseCode.Logic.Vehicles;
using VehicleBase = BaseCode.Logic.Vehicles.Vehicles.VehicleBase;

namespace BaseCode.Logic.Entity
{
    public interface ILight
    {
        void ChangeLight();
        void SetChangeoverState();
        void AddVehicle(VehicleBase vehicle);
        void RemoveVehicle(VehicleBase vehicle);
    }
}