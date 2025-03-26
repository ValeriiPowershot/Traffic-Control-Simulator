using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.ScriptableObject
{
    [CreateAssetMenu(fileName = "ConfigSo", menuName = "So/ConfigSo", order = 0)]
    public class ConfigSo : UnityEngine.ScriptableObject
    {
        public float scoreCarUpdateTime = 1.5f;
        public const string ScoreMessage = "Score: ";

        public GameSettings gameSettings;
    }

    [Serializable]
    public class GameSettings
    {
        public bool isNotificationsOn;
        public float soundVolume;
        public float musicVolume;
    }
}