using BaseCode.Logic.Vehicles.States;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public interface IVehicleMovementState : IVehicleState
    {
        void MovementEnter();

        void MovementUpdate();

        void MovementExit();
    }

}