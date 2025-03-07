using System;
using UnityEngine;
using UnityEngine.UI;

namespace BaseCode.Logic.ScoringSystem
{
    [Serializable]
    public class ScoringMaterials
    {
        public Image indicatorOfScore;

        public Color good;
        public Color neutral;
        public Color bad;
        
        public void SetNewMaterial(ScoreType scoreType)
        {
            switch (scoreType)
            {
                case ScoreType.Good:
                    indicatorOfScore.color = good;
                    break;
                case ScoreType.Neuteral:
                    indicatorOfScore.color = neutral;
                    break;
                case ScoreType.Bad:
                    indicatorOfScore.color = bad;
                    break;
            }
        }
    }
}
