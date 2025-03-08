using BaseCode.Interfaces;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.ScoringSystem
{
    public class ScoreObjectCarBase : MonoBehaviour, IScoringObject
    {
        public ScoringMaterials scoreMaterialsComponent;

        private ScoringManager _manager;
        private VehicleBase _car;

        private float _totalWaitingTime;
    
        private void Awake() => _car = GetComponentInParent<VehicleBase>();

        public void Initialize(ScoringManager manager)
        {
            _manager = manager;
        }
        
        public virtual void Calculate(float deltaTime)
        {
            ProcessWaitingAtRedLight(deltaTime);
        }

        private void ProcessWaitingAtRedLight(float deltaTime)
        {
            _totalWaitingTime += deltaTime;
            UpdateScoreType();
        }
        private void UpdateScoreType()
        {
            scoreMaterialsComponent.SetNewMaterial(GetScoreColor());
        }

        private Color GetScoreColor()
        {
            if (_totalWaitingTime <= VehicleSo.acceptableWaitingTime)
                return scoreMaterialsComponent.good;

            float maxScore = VehicleSo.successPoints;
            float minScore = 0f;
    
            float score = CalculateResult(); 
            float t = Mathf.InverseLerp(minScore, maxScore, score); 

            return Color.Lerp(scoreMaterialsComponent.bad, scoreMaterialsComponent.good, t);
        }
        private float CalculateResult()
        {
            if (_totalWaitingTime <= VehicleSo.acceptableWaitingTime)
                return VehicleSo.successPoints;

            float lambda = _manager.ConfigSo.penaltyLambda; 
            float penaltyFactor = Mathf.Exp(-lambda * (_totalWaitingTime - VehicleSo.acceptableWaitingTime));
            
            return VehicleSo.successPoints * penaltyFactor;
        } 
        public void OnReachedDestination()
        {
            float resultPoints = CalculateResult();
            _manager.ChangeScore(resultPoints);
            _totalWaitingTime = 0f;
            scoreMaterialsComponent.SetNewMaterial(scoreMaterialsComponent.good);
        }
        public bool IsActive() => _car.isActiveAndEnabled;
        private VehicleScriptableObject VehicleSo => _car.VehicleScriptableObject; 
    }
    
    
}