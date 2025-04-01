using System.Collections;
using BaseCode.Extensions.UI;
using BaseCode.Logic.Managers;
using BaseCode.Logic.PopUps;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.InterfaceHandler.Car;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Wave
{
    public class CarWaveUIController
    {
        private readonly CarSpawnServiceHandler _carSpawnServiceHandler;
        public CarWaveUIController(CarSpawnServiceHandler carSpawnServiceHandler)
        {
            _carSpawnServiceHandler = carSpawnServiceHandler;
        }
        public IEnumerator ShowEndGamePopup()
        {
            if (ScoreManager.PlayerScore <= 0)
            {
                PopUpManager.ShowPopUp<PopUpLoseMenuGamePopUp>();
            }
            else
            {
                CarWaveController.OpenLockOfCurrentLevel();
                PopUpManager.ShowPopUp<PopUpWinMenuGamePopUp>();
            }
            yield break;
        }

        public IEnumerator ShowWavePopup()
        {
            PopUpGameMenu popUpLevelsMenu = PopUpManager.GetPopUp<PopUpGameMenu>();
            var informationText = popUpLevelsMenu.informationText;

            informationText.Toggle();
            informationText.SetText("You Passed");
            
            yield return new WaitForSeconds(1);
            
            informationText.text = WavesRankSo.GetWaveWord(CarWaveController.GetCurrentWave().waveRank);

            yield return new WaitForSeconds(1);
            
            CarWaveController.AdvanceToNextWave();
            
            informationText.Toggle();
        }
        public PopUpManager PopUpManager => CarManager.GameManager.popUpManager;
        public ScoringManager ScoreManager => CarManager.GameManager.scoringManager;
        public CarManager CarManager => _carSpawnServiceHandler.CarManager;
        public CarWaveController CarWaveController => _carSpawnServiceHandler.CarWaveController;
        public WavesRankScriptableObject WavesRankSo => _carSpawnServiceHandler.wavesRankSo;
    }
}