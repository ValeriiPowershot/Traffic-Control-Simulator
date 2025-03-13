using UnityEngine;
using UnityEngine.UI;

namespace BaseCode.Logic.ScoringSystem.Vehicle
{
    public class ScoreObjectCarWithTime : ScoreObjectCarBase
    {
        public override void Calculate(float deltaTime)
        {
            CalculateResult(deltaTime);
            SetNewTimeImageWeight();
        }
    
        private void SetNewTimeImageWeight()
        {
            float t = Mathf.InverseLerp(0, VehicleSo.acceptableWaitingTime, TotalWaitingTime);
            scoreMaterialsComponent.indicatorOfScore.fillAmount = Mathf.Lerp(1f,0f, t);
        }

        protected override void CalculateResult(float deltaTime)
        {
            CurrentScore = CalculateScore(deltaTime); 
        }
        
    }
}