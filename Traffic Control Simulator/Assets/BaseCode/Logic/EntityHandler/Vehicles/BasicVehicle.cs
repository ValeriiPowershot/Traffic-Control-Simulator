using BaseCode.Infrastructure.ScriptableObject;
using BaseCode.Logic.Ways;
using UnityEngine;


namespace BaseCode.Logic.EntityHandler.Vehicles
{
    public class BasicVehicle : VehicleBase
    {
        public Transform rayStartPoint;
        public Transform arrowIndicatorEndPoint;
        
        public override void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            base.Starter(manager, currentCar);
            vehicleController.Starter(this);
        }
        
        public void Update() => vehicleController.Update();
        
        public override void AssignNewPathContainer()
        {
            PathContainerService.SetNewPathContainerRandomly();
            
            transform.SetPositionAndRotation(PathContainerService.GetFirstPosition(), Quaternion.identity);
            
            vehicleController.StateController.InitializePath();
        }
    }
}