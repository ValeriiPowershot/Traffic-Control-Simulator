using BaseCode.Core.ObjectPool.Base;
using BaseCode.Interfaces;
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
        
        public ICarLightService CarLightService { get; } = new CarLightServiceHandler();
        public IPathFindingService PathContainerService { get; } = new PathContainerService();

        public bool NeedToTurn;
        public int SpawnIndex;
        public bool lostScore;

        public float acceptableWaitingTime;
        public float successPoints;

        public IScoringObject ScoringObject;
        
        public virtual void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            VehicleScriptableObject = currentCar;
            _carManager = manager;
            AssignCollisionController();
            
            ((CarLightServiceHandler)CarLightService).Starter(VehicleController);  // we can use it in initializer but i forgot why i did like this.
            ((PathContainerService)PathContainerService).Starter(manager);
            AssignNewTimeSores();
            
            ScoringObject = GetComponent<IScoringObject>();
            ScoringObject.Initialize(_carManager.GameManager.scoringManager);
        }

        protected virtual void AssignCollisionController()
        {
            VehicleController.VehicleCollisionController = new VehicleCollisionControllerBase();
            VehicleController.VehicleCollisionController.Starter(this);
        }

        public virtual void AssignNewPathContainer()
        {
            
        }
        public virtual void DestinationReached()
        {
            Pool.DestroyObject(this);
            
            CarManager.CarSpawnServiceHandler.OnCarReachedDestination(this);
            UpdateCarScore();
            AssignNewRandomValues();
        }
        private void UpdateCarScore()
        {
            ScoringObject.OnReachedDestination(lostScore);
            lostScore = false;
        }
        private void AssignNewRandomValues()
        {
            GoState.AssignNewSpeedValues();
            AssignNewTimeSores();
        }
        private void AssignNewTimeSores()
        {
            acceptableWaitingTime = VehicleScriptableObject.AcceptableWaitingTime;
            successPoints = VehicleScriptableObject.SuccessPoints;
        }

        public virtual void StartToMove()
        {
            gameObject.SetActive(true);
        }
        
        public bool IsCarReachedEnd() => lostScore;
        public void SetCarDiedOnCollision() => lostScore = true;
        public CarManager CarManager => _carManager;
        public VehicleMovementGoState GoState => VehicleController.StateController.GetState<VehicleMovementGoState>();
    }

    public enum LightState
    {
        None,
        Green,
        Red,
        Yellow,
    }
    
}