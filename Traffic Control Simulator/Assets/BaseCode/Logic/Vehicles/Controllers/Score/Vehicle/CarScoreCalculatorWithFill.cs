using System.Collections;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Score.Vehicle
{
    public class CarScoreCalculatorWithFill : CarScoreCalculatorBase
    {
        public override void Calculate(float deltaTime)
        {
            CalculateResult(deltaTime);
            StartCoroutine(SmoothFillTransition(0.5f)); 
        }
        
        private IEnumerator SmoothFillTransition(float duration)
        {
            float startFill = scoreMaterialsComponent.indicatorOfScore.fillAmount;
            float targetFill = Mathf.InverseLerp(AcceptableWaitingTime,0, TotalWaitingTime);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                scoreMaterialsComponent.indicatorOfScore.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime / duration);
                yield return null; 
            }
            scoreMaterialsComponent.indicatorOfScore.fillAmount = targetFill; // Ensure final value is set
        }

        protected override void CalculateResult(float deltaTime)
        {
            CurrentScore = CalculateScore(deltaTime); 
        }
    }
}