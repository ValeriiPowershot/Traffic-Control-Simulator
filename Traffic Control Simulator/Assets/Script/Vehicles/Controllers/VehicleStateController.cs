using System;
using System.Collections.Generic;
using Script.Vehicles.States;
using UnityEngine;

namespace Script.Vehicles.Controllers
{
    public class VehicleStateController
    {
        // Vehicle Controller
        private readonly VehicleController _vehicleController;
        
        // States
        private readonly Dictionary<Type, IVehicleState> _states = new Dictionary<Type, IVehicleState>();
        
        // Base State
        private IVehicleState _currentMovementState;

        public VehicleStateController(VehicleController vehicleController)
        {
            _vehicleController = vehicleController;

            _states[typeof(VehicleGoState)] = new VehicleGoState();
            _states[typeof(VehicleSlowDownState)] = new VehicleSlowDownState();
            _states[typeof(VehicleStopState)] = new VehicleStopState();

            SetState<VehicleGoState>();
        }

        public void SetState<T>() where T : IVehicleState
        {
            if (_states.TryGetValue(typeof(T), out var newState))
            {
                _currentMovementState = newState;
            }
            else
            {
                Debug.LogWarning($"State {typeof(T)} not found in the dictionary.");
            }
        }

        public void Update()
        {
            _currentMovementState.MovementStateHandler(_vehicleController);

            InputTest();
        }

        // Used to test the states
        private void InputTest()
        {
            if(Input.GetKey(KeyCode.W))
                SetState<VehicleGoState>();
            else if(Input.GetKey(KeyCode.D))
                SetState<VehicleSlowDownState>();
            else if(Input.GetKey(KeyCode.Space) || (Input.GetKey(KeyCode.S)))
                SetState<VehicleStopState>();
        }
    }
}