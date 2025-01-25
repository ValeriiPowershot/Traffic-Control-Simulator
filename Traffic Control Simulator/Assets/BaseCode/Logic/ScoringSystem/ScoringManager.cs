using System.Collections.Generic;
using BaseCode.Interfaces;
using TMPro;
using UnityEngine;

namespace BaseCode.Logic.ScoringSystem
{
    // egzoz efekti
    // genisleme gerileme efekti
    
    public class ScoringManager : ManagerBase
    {
        [SerializeField] private TMP_Text _scoreText;

        private List<IScoringObject> _scoringObjects = new();

        private float _deltaTime;
        private float _playerScore;
        private const string SCORE_MESSAGE = "Score: ";

        public float PlayerScore { get { return _playerScore; } }

        public void AddCar(IScoringObject ScoringObj)
        {
            ScoringObj.Initialize(this);
            _scoringObjects.Add(ScoringObj);
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


        public void ChangeScore(float Change)
        {
            _playerScore += Change;

            _playerScore = _playerScore < 0 ? 0 : _playerScore;// if below 0 set to 0

            _scoreText.text = SCORE_MESSAGE + _playerScore.ToString("F0");
        }
    }
}
