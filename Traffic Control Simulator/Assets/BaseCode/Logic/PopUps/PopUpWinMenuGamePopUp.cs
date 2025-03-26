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
    public class PopUpWinMenuGamePopUp : PopUpGameBase
    {
        public ButtonExtension home;
        public ButtonExtension reload;
        public ButtonExtension nextLevel;

        public TextMeshProUGUI score;
        public TextMeshProUGUI combo;
        public TextMeshProUGUI coin;

        public List<Transform> stars = new List<Transform>();
        private void Start()
        {
            home.onClick.AddListener(OnOpenHomeButtonClicked);
            reload.onClick.AddListener(OnOpenReloadButtonClicked);
            nextLevel.onClick.AddListener(OnOpenNextLevelButtonClicked);
        }

        public override void OnStartShow()
        {
            base.OnStartShow();
            SetStars();
        }

        protected virtual void SetStars()
        {
            int numberOfStars = 3;
            
            stars.SequenceOpenerSetActive(amount:numberOfStars);

            score.text = 1.ToString();
            combo.text = 1.ToString();
            coin.text = 1.ToString();
        }

        private void OnOpenNextLevelButtonClicked()
        {
            // load next level
        }

        private void OnOpenReloadButtonClicked()
        {
            // reload this level
        }

        private void OnOpenHomeButtonClicked()
        {
            // load main menu
        }
    }
}