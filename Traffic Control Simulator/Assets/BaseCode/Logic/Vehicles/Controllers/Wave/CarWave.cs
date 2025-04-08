using System;
using System.Collections.Generic;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.Managers;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.InterfaceHandler.Car;
using BaseCode.Logic.Vehicles.Vehicles;

namespace BaseCode.Logic.Vehicles.Controllers.Wave
{
    [Serializable]
    public class CarWave
    {
        private CarManager _carManager;
        private CarObjectPools _carObjectPools;

        public int currentIndex;
        public WaveRank waveRank = WaveRank.Easy;
        
        public List<CarSpawnObject> carSpawnObjects = new();
        
        public void Initialize(CarManager carManagerInScene,CarObjectPools carObjectPools)
        {
            _carManager = carManagerInScene;
            _carObjectPools = carObjectPools;
            
            foreach (CarSpawnObject wave in carSpawnObjects)
            {
                wave.Initialize(_carManager, (CarPool)_carObjectPools.GetPool(wave.carSoObjects));
            }
        }
        public void Update()
        {
            if (carSpawnObjects[currentIndex].IsAllCarsSpawnedOnMap())
            {
                currentIndex++;

                if (IsCurrentIndexWaveFinished())
                    _carManager.ExitWave();
            }
            else
                carSpawnObjects[currentIndex].Update();
            
        }

        public void ResetWave()
        {
            currentIndex = 0;
            foreach (CarSpawnObject carSpawnObject in carSpawnObjects)
                carSpawnObject.ResetCarSpawnObject();
            
            foreach (CarDetector carDetector in CreatedCarDetectors)
                carDetector.ResetDetector();
            
            foreach (VehicleBase vehicleBase in new List<VehicleBase>(OnBoardGameCars))
                vehicleBase.DestinationReached(true);
        }
        public bool IsCurrentIndexWaveFinished()
        {
            var result = currentIndex >= carSpawnObjects.Count; 
            return result;
        }
        public List<VehicleBase> OnBoardGameCars { get; } = new List<VehicleBase>();
        public List<CarDetector> CreatedCarDetectors { get; } = new List<CarDetector>();
    }
}