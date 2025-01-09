using UnityEngine;

namespace BaseCode.Core.ObjectPool.Base {
   
   public interface IPoolObject
   {
      void Initialize(Pool pool, GameObject poolObject);
      void OnObjectInstantiate();
      void OnObjectDestroy();
      void InvokeDestroy(float delay);
      void Destroy();

      Pool Pool { get; set; }
      GameObject PoolObject { get; set; }
   }
 
}