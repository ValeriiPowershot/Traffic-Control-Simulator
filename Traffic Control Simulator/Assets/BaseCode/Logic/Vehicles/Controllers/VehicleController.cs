using BaseCode.Logic.Vehicles.States.Movement;

namespace BaseCode.Logic.Vehicles.Controllers
{
    public class VehicleController 
    {
        public VehicleMovementStateController StateController { get; private set; }

        public void Starter(Vehicles.BasicCar basicCar)
        {
            BasicCar = basicCar;
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
        
        public Vehicles.BasicCar BasicCar { get; private set; }
    }
}