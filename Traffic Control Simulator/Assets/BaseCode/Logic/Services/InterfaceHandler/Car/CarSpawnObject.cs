using System;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.Managers;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers.Path;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using BaseCode.Utilities;
using UnityEngine;

namespace BaseCode.Logic.Services.InterfaceHandler.Car
{
    [Serializable]
    public class CarSpawnObject : TimerBase
    {
        public CarManager CarManager { get; set; }
        public VehicleScriptableObject carSoObjects;

        [SerializeField] public int size;
        [SerializeField] private float timeToSpawn;

        private VehicleBase _newCar;
        private CarDetector _carDetector;
        
        private bool _isCarWaiting;
        private int _currentIndex;
        
        private CarPool _carPool;
        public void Initialize(CarManager carManagerInScene, CarPool carPool)
        {
            CarManager = carManagerInScene;
            _carPool = carPool;
            _currentIndex = 0;
        }

        public void Update()
        {
            if (IsThereACarWaitingToBeActive())
            {
                if (IsSpawnPointClear())
                {
                    StartCarEngine();
                }
            }
            else
            {
                if (IsAllCarsSpawned()) 
                    return;
                    
                if (CanSpawnNewCar())
                    SpawnNewCar();
            }
        }

        private void StartCarEngine()
        {
            _newCar.StartToMove();
            _newCar = null;
            _isCarWaiting = false;
            ResetTimer();
        }

        private bool CanSpawnNewCar()
        {
            return _carPool.IsPoolEmpty() == false && IsTimerUp();
        }

        public void ResetCarSpawnObject()
        {
            _currentIndex = 0; 
            _newCar = null;
            _isCarWaiting = false;
        }
        public void SpawnNewCar()
        {
            _currentIndex++;
            AddDelay(timeToSpawn);
            
            _newCar = (VehicleBase)_carPool.InstantiateObject();
            _newCar.AssignNewPathContainer();
            _newCar.vehicleController.VehicleStateController.SetState<VehicleMovementGoState>();
            _newCar.gameObject.SetActive(false);
            CarSpawnServiceHandler.CarWaveController.GetCurrentWave().OnBoardGameCars.Add(_newCar);
            
            _carDetector = PathContainer.GetCarDetector();
            _isCarWaiting = true;
        }
        
        public bool IsAllCarsSpawned() => _currentIndex > size;
        
        public bool IsAllCarsSpawnedOnMap() => _isCarWaiting == false && IsAllCarsSpawnedInSize();

        private bool IsAllCarsSpawnedInSize() => _currentIndex > size - 1;
        
        private bool IsThereACarWaitingToBeActive() => _isCarWaiting;

        private bool IsSpawnPointClear() => _carDetector.IsThereCarInSpawnPoint() == false;
        private PathContainer PathContainer => _newCar.vehicleController.VehiclePathController.PathContainer;

        private CarSpawnServiceHandler CarSpawnServiceHandler => CarManager.CarSpawnServiceHandler;
    }
}