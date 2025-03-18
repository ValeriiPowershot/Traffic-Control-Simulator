using BaseCode.Interfaces;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Score
{
    public class ScoreObjectCarBase : MonoBehaviour, IScoringObject
    {
        public ScoringMaterials scoreMaterialsComponent;

        private ScoringManager _manager;
        private VehicleBase _car;

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

            return -(SuccessPoints - penalty);
        }

        public void OnReachedDestination()
        {
            _manager.ChangeScore(CurrentScore);
            Debug.Log("Earned Score " + CurrentScore + " " + LeftTime);   
            _totalWaitingTime = 0f;
            scoreMaterialsComponent.SetNewMaterial(scoreMaterialsComponent.good);
            
            if(scoreMaterialsComponent.ColorTransformationCoroutine != null)
                StopCoroutine(scoreMaterialsComponent.ColorTransformationCoroutine);
        }
        
        public bool IsActive() => _car.isActiveAndEnabled;
        private float LeftTime => AcceptableWaitingTime - _totalWaitingTime;
        public float TotalWaitingTime => _totalWaitingTime;
        public float AcceptableWaitingTime=> _car.acceptableWaitingTime;
        public float SuccessPoints => _car.successPoints;
        public VehicleScriptableObject VehicleSo => _car.VehicleScriptableObject; 
        
    }
    
    
}