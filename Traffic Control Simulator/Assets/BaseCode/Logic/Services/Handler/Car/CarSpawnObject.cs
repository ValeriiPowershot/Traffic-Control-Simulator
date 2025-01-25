using System;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Interfaces;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class CarSpawnObject : ITimeUsable
    {
        public CarPool Pool { get; private set; }
        public CarManager CarManager { get; set; }
        public float Timer { get; set; }

        [SerializeField] private int poolSize;
        [SerializeField] private float timeToSpawn;

        [SerializeField] private VehicleScriptableObject carSoObjects;

        private VehicleBase _newCar;
        private CarDetector _carDetector;
        public void Initialize(CarManager carManagerInScene, CarSpawnServiceHandler carSpawnServiceHandler)
        {
            CarManager = carManagerInScene;
            Pool = new CarPool(carSpawnServiceHandler, carSoObjects.VehiclePrefab, poolSize, carSoObjects,
                carManagerInScene.ScoringManager);
        }

        public void Update()
        {
            if (_carDetector != null && _carDetector.isCarWaiting)
            {
                if (_carDetector.IsThereCarInSpawnPoint() == false)
                {
                    _newCar.gameObject.SetActive(true);
                    _carDetector.isCarWaiting = false;
                }
            }
            else
            {
                if (Pool.IsThereCar && IsTimerUp())
                {
                    SpawnNewCar();
                }
            }
            
        }
        
        public void SpawnNewCar()
        {
            AddDelay(timeToSpawn);

            _newCar = (VehicleBase)Pool.InstantiateObject();
            _newCar.AssignNewPathContainer();

            _newCar.gameObject.SetActive(false);

            _carDetector = _newCar.PathContainerService.GetIndexWaypoint(0).point.GetChild(0).GetComponent<CarDetector>();
            _carDetector.isCarWaiting = true;
        }
   
        public void AddDelay(float delay)
        {
            Timer = Time.time + delay;
        }

        public bool IsTimerUp()
        {
            return Time.time >= Timer;
        }
    }
}