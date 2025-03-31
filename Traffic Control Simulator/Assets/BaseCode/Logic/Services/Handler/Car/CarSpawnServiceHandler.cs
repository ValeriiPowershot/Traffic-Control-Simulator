using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Extensions.UI;
using BaseCode.Logic.Lights.Handler.Abstracts;
using BaseCode.Logic.PopUps;
using BaseCode.Logic.Roads;
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

        public List<CarLevel> carLevels = new List<CarLevel>();
        public PopUpLevelsMenu popUpLevelsMenu;
        private Coroutine _updateCoroutine;
        private int _currentLevelIndex;
        public void Initialize(CarManager carManagerInScene)
        {
            CarManager = carManagerInScene;
            InitializeLevel();
        }

        private void InitializeLevel()
        {
            foreach (var carLevel in carLevels)
            {
                carLevel.InitializeLevel(CarManager);
            }
        }

        public void Update()
        {
            GetCurrentWave().Update();
        }
        public void OnCarReachedDestination(VehicleBase vehicleBase)
        {
            GetCurrentWave().onBoardGameCars.Remove(vehicleBase);
            
            if (GetCurrentWave().onBoardGameCars.Count> 0) return;
            CheckForEndGame();
        }

        private void CheckForEndGame()
        {
            /*
            if (GetCurrentLevel().looped)
            {
                foreach (var carSpawnObject in GetCurrentLevel().waves.SelectMany(wave => wave.carSpawnObjects))
                {
                    carSpawnObject.ResetCarSpawnObject();
                }
                return;
            };
            */
            
            CarManager.StartCoroutine(GetCurrentLevel().currentWaveIndex == GetCurrentLevel().waves.Count - 1 ? EndShower() : WaveShower());
        }

        private IEnumerator EndShower()
        {
            GetCurrentLevel().currentWaveIndex = 0;
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
            
            informationText.text = wavesRankSo.GetWaveWord(GetCurrentWave().waveRank);
            
            yield return new WaitForSeconds(1);
            
            GetCurrentLevel().currentWaveIndex++;
            StartNewWave();
            informationText.Toggle();
        }

        public void StartNewWave()
        {
            CarManager.StartGame();
        }
        public CarLevel GetCurrentLevel()
        {
            return carLevels[CurrentLevelIndex];
        }

        public CarWave GetCurrentWave()
        {
            return GetCurrentLevel().GetCurrentWave();
        }

        public PopUpManager PopUpManager => CarManager.GameManager.popUpManager;
        public ScoringManager ScoreManager => CarManager.GameManager.scoringManager;
        public int CurrentLevelIndex
        {
            get => _currentLevelIndex;
            set
            {
                _currentLevelIndex = value; 
                var currentLevel = GetCurrentLevel();
                CarManager.GameManager.cameraManager.SetNewPosition(currentLevel.cameraPosition);
            }
        }
    }
}