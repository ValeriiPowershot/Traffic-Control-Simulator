using System;
using System.Collections;
using System.Collections.Generic;
using BaseCode.Logic.Services.Interfaces.Car;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class CarSpawnServiceHandler : ICarSpawnService
    {
        public List<CarSpawnObject> spawnObjects = new();
        
        public CarManager CarManager { get; set; }

        private Coroutine _updateCoroutine;
        public void Initialize(CarManager carManagerInScene)
        {
            CarManager = carManagerInScene;

            foreach (var spawnObject in spawnObjects)
            {
                spawnObject.Initialize(CarManager, this);
            }

            SpawnRoadDetectors();
        }

        public void Update()
        {
            if (_updateCoroutine == null)
            {
                _updateCoroutine = CarManager.StartCoroutine(UpdateWithDelay());
            }
        }

        private IEnumerator UpdateWithDelay()
        {
            foreach (var spawnObject in spawnObjects)
            {
                yield return null;
                spawnObject.Update();
            }
            _updateCoroutine = null;
        }
        
        private void SpawnRoadDetectors()
        {
            foreach (var container in CarManager.allWaysContainer.allWays)
            {
                var firstElement = container.roadPoints[0].point.transform;
                Object.Instantiate(CarManager.allWaysContainer.carDetectorPrefab, firstElement.position, firstElement.rotation, firstElement);
            }
        }

    }
}