using System.Collections.Generic;
using BaseCode.Extensions.UI;
using BaseCode.Logic.Managers;
using BaseCode.Logic.PopUps.PopUp_Base;
using BaseCode.Logic.ScriptableObject;
using TMPro;
using UnityEngine;

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

        private SceneIDData _scenes;
        
        private SceneLoadManager SceneLoadManager => GameManager.sceneLoadManager;
        private CarManager CarManager => GameManager.carManager;
        private ScoringManager ScoreManager => GameManager.scoringManager;
        public SceneSo SceneSo => GameManager.saveManager.sceneSo;
        
        private int CurrentLevelIndex
        {
            get => CarManager.CarSpawnServiceHandler.CarWaveController.CurrentLevelIndex;
            set => CarManager.CarSpawnServiceHandler.CarWaveController.CurrentLevelIndex = value;
        }
        
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
                GameManager.carManager.StartGame();
                GameManager.cameraManager.ChangeCameraSizeToGame();
                PopUpManager.ShowPopUp<PopUpGameMenu>();
            });
            openNextLevelButton.onClick.AddListener(() =>
            {
                if (CurrentLevelIndex >= _scenes.sceneNames.Count - 1) return;
                
                Debug.Log("Opening next level");
                
                CurrentLevelIndex++;
                SetStarAmountUI();
            });
            openPreviousLevelButton.onClick.AddListener(() =>
            {
                if (CurrentLevelIndex <= 0) return;
                
                Debug.Log("Opening previous level");
                
                CurrentLevelIndex--;
                SetStarAmountUI();
            });
        }

        private void LoadGame()
        {
            Debug.Log("Loading game " + CurrentLevelIndex);
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
            CurrentLevelIndex = 0;
            for (int i = 0; i < _scenes.sceneNames.Count; i++)
            {
                if (_scenes.sceneNames[i].IsUnLocked)
                    CurrentLevelIndex = i;
                else
                    break;
            }
            SetStarAmountUI();
        }

        private void SetStarAmountUI()
        {
            int currentStarAmount = _scenes.sceneNames[CurrentLevelIndex].AmountOfStar;
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].gameObject.SetActive(i < currentStarAmount);
            }
            
            currentLevelText.text = (CurrentLevelIndex + 1).ToString();
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

       

       
        
    }
}