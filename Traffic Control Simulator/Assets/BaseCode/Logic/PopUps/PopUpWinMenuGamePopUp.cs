using System;
using System.Collections.Generic;
using System.Globalization;
using BaseCode.Extensions;
using BaseCode.Extensions.UI;
using BaseCode.Logic.Managers;
using BaseCode.Logic.PopUps.PopUp_Base;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Utilities;
using TMPro;
using Unity.VisualScripting;
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
            score.AnimateScore(ScoringManager.PlayerScore, 1f, this);            
        }

        public override void OnStartDoTween(Action action = null)
        {
            stars.SetLocalScales(Vector3.zero);
            base.OnStartDoTween();
        }   

        private void OnOpenNextLevelButtonClicked()
        {
            GameManager.carManager.ExitGame();
            PopUpManager.HidePopUp(this);
            PopUpManager.ShowPopUp<PopUpLevelsMenu>();
        }

        private void OnOpenReloadButtonClicked()
        {
            // reload this level
        }

        private void OnOpenHomeButtonClicked()
        {
            // load main menu
        }

        public ScoringManager ScoringManager => GameManager.scoringManager;
    }
}