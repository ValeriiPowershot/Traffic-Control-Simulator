using System;
using System.Collections.Generic;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.Managers;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers.Wave;
using BaseCode.Logic.Vehicles.Vehicles;
using BaseCode.Logic.Ways;

namespace BaseCode.Logic.Services.InterfaceHandler.Car
{
    [Serializable]
    public class CarSpawnServiceHandler : ICarSpawnService
    {
        public CarManager CarManager { get; set; }
        public CarObjectPools CarObjectPools = new();
        public WavesRankScriptableObject wavesRankSo;

        public List<CarLevels> carLevels = new List<CarLevels>();
        
        public CarWaveUIController CarWaveUIController;
        public CarWaveController CarWaveController;
        
        public void Initialize(CarManager carManagerInScene)
        {
            CarManager = carManagerInScene;
            
            CarWaveUIController = new CarWaveUIController(this);
            CarWaveController = new CarWaveController(this);
            
            CarWaveController.InitializeLevel();
        }
        public void Update()
        {
            CarWaveController.Update();
        }
        public void OnCarReachedDestination(VehicleBase vehicleBase)
        {
            CarWaveController.OnCarReachedDestination(vehicleBase);
        }

        public List<WaypointContainer> GetCurrentContainerList()
        {
            int currentIndex = CarManager.CarSpawnServiceHandler.CarWaveController.CurrentLevelIndex;
            return GetContainerListByIndex(currentIndex);
        }
        public List<WaypointContainer> GetContainerListByIndex(int ind)
        {
            return CarManager.allWaysContainer.GetWayGroupByIndex(ind);
        }
        public int GetLevelIndex(CarLevels find)
        {
            return carLevels.IndexOf(find);
        }
    }
 
    
    
}