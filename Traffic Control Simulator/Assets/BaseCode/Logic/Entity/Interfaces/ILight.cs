using VehicleBase = BaseCode.Logic.Vehicles.Vehicles.VehicleBase;

namespace BaseCode.Logic.Entity.Interfaces
{
    public interface ILight : IEntity
    {
        void ChangeLight();
        void SetChangeoverState();
        void AddVehicle(VehicleBase vehicle);
        void RemoveVehicle(VehicleBase vehicle);
    }
}