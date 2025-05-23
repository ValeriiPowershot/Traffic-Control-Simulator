using System;
using System.Collections.Generic;
using BaseCode.Logic.Vehicles.States.Movement;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers
{
    public class VehicleMovementStateController
    {
        private readonly Dictionary<Type, IVehicleMovementState> _states = new();
        private IVehicleMovementState _currentMovementMovementState;
        public VehicleMovementStateController(VehicleController vehicleController)
        {
            _states[typeof(VehicleMovementGoState)] = new VehicleMovementGoState(vehicleController);
            _states[typeof(VehicleMovementCrashState)] = new VehicleMovementCrashState(vehicleController);
            _states[typeof(VehicleMovementStopState)] = new VehicleMovementStopState(vehicleController);
        }
        
        public void Update() => _currentMovementMovementState.MovementUpdate();

        public void SetState<T>() where T : IVehicleMovementState
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

        public T GetState<T>() where T : IVehicleMovementState =>
            (T)_states[typeof(T)];
        
        public bool IsOnState<T>() where T : IVehicleMovementState => _currentMovementMovementState is T;

        public IVehicleMovementState GetStateCurrentState() =>
            _currentMovementMovementState;

        public Dictionary<Type, IVehicleMovementState> GetStatesDict() =>
            _states;

        public void RestartVehicleMovementStateController()
        {
            GetState<VehicleMovementGoState>().AssignNewSpeedValues();
            
        }
    }
}