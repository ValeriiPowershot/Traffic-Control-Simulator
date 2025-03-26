using BaseCode.Core;
using BaseCode.Logic.PopUps;
using UnityEngine;

namespace BaseCode.Logic
{
    public class GameManager : Singleton<GameManager>
    {
        public CarManager carManager;
        public FxManager fxManager;
        public InputManager inputManager;
        public CameraManager cameraManager;
        public ScoringManager scoringManager;
        public SaveManager saveManager;
        public VfxManager vfxManager;
        public SceneLoadManager sceneLoadManager;
        public PopUpController popUpController;

        private void Start()
        {
            popUpController.ShowPopUp<PopUpMainMenu>();
        }

        public void ExitGame()
        {
            Debug.Log("Exit Game");
        }
    }
}