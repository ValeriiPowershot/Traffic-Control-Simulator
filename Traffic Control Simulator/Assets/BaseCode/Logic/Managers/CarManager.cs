using System.Collections;
using BaseCode.Logic.Services.InterfaceHandler.Car;
using BaseCode.Logic.Ways;
using UnityEngine;

namespace BaseCode.Logic.Managers
{
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
            _startLoadingUpdateSpawning = StartCoroutine(StartLoadingUpdateSpawning());
        }

        public void ExitGame()
        {
            ExitWave();
            
            foreach (var carLevel in carSpawnServiceHandler.carLevels)
                carLevel.ResetLevel();
        }

        public void ExitWave()
        {
            if(_startLoadingUpdateSpawning != null)
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

        public void OnDestroy()
        {
            carSpawnServiceHandler.CarObjectPools.Pool.Clear();
        }

        public ScoringManager ScoringManager => GameManager.scoringManager;
        public CarSpawnServiceHandler CarSpawnServiceHandler => carSpawnServiceHandler;
    }
}
