using System;
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

        protected float TotalWaitingTime;
        protected float CurrentScore = 1;
        
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
            TotalWaitingTime += deltaTime;
            UpdateScoreType();
        }
        private void UpdateScoreType()
        {
            CalculateResult();
            scoreMaterialsComponent.SetNewMaterial(GetScoreColor());
        }
        private Color GetScoreColor()
        {
            var leftTime = LeftTime;
            
            if(leftTime <= 0f)
                return scoreMaterialsComponent.bad;

            float maxTime = VehicleSo.acceptableWaitingTime;
            float minTime = 0f;
    
            float t = Mathf.InverseLerp(minTime, maxTime, leftTime); 
            return Color.Lerp(scoreMaterialsComponent.bad, scoreMaterialsComponent.good, t);
        }

        protected virtual void CalculateResult()
        {
            if (CurrentScore <= 0)
            {
                CurrentScore = 0;
                return;
            }
            
            CurrentScore = CalculateScore(); 
        }

        public float CalculateScore()
        {
            if (TotalWaitingTime <= VehicleSo.acceptableWaitingTime)
                return VehicleSo.successPoints;

            float penaltyTime = TotalWaitingTime - VehicleSo.acceptableWaitingTime;

            float penalty = penaltyTime * (VehicleSo.successPoints / VehicleSo.acceptableWaitingTime);

            return -(VehicleSo.successPoints - penalty);
        }

        public void OnReachedDestination()
        {
            _manager.ChangeScore(CurrentScore);
            Debug.Log("Earned Score " + CurrentScore + " " + LeftTime);   
            TotalWaitingTime = 0f;
            scoreMaterialsComponent.SetNewMaterial(scoreMaterialsComponent.good);
        }
        
        public bool IsActive() => _car.isActiveAndEnabled;
        public float LeftTime => VehicleSo.acceptableWaitingTime - TotalWaitingTime;
        protected VehicleScriptableObject VehicleSo => _car.VehicleScriptableObject; 
        
    }
    
    
}