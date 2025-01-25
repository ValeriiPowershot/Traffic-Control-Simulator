using BaseCode.Logic.Vehicles.States;
using UnityEngine;

namespace BaseCode.Logic.Npcs.States.Movement
{
    public interface INpcMovementState : INpcState
    {
        void MovementEnter();
        void MovementUpdate();
        void MovementExit();
    }
    
}