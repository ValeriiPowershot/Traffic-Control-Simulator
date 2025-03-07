using System;
using System.Collections.Generic;
using BaseCode.Interfaces;
using BaseCode.Logic.PopUps;
using BaseCode.Logic.ScriptableObject;
using TMPro;
using UnityEngine;

namespace BaseCode.Logic.ScoringSystem
{
    public class ScoringManager : ManagerBase
    {
        private GameMenuPopUp _gameMenuPopUp;
        
        private readonly List<IScoringObject> _scoringObjects = new();

        private float _deltaTime;

        private float PlayerScore
        {
            get => PlayerSo.playerScore;
            set => PlayerSo.playerScore = value;
        }

        private void Start()
        {
            _gameMenuPopUp = gameManager.popUpController.GetPopUp<GameMenuPopUp>();
        }
        public void AddCar(IScoringObject scoringObj)
        {
            scoringObj.Initialize(this);
            _scoringObjects.Add(scoringObj);
        }
        
        private void Update()
        {
            AccumulateDeltaTime();

            if (ShouldUpdateScoringObjects())
            {
                UpdateScoringObjects();
                ResetDeltaTime();
            }
        }

        private void AccumulateDeltaTime()
        {
            _deltaTime += Time.deltaTime;
        }

        private bool ShouldUpdateScoringObjects()
        {
            return Time.frameCount % 2 == 0;
        }

        private void UpdateScoringObjects()
        {
            foreach (var scoringObj in _scoringObjects)
            {
                scoringObj.Calculate(_deltaTime);
            }
        }

        private void ResetDeltaTime()
        {
            _deltaTime = 0f;
        }
        
        
        public void ChangeScore(float change)
        {
            PlayerScore = Mathf.Max(0, PlayerScore + change);
            _gameMenuPopUp.soreText.text = $"{ConfigSo.ScoreMessage}{PlayerScore:F0}";
        }
        
        private PlayerSo PlayerSo => gameManager.saveManager.playerSo;
    }
}
