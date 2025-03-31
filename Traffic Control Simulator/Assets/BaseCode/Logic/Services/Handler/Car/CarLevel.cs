using System;
using System.Collections.Generic;
using BaseCode.Core.ObjectPool.Base;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.Lights.Handler.Abstracts;
using BaseCode.Logic.Roads;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Ways;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class CarLevel
    {
        private CarManager _carManager;
        private CarObjectPools _carObjectPools;
        
        public Transform cameraPosition; // after testing move this to cinema machine
                
        public List<CarWave> waves = new();
        public List<TripleRoadIntersection> roads = new List<TripleRoadIntersection>();
        public List<LightBase> createdLights = new List<LightBase>();

        public int currentWaveIndex;
        public bool looped;
        private int _spawnedCarIndex;
        
        public void InitializeLevel(CarManager carManager)
        {
            _carManager = carManager;
            _carObjectPools = _carManager.CarSpawnServiceHandler.CarObjectPools;
            
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
                createdLights.AddRange(road.basicLights);
            }
        }
        public void ResetWave()
        {
            foreach (CarWave wave in waves) 
                wave.ResetWave();
            
            foreach (var lightBase in createdLights)
                lightBase.RemoveAllVehicle();
        }
        private void InitializePools()
        {
            Dictionary<VehicleScriptableObject, int> setOfCurrentWaveSo = GetCurrentSetsWithCounts();
            
            foreach (KeyValuePair<VehicleScriptableObject, int> uniqueVehicleSo in setOfCurrentWaveSo) 
                _carObjectPools.AddCarToCarPool(_carManager.CarSpawnServiceHandler, uniqueVehicleSo.Key, uniqueVehicleSo.Value);
        }

        private void SpawnRoadDetectors()
        {
            foreach (WaypointContainer container in _carManager.allWaysContainer.allWays)
            {
                Transform firstElement = container.roadPoints[0].point.transform;
                CarDetector carDetectorObject =  Object.Instantiate(_carManager.allWaysContainer.carDetectorPrefab, firstElement.position, firstElement.rotation, firstElement);
                GetCurrentWave().createdCarDetectors.Add(carDetectorObject);
                
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
    }
}