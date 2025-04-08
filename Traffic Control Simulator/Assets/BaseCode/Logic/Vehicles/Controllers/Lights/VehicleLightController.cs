using System;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;

namespace BaseCode.Logic.Vehicles.Controllers.Lights
{
    
    public class VehicleLightController 
    {
        
        private VehicleController VehicleController { get; }

        private LightState _carLightState;
        private LightPlace _lightPlaceSave;
        public bool NeedToTurn;
        
        public VehicleLightController(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
 
        public void PassLightState(LightState state)
        {
            _carLightState = state;
            
            switch (_carLightState)
            {
                case LightState.Green:
                    VehicleController.SetState<VehicleMovementGoState>();
                    break;
                case LightState.Yellow:
                    VehicleController.SetState<VehicleMovementGoState>();
                    break;
                case LightState.Red:
                    break;
                case LightState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
        }
        public void PassLightPlaceState(LightPlace lightPlace) => _lightPlaceSave = lightPlace;
        public void ExitLightControl() => RestartVehicleLightController();
        public void RestartVehicleLightController() => _carLightState = LightState.None;
        public void ExitIntersection() => _lightPlaceSave = LightPlace.None;

        public LightState CarLightState => _carLightState;
        public LightPlace LightPlaceSave
        {
            get => _lightPlaceSave;
            set => _lightPlaceSave = value;
        }
    }
}