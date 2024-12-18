using System;
using Script.So;
using Script.Vehicles.Controllers;
using Script.Vehicles.States;
using UnityEngine;

namespace Script.Vehicles
{
    public class Vehicle : MonoBehaviour, ICar
    {
        public VehicleController vehicleController;
        public VehicleSo vehicleSo;

        private void Start() // this will be called by spawn manager
        {
            vehicleController.Starter(this);
        }

        public void Update()
        {
            // every car had a update but then i added dotween and it was not needed anymore, i realize dotween is bullshit for managing multiple cars 
        //    vehicleController.Update();
        }

        public void OnDestroy()
        {
            vehicleController.CleanUp();
        }

        public void PassLightState(LightState state)
        {
            Debug.Log("Passing light state to vehicle " + state);
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
        }
    }
}