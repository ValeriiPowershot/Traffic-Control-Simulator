using BaseCode.Core.ObjectPool;
using BaseCode.Core.ObjectPool.Base;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Handler.Car;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles.Controllers;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class VehicleBase : PoolObjectBase
    {
        protected readonly VehicleController VehicleController = new VehicleController();
        public VehicleScriptableObject VehicleScriptableObject { get; private set; }

        public ICarLightService CarLightService { get; set; } = new CarLightServiceHandler();
        public IPathFindingService PathContainerService { get; set; } = new PathContainerService();
        
        public virtual void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            VehicleScriptableObject = currentCar;
            
            ((CarLightServiceHandler)CarLightService).Starter(VehicleController);  // we can use it in initializer but i forgot why i did like this.
            ((PathContainerService)PathContainerService).Starter(manager);
        }

        public virtual void AssignNewPathContainer()
        {
            
        }
        public virtual void DestinationReached()
        {
            Debug.Log("Reached");
            Pool.DestroyObject(this);
        }

    }

    public enum LightState
    {
        None,
        Green,
        Red,
        Yellow,
    }
    
}