using System.Collections.Generic;
using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.Base;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BaseCode.Logic.PopUps
{
    public class PopUpLevelsMenu : PopUpGameBase
    {
        public ButtonExtension openMainMenuButton;
        
        public ButtonExtension openNextLevelButton;
        public ButtonExtension openPreviousLevelButton;
        public ButtonExtension startGameButton;
        public ButtonExtension settingsGameButton;
        public ButtonExtension tapToStartGameButton;
        
        public TextMeshProUGUI starAmountText;
        public TextMeshProUGUI currentLevelText;

        public List<Transform> stars;
        public int currentLevelIndex;

        private SceneIDData _scenes;
        
        private void Start()
        {
            openMainMenuButton.onClick.AddListener(OnOpenMainMenuButtonClicked);
            startGameButton.onClick.AddListener(LoadGame);

            settingsGameButton.onClick.AddListener(() =>
            {
                PopUpManager.ShowPopUpFromBase(PopUpManager.GetPopUp<PopUpSettingMenu>());
            });
            tapToStartGameButton.onClick.AddListener(() =>
            {
                GameManager.cameraManager.ChangeCameraSizeToGame();
                PopUpManager.ShowPopUp<PopUpGameMenu>();
                
                CarManager.Initalize(); // there will be change
                ScoreManager.Initalize();
            });
            openNextLevelButton.onClick.AddListener(() =>
            {
                if (currentLevelIndex >= _scenes.sceneNames.Count - 1) return;
                
                currentLevelIndex++;
                SetStarAmountUI();
            });
            openPreviousLevelButton.onClick.AddListener(() =>
            {
                if (currentLevelIndex <= 0) return;
                
                currentLevelIndex--;
                SetStarAmountUI();
            });
        }

        private void LoadGame()
        {
            Debug.Log("Loading game " + currentLevelIndex);
        }

        private void OnOpenMainMenuButtonClicked()
        {
            SceneLoadManager.LoadSceneNormal(SceneID.MainMenu);
        }

        public override void OnStartShow()
        {
            base.OnStartShow();

            _scenes = SceneSo.GetScenesFromId(SceneID.Levels); 
            SetCurrentLevel();
            SetStarAmount();
            GameManager.cameraManager.ChangeCameraSizeToLevel();
        }

        private void SetCurrentLevel()
        {
            currentLevelIndex = 0;
            for (int i = 0; i < _scenes.sceneNames.Count; i++)
            {
                if (_scenes.sceneNames[i].IsUnLocked)
                    currentLevelIndex = i;
                else
                    break;
            }
            SetStarAmountUI();
        }

        private void SetStarAmountUI()
        {
            int currentStarAmount = _scenes.sceneNames[currentLevelIndex].AmountOfStar;
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].gameObject.SetActive(i < currentStarAmount);
            }
            
            currentLevelText.text = (currentLevelIndex + 1).ToString();
        }

        private void SetStarAmount()
        {
            int totalMaxStar = _scenes.sceneNames.Count * 3;
            int totalStar = GetTotalEarnedStar();
            
            starAmountText.text = totalStar + "/" + totalMaxStar;
        }
        private int GetTotalEarnedStar()
        {
            int totalStar = 0;
            foreach (var scene in _scenes.sceneNames)
            {
                if (scene.IsUnLocked)
                    totalStar += scene.AmountOfStar;
                else
                    break;
            }

            return totalStar;
        }

        private SceneLoadManager SceneLoadManager => GameManager.sceneLoadManager;
        private CarManager CarManager => GameManager.carManager;
        private ScoringManager ScoreManager => GameManager.scoringManager;
        public SceneSo SceneSo => GameManager.saveManager.sceneSo;
        
    }
}