using System;
using System.Collections.Generic;
using BaseCode.Logic.Ways;
using Script.Vehicles.States;
using UnityEngine;

namespace Script.Vehicles.Controllers
{
    public class VehicleMovementStateController
    {
        private readonly VehicleController _vehicleController;
        private readonly Dictionary<Type, IVehicleState> _states = new Dictionary<Type, IVehicleState>();
        private IVehicleState _currentMovementState;

        public VehicleMovementStateController(VehicleController vehicleController)
        {
            _vehicleController = vehicleController;

            _states[typeof(VehicleGoState)] = new VehicleGoState(_vehicleController);
            _states[typeof(VehicleSlowDownState)] = new VehicleSlowDownState(_vehicleController);
            _states[typeof(VehicleStopState)] = new VehicleStopState(_vehicleController);
        }
        
        public void Update() =>
            _currentMovementState.MovementUpdate();

        public void SetState<T>() where T : IVehicleState
        {
            if (_states.TryGetValue(typeof(T), out var newState))
            {
                _currentMovementState?.MovementExit();
                _currentMovementState = newState;
                _currentMovementState.MovementEnter();
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

        // Used to test the states
        
    }
}