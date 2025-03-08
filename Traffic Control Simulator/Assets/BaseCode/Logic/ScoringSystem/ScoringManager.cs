using System;
using System.Collections.Generic;
using BaseCode.Interfaces;
using BaseCode.Logic.PopUps;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Handler;
using TMPro;
using UnityEngine;

namespace BaseCode.Logic.ScoringSystem
{
    public class ScoringManager : ManagerBase
    {
        private readonly TimerBase _timerBase = new TimerBase();
        private readonly List<IScoringObject> _scoringObjects = new();

        private GameMenuPopUp _gameMenuPopUp;
        private float PlayerScore
        {
            get => PlayerSo.playerScore;
            set => PlayerSo.playerScore = value;
        }
        private void Start()
        {
            _timerBase.AddDelay(UpdateTime);
            _gameMenuPopUp = gameManager.popUpController.GetPopUp<GameMenuPopUp>();
        }
        public void AddCar(IScoringObject scoringObj)
        {
            scoringObj.Initialize(this);
            _scoringObjects.Add(scoringObj);
        }
        
        private void Update()
        {
            if (!IsTimerUp()) return;
            
            _timerBase.AddDelay(UpdateTime);
            UpdateScoringObjects();
        }
        private bool IsTimerUp()
        {
            return _timerBase.IsTimerUp();
        }
        private void UpdateScoringObjects()
        {
            foreach (var scoringObj in _scoringObjects)
            {
                scoringObj.Calculate(UpdateTime);
            }
        }
        public void ChangeScore(float change)
        {
            PlayerScore = Mathf.Max(0, PlayerScore + change);
            _gameMenuPopUp.soreText.text = $"{ConfigSo.ScoreMessage}{PlayerScore:F0}";
        }

        public ConfigSo ConfigSo => gameManager.saveManager.configSo;
        private float UpdateTime => ConfigSo.updateTime;
        private PlayerSo PlayerSo => gameManager.saveManager.playerSo;
    }
}
