using UnityEngine;

namespace BaseCode.Logic.City
{
    public class ElectionSystem : MonoBehaviour
    {
        [SerializeField]
        private int requiredReputation = 60;


        public bool CheckElection(int reputation)
        {
            return reputation >= requiredReputation;
        }
    }
}
