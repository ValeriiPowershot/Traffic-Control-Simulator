using UnityEngine;

namespace BaseCode.Logic.ScriptableObject
{
    [CreateAssetMenu(fileName = "Vehicle", menuName = "ScriptableObject/Vehicle", order = 0)]
    public class VehicleScriptableObject : UnityEngine.ScriptableObject
    {
        public GameObject VehiclePrefab;
        
        [SerializeField] private int _speed = 5;
        [SerializeField] private int _slowdownSpeed = 4;
        [SerializeField] private int _accelerationSpeed = 6;
        
        public float RayLenght = 5f;
        public int IndexPath;

        public int DefaultSpeed
        {
            get => _speed;
            set => _speed = value;
        }

        public int AccelerationSpeed
        {
            get => _accelerationSpeed;
            set => _accelerationSpeed = value;
        }
        
        public int SlowdownSpeed
        {
            get => _slowdownSpeed;
            set => _slowdownSpeed = value;
        }

        public int RotationSpeed => 960;
    }
}