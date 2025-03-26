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
    // dont check this file - it will be reviewed!
    public class PopUpLevelsMenu : PopUpGameBase
    {
        public ButtonExtension openMainMenuButton;
        
        public ButtonExtension openNextLevelButton;
        public ButtonExtension openPreviousLevelButton;
        public ButtonExtension startGameButton;
        
        public TextMeshProUGUI starAmountText;
        public TextMeshProUGUI currentLevelText;

        public List<Transform> stars;
        public int currentLevelIndex;

        public OnLoadToggleObject mainMenuToggleObject; 
        private SceneIDData _scenes;
        
        private void Start()
        {
            openMainMenuButton.onClick.AddListener(OnOpenMainMenuButtonClicked);
            startGameButton.onClick.AddListener(LoadGame);

            openNextLevelButton.onClick.AddListener(() =>
            {
                if (currentLevelIndex >= _scenes.sceneNames.Count - 1) return;
                
                currentLevelIndex++;
                SetStarAmountUI();
                LoadCurrentIndexLastScene();
            });
            openPreviousLevelButton.onClick.AddListener(() =>
            {
                if (currentLevelIndex <= 0) return;
                
                currentLevelIndex--;
                SetStarAmountUI();
                LoadCurrentIndexLastScene();
            });
        }

        private void LoadGame()
        {
            Debug.Log("Loading game " + currentLevelIndex);
        }

        private void OnOpenMainMenuButtonClicked()
        {
            PopUpController.ShowPopUp<PopUpMainMenu>();
            gameManager.sceneLoadManager.UnLoadScene();
            mainMenuToggleObject.ToggleObjects();
        }

        public override void OnStartShow()
        {
            base.OnStartShow();

            _scenes = SceneSo.GetScenesFromId(SceneID.Levels);
            SetCurrentLevel();
            SetStarAmount();
            LoadCurrentIndexLastScene();
        }

        private void LoadCurrentIndexLastScene()
        {
            UnityAction afterLoad = () =>
            {
                mainMenuToggleObject.ToggleObjects();
            };
            
            gameManager.sceneLoadManager.LoadScene(SceneID.Levels, currentLevelIndex, afterLoad:afterLoad);
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


        public SceneSo SceneSo => gameManager.saveManager.sceneSo;
    }
}