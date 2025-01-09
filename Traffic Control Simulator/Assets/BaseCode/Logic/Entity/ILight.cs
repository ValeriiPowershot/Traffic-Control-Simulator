using BaseCode.Logic.Vehicles;

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