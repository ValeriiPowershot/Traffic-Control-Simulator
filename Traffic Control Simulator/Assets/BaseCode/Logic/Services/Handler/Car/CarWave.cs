using System;
using System.Collections.Generic;
using BaseCode.Core.ObjectPool.CarPool;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class CarWave
    {
        private CarManager _carManager;
        public List<CarSpawnObject> carSpawnObjects = new();
        public WaveRank waveRank = WaveRank.Easy;
        
        private CarObjectPools _carObjectPools;
        public int currentIndex;
        
        public void Initialize(CarManager carManagerInScene,CarObjectPools carObjectPools)
        {
            _carManager = carManagerInScene;
            _carObjectPools = carObjectPools;
            
            foreach (CarSpawnObject wave in carSpawnObjects)
            {
                wave.Initialize(_carManager, (CarPool)_carObjectPools.GetPool(wave.carSoObjects));
            }
        }
        public void Update()
        {
            if (carSpawnObjects[currentIndex].IsOnMap())
            {
                currentIndex++;

                if (IsCurrentIndexWaveFinished())
                    _carManager.ExitWave();
            }
            else
                carSpawnObjects[currentIndex].Update();
            
        }

        public void ResetWave()
        {
            currentIndex = 0;
            foreach (var carSpawnObject in carSpawnObjects)
            {
                carSpawnObject.ResetCarSpawnObject();
            }
        }

        public bool IsCurrentIndexWaveFinished()
        {
            var result = currentIndex >= carSpawnObjects.Count; 
            Debug.Log(result);
            return result;
        }

    }
}