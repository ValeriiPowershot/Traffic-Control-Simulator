using System;
using BaseCode.Logic.Ways;
using Script.Vehicles.States;

namespace Script.Vehicles.Controllers
{
    [Serializable]
    public class VehicleController 
    {
        private VehicleMovementStateController _movementStateController;
        
        public VehicleMovementStateController StateController { get { return _movementStateController; } }

        public void Starter(Vehicle vehicle)
        {
            Vehicle = vehicle;
            _movementStateController = new VehicleMovementStateController(this);
            
            StartEngine();
        }

        public void StartEngine()
        {
            SetState<VehicleGoState>(); // Start in the stopped state
        }

        public void Update()
        {
            _movementStateController.Update();
        }
 
        public void SetState<T>() where T : IVehicleState
        {
            _movementStateController.SetState<T>(); // Start in the stopped state
        }
        
        public Vehicle Vehicle { get; private set; }
    }
}













