using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BaseCode.Core.ObjectPool
{
    public abstract class PoolObjectBase : MonoBehaviour,IPoolObject
    {
        public void Initialize(Pool pool, GameObject poolObject)
        {
            Pool = pool;
            PoolObject = poolObject;
        }

        public virtual void OnObjectInstantiate()
        {
            PoolObject.SetActive(true);
        }

        public virtual void OnObjectDestroy()
        {
            PoolObject.SetActive(false);
        }

        public virtual async void InvokeDestroy(float delay)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            Destroy();
        }

        public virtual void Destroy()
        {
            Pool.DestroyObject(this);
        }

        public Pool Pool { get; set; }
        public GameObject PoolObject { get; set; }
    }
}