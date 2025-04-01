using BaseCode.Logic.Entity.Npcs.Controllers;
using BaseCode.Logic.Entity.Npcs.States.Movement;
using UnityEngine;
using LightState = BaseCode.Logic.Vehicles.Vehicles.LightState;

namespace BaseCode.Logic.Entity.Npcs.Npc
{
    public class NpcBase : MonoBehaviour
    {
        public NpcController npcController;

        public virtual void Start()
        {
            npcController.StartState(this);
        }

        public virtual void Update() => npcController.Update();

        // TODO
        // add this function to a service
        public void PassLightState(LightState state)
        {
            npcController.currentLight = state;
            
            switch (npcController.currentLight)
            {
                case LightState.Red:
                    npcController.SetState<NpcMovementGoState>();
                    break;
                case LightState.Green:
                    npcController.SetState<NpcMovementStopState>();
                    break;
            }
        }
    }
}