using System;
using System.Collections.Generic;
using Script.Vehicles.States;
using UnityEngine;

namespace Script.Vehicles.Controllers
{
    public class VehicleMovementStateController
    {
        // Vehicle Controller
        private readonly VehicleController _vehicleController;
        
        // States
        private readonly Dictionary<Type, IVehicleState> _states = new Dictionary<Type, IVehicleState>();
        
        // Base State
        private IVehicleState _currentMovementState;

        public VehicleMovementStateController(VehicleController vehicleController)
        {
            _vehicleController = vehicleController;

            _states[typeof(VehicleGoState)] = new VehicleGoState(_vehicleController);
            _states[typeof(VehicleSlowDownState)] = new VehicleSlowDownState(_vehicleController);
            _states[typeof(VehicleStopState)] = new VehicleStopState(_vehicleController);
        }

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

        public void Update()
        {
            _currentMovementState.MovementUpdate();
 
        }

        // Used to test the states
        
    }
}