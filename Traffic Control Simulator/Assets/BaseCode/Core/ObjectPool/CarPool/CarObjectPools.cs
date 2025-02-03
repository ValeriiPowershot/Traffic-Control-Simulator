using BaseCode.Core.ObjectPool.Base;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Interfaces.Car;
using UnityEngine;

namespace BaseCode.Core.ObjectPool.CarPool
{
    public class CarObjectPools : ObjectPoolsBase
    {
        public override Pool GetPool(VehicleScriptableObject poolObjectPrefab)
        {
            return (CarPool)Pool[poolObjectPrefab];
        }
        
        public void AddCarToCarPool(ICarSpawnService carSpawnService, VehicleScriptableObject currentCar, int maxCarsCount)
        {
            Pool.Add(currentCar, new CarPool(carSpawnService,currentCar,maxCarsCount));
        }
    }
}