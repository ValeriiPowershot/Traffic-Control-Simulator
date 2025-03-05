using System;
using System.Collections.Generic;
using BaseCode.Core.ObjectPool.Base;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;

namespace BaseCode.Core.ObjectPool
{
    [Serializable]
    public class ObjectPoolsBase
    {
        public Dictionary<VehicleScriptableObject, Pool> Pool = new();
        
        public virtual Pool GetPool(VehicleScriptableObject poolObjectPrefab)
        {
            return Pool[poolObjectPrefab];
        }
        
        public virtual void AddPool(VehicleScriptableObject poolObjectPrefab, Transform spawnPoint = null)
        {
            Pool.Add(poolObjectPrefab, new Pool(poolObjectPrefab.vehiclePrefab, spawnPoint));
        }
    }
}