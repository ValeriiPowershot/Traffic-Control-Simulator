using UnityEngine;

namespace BaseCode.Logic.City
{
    public class DayManager : MonoBehaviour
    {
        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private ReputationSystem reputationSystem;


        public void FinishDay(CityStatistics stats)
        {
            int oldReputation = saveSystem.Data.Reputation;


            reputationSystem.CalculateReputation(stats);


            DayResult result = new DayResult();

            result.CarsPassed = stats.CarsPassed;
            result.Accidents = stats.Accidents;
            result.Score = stats.Score;

            result.ReputationChange =
                saveSystem.Data.Reputation - oldReputation;


            saveSystem.Data.Results.Add(result);


            saveSystem.Data.CurrentDay++;


            saveSystem.Save();


            Debug.Log(
                $"Day finished! Reputation {oldReputation} -> {saveSystem.Data.Reputation}"
            );
        }
    }
}
