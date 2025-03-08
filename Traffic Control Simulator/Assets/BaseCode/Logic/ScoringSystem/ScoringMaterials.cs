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
        public Color bad;
        
        public void SetNewMaterial(Color color)
        {
            indicatorOfScore.color = color;
        }
    }
}
