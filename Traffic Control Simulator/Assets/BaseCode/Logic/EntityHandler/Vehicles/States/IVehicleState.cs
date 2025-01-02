using BaseCode.Logic.EntityHandler.Vehicles.Controllers;
using BaseCode.Logic.EntityHandler.Vehicles.StateBase;

namespace BaseCode.Logic.EntityHandler.Vehicles.States
{
    public interface IVehicleMovementState : IVehicleState
    {
        void MovementEnter();
        void MovementUpdate();
        void MovementExit();
    }

}