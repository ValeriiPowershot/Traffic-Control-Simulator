using System;
using BaseCode.Logic.PopUps.Base;
using UnityEngine.UI;

namespace BaseCode.Logic.PopUps
{
    public class PopUpMainMenu : PopUpGameBase
    {
        public Button openSettingsButton;

        private void Start()
        {
            openSettingsButton.onClick.AddListener(OnOpenSettingsButtonClicked);
        }
        private void OnOpenSettingsButtonClicked()
        {
            gameManager.popUpController.ShowPopUp<PopUpSettingMenu>();
        }
    }
}