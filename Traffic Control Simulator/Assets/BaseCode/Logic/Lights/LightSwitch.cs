using UnityEngine;

namespace BaseCode.Logic.Lights
{
    public class LightSwitch : MonoBehaviour, IInteractable
    {
        private BasicLight _light;
        private bool _interactCalled;

        private float _switchTimer;

        private void Awake()
        {
            _light = GetComponentInChildren<BasicLight>();
        }

        private void Update()
        {
            if(_interactCalled && Time.time > _switchTimer)
            {
                _interactCalled = false;
                _light.ChangeLight();
            }
        }

        public void Interact()
        {
            if (!_interactCalled)
            {
                _interactCalled = true;
                _switchTimer = Time.time + _light.lightData.SwitchDelay;
            }
        }
    }
}
