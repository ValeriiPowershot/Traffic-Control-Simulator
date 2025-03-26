using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.Base;
using UnityEngine;
using UnityEngine.UI;

namespace BaseCode.Logic.PopUps
{
    public class PopUpHelpMenu : PopUpGameBase
    {
        public ButtonExtension closeThisPopUp;

        private void Start()
        {
            closeThisPopUp.onClick.AddListener(OnOpenSettingsButtonClicked);
        }
        private void OnOpenSettingsButtonClicked()
        {
            GameManager.popUpManager.HidePopUp<PopUpHelpMenu>();
        }
    }
}