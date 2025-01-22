
using System;
using BaseCode.Logic.Npcs.Npc;
using BaseCode.Logic.Npcs.States.Movement;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.Npcs.Controllers
{
    [Serializable]
    public class NpcController
    {
        public LightState currentLight;
        private NpcBase _npcBase;
        public NpcMovementStateController StateController { get; private set; }
        public NpcScriptableObject npcScriptableObject;

        public Animator animator;
        
        // make these private
        public Transform target; // Current target (A or B)
        public Transform a; // Point A
        public Transform b; // Point B
        
        public void StartState(NpcBase npcBase)
        {
            _npcBase = npcBase;
            
            StateController = new NpcMovementStateController(_npcBase);
            
            SetState<NpcMovementGoState>(); // Start in the stopped state
        }

        public void Update()
        {
            StateController.Update();
        }

        public void SetState<T>() where T : INpcMovementState
        {
            StateController.SetState<T>(); // Start in the stopped state
        }
        
        public NpcBase NpcBase => _npcBase;

        public void SetToIdle()
        {
            animator.SetFloat("Movement",0);
        }
        public void SetToWalk()
        {
            animator.SetFloat("Movement",0.5f);
        }
        public void SetToRun()
        {
            animator.SetFloat("Movement",1);
        }
    }
}