using UnityEngine;

namespace Script.ScoringSystem
{
    public class ScoreObjectCar : MonoBehaviour
    {
        [SerializeField] private float ACCEPTABLE_WAITING_TIME = 1f;
        [Tooltip("Amount of points player will gain in best scenario (for this certain car)")]
        [SerializeField] private float SUCCESS_POINTS;
        [Tooltip("(Negative value) this amount of points player will lose in worst scenario (for this certain car)")]
        [SerializeField] private float FAIL_POINTS;
        [Tooltip("Amount of time have to pass after acceptable time runs out to reach worst scenario")]
        [SerializeField] private float TIME_TO_WORST_SCORE;

        private BasicCar _car;
        private Vector3 _prevPosition;
        private float _waitingTime;

        private float MIN_MOVING_RANGE = 0.001f;

        private void Awake()
        {
            _car = GetComponent<BasicCar>();
        }

        private void OnEnable()
        {
            _car.LightExited += ExitLight;
        }
        private void OnDisable()
        {
            _car.LightExited -= ExitLight;
        }

        public void Update()
        {
            float DeltaTime = Time.deltaTime;

            if (_car.CarLightState == LightState.Red)
            {
                //recognize if car is standing still 
                //red light shows that car stands because players actions
                if ((_prevPosition - transform.position).sqrMagnitude <= MIN_MOVING_RANGE * MIN_MOVING_RANGE)
                {
                    _waitingTime += DeltaTime;
                }
            }
            _prevPosition = transform.position;
        }

        private void ExitLight()
        {
            float ResultPoints = SUCCESS_POINTS;

            //Cars waited more than acceptable
            //player losses points
            if(_waitingTime > ACCEPTABLE_WAITING_TIME)
            {
                float UnAcceptWaitTime = _waitingTime - ACCEPTABLE_WAITING_TIME;
                //even if cars waited more than acceptable player can gain points
                //if was fast enough
                float Ratio = (UnAcceptWaitTime / TIME_TO_WORST_SCORE) < 1 ? (UnAcceptWaitTime / TIME_TO_WORST_SCORE) : 1;
                //reaching TIME_TO_WORST_SCORE leads to losing FAIL_POINTS amount of points
                ResultPoints += (FAIL_POINTS - SUCCESS_POINTS) * Ratio;
            }
            _waitingTime = 0f;
            //here scoring system will be notified of losing or gaining points
        }
    }
}
