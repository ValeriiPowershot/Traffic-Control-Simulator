using System;
using BaseCode.Logic;
using BaseCode.Logic.Managers;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BaseCode.Extensions.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonExtension : MonoBehaviour
    {
        private Button _button;
        private UnityAction _defaultClickSoundButtonAction;

        public Button.ButtonClickedEvent onClick
        {
            set => _button.onClick = value;
            get => _button.onClick;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public virtual void Start()
        {
            onClick.AddListener(DefaultOnClickFunctions);
            SetNewSoundEffect(VfxTypes.ButtonClickVfx);
        }

        private void DefaultOnClickFunctions()
        {
            ClickSound();
        }

        public virtual void ClickSound()
        {
            _defaultClickSoundButtonAction.Invoke();
        }

        public void SetNewSoundEffect(VfxTypes vfxTypes)
        {
            _defaultClickSoundButtonAction = () => VfxManager.PlayVfx(vfxTypes);
        }

        public GameManager GameManager => (GameManager)GameManager.Instance;
        public VfxManager VfxManager => GameManager.vfxManager;
        public GameSettings GameSettings => GameManager.saveManager.configSo.gameSettings;
    }
}