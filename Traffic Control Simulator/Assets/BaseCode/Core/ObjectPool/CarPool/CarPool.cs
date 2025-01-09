using System;
using BaseCode.Core.ObjectPool.Base;
using BaseCode.Interfaces;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Core.ObjectPool.CarPool
{
    [Serializable]
    public class CarPool : Pool
    {
        private ICarSpawnService _carSpawnService;
        private VehicleScriptableObject _currentCar;
        public CarPool(ICarSpawnService carSpawnService, GameObject poolObjectPrefab, int maxCarsCount, VehicleScriptableObject currentCar) : 
            base(poolObjectPrefab)
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