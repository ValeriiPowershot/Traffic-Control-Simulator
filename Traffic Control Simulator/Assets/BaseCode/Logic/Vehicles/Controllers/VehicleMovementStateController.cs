using System;
using System.Collections.Generic;
using BaseCode.Logic.Vehicles.States.Movement;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers
{
    public class VehicleMovementStateController
    {
        private readonly Dictionary<Type, IVehicleMovementState> _states = new Dictionary<Type, IVehicleMovementState>();
        private IVehicleMovementState _currentMovementMovementState;

        public VehicleMovementStateController(VehicleController vehicleController)
        {
            _states[typeof(VehicleMovementGoState)] = new VehicleMovementGoState(vehicleController);
            _states[typeof(VehicleMovementSlowDownState)] = new VehicleMovementSlowDownState(vehicleController);
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
        
        public void InitializePath()
        {
            ((VehicleMovementGoState)_states[typeof(VehicleMovementGoState)]).InitializePath();
        }

        // Used to test the states

        public IVehicleMovementState GetStateCurrentState()
        {
            return _currentMovementMovementState;
        }
    }
}