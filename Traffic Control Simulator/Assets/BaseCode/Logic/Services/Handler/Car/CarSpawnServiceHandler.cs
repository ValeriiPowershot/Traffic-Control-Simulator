using System;
using System.Collections.Generic;
using BaseCode.Logic.Services.Interfaces.Car;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class CarSpawnServiceHandler : ICarSpawnService
    {
        public List<CarSpawnObject> spawnObjects = new List<CarSpawnObject>();
        
        public CarManager CarManager { get; set; }

        public void Initialize(CarManager carManagerInScene)
        {
            CarManager = carManagerInScene;

            foreach (var spawnObject in spawnObjects)
            {
                spawnObject.Initialize(CarManager, this);
            }
        }

        public void Update()
        {
            foreach (var spawnObject in spawnObjects)
            {
                spawnObject.Update();
            }
        }
        
    }
}