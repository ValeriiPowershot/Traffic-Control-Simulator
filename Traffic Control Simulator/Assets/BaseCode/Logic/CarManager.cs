using BaseCode.Logic.ScoringSystem;
using BaseCode.Logic.Ways;
using BaseCode.Logic.Services.Handler.Car;
using UnityEngine;

namespace BaseCode.Logic
{
    public class CarManager : ManagerBase
    {
        public AllWaysContainer allWaysContainer;
        
        [SerializeField] private CarSpawnServiceHandler carSpawnServiceHandler;

        private void Awake()
        {
            carSpawnServiceHandler.Initialize(this);
        }

        private void Update()
        {
            carSpawnServiceHandler.Update();
        }

        public ScoringManager ScoringManager => gameManager.scoringManager;
    }
}
