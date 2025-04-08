using BaseCode.Interfaces;
using BaseCode.Logic.Managers;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Score
{
    public class CarScoreCalculatorBase : MonoBehaviour
    {
        private ScoringManager _manager;
        private VehicleBase _car;
        
        public CarScoreVisualizer scoreMaterialsComponent;

        private float _totalWaitingTime;
        protected float CurrentScore = 1;
        
        private void Awake() => _car = GetComponentInParent<VehicleBase>();
        
        public void Initialize(ScoringManager manager)
        {
            _manager = manager;
            scoreMaterialsComponent.Initialize(this);
        }
        
        public virtual void Calculate(float deltaTime)
        {
            CalculateResult(deltaTime);
            scoreMaterialsComponent.CheckAndAssignNewColor();
        }
        protected virtual void CalculateResult(float deltaTime)
        {
            if (CurrentScore <= 0)
            {
                CurrentScore = 0;
                return;
            }
            
            CurrentScore = CalculateScore(deltaTime); 
        }

        protected float CalculateScore(float deltaTime)
        {
            _totalWaitingTime += deltaTime;

            if (_totalWaitingTime <= AcceptableWaitingTime)
                return SuccessPoints;

            float penaltyTime = _totalWaitingTime - AcceptableWaitingTime;

            float penalty = penaltyTime * (SuccessPoints / AcceptableWaitingTime);

            return (SuccessPoints - penalty);
        }

        public void OnReachedDestination(bool isLostScore)
        {
            if(!isLostScore)
                _manager.ChangeScore(CurrentScore);
            
            ResetScoreObjectCarBase();
        }

        private void ResetScoreObjectCarBase()
        {
            CurrentScore = 1;
            _totalWaitingTime = 0f;
            
            scoreMaterialsComponent.ResetScoringMaterial();
        }
        
        public bool IsActive() => _car.isActiveAndEnabled;
        public float TotalWaitingTime => _totalWaitingTime;
        public float AcceptableWaitingTime=> _car.vehicleController.VehicleScoreController.acceptableWaitingTime;
        public float SuccessPoints => _car.vehicleController.VehicleScoreController.successPoints;
    }
    
    
}