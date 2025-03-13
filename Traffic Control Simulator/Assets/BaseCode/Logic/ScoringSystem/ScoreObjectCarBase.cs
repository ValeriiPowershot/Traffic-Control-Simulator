using System;
using System.Collections;
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

        public float CalculateScore(float deltaTime)
        {
            _totalWaitingTime += deltaTime;

            if (_totalWaitingTime <= VehicleSo.acceptableWaitingTime)
                return VehicleSo.successPoints;

            float penaltyTime = _totalWaitingTime - VehicleSo.acceptableWaitingTime;

            float penalty = penaltyTime * (VehicleSo.successPoints / VehicleSo.acceptableWaitingTime);

            return -(VehicleSo.successPoints - penalty);
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
        private float LeftTime => VehicleSo.acceptableWaitingTime - _totalWaitingTime;
        public float TotalWaitingTime => _totalWaitingTime;
        public VehicleScriptableObject VehicleSo => _car.VehicleScriptableObject; 
        
    }
    
    
}