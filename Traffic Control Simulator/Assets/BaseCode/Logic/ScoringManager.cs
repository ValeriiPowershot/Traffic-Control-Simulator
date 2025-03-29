using System.Collections.Generic;
using BaseCode.Interfaces;
using BaseCode.Logic.PopUps;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Handler;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic
{
    public class ScoringManager : ManagerBase<ScoringManager>
    {
        public override GameManager GameManager
        {
            get
            {
                var baseGameManager = base.GameManager;
                baseGameManager.scoringManager = this; 
                return baseGameManager; 
            }
        }
        private readonly TimerBase _timerBase = new TimerBase();

        private PopUpGameMenu _popUpGameMenu;
        public float PlayerScore
        {
            get => PlayerSo.playerScore;
            private set => PlayerSo.playerScore = value;
        }

        public override void Start()
        {
            base.Start();
            
            _timerBase.AddDelay(ScoreCarUpdateTime);
            _popUpGameMenu = GameManager.popUpManager.GetPopUp<PopUpGameMenu>();
            PlayerScore = 0;
        }
        private void Update()
        {
            if (!IsTimerUp()) return;
            _timerBase.AddDelay(ScoreCarUpdateTime);
            UpdateScoringObjects();
        }
        private bool IsTimerUp()
        {
            return _timerBase.IsTimerUp();
        }
        private void UpdateScoringObjects()
        {
            foreach (var scoringObj in VehicleBases())
            {
                IScoringObject scoringObject = scoringObj.ScoringObject;
                
                if(scoringObject.IsActive())
                    scoringObject.Calculate(ScoreCarUpdateTime);
            }
        }
        public void ChangeScore(float change)
        {
            PlayerScore += change;
            Debug.Log("Player Score " + PlayerScore + " Made Score" + change);   
            _popUpGameMenu.soreText.text = $"{ConfigSo.ScoreMessage}{PlayerScore:F0}";
        }

        public List<VehicleBase> VehicleBases() => _gameManager.carManager.CarSpawnServiceHandler.onBoardGameCars;
        public ConfigSo ConfigSo => GameManager.saveManager.configSo;
        private float ScoreCarUpdateTime => ConfigSo.scoreCarUpdateTime;
        private PlayerSo PlayerSo => GameManager.saveManager.playerSo;

    }
}
