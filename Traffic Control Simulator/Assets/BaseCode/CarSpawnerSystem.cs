using System.Collections;
using Realistic_Traffic_Controller.Scripts;
using UnityEngine;

namespace BaseCode
{
    [DisallowMultipleComponent]
    public class CarSpawnerSystem : MonoBehaviour
    {
        [System.Serializable]
        public class SpawnerConfig
        {
            public Transform SpawnPoint;
            public CarPoolScriptableObject Config;
            public RTC_Waypoint StartWaypoint;
            public int SpawnPointIndex;
        }

        public SpawnerConfig[] spawners;

        [SerializeField] private int _maxSpawnedCarsOnScene;

        private void Start()
        {
            foreach (SpawnerConfig spawner in spawners)
            {
                if (spawner.Config == null)
                {
                    Debug.LogWarning("SpawnerConfig: не назначен конфиг!", this);
                    continue;
                }

                if (spawner.SpawnPoint == null)
                {
                    Debug.LogWarning("SpawnerConfig: не назначена точка спавна!", this);
                    continue;
                }

                StartCoroutine(RunSpawner(spawner));
            }
        }

        private IEnumerator RunSpawner(SpawnerConfig spawner)
        {
            foreach (CarPoolScriptableObject.CarSpawnData carData in spawner.Config.carsToSpawn)
            {
                yield return new WaitForSeconds(carData.initialDelay);

                for (int i = 0; i < carData.count; i++)
                {
                    while (transform.childCount >= _maxSpawnedCarsOnScene)
                    {
                        yield return null;
                    }

                    if (carData.car != null && carData.car.Prefab != null)
                    {
                        GameObject car = Instantiate(
                            carData.car.Prefab,
                            spawner.SpawnPoint.position,
                            spawner.SpawnPoint.rotation,
                            transform
                        );

                        RTC_CarController controller = car.GetComponent<RTC_CarController>();
                        if (controller != null)
                        {
                            controller.nextWaypoint = spawner.StartWaypoint;
                            controller.CarSpawnIndex = spawner.SpawnPointIndex;
                        }
                        else
                        {
                            Debug.LogWarning("У заспавненной машины нет RTC_CarController!", car);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Car или его Prefab отсутствует в конфиге!", this);
                    }

                    yield return new WaitForSeconds(carData.delayBetweenSpawns);
                }
            }
        }
    }
}
