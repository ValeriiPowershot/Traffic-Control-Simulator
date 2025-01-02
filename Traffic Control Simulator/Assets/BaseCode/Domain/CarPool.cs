using System;
using BaseCode.Core.ObjectPool;
using BaseCode.Infrastructure.ScriptableObject;
using BaseCode.Logic.EntityHandler;
using BaseCode.Logic.EntityHandler.Vehicles;
using BaseCode.Logic.Services.Interfaces.Car;
using UnityEngine;

namespace BaseCode.Domain
{
    [Serializable]
    public class CarPool : Pool
    {
        private ICarSpawnService _carSpawnService;
        private VehicleScriptableObject _currentCar;
        public CarPool(ICarSpawnService carSpawnService, GameObject poolObjectPrefab, Transform spawnPoint, int maxCarsCount, VehicleScriptableObject currentCar) : 
            base(poolObjectPrefab, spawnPoint)
        {
            _currentCar = currentCar;
            Capacity = maxCarsCount;
            _carSpawnService = carSpawnService;
            
            InitializeQueue(Capacity);
        }
        public override IPoolObject InsertObjectToQueue()
        {
            VehicleBase newCar = (VehicleBase)base.InsertObjectToQueue();
            
            _carSpawnService.CarManager.ScoringManager.AddCar(newCar.GetComponent<IScoringObject>());
            newCar.Starter(_carSpawnService.CarManager, _currentCar);

            return newCar;
        }
        public bool IsThereCar => Queue.Count > 0;
    }
}