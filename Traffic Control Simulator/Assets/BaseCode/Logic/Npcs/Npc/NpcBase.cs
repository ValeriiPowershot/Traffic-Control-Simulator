using System;
using BaseCode.Logic.Npcs.Controllers;
using BaseCode.Logic.Npcs.States.Movement;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;
using UnityEngine.Serialization;
using LightState = BaseCode.Logic.Vehicles.Vehicles.LightState;

namespace BaseCode.Logic.Npcs.Npc
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