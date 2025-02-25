using BaseCode.Logic.PopUps.Base;
using UnityEngine.UI;

namespace BaseCode.Logic.PopUps
{
    public class PopUpSettingMenu : PopUpGameBase
    {
        public Button openMainMenu;

        private void Start()
        {
            openMainMenu.onClick.AddListener(OnOpenSettingsButtonClicked);
        }
        private void OnOpenSettingsButtonClicked()
        {
            gameManager.popUpController.ShowPopUp<PopUpMainMenu>();
        }
    }
}