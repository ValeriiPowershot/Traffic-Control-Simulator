using System;
using BaseCode.Extensions;
using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.Base;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;
using UnityEngine.UI;

namespace BaseCode.Logic.PopUps
{
    public class PopUpMainMenu : PopUpGameBase
    {
        public ButtonExtension openSettingsButton;
        public ButtonExtension levelsButton;
        public ButtonExtension continueButton;
        public ButtonExtension exitButton;

        private readonly IteratorRefExtension<float> _lastMusicTime = new IteratorRefExtension<float>();
        private void Start()
        {
            openSettingsButton.onClick.AddListener(OnOpenSettingsButtonClicked);
            exitButton.onClick.AddListener(OnOpenExitButtonClicked);
        }
        public override void OnStartShow()
        {
            base.OnStartShow();
            gameManager.vfxManager.PlayGameMusic(VfxTypes.GameMenuPopUpVfx, _lastMusicTime.Value);
        }
        public override void OnStartHidden()
        {
            base.OnStartHidden();
            Debug.Log("OnStartHidden");
            gameManager.StartCoroutine(gameManager.vfxManager.FadeOutMusic(_lastMusicTime));
        }

        private void OnOpenExitButtonClicked()
        {
            PopUpController.ShowPopUp<PopUpExitMenu>();
        }

        private void OnOpenSettingsButtonClicked()
        {
            PopUpController.ShowPopUp<PopUpSettingMenu>();
        }
    }
}