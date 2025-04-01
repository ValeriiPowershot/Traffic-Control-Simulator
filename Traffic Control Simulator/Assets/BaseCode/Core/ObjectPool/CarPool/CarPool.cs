using System;
using BaseCode.Core.ObjectPool.Base;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.InterfaceHandler.Car;
using BaseCode.Logic.Vehicles.Vehicles;

namespace BaseCode.Core.ObjectPool.CarPool
{
    [Serializable]
    public class CarPool : Pool
    {
        private ICarSpawnService _carSpawnService;
        private VehicleScriptableObject _currentCar;
        public CarPool(ICarSpawnService carSpawnService, VehicleScriptableObject currentCar, int maxCarsCount) : 
            base(currentCar.vehiclePrefab)
        {
            _currentCar = currentCar;
            Capacity = maxCarsCount;
            _carSpawnService = carSpawnService;
            
            InitializeQueue(Capacity);
        } 

        public override IPoolObject InsertObjectToQueue()
        {
            VehicleBase newCar = (VehicleBase)base.InsertObjectToQueue();
            newCar.Starter(_carSpawnService.CarManager, _currentCar);
            return newCar;
        }
        
        public bool IsThereCar => Queue.Count > 0;

        public bool IsPoolEmpty()
        {
            return IsThereCar == false;
        }
    }
}