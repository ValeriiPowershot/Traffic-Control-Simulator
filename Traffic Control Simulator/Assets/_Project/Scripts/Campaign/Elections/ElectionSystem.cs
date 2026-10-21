using UnityEngine;

namespace BaseCode.Logic.City
{
    public class ElectionSystem : MonoBehaviour
    {
        [SerializeField] private TrafficGameSettings settings;

        [SerializeField]
        private int requiredReputation = 60;


        public bool CheckElection(int reputation)
        {
            return reputation >= (settings != null ? settings.ElectionThreshold : requiredReputation);
        }
    }
}
