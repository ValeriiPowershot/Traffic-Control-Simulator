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
    public class PopUpPauseMenu : PopUpGameBase
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
            PopUpManager.HidePopUp(this);
        }

        private void OnOpenSettingMenuButtonClicked()
        {
            PopUpManager.ShowPopUpFromBase(PopUpManager.GetPopUp<PopUpSettingMenu>());
        }

        private void OnOpenMainMenuButtonClicked()
        {
            PopUpManager.HidePopUp(this);
            PopUpManager.ShowPopUp<PopUpLevelsMenu>();
        }
    }
}