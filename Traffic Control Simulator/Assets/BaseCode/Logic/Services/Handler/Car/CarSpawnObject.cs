using System;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class CarSpawnObject : TimerBase
    {
        public CarManager CarManager { get; set; }

        [SerializeField] public int size;
        [SerializeField] private float timeToSpawn;

        private VehicleBase _newCar;
        private CarDetector _carDetector;
        
        private bool _isCarWaiting;
        private int _currentIndex;
        
        public VehicleScriptableObject carSoObjects;
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
            _newCar.gameObject.SetActive(false);
            CarSpawnServiceHandler.onBoardGameCars.Add(_newCar);
            
            _carDetector = _newCar.PathContainerService.GetCarDetector();
            _isCarWaiting = true;
        }
        
        public bool IsAllCarsSpawned() => _currentIndex > size;
        
        public bool IsOnMap() => _isCarWaiting == false && IsAllCarsSpawnedInSize();

        private bool IsAllCarsSpawnedInSize() => _currentIndex > size - 1;
        
        private bool IsThereACarWaitingToBeActive() => _isCarWaiting;

        private bool IsSpawnPointClear() => _carDetector.IsThereCarInSpawnPoint() == false;

        private CarSpawnServiceHandler CarSpawnServiceHandler => CarManager.CarSpawnServiceHandler;
    }
}