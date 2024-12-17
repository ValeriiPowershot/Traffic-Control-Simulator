using UnityEngine;

namespace Script.So
{
    [CreateAssetMenu(fileName = "Vehicle", menuName = "So/Vehicle", order = 0)]
    public class VehicleSo : ScriptableObject
    {
        public GameObject vehiclePrefab;
        
        public float Speed = 10f;
        public float SlowDownSpeed = 3f;

    }
}