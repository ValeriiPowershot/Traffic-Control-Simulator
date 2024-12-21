using System;
using Script.ScriptableObject;
using Script.Vehicles.Controllers;
using Script.Vehicles.States;
using UnityEngine;

namespace Script.Vehicles
{
    public class Vehicle : BasicCar
    {
        [SerializeField] private VehicleController _vehicleController;
        
        public VehicleScriptableObject VehicleScriptableObject;
        public Transform RayStartPoint;

        // this will be called by spawn manager
        private void Start() =>
            _vehicleController.Starter(this);

        public void Update() =>
            _vehicleController.Update();

        public void OnDestroy() =>
            _vehicleController.CleanUp();

        public override void PassLightState(LightState state)
        {
            Debug.Log("Passing light state to vehicle " + state);
            base.PassLightState(state);

            switch (state)
            {
                case LightState.Green:
                    _vehicleController.SetState<VehicleGoState>();
                    break;
                case LightState.Red:
                    _vehicleController.SetState<VehicleStopState>();
                    break;
                case LightState.Yellow:
                    _vehicleController.SetState<VehicleSlowDownState>();
                    break;
                case LightState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}