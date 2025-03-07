using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BaseCode.Logic.ScriptableObject
{
    [CreateAssetMenu(fileName = "Vehicle", menuName = "ScriptableObject/Vehicle", order = 0)]
    public class VehicleScriptableObject : UnityEngine.ScriptableObject
    {
        public GameObject vehiclePrefab;

        public float minSpeed = 6;
        public float maxSpeed = 10;
        
        public float rayLenght = 5f;
        public float rotationSpeed;
        public float MIN_MOVING_RANGE = 0.001f;

        private float SpeedStep => (maxSpeed - minSpeed) / 3;
        public float DefaultSpeed => Random.Range(minSpeed + SpeedStep, minSpeed + SpeedStep * 2);
        public float AccelerationSpeed => Random.Range(minSpeed + SpeedStep * 2, maxSpeed);
        public float SlowdownSpeed => Random.Range(minSpeed, minSpeed + SpeedStep);
        public float AcceptableWaitingTime { get; set; }
        public float TimeToWorstScore { get; set; }
        public float SuccessPoints;
        public float FailPoints;
    }
}