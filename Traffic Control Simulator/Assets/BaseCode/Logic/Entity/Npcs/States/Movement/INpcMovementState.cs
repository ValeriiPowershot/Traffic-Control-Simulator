namespace BaseCode.Logic.Entity.Npcs.States.Movement
{
    public interface INpcMovementState : INpcState
    {
        void MovementEnter();
        void MovementUpdate();
        void MovementExit();
    }
    
}