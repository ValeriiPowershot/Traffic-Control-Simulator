using System.Collections.Generic;
using BaseCode.Interfaces;
using BaseCode.Logic.PopUps;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Handler;

namespace BaseCode.Logic
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
            _timerBase.AddDelay(ScoreCarUpdateTime);
            _gameMenuPopUp = gameManager.popUpController.GetPopUp<GameMenuPopUp>();
            PlayerScore = 0;
        }
        public void AddCar(IScoringObject scoringObj)
        {
            scoringObj.Initialize(this);
            _scoringObjects.Add(scoringObj);
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
            foreach (var scoringObj in _scoringObjects)
            {
                if(scoringObj.IsActive())
                    scoringObj.Calculate(ScoreCarUpdateTime);
            }
        }
        public void ChangeScore(float change)
        {
            PlayerScore += change;
            _gameMenuPopUp.soreText.text = $"{ConfigSo.ScoreMessage}{PlayerScore:F0}";
        }

        public ConfigSo ConfigSo => gameManager.saveManager.configSo;
        private float ScoreCarUpdateTime => ConfigSo.scoreCarUpdateTime;
        private PlayerSo PlayerSo => gameManager.saveManager.playerSo;
    }
}
