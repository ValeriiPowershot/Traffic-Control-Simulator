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

        private bool tempCheck = false;

        protected override void Awake()
        {
            
        }

        public void Initalize()
        {
            carSpawnServiceHandler.Initialize(this);
            tempCheck = true;
        }

        private void Update()
        {
            if(tempCheck)
               carSpawnServiceHandler.Update();
        }

        public ScoringManager ScoringManager => GameManager.scoringManager;
        public CarSpawnServiceHandler CarSpawnServiceHandler => carSpawnServiceHandler;
    }
}
