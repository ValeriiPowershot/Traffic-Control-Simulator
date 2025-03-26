using UnityEngine;

namespace BaseCode.Core
{
    public class SingletonPersistent<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance => _instance;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}