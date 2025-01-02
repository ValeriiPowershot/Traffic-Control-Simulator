using System;
using System.Collections.Generic;
using BaseCode.Logic.EntityHandler.Vehicles.States;
using UnityEngine;

namespace BaseCode.Logic.EntityHandler.Vehicles.Controllers
{
    public class VehicleMovementStateController
    {
        private readonly Dictionary<Type, IVehicleMovementState> _states = new();
        private IVehicleMovementState _currentMovementMovementState;

        public VehicleMovementStateController(VehicleController vehicleController)
        {
            _states[typeof(VehicleGoState)] = new VehicleGoState(vehicleController);
            _states[typeof(VehicleSlowDownState)] = new VehicleSlowDownState(vehicleController);
            _states[typeof(VehicleStopState)] = new VehicleStopState(vehicleController);
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
        
        public void InitializePath()
        {
            ((VehicleGoState)_states[typeof(VehicleGoState)]).InitializePath();
        }
        
    }
}