using UnityEngine;

namespace Script.ScriptableObject
{
    [CreateAssetMenu(fileName = "Vehicle", menuName = "So/Vehicle", order = 0)]
    public class VehicleScriptableObjects : UnityEngine.ScriptableObject
    {
        public GameObject vehiclePrefab;
        
        public float speed = 10f;
        public float slowDownSpeed = 3f;
    }
}