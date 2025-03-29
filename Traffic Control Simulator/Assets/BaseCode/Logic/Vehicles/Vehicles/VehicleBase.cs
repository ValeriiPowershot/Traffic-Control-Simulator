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
        public VehicleCollisionControllerBase VehicleCollisionController;
        
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
            VehicleCollisionController = new VehicleCollisionControllerBase();
            VehicleCollisionController.Starter(this);
        }

        public virtual void AssignNewPathContainer()
        {
            
        }
        public virtual void DestinationReached()
        {
            Pool.DestroyObject(this);
            
            CarManager.CarSpawnServiceHandler.RemoveThisAndCheckAllCarPoolMaxed(this);
            UpdateCarScore();

            GoState.AssignNewSpeedValues();
            AssignNewTimeSores();
        }

        private void UpdateCarScore()
        {
            GetComponent<ScoreObjectCarBase>().OnReachedDestination(lostScore);
            lostScore = false;
        }
        private void AssignNewTimeSores()
        {
            acceptableWaitingTime = VehicleScriptableObject.AcceptableWaitingTime;
            successPoints = VehicleScriptableObject.SuccessPoints;
            // Debug.Log("Current Car time:" + acceptableWaitingTime + " point: " + successPoints);
        }
        public CarManager CarManager => _carManager;
        public VehicleMovementGoState GoState => VehicleController.StateController.GetState<VehicleMovementGoState>();

        public virtual void StartToMove()
        {
            gameObject.SetActive(true);
        }
        public void SetCarDiedOnCollision() => lostScore = true;
    }

    public enum LightState
    {
        None,
        Green,
        Red,
        Yellow,
    }
    
}