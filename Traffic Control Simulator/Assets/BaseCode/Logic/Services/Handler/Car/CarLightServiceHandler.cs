using System;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.States;

namespace BaseCode.Logic.Services.Handler.Car
{
    public class CarLightServiceHandler : ICarLightService
    {
        /*private VehicleController _vehicleController;
        private LightState _carLightState;
        private LightPlace _lightPlaceSave;
        
        public event Action LightExited;

        public void Starter(VehicleController vehicleController)
        {
            _vehicleController = vehicleController;
        }
        
        public void PassLightState(LightState state)
        {
            _carLightState = state;
            
            switch (_carLightState)
            {
                case LightState.Green:
                    _vehicleController.SetState<VehicleGoState>();
                    break;
                case LightState.Red:
                    //vehicleController.SetState<VehicleStopState>();
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
        public void PassLightPlaceState(LightPlace lightPlace)
        {
            _lightPlaceSave = lightPlace;
        }

        public void ExitLightControl()
        {
            _carLightState = LightState.None;
            LightExited?.Invoke(); //necessary for scoring system - (looked by tolga, its ok :D)
        }

        public void ExitIntersection()
        {
            _lightPlaceSave = LightPlace.None;
        }

        public LightState CarLightState => _carLightState;

        public LightPlace LightPlaceSave
        {
            get => _lightPlaceSave;
            set => _lightPlaceSave = value;
        }*/
        public void PassLightState(LightState carLightState)
        {
        }

        public void PassLightPlaceState(LightPlace lightPlace)
        {
        }

        public void ExitLightControl()
        {
        }

        public void ExitIntersection()
        {
        }

        public LightState CarLightState { get; }
        public LightPlace LightPlaceSave { get; set; }
        public event Action LightExited;
    }
}