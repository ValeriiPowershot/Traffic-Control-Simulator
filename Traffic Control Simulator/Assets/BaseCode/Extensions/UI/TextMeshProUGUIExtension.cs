using System;
using System.Collections;
using System.Globalization;
using BaseCode.Logic;
using BaseCode.Logic.ScriptableObject;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BaseCode.Extensions.UI
{
    public static class TextMeshProUGUIExtension
    {
        public static void Toggle(this TextMeshProUGUI textMeshProUGUI)
        {
            textMeshProUGUI.gameObject.SetActive(!textMeshProUGUI.isActiveAndEnabled);
        }
        public static void SetText(this TextMeshProUGUI textMeshProUGUI, string text)
        {
            textMeshProUGUI.text = text;
        }
        public static void AnimateScore(this TextMeshProUGUI scoreText, float targetScore, float duration, MonoBehaviour context)
        {
            scoreText.text = "";
            context.StartCoroutine(AnimateScoreCoroutine(scoreText, targetScore, duration));
        }

        private static IEnumerator AnimateScoreCoroutine(TextMeshProUGUI scoreText, float targetScore, float duration)
        {
            float currentScore = 0;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newScore = Mathf.RoundToInt(Mathf.Lerp(currentScore, targetScore, elapsedTime / duration));
                scoreText.text = newScore.ToString(CultureInfo.InvariantCulture);
                yield return null;
            }
            scoreText.text = ((int)targetScore).ToString();
        }
    }
} 