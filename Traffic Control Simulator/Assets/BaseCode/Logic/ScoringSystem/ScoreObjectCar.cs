using BaseCode.Interfaces;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.ScoringSystem
{
    public class ScoreObjectCar : MonoBehaviour, IScoringObject
    {
        public ScoringMaterials scoreMaterialsComponent;

        private ScoringManager _manager;
        private VehicleBase _car;

        private ScoreType _scoreType = ScoreType.Good;
        private float _totalWaitingTime;
    
        private void Awake() => _car = GetComponentInParent<VehicleBase>();

        private void OnEnable() => _car.CarLightService.LightExited += ExitLight;

        private void OnDisable() => _car.CarLightService.LightExited -= ExitLight;
        
        public void Initialize(ScoringManager manager)
        {
            _manager = manager;
        }
        
        public void Calculate(float deltaTime)
        {
            if (IsLightState(LightState.Red))
            {
                ProcessWaitingAtRedLight(deltaTime);
            }
        }
        
        private void ExitLight()
        {
            float resultPoints = CalculateResult();
            
            _manager.ChangeScore(resultPoints);
            ResetLightState();
        }

        private void ProcessWaitingAtRedLight(float deltaTime)
        {
            if (IsCarOnStopState())
            {
                _totalWaitingTime += deltaTime;

                if (ShouldChangeToNeutral())
                {
                    UpdateScoreType(ScoreType.Neuteral);
                }
                else if (ShouldChangeToBad())
                {
                    UpdateScoreType(ScoreType.Bad);
                }
            }
        }
 
        private float CalculateResult()
        {
            if (_totalWaitingTime <= VehicleSo.acceptableWaitingTime)
                return VehicleSo.successPoints;

            float lambda = _manager.ConfigSo.penaltyLambda; 
            float penaltyFactor = Mathf.Exp(-lambda * (_totalWaitingTime - VehicleSo.acceptableWaitingTime));
            
            return VehicleSo.successPoints * penaltyFactor;
        }

        private bool IsCarOnStopState()
        {
            return _car.VehicleController.StateController.IsOnState<VehicleMovementStopState>();
        }

        private bool ShouldChangeToNeutral()
        {
            return IsScoreType(ScoreType.Good) && 
                   _totalWaitingTime >= VehicleSo.acceptableWaitingTime;
        }

        private bool ShouldChangeToBad()
        {
            return IsScoreType(ScoreType.Neuteral) &&
                   _totalWaitingTime - VehicleSo.acceptableWaitingTime >= 0;
        }
        
        private void ResetLightState()
        {
            _totalWaitingTime = 0f;
            UpdateScoreType(ScoreType.Good);
        }
        private void UpdateScoreType(ScoreType newScoreType)
        {
            _scoreType = newScoreType;
            scoreMaterialsComponent.SetNewMaterial(newScoreType);
        }
        private bool IsScoreType(ScoreType scoreType)
        {
            return _scoreType == scoreType;
        }
        private bool IsLightState(LightState state)
        {
            return _car.CarLightService.CarLightState == state;
        }
        private VehicleScriptableObject VehicleSo => _car.VehicleScriptableObject; 

    }

    public enum ScoreType
    {
        None,
        Good,
        Neuteral,
        Bad,
    }
}