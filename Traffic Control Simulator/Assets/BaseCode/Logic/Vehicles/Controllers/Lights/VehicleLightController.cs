using System;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;

namespace BaseCode.Logic.Vehicles.Controllers.Lights
{
    public class VehicleLightController 
    {
        private VehicleController _vehicleController;
        private LightState _carLightState;
        private LightPlace _lightPlaceSave;
        // public event Action LightExited;
        public bool NeedToTurn;

        public VehicleLightController(VehicleController vehicleController)
        {
            _vehicleController = vehicleController;
        }
        
        public void PassLightState(LightState state)
        {
            _carLightState = state;
            
            switch (_carLightState)
            {
                case LightState.Green:
                    _vehicleController.SetState<VehicleMovementGoState>();
                    break;
                case LightState.Yellow:
                    _vehicleController.SetState<VehicleMovementGoState>();
                    break;
                case LightState.Red:
                    break;
                case LightState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
        }
        public void PassLightPlaceState(LightPlace lightPlace) =>
            _lightPlaceSave = lightPlace;

        public void ExitLightControl()
        {
            RestartVehicleLightController();
            // LightExited?.Invoke(); //necessary for scoring system - (looked by tolga, its ok :D)
        }
        public void RestartVehicleLightController()
        {
            _carLightState = LightState.None;
        }
        public void ExitIntersection() =>
            _lightPlaceSave = LightPlace.None;

        public LightState CarLightState => _carLightState;

        public LightPlace LightPlaceSave
        {
            get => _lightPlaceSave;
            set => _lightPlaceSave = value;
        }
    }
}