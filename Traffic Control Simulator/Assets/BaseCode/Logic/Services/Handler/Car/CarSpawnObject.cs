using System;
using BaseCode.Core;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Interfaces;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;
using Random = UnityEngine.Random;

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
        
        public void Initialize(CarManager carManagerInScene,CarSpawnServiceHandler carSpawnServiceHandler)
        {
            CarManager = carManagerInScene;
            Pool = new CarPool(carSpawnServiceHandler,carSoObjects.VehiclePrefab, poolSize, carSoObjects, carManagerInScene.ScoringManager);
        }
        
        public void Update()
        {
            if(Pool.IsThereCar && IsTimerUp())
            {
                SpawnNewCar();
            }
        }
        
        public void SpawnNewCar()
        {
            AddDelay(timeToSpawn);
            
            VehicleBase newCar = (VehicleBase)Pool.InstantiateObject();
            newCar.AssignNewPathContainer();
        }
        
        public void AddDelay(float delay)
        {
            Timer = Time.time + timeToSpawn;
        }
        public bool IsTimerUp()
        {
            return Time.time >= Timer;
        }
    }
}