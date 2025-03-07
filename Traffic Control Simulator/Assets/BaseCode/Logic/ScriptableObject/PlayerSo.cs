using UnityEngine;

namespace BaseCode.Logic.ScriptableObject
{
    [CreateAssetMenu(fileName = "PlayerSo", menuName = "So/PlayerSo", order = 0)]
    public class PlayerSo : UnityEngine.ScriptableObject
    {
        public float playerScore;
        
    }
}