using System;
using System.Collections.Generic;
using BaseCode.Logic.Npcs.Npc;
using BaseCode.Logic.Npcs.States.Movement;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;

namespace BaseCode.Logic.Npcs.Controllers
{
    public class NpcMovementStateController
    {
        private readonly Dictionary<Type, INpcMovementState> _states = new Dictionary<Type, INpcMovementState>();
        
        private INpcMovementState _currentMovementMovementState;
        public NpcMovementStateController(NpcBase npcBase)
        {
            Controller = npcBase.npcController;
            _states[typeof(NpcMovementGoState)] = new NpcMovementGoState(this);
            _states[typeof(NpcMovementStopState)] = new NpcMovementStopState(this);
        }
        
        public void Update() => _currentMovementMovementState.MovementUpdate();

        public void SetState<T>() where T : INpcMovementState
        {
            if (_states.TryGetValue(typeof(T), out var newState))
            {
                _currentMovementMovementState?.MovementExit();
                _currentMovementMovementState = newState;
                _currentMovementMovementState.MovementEnter();
            }
            else
            {
                Debug.LogWarning($"State {typeof(T)} not found in the dictionary.");
            }
        }
        public NpcController Controller { get; set; }
    }
}