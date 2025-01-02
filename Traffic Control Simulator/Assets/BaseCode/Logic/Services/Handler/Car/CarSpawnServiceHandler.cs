using System;
using BaseCode.Domain;
using BaseCode.Infrastructure.ScriptableObject;
using BaseCode.Logic.EntityHandler.Vehicles;
using BaseCode.Logic.Services.Interfaces.Car;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class CarSpawnServiceHandler : ICarSpawnService
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private VehicleScriptableObject[] carSoObjects;

        [SerializeField] private int poolSize;
        [SerializeField] private float spawnTimer;
        [SerializeField] private float timeToSpawn;
        
        public CarPool Pool { get; private set; }
        public CarManager CarManager { get; set; }

        public void Initialize(CarManager carManagerInScene)
        {
            CarManager = carManagerInScene; 
                
            //-----------------
            // this will change on level design
            VehicleScriptableObject currentCar = carSoObjects[Random.Range(0, carSoObjects.Length)]; 
            Pool = new CarPool(this,currentCar.vehiclePrefab, spawnPoint, poolSize, currentCar);
            //-----------------
        }

        public void Update()
        {
            if(Pool.IsThereCar && Time.time >= spawnTimer)
            {
                SpawnNewCar();
            }
        }

        public void SpawnNewCar()
        {
            spawnTimer = Time.time + timeToSpawn;
            
            VehicleBase newCar = (VehicleBase)Pool.InstantiateObject();
            
            newCar.AssignNewPathContainer();
        }
    }
}