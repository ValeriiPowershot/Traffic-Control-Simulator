using UnityEngine;
using UnityEngine.UI;

namespace BaseCode.Logic.ScoringSystem.Vehicle
{
    public class ScoreObjectCarWithTime : ScoreObjectCarBase
    {
        public override void Calculate(float deltaTime)
        {
            base.Calculate(deltaTime);
            SetNewTimeImageWeight();
        }
    
        private void SetNewTimeImageWeight()
        {
            var currentColor = scoreMaterialsComponent.indicatorOfScore.color.grayscale;
            
            float t = Mathf.InverseLerp(scoreMaterialsComponent.bad.grayscale, 
                scoreMaterialsComponent.good.grayscale, currentColor);
            
            scoreMaterialsComponent.indicatorOfScore.fillAmount = Mathf.Lerp(0f, 1f, t);
        }

        protected override void CalculateResult()
        {
            CurrentScore = CalculateScore(); 
        }
    }
}