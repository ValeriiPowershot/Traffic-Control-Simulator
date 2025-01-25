using BaseCode.Interfaces;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.ScoringSystem
{
    public class ScoreObjectCar : MonoBehaviour, IScoringObject
    {
        public float AcceptableWaitingTime = 1f;
        [Tooltip("Amount of points player will gain in best scenario (for this certain car)")]
        public float SuccessPoints;
        [Tooltip("(Negative value) this amount of points player will lose in worst scenario (for this certain car)")]
        public float FailPoints;
        [Tooltip("Amount of time have to pass after acceptable time runs out to reach worst scenario")]
        public float TimeToWorstScore;

        public ScoringMaterials ScoreMaterialsComponent;

        private ScoringManager _manager;
        private VehicleBase _car;

        private ScoreType _scoreType = ScoreType.Good;
        private Vector3 _prevPosition;
        private float _waitingTime;

        private const float MIN_MOVING_RANGE = 0.001f;

        private void Awake() =>
            _car = GetComponentInParent<VehicleBase>();

        private void OnEnable() =>
            _car.CarLightService.LightExited += ExitLight;

        private void OnDisable() =>
            _car.CarLightService.LightExited -= ExitLight;

        public void Initialize(ScoringManager Manager)
        {
            _manager = Manager;
            Debug.Log("Initialized");
        }
        public void Calculate(float DeltaTime)
        {
            if (_car.CarLightService.CarLightState == LightState.Red)
            {
                ProcessWaitingAtRedLight(DeltaTime);
            }
            _prevPosition = transform.position;
        }
        public void ExitLight()
        {
            float resultPoints = CalculateResultPoints();
            ResetLightState();
            _manager.ChangeScore(resultPoints);
        }

        private void ProcessWaitingAtRedLight(float DeltaTime)
        {
            if (IsCarMoving())
            {
                _waitingTime += DeltaTime;

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
            return (_prevPosition - transform.position).sqrMagnitude <= MIN_MOVING_RANGE * MIN_MOVING_RANGE;
        }

        private bool ShouldChangeToNeutral()
        {
            return _scoreType == ScoreType.Good && _waitingTime >= AcceptableWaitingTime;
        }

        private bool ShouldChangeToBad()
        {
            return _scoreType == ScoreType.Neuteral && 
                   _waitingTime - AcceptableWaitingTime >= TimeToWorstScore;
        }

        private void UpdateScoreType(ScoreType newScoreType)
        {
            _scoreType = newScoreType;
            ScoreMaterialsComponent.SetNewMaterial(newScoreType);
        }
        
        private float CalculateResultPoints()
        {
            float resultPoints = SuccessPoints;

            if (_waitingTime > AcceptableWaitingTime)
            {
                float unAcceptWaitTime = _waitingTime - AcceptableWaitingTime;
                float ratio = Mathf.Min(unAcceptWaitTime / TimeToWorstScore, 1);
                resultPoints += (FailPoints - SuccessPoints) * ratio;
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