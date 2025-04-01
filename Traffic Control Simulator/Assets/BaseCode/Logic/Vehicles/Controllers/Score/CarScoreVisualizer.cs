using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BaseCode.Logic.Vehicles.Controllers.Score
{
    [Serializable]
    public class CarScoreVisualizer
    {
        public Coroutine ColorTransformationCoroutine;
        private CarScoreCalculatorBase _carScoreCalculatorBase;

        public Image indicatorOfScore;

        public Color good = Color.green;
        public Color bad = Color.red;
        public Color neutral = Color.yellow;

        public void Initialize(CarScoreCalculatorBase carScoreCalculatorBase)
        {
            _carScoreCalculatorBase = carScoreCalculatorBase;
            SetGreen();
        }
        
        public void SetNewMaterial(Color color)
        {
            indicatorOfScore.color = color;
        }
        
        public void CheckAndAssignNewColor()
        {
            if (indicatorOfScore.color == bad)
                return;
            
            if (IsHalfTimeFinished() && !IsEndTimeFinished())
            {
                GreenToYellow();
            }
            else if (IsEndTimeFinished())
            {
                YellowToRed();
            }
        }

        private bool IsEndTimeFinished() => TotalWaitingTime >= AcceptableWaitingTime;

        private bool IsHalfTimeFinished() => TotalWaitingTime >= AcceptableWaitingTime / 2;

        private void YellowToRed()
        {
            if(ColorTransformationCoroutine == null)
            {
                ColorTransformationCoroutine = 
                    _carScoreCalculatorBase.StartCoroutine(
                        TransitionToNewColor(AcceptableWaitingTime / 2, neutral, bad));
            }
        }

        private void GreenToYellow()
        {
            if(ColorTransformationCoroutine == null)
            {
                ColorTransformationCoroutine = 
                    _carScoreCalculatorBase.StartCoroutine(
                        TransitionToNewColor(AcceptableWaitingTime / 2,good, neutral));
            }
        }
        
        private void SetGreen()
        {
            SetNewMaterial(good);
        }

        private IEnumerator TransitionToNewColor(float duration,Color startColor, Color targetColor)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                SetNewMaterial(Color.Lerp(startColor, targetColor, t));
                yield return null;
            }

            SetNewMaterial(targetColor);
            _carScoreCalculatorBase.StopCoroutine(ColorTransformationCoroutine);
            ColorTransformationCoroutine = null;
        }

        public void ResetScoringMaterial()
        {
            SetNewMaterial(good);
            
            if(ColorTransformationCoroutine != null)
                _carScoreCalculatorBase.StopCoroutine(ColorTransformationCoroutine);
            ColorTransformationCoroutine = null;
        }

        public float TotalWaitingTime => _carScoreCalculatorBase.TotalWaitingTime;
        public float AcceptableWaitingTime => _carScoreCalculatorBase.AcceptableWaitingTime;

    }
}
