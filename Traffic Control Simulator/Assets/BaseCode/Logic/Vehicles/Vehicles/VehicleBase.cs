using BaseCode.Core.ObjectPool.Base;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Handler.Car;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class VehicleBase : PoolObjectBase
    {
        private CarManager _carManager;
        public VehicleScriptableObject VehicleScriptableObject { get; private set; }
        
        public readonly VehicleController VehicleController = new();
        public VehicleCollisionControllerBase VehicleCollisionController;
        
        public ICarLightService CarLightService { get; } = new CarLightServiceHandler();
        public IPathFindingService PathContainerService { get; } = new PathContainerService();
        
        public virtual void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            VehicleScriptableObject = currentCar;
            _carManager = manager;
            AssignCollisionController();
            
            ((CarLightServiceHandler)CarLightService).Starter(VehicleController);  // we can use it in initializer but i forgot why i did like this.
            ((PathContainerService)PathContainerService).Starter(manager);
        }

        protected virtual void AssignCollisionController()
        {
            VehicleCollisionController = new VehicleCollisionControllerBase();
            VehicleCollisionController.Starter(this);
        }

        public virtual void AssignNewPathContainer()
        {
            
        }

        public virtual void DestinationReached()
        {
            Pool.DestroyObject(this);
            CarManager.CarSpawnServiceHandler.CheckAllCarPoolMaxed();
        }
        
        public CarManager CarManager => _carManager;
        
    }

    public enum LightState
    {
        None,
        Green,
        Red,
        Yellow,
    }
    
}