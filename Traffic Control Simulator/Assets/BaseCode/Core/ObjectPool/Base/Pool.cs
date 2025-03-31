using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace BaseCode.Core.ObjectPool.Base {

   [Serializable]
   public class Pool 
   {
      [SerializeField] private bool autoGrow = false;
      protected int Capacity;
      
      private GameObject _poolObjectPrefab;
      private Transform _spawnPoint;

      private UnityAction<IPoolObject> _onObjectInstantiated;
      private UnityAction<IPoolObject> _onObjectDestroyed;

      protected readonly Queue<IPoolObject> Queue = new Queue<IPoolObject>();
      
      public Pool(GameObject poolObjectPrefab, Transform spawnPoint = null)
      {
         _poolObjectPrefab = poolObjectPrefab;
         _spawnPoint = spawnPoint;
      }
      
      public void InitializeQueue(int capacity)
      {
         Capacity = capacity;
         AddToQueue(Capacity);
      }
      public void AddToQueue(int amount)
      {
         for (int i = 0; i < amount; i++)
            InsertObjectToQueue();
      }

      public virtual IPoolObject InsertObjectToQueue()
      {
         var createdPoolObj = Object.Instantiate(_poolObjectPrefab, _spawnPoint, true);
         var poolObj = createdPoolObj.GetComponent<IPoolObject>();
         
         poolObj.Initialize(this, createdPoolObj);
         poolObj.OnObjectDestroy();
         
         Queue.Enqueue(poolObj);

         return poolObj;
      }
      
      public virtual IPoolObject InstantiateObject(Vector3 position, Quaternion rotation)
      {
         var poolObj = InstantiateObject();
         if (poolObj == null) return null;
         
         poolObj.PoolObject.transform.position = position;
         poolObj.PoolObject.transform.rotation = rotation;
         
         return poolObj;
      }
      public virtual IPoolObject InstantiateObject()
      {
         if (Queue.Count == 0)
         {
            if (autoGrow == false) return null;
            
            Capacity++;
            InsertObjectToQueue();
         }

         var poolObj = Queue.Dequeue();
         poolObj.OnObjectInstantiate();
         
         _onObjectInstantiated?.Invoke(poolObj);

         return poolObj;
      }

      public virtual void DestroyObject(IPoolObject poolObj)
      {
         Queue.Enqueue(poolObj);
         _onObjectDestroyed?.Invoke(poolObj);
         poolObj.OnObjectDestroy();
      }

      public virtual void DestroyObject(IPoolObject poolObj, float delay) => poolObj.InvokeDestroy(delay);
      
      public int GetDeActiveAmount() => Queue.Count;
      public int GetActiveAmount() => Capacity - GetDeActiveAmount();
      public GameObject GetPoolObjectPrefab() => _poolObjectPrefab;

      public int GetPoolSize() => Queue.Count;
   }


}