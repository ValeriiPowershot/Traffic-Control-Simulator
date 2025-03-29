using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.Base;
using UnityEngine;
using UnityEngine.UI;

namespace BaseCode.Logic.PopUps
{
    public class PopUpExitMenu : PopUpGameBase
    {
        public ButtonExtension cancelButton;
        public ButtonExtension okButton;

        private void Start()
        {
            cancelButton.onClick.AddListener(ResumeGame);
            // okButton.onClick.AddListener(ExitGame);
        }
        private void ResumeGame()
        {
            PopUpManager.ShowPopUp<PopUpMainMenu>();
        }
        private void ExitGame()
        {
            GameManager.ExitGame();
        }
    }
}