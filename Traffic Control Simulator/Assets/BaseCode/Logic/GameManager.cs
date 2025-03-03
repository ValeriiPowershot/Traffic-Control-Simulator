using BaseCode.Logic.PopUps;
using BaseCode.Logic.ScoringSystem;
using UnityEngine;

namespace BaseCode.Logic
{
    public class GameManager : MonoBehaviour
    {
        public CarManager carManager;
        public FxManager fxManager;
        public InputManager inputManager;
        public ScoringManager scoringManager;
        public PopUpController popUpController;

        private void Start()
        {
            popUpController.ShowPopUp<PopUpMainMenu>();
        }
    }
}