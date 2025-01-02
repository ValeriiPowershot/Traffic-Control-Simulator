using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BaseCode.Logic.ScoringSystem
{
    public class ScoringManager : MonoBehaviour
    {
        //temporary implemetation, should be replaced with cars manager or smthn
        [SerializeField] private Transform _carsHandler;
        [SerializeField] private TMP_Text _scoreText;

        private List<IScoringObject> _scoringObjects = new();

        private float _deltaTime;
        private float _playerScore;
        private const string SCORE_MESSAGE = "Score: ";

        public float PlayerScore { get { return _playerScore; } }

        private void Awake()
        {
            /*_scoringObjects.AddRange(_carsHandler.GetComponentsInChildren<IScoringObject>());

            foreach (var ScoringObj in _scoringObjects)
                ScoringObj.Initialize(this);

            ChangeScore(10);*/
        }

        public void AddCar(IScoringObject ScoringObj)
        {
            ScoringObj.Initialize(this);
            _scoringObjects.Add(ScoringObj);
        }

        private void Update()
        {
            _deltaTime += Time.deltaTime;

            //executing once per 2 frames / may be replaced by async method for better perfomance
            if(Time.frameCount % 2 == 0)
            {
                foreach(var ScoringObj in _scoringObjects)
                {
                    ScoringObj.Calculate(_deltaTime);
                }
                _deltaTime = 0f;
            }
        }

        public void ChangeScore(float Change)
        {
            _playerScore += Change;

            _playerScore = _playerScore < 0 ? 0 : _playerScore;// if below 0 set to 0

            _scoreText.text = SCORE_MESSAGE + _playerScore.ToString("F0");
        }
    }
}
