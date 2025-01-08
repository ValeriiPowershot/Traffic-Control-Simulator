using BaseCode.Interfaces;
using BaseCode.Logic.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.ScoringSystem
{
    public class ScoreObjectCar : MonoBehaviour, IScoringObject
    {
        [SerializeField] private float _acceptableWaitingTime = 1f;
        [Tooltip("Amount of points player will gain in best scenario (for this certain car)")]
        [SerializeField] private float _successPoints;
        [Tooltip("(Negative value) this amount of points player will lose in worst scenario (for this certain car)")]
        [SerializeField] private float _failPoints;
        [Tooltip("Amount of time have to pass after acceptable time runs out to reach worst scenario")]
        [SerializeField] private float _timeToWorstScore;

        //temporary 
        [SerializeField] private MeshRenderer _indicatorOfScore;
        [SerializeField] private ScoringMaterials _materials;

        private ScoringManager _manager;
        private BasicCar _car;

        private ScoreType _scoreType = ScoreType.Good;
        private Vector3 _prevPosition;
        private float _waitingTime;

        private const float MIN_MOVING_RANGE = 0.001f;

        private void Awake() =>
            _car = GetComponent<BasicCar>();

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
                        _waitingTime >= _acceptableWaitingTime)
                    {
                        _indicatorOfScore.material = _materials.Neutral;
                        _scoreType = ScoreType.Neuteral;
                    }

                    else if (_scoreType == ScoreType.Neuteral && 
                             _waitingTime - _acceptableWaitingTime >= _timeToWorstScore)
                    {
                        _indicatorOfScore.material = _materials.Bad;
                        _scoreType = ScoreType.Bad;
                    }
                }
            }
            _prevPosition = transform.position;
        }

        private void ExitLight()
        {
            float ResultPoints = _successPoints;

            //Cars waited more than acceptable
            //player losses points
            if(_waitingTime > _acceptableWaitingTime)
            {
                float UnAcceptWaitTime = _waitingTime - _acceptableWaitingTime;
                //even if cars waited more than acceptable player can gain points
                //if was fast enough
                float Ratio = (UnAcceptWaitTime / _timeToWorstScore) < 1 ? (UnAcceptWaitTime / _timeToWorstScore) : 1;
                //reaching TIME_TO_WORST_SCORE leads to losing FAIL_POINTS amount of points
                ResultPoints += (_failPoints - _successPoints) * Ratio;
            }
            _waitingTime = 0f;
            _indicatorOfScore.material = _materials.Good;
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