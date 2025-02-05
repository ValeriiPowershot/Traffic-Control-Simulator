using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;

namespace BaseCode.Logic.Vehicles.States
{
    public interface IVehicleState
    {
        public VehicleController VehicleController { get; set; }
    }
}