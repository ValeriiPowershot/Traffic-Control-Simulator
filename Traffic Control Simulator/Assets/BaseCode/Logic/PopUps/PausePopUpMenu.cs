using System.Collections.Generic;
using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.Base;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BaseCode.Logic.PopUps
{
    public class PausePopUpMenu : PopUpGameBase
    {
        public ButtonExtension resume;
        public ButtonExtension settings;
        public ButtonExtension exit;
        
        private void Start()
        {
            exit.onClick.AddListener(OnOpenMainMenuButtonClicked);
            settings.onClick.AddListener(OnOpenSettingMenuButtonClicked);
            resume.onClick.AddListener(OnOpenResumeMenuButtonClicked);
        }

        private void OnOpenResumeMenuButtonClicked()
        {
            PopUpController.HidePopUp(this);
        }

        private void OnOpenSettingMenuButtonClicked()
        {
            PopUpController.ShowPopUp(PopUpController.GetPopUp<PopUpSettingMenu>());
        }

        private void OnOpenMainMenuButtonClicked()
        {
            // load main menu
        }
    }
}