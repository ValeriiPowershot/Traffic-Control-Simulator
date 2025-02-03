using System;
using System.Collections.Generic;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.ScriptableObject;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class CarWave
    {
        public List<CarSpawnObject> carSpawnObjects = new();
        public WaveRank waveRank = WaveRank.Easy;
        
        private CarObjectPools _carObjectPools;
        public void Initialize(CarManager carManagerInScene,CarObjectPools carObjectPools)
        {
            _carObjectPools = carObjectPools;
            
            foreach (var wave in carSpawnObjects)
            {
                wave.Initialize(carManagerInScene, (CarPool)_carObjectPools.GetPool(wave.carSoObjects));
            }
        }
    
    }
}