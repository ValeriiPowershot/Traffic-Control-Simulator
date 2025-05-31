using System;
using BaseCode.Extensions.UI;
using BaseCode.Logic.Managers;
using BaseCode.Logic.PopUps.PopUp_Base;
using BaseCode.Logic.ScriptableObject;
using TMPro;
using UnityEngine;

namespace BaseCode.Logic.PopUps
{
    public class PopUpGameMenu : PopUpGameBase
    {
        public ButtonExtension pauseButtonExtension;
        public TextMeshProUGUI soreText;
        public TextMeshProUGUI informationText;

        private void Start()
        {
            pauseButtonExtension.onClick.AddListener(
                () =>
                {
                    PopUpManager.ShowPopUpFromBase(PopUpManager.GetPopUp<PopUpPauseMenu>(), ()=> Time.timeScale = 0);
                });
        }

        public override void OnStartShow()
        {
            base.OnStartShow();
            AudioSource createdVfx = VfxManager.PlayVfx(VfxTypes.GameMenuPopUpVfx);
        }
        
        public VfxManager VfxManager => GameManager.vfxManager;
    }
}