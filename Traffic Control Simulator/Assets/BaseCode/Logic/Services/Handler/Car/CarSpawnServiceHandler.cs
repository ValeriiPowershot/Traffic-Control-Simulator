using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Extensions.UI;
using BaseCode.Logic.Lights.Handler.Abstracts;
using BaseCode.Logic.PopUps;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles.Vehicles;
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
        public bool looped;
        
        private Coroutine _updateCoroutine;
        private int _spawnedCarIndex;

        public List<VehicleBase> onBoardGameCars = new List<VehicleBase>();
        public List<CarDetector> createdCarDetectors = new List<CarDetector>();
        public List<LightBase> createdLights = new List<LightBase>();
        
        public void Initialize(CarManager carManagerInScene)
        {
            CarManager = carManagerInScene;

            InitializePools();
            InitializeWave();
            SpawnRoadDetectors();
            FindLights();
        }

        public void Update()
        {
            waves[currentWaveIndex].Update();
        }
        private void FindLights()
        {
            createdLights.AddRange(Object.FindObjectsByType<LightBase>(FindObjectsInactive.Exclude,FindObjectsSortMode.None));
        }

        private void InitializeWave()
        {
            foreach (CarWave wave in waves) 
                wave.Initialize(CarManager, CarObjectPools);
            
            currentWaveIndex = 0;
        }
        public void ResetWave()
        {
            foreach (CarWave wave in waves) 
                wave.ResetWave();
            
            foreach (var carDetector in createdCarDetectors)
                carDetector.ResetDetector();

            foreach (var lightBase in createdLights)
                lightBase.RemoveAllVehicle();
            
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
                Transform firstElement = container.roadPoints[0].point.transform;
                CarDetector carDetectorObject =  Object.Instantiate(CarManager.allWaysContainer.carDetectorPrefab, firstElement.position, firstElement.rotation, firstElement);
                createdCarDetectors.Add(carDetectorObject);
                
                carDetectorObject.carDetectorSpawnIndex = _spawnedCarIndex;
                _spawnedCarIndex++;
            }
        }
        
        private Dictionary<VehicleScriptableObject, int> GetCurrentSetsWithCounts()
        {
            Dictionary<VehicleScriptableObject, int> vehicleCounts = new();

            foreach (CarWave wave in waves)
            {
                foreach (CarSpawnObject carSpawnObject in wave.carSpawnObjects)
                {
                    VehicleScriptableObject carSo = carSpawnObject.carSoObjects;
                    int requestSize = carSpawnObject.size;

                    if (vehicleCounts.ContainsKey(carSo))
                    {
                        requestSize -= vehicleCounts[carSo];
                        if(requestSize < 0 )
                            continue;
                        
                        vehicleCounts[carSo] += requestSize;
                    }
                    else
                    {
                        vehicleCounts[carSo] = requestSize;
                    }
                      
                }
            }

            return vehicleCounts;
        }

        public void RemoveThisAndCheckAllCarPoolMaxed(VehicleBase vehicleBase)
        {
            onBoardGameCars.Remove(vehicleBase);
            
            if (onBoardGameCars.Count> 0) return;
            CheckForEndGame();
        }

        private void CheckForEndGame()
        {
            if (looped)
            {
                foreach (var carSpawnObject in waves.SelectMany(wave => wave.carSpawnObjects))
                {
                    carSpawnObject.ResetCarSpawnObject();
                }
                return;
            };
            
            CarManager.StartCoroutine(currentWaveIndex == waves.Count - 1 ? EndShower() : WaveShower());
        }

        private IEnumerator EndShower()
        {
            currentWaveIndex = 0;
            float currentScore = ScoreManager.PlayerScore;
            if (currentScore < 0)
            {
                PopUpManager.ShowPopUp<PopUpLoseMenuGamePopUp>();
            }
            else
            {
                PopUpManager.ShowPopUp<PopUpWinMenuGamePopUp>();
            }
            yield break;
        }

        private IEnumerator WaveShower()
        {
            float currentScore = ScoreManager.PlayerScore;
            if (currentScore < 0)
            {
                PopUpManager.ShowPopUp<PopUpLoseMenuGamePopUp>();
                yield break;
            }
            
            PopUpGameMenu popUpLevelsMenu = PopUpManager.GetPopUp<PopUpGameMenu>();
            var informationText = popUpLevelsMenu.informationText;

            informationText.Toggle();
            informationText.SetText("You Passed");
            
            yield return new WaitForSeconds(1);
            
            informationText.text = wavesRankSo.GetWaveWord(waves[currentWaveIndex].waveRank);
            
            yield return new WaitForSeconds(1);
            
            currentWaveIndex++;
            StartNewWave();
            informationText.Toggle();
        }

        public void StartNewWave()
        {
            CarManager.StartGame();
        }
        
        public PopUpManager PopUpManager => CarManager.GameManager.popUpManager;
        public ScoringManager ScoreManager => CarManager.GameManager.scoringManager;

    }
}