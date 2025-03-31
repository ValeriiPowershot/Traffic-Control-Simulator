using System;
using System.Collections.Generic;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Services.Handler.Car
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
            if (carSpawnObjects[currentIndex].IsOnMap())
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
            foreach (var carSpawnObject in carSpawnObjects)
            {
                carSpawnObject.ResetCarSpawnObject();
            }
            
            foreach (var carDetector in createdCarDetectors)
                carDetector.ResetDetector();
            ReleaseCars();
        }
        public void ReleaseCars()
        {
            var sentToDestination = new List<VehicleBase>();
            sentToDestination.AddRange(onBoardGameCars);
            
            foreach (var vehicleBase in sentToDestination)
            {
                vehicleBase.lostScore = true;
                vehicleBase.GoState.VehiclePathController.SetPathToEndPosition();
            }
        }
        public bool IsCurrentIndexWaveFinished()
        {
            var result = currentIndex >= carSpawnObjects.Count; 
            Debug.Log(result);
            return result;
        }

    }
}