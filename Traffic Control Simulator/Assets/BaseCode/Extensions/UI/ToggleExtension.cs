using BaseCode.Logic.ScriptableObject;
using UnityEngine;
using UnityEngine.UI;

namespace BaseCode.Extensions.UI
{
    public class ToggleExtension : ButtonExtension
    {
        [SerializeField] private Transform on;
        [SerializeField] private Transform off;

        private bool _isOn;

        private bool IsOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                on.gameObject.SetActive(_isOn);
                off.gameObject.SetActive(!_isOn);
                GameSettings.isNotificationsOn = _isOn;
            }
        }

        public override void Start()
        {
            base.Start();   
            onClick.AddListener(OnClicked);
        }

        public void OnClicked()
        {
            IsOn = !IsOn;
        }

        public void SetIsOn(bool isOn)
        {
            IsOn = isOn;
        }
    }
}