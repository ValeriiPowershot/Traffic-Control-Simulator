using System;
using Script.ScriptableObject;
using Script.Vehicles.Controllers;
using Script.Vehicles.States;
using UnityEngine;

namespace Script.Vehicles
{
    public class Vehicle : MonoBehaviour, ICar
    {
        [SerializeField] private VehicleController _vehicleController;
        
        public VehicleScriptableObjects VehicleScriptableObject;
        public LightState CarLightState { get; private set; }
        public Transform RayStartPoint;

        private void Start() =>
            _vehicleController.Starter(this);

        public void Update() =>
            _vehicleController.Update();

        public void OnDestroy() =>
            _vehicleController.CleanUp();

        public void PassLightState(LightState state)
        {
            Debug.Log("Passing light state to vehicle " + state);
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

            CarLightState = state;
        }
    }
}