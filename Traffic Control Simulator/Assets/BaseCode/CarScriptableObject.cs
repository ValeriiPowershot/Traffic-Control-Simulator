using UnityEngine;

namespace BaseCode
{
    [CreateAssetMenu(fileName = "NewCar", menuName = "Spawning/Car")]
    public class CarScriptableObject : ScriptableObject
    {
        public GameObject Prefab;
    }
}
