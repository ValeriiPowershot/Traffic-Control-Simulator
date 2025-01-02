using System;
using BaseCode.Logic.EntityHandler.Vehicles.Controllers;
using BaseCode.Logic.EntityHandler.Vehicles.States;

namespace BaseCode.Logic.EntityHandler.Vehicles
{
    [Serializable]
    public class VehicleController 
    {
        public VehicleMovementStateController StateController { get; private set; }
        
        public void Starter(BasicVehicle basicVehicle)
        {
            BasicVehicle = basicVehicle;
            StateController = new VehicleMovementStateController(this);
            StartEngine();
        }

        public void StartEngine()
        {
            SetState<VehicleGoState>(); // Start in the stopped state
        }

        public void Update()
        {
            StateController.Update();
        }
 
        public void SetState<T>() where T : IVehicleMovementState
        {
            StateController.SetState<T>(); // Start in the stopped state
        }
        
        public BasicVehicle BasicVehicle { get; private set; }
    }
}













