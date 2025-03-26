using System.Collections.Generic;
using BaseCode.Interfaces;
using BaseCode.Logic.PopUps;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Handler;

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
        private readonly List<IScoringObject> _scoringObjects = new();

        private PopUpGameMenu _popUpGameMenu;
        public float PlayerScore
        {
            get => PlayerSo.playerScore;
            private set => PlayerSo.playerScore = value;
        }

        public bool canCheck = false;
        
        public void Initalize()
        {
            _timerBase.AddDelay(ScoreCarUpdateTime);
            _popUpGameMenu = GameManager.popUpManager.GetPopUp<PopUpGameMenu>();
            PlayerScore = 0;
            canCheck = true;
        }

        public void AddCar(IScoringObject scoringObj)
        {
            scoringObj.Initialize(this);
            _scoringObjects.Add(scoringObj);
        }
        
        private void Update()
        {
            if(canCheck == false) return;
            
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
            if(_popUpGameMenu == null) // i wanna use it in main menu
                return;
            _popUpGameMenu.soreText.text = $"{ConfigSo.ScoreMessage}{PlayerScore:F0}";
        }

        public ConfigSo ConfigSo => GameManager.saveManager.configSo;
        private float ScoreCarUpdateTime => ConfigSo.scoreCarUpdateTime;
        private PlayerSo PlayerSo => GameManager.saveManager.playerSo;

    }
}
