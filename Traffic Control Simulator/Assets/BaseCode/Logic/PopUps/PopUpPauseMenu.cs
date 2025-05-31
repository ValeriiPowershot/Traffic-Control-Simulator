using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.PopUp_Base;
using UnityEngine;

namespace BaseCode.Logic.PopUps
{
    public class PopUpPauseMenu : PopUpGameBase
    {
        public ButtonExtension resume;
        public ButtonExtension settings;
        public ButtonExtension exit;

        private void Start()
        {
            exit.onClick.AddListener(OnOpenExitMenuButtonClicked);
            settings.onClick.AddListener(OnOpenSettingMenuButtonClicked);
            resume.onClick.AddListener(OnOpenResumeMenuButtonClicked);
        }

        public override void OnStartShow() =>
            Time.timeScale = 0;

        public override void OnStartHidden() =>
            Time.timeScale = 1;

        private void OnOpenResumeMenuButtonClicked()
        {
            Time.timeScale = 1;
            PopUpManager.HidePopUp(this);
        }

        private void OnOpenSettingMenuButtonClicked()
        {
            PopUpManager.ShowPopUpFromBase(PopUpManager.GetPopUp<PopUpSettingMenu>());
        }

        private void OnOpenExitMenuButtonClicked()
        {
            GameManager.carManager.ExitGame();
            PopUpManager.HidePopUp(this);
        }
    }
}
