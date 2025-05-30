using System;
using System.Collections.Generic;
using BaseCode.Core.ObjectPool.Base;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.Lights.Handler.Abstracts;
using BaseCode.Logic.Managers;
using BaseCode.Logic.Roads;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers.Wave;
using BaseCode.Logic.Ways;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BaseCode.Logic.Services.InterfaceHandler.Car
{
    [Serializable]
    public class CarLevels
    {
        private CarManager _carManager;
        private CarObjectPools _carObjectPools;
        
        public Transform cameraPosition; // after testing move this to cinema machine
                
        public List<CarWave> waves = new();
        public List<TripleRoadIntersection> roads = new List<TripleRoadIntersection>();
        private List<LightBase> _createdLights = new List<LightBase>();

        private int _spawnedCarIndex;
        public int currentWaveIndex;
        public void InitializeLevel(CarManager carManager)
        {
            _carManager = carManager;
            _carObjectPools = CarSpawnServiceHandler.CarObjectPools;
            
            InitializePools();
            InitializeWave();
            SpawnRoadDetectors();
            FindLights();
        }
        private void InitializeWave()
        {
            foreach (CarWave wave in waves) 
                wave.Initialize(_carManager, _carObjectPools);
            
            currentWaveIndex = 0;
        }
        
        private void FindLights()
        {
            foreach (var road in roads)
            {
                _createdLights.AddRange(road.basicLights);
            }
        }
        public void ResetLevel()
        {
            foreach (CarWave wave in waves) 
                wave.ResetWave();
            
            foreach (var lightBase in _createdLights)
                lightBase.RemoveAllVehicle();
        }
        private void InitializePools()
        {
            Dictionary<VehicleScriptableObject, int> setOfCurrentWaveSo = GetCurrentSetsWithCounts();
            
            foreach (KeyValuePair<VehicleScriptableObject, int> uniqueVehicleSo in setOfCurrentWaveSo) 
                _carObjectPools.AddCarToCarPool(CarSpawnServiceHandler, uniqueVehicleSo.Key, uniqueVehicleSo.Value);
        }

        public void SpawnRoadDetectors()
        {
            int currentLevelIndex = CarSpawnServiceHandler.GetLevelIndex(this);
            foreach (WaypointContainer container in CarSpawnServiceHandler.GetContainerListByIndex(currentLevelIndex))
            {
                Transform firstElement = container.roadPoints[0].point.transform;
                CarDetector carDetectorObject =  Object.Instantiate(AllWaysContainer.carDetectorPrefab, firstElement.position, firstElement.rotation, firstElement);
                GetCurrentWave().CreatedCarDetectors.Add(carDetectorObject);
                
                carDetectorObject.carDetectorSpawnIndex = _spawnedCarIndex;
                _spawnedCarIndex++;
            }
        }
        private Dictionary<VehicleScriptableObject, int> GetCurrentSetsWithCounts()
        {
            Dictionary<VehicleScriptableObject, Pool> currentPool = _carObjectPools.Pool;
            Dictionary<VehicleScriptableObject, int> vehicleCounts = new();

            foreach (CarWave wave in waves)
            {
                CheckCarsForPrePoolItemCounter(wave, ref vehicleCounts, ref currentPool);
            }

            return vehicleCounts;
        }

        private void CheckCarsForPrePoolItemCounter(CarWave wave, ref Dictionary<VehicleScriptableObject, int> vehicleCounts, ref Dictionary<VehicleScriptableObject, Pool> currentPool)
        {
            foreach (CarSpawnObject carSpawnObject in wave.carSpawnObjects)
            {
                VehicleScriptableObject carSo = carSpawnObject.carSoObjects;
                int requestSize = carSpawnObject.size;

                if (currentPool.ContainsKey(carSo))
                {
                    requestSize -= currentPool[carSo].GetPoolSize();
                    if(requestSize < 0 )
                        continue;
                        
                    currentPool[carSo].AddToQueue(requestSize);
                }
                else
                {
                    vehicleCounts[carSo] = requestSize;
                }
            }
        }

        public CarWave GetCurrentWave()
        {
            return waves[currentWaveIndex];
        }

        private AllWaysContainer AllWaysContainer => _carManager.allWaysContainer;
        public CarSpawnServiceHandler CarSpawnServiceHandler => _carManager.CarSpawnServiceHandler;

    }
}