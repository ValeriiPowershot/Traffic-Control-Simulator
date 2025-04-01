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
        public List<VehicleBase> onBoardGameCars = new List<VehicleBase>();
        public List<CarDetector> createdCarDetectors = new List<CarDetector>();

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
            
            foreach (CarDetector carDetector in createdCarDetectors)
                carDetector.ResetDetector();
            
            foreach (VehicleBase vehicleBase in new List<VehicleBase>(onBoardGameCars))
                vehicleBase.DestinationReached(true);
        }
        public bool IsCurrentIndexWaveFinished()
        {
            var result = currentIndex >= carSpawnObjects.Count; 
            return result;
        }

    }
}