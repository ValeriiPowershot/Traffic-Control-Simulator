using UnityEngine;
using UnityEngine.Serialization;

namespace Script.So
{
    [CreateAssetMenu(fileName = "Vehicle", menuName = "So/Vehicle", order = 0)]
    public class VehicleSo : ScriptableObject
    {
        public GameObject vehiclePrefab;
        
        public float speed = 10f;
        public float slowDownSpeed = 3f;
    }
}