using UnityEngine;

namespace BaseCode
{
    [CreateAssetMenu(fileName = "NewCarSpawnPool", menuName = "Spawning/Car Spawn Pool")]
    public class CarPoolScriptableObject : ScriptableObject
    {
        [System.Serializable]
        public class CarSpawnData
        {
            public CarScriptableObject car;
            public int count = 1;
            public float delayBetweenSpawns = 1f;
            public float initialDelay = 0f;
        }

        public CarSpawnData[] carsToSpawn;
    }
}
