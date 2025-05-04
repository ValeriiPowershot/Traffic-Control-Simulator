using BaseCode.Logic.Managers;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers.Lights;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class BasicCar : VehicleBase
    {
        public override void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            base.Starter(manager, currentCar);
            vehicleController.Starter(this);
        }
        public virtual void Update()
        {
            vehicleController.Update(); 
        }

        public override void AssignNewPathContainer()
        {
            vehicleController.VehiclePathController.InitializeNewPath();
        }
    }
}