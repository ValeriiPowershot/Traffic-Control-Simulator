using BaseCode.Logic.PopUps;
using UnityEngine;

namespace BaseCode.Logic
{
    public class GameManager : MonoBehaviour
    {
        public CarManager carManager;
        public FxManager fxManager;
        public InputManager inputManager;
        public CameraManager cameraManager;
        public ScoringManager scoringManager;
        public SaveManager saveManager;
        public PopUpController popUpController;
        
        private void Start()
        {
            popUpController.ShowPopUp<GameMenuPopUp>();
        }
    }
}