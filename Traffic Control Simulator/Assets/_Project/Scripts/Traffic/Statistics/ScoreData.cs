using System;
using UnityEngine;

namespace BaseCode
{
    [DisallowMultipleComponent]
    public class ScoreData : MonoBehaviour
    {
        public static ScoreData Instance;

        public int Score {get; set; }

        private void Awake() =>
            Instance = this;
    }
}
