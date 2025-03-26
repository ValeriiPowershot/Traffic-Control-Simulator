
using System;
using BaseCode.Core;

namespace BaseCode.Logic
{
    public class ManagerBase<T> : SingletonManagerBase<T>
    {
        public virtual void Start()
        {
            _gameManager = GameManager;
        }

        protected override void Awake()
        {
        }
        
    }
    public class SingletonManagerBase<T> : SingletonPersistent<SingletonManagerBase<T>> 
    {
        protected GameManager _gameManager;

        public virtual GameManager GameManager
        {
            get
            {
                if (_gameManager == null) 
                    _gameManager = FindObjectOfType<GameManager>();
                return _gameManager;
            }
        }
    }
}