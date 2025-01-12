using BaseCode.Interfaces;
using BaseCode.Logic.Entity.Lights;
using UnityEngine;

namespace BaseCode.Logic.Lights.Controllers
{
    public class LightSwitchController : MonoBehaviour, IInteractable,ITimeUsable
    {
        private BasicLight _light;
        private bool _interactCalled;

        public float Timer { get; set; }

        private void Awake()
        {
            _light = GetComponentInChildren<BasicLight>();
        }

        private void Update()
        {
            if(_interactCalled && IsTimerUp())
            {
                _interactCalled = false;
                _light.ChangeLight();
            }
        }

        public void Interact()
        {
            if (!_interactCalled)
            {
                _light.SetChangeoverState();
                _interactCalled = true;
                AddDelay(_light.lightData.SwitchDelay);
            }
        }
        
        public bool IsTimerUp()
        {
            return Time.time > Timer;
        }

        public void AddDelay(float delay)
        {
            Timer = Time.time + delay;
        }
    }
}
