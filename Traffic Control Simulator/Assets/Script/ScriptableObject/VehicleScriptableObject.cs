using UnityEngine;
using UnityEngine.Serialization;

namespace Script.ScriptableObject
{
    [CreateAssetMenu(fileName = "Vehicle", menuName = "So/Vehicle", order = 0)]
    public class VehicleScriptableObject : UnityEngine.ScriptableObject
    {
        public GameObject vehiclePrefab;
        
        [SerializeField] private int rotationSpeed = 4;
        
        [SerializeField] private int speed = 5;
        [SerializeField] private int slowDownSpeed = 3;

        public int Speed
        {
            get => speed;
            set => speed = value;
        }

        public int SlowDownSpeed
        {
            get => slowDownSpeed;
            set => slowDownSpeed = value;
        }

        public int RotationSpeed
        {
            get => rotationSpeed;
            private set => rotationSpeed = value;
        }
    }
}