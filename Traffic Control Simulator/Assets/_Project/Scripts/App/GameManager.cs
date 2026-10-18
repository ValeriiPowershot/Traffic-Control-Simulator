using UnityEngine;

namespace BaseCode.Logic.City
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GameState CurrentState;

        private void Awake()
        {
            Instance = this;
        }

        public void StartGame()
        {
            CurrentState = GameState.Playing;
        }

        public void FinishGame()
        {
            CurrentState = GameState.Results;

            StatisticsTracker.Instance.CalculateResult();
        }
    }
}
