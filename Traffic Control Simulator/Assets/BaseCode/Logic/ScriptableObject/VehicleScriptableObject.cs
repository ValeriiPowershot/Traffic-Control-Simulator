using UnityEngine;

namespace BaseCode.Logic.ScriptableObject
{
    [CreateAssetMenu(fileName = "Vehicle", menuName = "ScriptableObject/Vehicle", order = 0)]
    public class VehicleScriptableObject : UnityEngine.ScriptableObject
    {
        public GameObject VehiclePrefab;
        
        [SerializeField] private float _speed = 5;
        [SerializeField] private float _slowdownSpeed = 4;
        [SerializeField] private float _accelerationSpeed = 6;
        
        public float RayLenght = 5f;

        public float DefaultSpeed
        {
            get => _speed;
            set => _speed = value;
        }

        public float AccelerationSpeed
        {
            get => _accelerationSpeed;
            set => _accelerationSpeed = value;
        }
        
        public float SlowdownSpeed
        {
            get => _slowdownSpeed;
            set => _slowdownSpeed = value;
        }

        public float RotationSpeed => 960;
    }
}