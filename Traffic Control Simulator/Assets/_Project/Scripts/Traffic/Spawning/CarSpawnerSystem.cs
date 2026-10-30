using System.Collections;
using System.Collections.Generic;
using Realistic_Traffic_Controller.Scripts;
using UnityEngine;

namespace BaseCode
{
    [DisallowMultipleComponent]
    public class CarSpawnerSystem : MonoBehaviour
    {
        [SerializeField] private TrafficGameSettings settings;

        [System.Serializable]
        public class SpawnerConfig
        {
            [Header("Spawn Point")]
            public Transform SpawnPoint;
            public RTC_Waypoint StartWaypoint;
            public int SpawnPointIndex;

            [Header("Cars")]
            public CarScriptableObject[] Cars;

            [Header("Spawn Settings")]
            public float SpawnDelay = 2f;

            [System.NonSerialized] public CarInARowCheck RowCheck;
        }

        [Header("Spawners")]
        public SpawnerConfig[] spawners;

        [Header("Systems")]
        [SerializeField] private DifficultyController _difficulty;

        [Header("Fallback")]
        [SerializeField] private int _maxCarsFallback = 20;

        private void Start()
        {
            if (spawners == null) return;
            foreach (var spawner in spawners)
            {
                if (spawner == null || spawner.SpawnPoint == null)
                {
                    Debug.LogWarning("Spawner не настроен!", this);
                    continue;
                }

                // Кэшируем проверку ряда
                spawner.RowCheck = spawner.SpawnPoint.GetComponent<CarInARowCheck>();

                if (spawner.RowCheck == null)
                {
                    Debug.LogWarning($"No CarInARowCheck on {spawner.SpawnPoint.name}", this);
                }

                StartCoroutine(RunSpawner(spawner));
            }
        }

        private IEnumerator RunSpawner(SpawnerConfig spawner)
        {
            while (true)
            {
                // Ждём пока можно спавнить
                while (CampaignBlocked() || IsMaxCarsReached() || IsRowFull(spawner))
                    yield return null;

                SpawnRandomCar(spawner);

                float delay = Mathf.Max(0.01f, spawner.SpawnDelay * (settings != null ? settings.SpawnIntervalMultiplier : 1f));
                yield return new WaitForSeconds(delay);
            }
        }

        private bool CampaignBlocked()
        {
            var director = BaseCode.Logic.City.GameManager.ForScene(gameObject.scene);
            return director != null && !director.IsPlaying;
        }

        private bool IsMaxCarsReached()
        {
            int maxCars = settings != null && settings.FallbackCarLimit > 0 ? settings.FallbackCarLimit : _maxCarsFallback;

            if (_difficulty != null && _difficulty.CurrentStage != null)
                maxCars = Mathf.Max(1, _difficulty.CurrentStage.MaxCarsOnScreen);

            return transform.childCount >= maxCars;
        }

        private bool IsRowFull(SpawnerConfig spawner)
        {
            if (spawner.RowCheck == null)
                return false;

            return spawner.RowCheck.IsMaxCarsInRowReached();
        }

        private void SpawnRandomCar(SpawnerConfig spawner)
        {
            if (spawner.Cars == null || spawner.Cars.Length == 0)
                return;

            var carData = spawner.Cars[Random.Range(0, spawner.Cars.Length)];

            if (carData == null || carData.Prefab == null)
            {
                Debug.LogWarning("Car или Prefab отсутствует!", this);
                return;
            }

            GameObject car = Instantiate(
                carData.Prefab,
                spawner.SpawnPoint.position,
                spawner.SpawnPoint.rotation,
                transform
            );

            var controller = car.GetComponent<RTC_CarController>();

            if (controller != null)
            {
                controller.SetWaypoint(spawner.StartWaypoint);
                controller.CarSpawnIndex = spawner.SpawnPointIndex;
                var director = BaseCode.Logic.City.GameManager.ForScene(gameObject.scene);
                if (director != null) director.RegisterVehicle(controller.gameObject);
            }
        }
    }
}
