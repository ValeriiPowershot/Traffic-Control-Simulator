using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers
{
    public class VehicleController 
    {
        public Transform CarTransform => VehicleBase.transform;
        public VehicleMovementStateController StateController { get; private set; }

        public void Starter(Vehicles.BasicCar basicCar)
        {
            VehicleBase = basicCar;
            StateController = new VehicleMovementStateController(this);
            
            StartEngine();
        }

        private void StartEngine()
        {
            SetState<VehicleMovementGoState>(); // Start in the stopped state
        }

        public void Update()
        {
            StateController.Update();
        }
 
        public void SetState<T>() where T : IVehicleMovementState
        {
            StateController.SetState<T>(); // Start in the stopped state
        }
        public IVehicleMovementState GetStateCurrentState()
        {
            return StateController.GetStateCurrentState();
        }
        public BasicCar VehicleBase { get; private set; }
    }
}