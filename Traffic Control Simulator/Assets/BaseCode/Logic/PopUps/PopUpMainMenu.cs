using System;
using BaseCode.Extensions;
using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.Base;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BaseCode.Logic.PopUps
{
    public class PopUpMainMenu : PopUpGameBase
    {
        public ButtonExtension openSettingsButton;
        public ButtonExtension levelsButton;
        public ButtonExtension continueButton;
        public ButtonExtension exitButton;

        private readonly IteratorRefExtension<float> _lastMusicTime = new IteratorRefExtension<float>(); // move to fx manager?
        private void Start()
        {
            openSettingsButton.onClick.AddListener(OnOpenSettingsButtonClicked);
            exitButton.onClick.AddListener(OnOpenExitButtonClicked);
            levelsButton.onClick.AddListener(OnOpenLevelsButtonClicked);
        }

        public override void OnStartShow()
        {
            base.OnStartShow();
            GameManager.vfxManager.PlayGameMusic(VfxTypes.GameMenuPopUpVfx, _lastMusicTime.Value);
        }
        public override void OnStartHidden()
        {
            base.OnStartHidden();
            GameManager.StartCoroutine(GameManager.vfxManager.FadeOutMusic(_lastMusicTime));
        }
        private void OnOpenLevelsButtonClicked()
        {
            GameManager.StartCoroutine(GameManager.vfxManager.FadeOutMusic(_lastMusicTime));
            SceneLoadManager.LoadSceneNormal(SceneID.Levels);
        }

        private void OnOpenExitButtonClicked()
        {
            PopUpManager.ShowPopUp<PopUpExitMenu>();
        }

        private void OnOpenSettingsButtonClicked()
        {
            PopUpManager.ShowPopUpFromBase(PopUpManager.GetPopUp<PopUpSettingMenu>());
        }

        private SceneLoadManager SceneLoadManager => GameManager.sceneLoadManager;
    }
}