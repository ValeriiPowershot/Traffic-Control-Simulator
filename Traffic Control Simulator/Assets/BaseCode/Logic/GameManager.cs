using UnityEngine;

namespace BaseCode.Logic
{
    public class GameManager : SingletonManagerBase<GameManager>
    {
        public CarManager carManager;
        public FxManager fxManager;
        public InputManager inputManager;
        public CameraManager cameraManager;
        public ScoringManager scoringManager;
        public SaveManager saveManager;
        public VfxManager vfxManager;
        public SceneLoadManager sceneLoadManager;
        public PopUpManager popUpManager;


        public void ExitGame()
        {
            Debug.Log("Exit Game");
        }
    }
}