using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.ScriptableObject
{
    [CreateAssetMenu(fileName = "ConfigSo", menuName = "So/ConfigSo", order = 0)]
    public class ConfigSo : UnityEngine.ScriptableObject
    {
        public float updateTime = 0.5f;
        public float penaltyLambda = 0.1f;
        public const string ScoreMessage = "Score: ";

    }
}