using BaseCode.Interfaces;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BaseCode.Logic.ScoringSystem
{
    public class ScoreObjectCar : MonoBehaviour, IScoringObject
    {
        public ScoringMaterials scoreMaterialsComponent;

        private ScoringManager _manager;
        private VehicleBase _car;

        private ScoreType _scoreType = ScoreType.Good;
        private Vector3 _prevPosition;
        private float _waitingTime;
    
        private void Awake() => _car = GetComponentInParent<VehicleBase>();

        private void OnEnable() => _car.CarLightService.LightExited += ExitLight;

        private void OnDisable() => _car.CarLightService.LightExited -= ExitLight;
        
        public void Initialize(ScoringManager manager)
        {
            _manager = manager;
        }
        
        public void Calculate(float deltaTime)
        {
            if (_car.CarLightService.CarLightState == LightState.Red)
            {
                ProcessWaitingAtRedLight(deltaTime);
            }
            _prevPosition = transform.position;
        }

        private void ExitLight()
        {
            float resultPoints = CalculateResultPoints();
            ResetLightState();
            _manager.ChangeScore(resultPoints);
        }

        private void ProcessWaitingAtRedLight(float deltaTime)
        {
            if (IsCarMoving())
            {
                _waitingTime += deltaTime;

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

        private bool IsCarMoving()
        {
            var minRange = _car.VehicleScriptableObject.MIN_MOVING_RANGE;
            return (_prevPosition - transform.position).sqrMagnitude <= minRange * minRange;
        }

        private bool ShouldChangeToNeutral()
        {
            return _scoreType == ScoreType.Good && _waitingTime >= _car.VehicleScriptableObject.AcceptableWaitingTime;
        }

        private bool ShouldChangeToBad()
        {
            return _scoreType == ScoreType.Neuteral && 
                   _waitingTime - _car.VehicleScriptableObject.AcceptableWaitingTime >= _car.VehicleScriptableObject.TimeToWorstScore;
        }

        private void UpdateScoreType(ScoreType newScoreType)
        {
            _scoreType = newScoreType;
            scoreMaterialsComponent.SetNewMaterial(newScoreType);
        }
        
        private float CalculateResultPoints()
        {
            float resultPoints = _car.VehicleScriptableObject.SuccessPoints;

            if (_waitingTime > _car.VehicleScriptableObject.AcceptableWaitingTime)
            {
                float unAcceptWaitTime = _waitingTime - _car.VehicleScriptableObject.AcceptableWaitingTime;
                float ratio = Mathf.Min(unAcceptWaitTime / _car.VehicleScriptableObject.TimeToWorstScore, 1);
                resultPoints += (_car.VehicleScriptableObject.FailPoints - 
                                 _car.VehicleScriptableObject.SuccessPoints) * ratio;
            }

            return resultPoints;
        }

        private void ResetLightState()
        {
            _waitingTime = 0f;
            UpdateScoreType(ScoreType.Good);
        }

    }

    public enum ScoreType
    {
        None,
        Good,
        Neuteral,
        Bad,
    }
}