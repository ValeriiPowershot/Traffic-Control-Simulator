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
                    _newCar.gameObject.SetActive(true);
                    _newCar = null;
                    _isCarWaiting = false;
                }
            }
            else
            {
                if(IsAllCarsSpawned())
                    return;
                
                if (_carPool.IsThereCar && IsTimerUp())
                {
                    SpawnNewCar();
                }
            }
        }

        private bool IsAllCarsSpawned() =>
            _currentIndex >= size;

        private bool IsThereACarWaitingToBeActive() =>
            _isCarWaiting;

        private bool IsSpawnPointClear() =>
            _carDetector.IsThereCarInSpawnPoint() == false;

        public void SpawnNewCar()
        {
            _currentIndex++;
            AddDelay(timeToSpawn);

            _newCar = (VehicleBase)_carPool.InstantiateObject();
            _newCar.AssignNewPathContainer();
            _newCar.gameObject.SetActive(false);

            _carDetector = _newCar.PathContainerService.GetCarDetector();
            _isCarWaiting = true;
        }
    }
}