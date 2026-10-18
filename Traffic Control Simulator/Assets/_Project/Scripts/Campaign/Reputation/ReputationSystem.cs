using UnityEngine;

namespace BaseCode.Logic.City
{
    public class ReputationSystem : MonoBehaviour
    {
        [SerializeField]
        private SaveSystem saveSystem;


        public int Reputation => saveSystem.Data.Reputation;


        public void CalculateReputation(CityStatistics stats)
        {
            int change = 0;

            change += stats.Score / 100;
            change -= stats.Accidents * 5;


            saveSystem.Data.Reputation += change;


            saveSystem.Data.Reputation =
                Mathf.Clamp(saveSystem.Data.Reputation, 0, 100);


            Debug.Log($"Reputation changed by {change}. Current: {Reputation}");
        }
    }
}
