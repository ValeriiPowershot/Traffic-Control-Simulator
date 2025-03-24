using System;
using BaseCode.Logic.PopUps.Base;
using BaseCode.Logic.ScriptableObject;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BaseCode.Logic.PopUps
{
    public class GameMenuPopUp : PopUpGameBase
    {
        public TextMeshProUGUI soreText;

        public override void OnStartShow()
        {
            base.OnStartShow();
            
            var createdVfx = VfxManager.PlayVfx(VfxTypes.GameMenuPopUpVfx);
        }
        
        public VfxManager VfxManager => gameManager.vfxManager;
    }
}