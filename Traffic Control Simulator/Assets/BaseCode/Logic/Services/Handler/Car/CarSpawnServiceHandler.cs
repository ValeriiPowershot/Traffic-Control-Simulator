using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Ways;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class CarSpawnServiceHandler : ICarSpawnService
    {
        public CarManager CarManager { get; set; }
        public CarObjectPools CarObjectPools = new();
        public WavesRankScriptableObject wavesRankSo;
        
        public List<CarWave> waves = new();
        public int currentWaveIndex;
        
        private Coroutine _updateCoroutine;

        private int _spawnedCarIndex;

        public TextMeshProUGUI textMeshProUGUI;
        public void Initialize(CarManager carManagerInScene)
        {
            CarManager = carManagerInScene;

            InitializePools();
            InitializeWave();
            SpawnRoadDetectors();
        }
        public void Update()
        {
            if (_updateCoroutine == null) 
                _updateCoroutine = CarManager.StartCoroutine(UpdateWithDelay());
        }
        private IEnumerator UpdateWithDelay()
        {
            foreach (CarSpawnObject spawnObject in waves[currentWaveIndex].carSpawnObjects)
            {
                yield return null;
                spawnObject.Update();
            }
            _updateCoroutine = null;
        }

        private void InitializeWave()
        {
            foreach (CarWave wave in waves) 
                wave.Initialize(CarManager, CarObjectPools);
            
            currentWaveIndex = 0;
        }

        private void InitializePools()
        {
            Dictionary<VehicleScriptableObject, int> setOfCurrentWaveSo = GetCurrentSetsWithCounts();
            
            foreach (KeyValuePair<VehicleScriptableObject, int> uniqueVehicleSo in setOfCurrentWaveSo) 
                CarObjectPools.AddCarToCarPool(this, uniqueVehicleSo.Key, uniqueVehicleSo.Value);
        }
        
        private void SpawnRoadDetectors()
        {
            foreach (WaypointContainer container in CarManager.allWaysContainer.allWays)
            {
                Debug.Log("yes");
                Transform firstElement = container.roadPoints[0].point.transform;
                CarDetector carDetectorObject =  Object.Instantiate(CarManager.allWaysContainer.carDetectorPrefab, firstElement.position, firstElement.rotation, firstElement);
                
                carDetectorObject.CarDetectorSpawnIndex = _spawnedCarIndex;
                _spawnedCarIndex++;
            }
        }
        
        private Dictionary<VehicleScriptableObject, int> GetCurrentSetsWithCounts()
        {
            Dictionary<VehicleScriptableObject, int> vehicleCounts = new ();

            foreach (CarWave wave in waves)
            {
                foreach (CarSpawnObject carSpawnObject in wave.carSpawnObjects)
                {
                    VehicleScriptableObject carSo = carSpawnObject.carSoObjects;

                    int poolSize = carSpawnObject.size;
                    
                    if (!vehicleCounts.TryAdd(carSo, poolSize)) 
                        vehicleCounts[carSo] += poolSize; // Add to existing count
                }
            }

            return vehicleCounts;
        }
        
        public void CheckAllCarPoolMaxed()
        {
            bool result = CarObjectPools.Pool.All(carObjectPool => carObjectPool.Value.GetActiveAmount() == 0);

            if (!result) return;

            CheckForEndGame();
        }

        // under these two function will move to another class
        private void CheckForEndGame() =>
            CarManager.StartCoroutine(currentWaveIndex == waves.Count - 1 ? EndShower() : WaveShower());

        private IEnumerator EndShower()
        {
            textMeshProUGUI.gameObject.SetActive(true);
            textMeshProUGUI.text = "You Won";
            yield return new WaitForSeconds(1);
            textMeshProUGUI.gameObject.SetActive(false);
        }

        private IEnumerator WaveShower()
        {
            textMeshProUGUI.gameObject.SetActive(true);

            textMeshProUGUI.text = "You Passed";
            yield return new WaitForSeconds(1);
            textMeshProUGUI.text = wavesRankSo.GetWaveWord(waves[currentWaveIndex].waveRank);
            yield return new WaitForSeconds(1);
            currentWaveIndex++;
            textMeshProUGUI.gameObject.SetActive(false);
        }
    }
}