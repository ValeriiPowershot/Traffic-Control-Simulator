using BaseCode.Core.ObjectPool;
using BaseCode.Domain.Entity;
using BaseCode.Infrastructure.ScriptableObject;
using BaseCode.Logic.EntityHandler.Vehicles.Controllers;
using BaseCode.Logic.Services.Handler.Car;
using BaseCode.Logic.Services.Interfaces.Car;
using UnityEngine;

namespace BaseCode.Logic.EntityHandler.Vehicles
{
    public class VehicleBase : PoolObjectBase, ICar
    {
        [SerializeField] protected VehicleController vehicleController;
        public VehicleScriptableObject VehicleScriptableObject { get; private set; }
        

        public ICarLightService CarLightService { get; set; } = new CarLightServiceHandler();
        public IPathFindingService PathContainerService { get; set; } = new PathContainerService();
        
        public virtual void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            VehicleScriptableObject = currentCar;
            
            ((CarLightServiceHandler)CarLightService).Starter(vehicleController); // sorry for this
            ((PathContainerService)PathContainerService).Starter(manager); // sorry for this
        }

        public virtual void AssignNewPathContainer()
        {
            
        }
        public void DestinationReached()
        {
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