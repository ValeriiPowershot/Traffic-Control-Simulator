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

        [Header("Systems")]
        [SerializeField] private DifficultyController _difficulty;

        [Header("Fallback")]
        [SerializeField] private int _maxCarsFallback ;

        private void Start()
        {
            foreach (SpawnerConfig spawner in spawners)
            {
                if (spawner.Config == null || spawner.SpawnPoint == null)
                {
                    Debug.LogWarning("Spawner не настроен!", this);
                    continue;
                }

                StartCoroutine(RunSpawner(spawner));
            }
        }

        private IEnumerator RunSpawner(SpawnerConfig spawner)
        {
            while (true)
            {
                foreach (CarPoolScriptableObject.CarSpawnData carData in spawner.Config.carsToSpawn)
                {
                    yield return new WaitForSeconds(carData.initialDelay);

                    while (true)
                    {
                        Debug.Log("Starting CarSpawnerSystem");


                        // 🔥 ограничение только по количеству машин на сцене
                        while (IsMaxCarsReached())
                        {
                            yield return null;
                        }

                        SpawnCar(spawner, carData);

                        yield return new WaitForSeconds(carData.delayBetweenSpawns);
                    }
                }
            }
        }

        private bool IsMaxCarsReached()
        {
            int maxCars = _maxCarsFallback;

            if (_difficulty != null)
                maxCars = _difficulty.CurrentStage.MaxCarsOnScreen;

            return transform.childCount >= maxCars;
        }

        private void SpawnCar(SpawnerConfig spawner, CarPoolScriptableObject.CarSpawnData carData)
        {
            if (carData.car == null || carData.car.Prefab == null)
            {
                Debug.LogWarning("Car или Prefab отсутствует!", this);
                return;
            }

            GameObject car = Instantiate(
                carData.car.Prefab,
                spawner.SpawnPoint.position,
                spawner.SpawnPoint.rotation,
                transform
            );

            var controller = car.GetComponent<RTC_CarController>();

            if (controller != null)
            {
                controller.nextWaypoint = spawner.StartWaypoint;
                controller.CarSpawnIndex = spawner.SpawnPointIndex;
            }
        }
    }
}
