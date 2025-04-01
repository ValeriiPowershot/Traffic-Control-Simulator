using System.Collections.Generic;
using BaseCode.Logic.Managers;
using BaseCode.Logic.Services.InterfaceHandler.Car;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Wave
{
    public class CarWaveController
    {
        private Coroutine _updateCoroutine;
        private int _currentLevelIndex;
        private CarSpawnServiceHandler _carSpawnServiceHandler;
        public CarWaveController(CarSpawnServiceHandler carSpawnServiceHandler)
        {
            _carSpawnServiceHandler = carSpawnServiceHandler;
        }
        public void InitializeLevel()
        {
            foreach (var carLevel in CarLevels)
            {
                carLevel.InitializeLevel(CarManager);
            }
        }
        public void Update() => GetCurrentWave().Update();
        
        public void AdvanceToNextWave()
        {
            GetCurrentLevel().currentWaveIndex++;
            CarManager.StartGame();
        }

        public void OnCarReachedDestination(VehicleBase vehicleBase)
        {
            GetCurrentWave().onBoardGameCars.Remove(vehicleBase);
            
            if (GetCurrentWave().onBoardGameCars.Count> 0) return;
            
            CheckForEndGame();
        }
        
        public void CheckForEndGame() => CarManager.StartCoroutine(IsLastWave() ? 
                CarWaveUIController.ShowEndGamePopup() :
                CarWaveUIController.ShowWavePopup());
        public void OpenLockOfCurrentLevel()
        {
            CarManager.GameManager.saveManager.sceneSo.UnlockScene(CurrentLevelIndex);
        }
        private bool IsLastWave() => GetCurrentLevel().currentWaveIndex >= GetCurrentLevel().waves.Count - 1;
        public CarLevels GetCurrentLevel() =>CarLevels[CurrentLevelIndex];
        public CarWave GetCurrentWave() => GetCurrentLevel().GetCurrentWave();
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
        public CarManager CarManager => _carSpawnServiceHandler.CarManager;
        public CarWaveUIController CarWaveUIController => _carSpawnServiceHandler.CarWaveUIController;
        public List<CarLevels> CarLevels => _carSpawnServiceHandler.carLevels;


    }
}