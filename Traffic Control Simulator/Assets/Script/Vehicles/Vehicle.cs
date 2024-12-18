using System;
using Script.So;
using Script.Vehicles.Controllers;
using Script.Vehicles.States;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Vehicles
{
    public class Vehicle : BasicCar
    {
        public VehicleController vehicleController;
        public VehicleSo vehicleSo;

        public LightState carLightState; // re u in light space or on free space - if it is none then it is on free space
        public Transform rayStartPoint;

        public event Action LightPassed;

        private void Start() // this will be called by spawn manager
        {
            vehicleController.Starter(this);
        }

        public void Update()
        {
            // every car had a update but then i added dotween and it was not needed anymore, i realize dotween is bullshit for managing multiple cars 
            vehicleController.Update();
        }

        public void OnDestroy()
        {
            vehicleController.CleanUp();
        }

        public override void PassLightState(LightState state)
        {
            Debug.Log("Passing light state to vehicle " + state);
            base.PassLightState(state);

            switch (state)
            {
                case LightState.Green:
                    vehicleController.SetState<VehicleGoState>();
                    break;
                case LightState.Red:
                    vehicleController.SetState<VehicleStopState>();
                    break;
                case LightState.Yellow:
                    vehicleController.SetState<VehicleSlowDownState>();
                    break;
                case LightState.None:
                    break;
            }

            carLightState = state;
        }
    }
}