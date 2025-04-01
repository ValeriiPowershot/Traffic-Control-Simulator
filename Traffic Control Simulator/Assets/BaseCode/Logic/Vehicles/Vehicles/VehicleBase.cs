using BaseCode.Core.ObjectPool.Base;
using BaseCode.Logic.Managers;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.InterfaceHandler.Car;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class VehicleBase : PoolObjectBase
    {
        private CarManager _carManager;
        public VehicleScriptableObject VehicleScriptableObject { get; private set; }
        public readonly VehicleController VehicleController = new();

        public virtual void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            VehicleScriptableObject = currentCar;
            _carManager = manager;
        }

        public virtual void AssignCollisionController()
        {
            VehicleController.VehicleCollisionController = new VehicleCollisionControllerBase();
            VehicleController.VehicleCollisionController.Starter(this);
        }

        public virtual void AssignNewPathContainer()
        {
            
        }
        public virtual void DestinationReached(bool isDied = false) // when it is reached to end without collision
        {
            Pool.DestroyObject(this);
            CarSpawnServiceHandler.OnCarReachedDestination(this);
            VehicleController.RestartControllers(isDied);
        }
        public virtual void StartToMove()
        {
            gameObject.SetActive(true);
        }
        public CarManager CarManager => _carManager;
        public CarSpawnServiceHandler CarSpawnServiceHandler => CarManager.CarSpawnServiceHandler;
    }

    public enum LightState
    {
        None,
        Green,
        Red,
        Yellow,
    }
    
}