using BaseCode.Logic.ScoringSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic
{
    public class GameManager : MonoBehaviour
    {
        public CarManager carManager;
        public FxManager fxManager;
        public InputManager inputManager;
        public ScoringManager scoringManager;
    }
}