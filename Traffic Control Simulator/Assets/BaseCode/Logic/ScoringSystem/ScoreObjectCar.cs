using BaseCode.Interfaces;
using BaseCode.Logic.Vehicles;
using UnityEngine;
using UnityEngine.Serialization;

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

        //temporary 
        public MeshRenderer IndicatorOfScore;
        public ScoringMaterials ScoreMaterialsComponent;

        private ScoringManager _manager;
        private VehicleBase _car;

        private ScoreType _scoreType = ScoreType.Good;
        private Vector3 _prevPosition;
        private float _waitingTime;

        private const float MIN_MOVING_RANGE = 0.001f;

        private void Awake() =>
            _car = GetComponent<VehicleBase>();

        private void OnEnable() =>
            _car.LightExited += ExitLight;

        private void OnDisable() =>
            _car.LightExited -= ExitLight;

        public void Initialize(ScoringManager Manager) =>
            _manager = Manager;

        public void Calculate(float DeltaTime)
        {
            if (_car.CarLightState == LightState.Red)
            {
                if ((_prevPosition - transform.position).sqrMagnitude <= MIN_MOVING_RANGE * MIN_MOVING_RANGE)
                {
                    _waitingTime += DeltaTime;

                    if (_scoreType == ScoreType.Good && 
                        _waitingTime >= AcceptableWaitingTime)
                    {
                        IndicatorOfScore.material = ScoreMaterialsComponent.Neutral;
                        _scoreType = ScoreType.Neuteral;
                    }

                    else if (_scoreType == ScoreType.Neuteral && 
                             _waitingTime - AcceptableWaitingTime >= TimeToWorstScore)
                    {
                        IndicatorOfScore.material = ScoreMaterialsComponent.Bad;
                        _scoreType = ScoreType.Bad;
                    }
                }
            }
            _prevPosition = transform.position;
        }

        private void ExitLight()
        {
            float ResultPoints = SuccessPoints;

            //Cars waited more than acceptable
            //player losses points
            if(_waitingTime > AcceptableWaitingTime)
            {
                float UnAcceptWaitTime = _waitingTime - AcceptableWaitingTime;
                //even if cars waited more than acceptable player can gain points
                //if was fast enough
                float Ratio = (UnAcceptWaitTime / TimeToWorstScore) < 1 ? (UnAcceptWaitTime / TimeToWorstScore) : 1;
                //reaching TIME_TO_WORST_SCORE leads to losing FAIL_POINTS amount of points
                ResultPoints += (FailPoints - SuccessPoints) * Ratio;
            }
            _waitingTime = 0f;
            IndicatorOfScore.material = ScoreMaterialsComponent.Good;
            _scoreType = ScoreType.Good;

            _manager.ChangeScore(ResultPoints);
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