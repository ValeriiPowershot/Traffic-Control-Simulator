using BaseCode.Core.ObjectPool.Base;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Handler.Car;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using BaseCode.Logic.Vehicles.Controllers.Score;
using BaseCode.Logic.Vehicles.States.Movement;
using UnityEngine;
using UnityEngine.Serialization;

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

        public bool NeedToTurn;
        public int SpawnIndex;
        public bool IsDiedOnCollision;

        public float acceptableWaitingTime;
        public float successPoints;
        
        public virtual void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            VehicleScriptableObject = currentCar;
            _carManager = manager;
            AssignCollisionController();
            
            ((CarLightServiceHandler)CarLightService).Starter(VehicleController);  // we can use it in initializer but i forgot why i did like this.
            ((PathContainerService)PathContainerService).Starter(manager);

            acceptableWaitingTime = VehicleScriptableObject.AcceptableWaitingTime;
            successPoints = VehicleScriptableObject.SuccessPoints;
            
            // Debug.Log("Current Car time:" + acceptableWaitingTime + " point: " + successPoints);
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

            if (IsDiedOnCollision == false) // willing to change this shit
            {
                GetComponent<ScoreObjectCarBase>().OnReachedDestination();
                IsDiedOnCollision = true;
            }
            
            CarManager.CarSpawnServiceHandler.CheckAllCarPoolMaxed();
            
            GoState.AssignNewSpeedValues();
        }
        
        public CarManager CarManager => _carManager;
        public VehicleMovementGoState GoState => VehicleController.StateController.GetState<VehicleMovementGoState>();

        public virtual void StartToMove()
        {
            gameObject.SetActive(true);
        }
        public void SetCarDiedOnCollision() => IsDiedOnCollision = true;
    }

    public enum LightState
    {
        None,
        Green,
        Red,
        Yellow,
    }
    
}