using System.Collections;
using BaseCode.Logic.Ways;
using BaseCode.Logic.Services.Handler.Car;
using UnityEngine;

namespace BaseCode.Logic
{
    // pass this class for now!
    public class CarManager : ManagerBase<CarManager>
    {
        public override GameManager GameManager
        {
            get
            {
                var baseGameManager = base.GameManager;
                baseGameManager.carManager = this; 
                return baseGameManager; 
            }
        }
        
        public AllWaysContainer allWaysContainer;
        [SerializeField] private CarSpawnServiceHandler carSpawnServiceHandler;

        private Coroutine _startLoadingUpdateSpawning;
        protected override void Awake()
        {
            carSpawnServiceHandler.Initialize(this);
        }

        public void StartGame()
        {
            Debug.Log("Start New Game!");
            ExitGame();
            
            if(_startLoadingUpdateSpawning != null)
                StopCoroutine(_startLoadingUpdateSpawning);
            
            _startLoadingUpdateSpawning = StartCoroutine(StartLoadingUpdateSpawning());
        }

        public void ExitGame()
        {
            carSpawnServiceHandler.ResetWave(); // car detector + lıghts clean
        }

        public void ExitWave()
        {
            StopCoroutine(_startLoadingUpdateSpawning);
            _startLoadingUpdateSpawning = null;
        }

        private IEnumerator StartLoadingUpdateSpawning()
        {
            while (true)
            {
                carSpawnServiceHandler.Update();
                yield return null;
            }
        }
        
        public ScoringManager ScoringManager => GameManager.scoringManager;
        public CarSpawnServiceHandler CarSpawnServiceHandler => carSpawnServiceHandler;
    }
}
