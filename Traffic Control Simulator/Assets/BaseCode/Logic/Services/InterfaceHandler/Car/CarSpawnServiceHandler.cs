using System;
using System.Collections.Generic;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.Managers;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles.Controllers.Wave;
using BaseCode.Logic.Vehicles.Vehicles;

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
    }
 
    
    
}