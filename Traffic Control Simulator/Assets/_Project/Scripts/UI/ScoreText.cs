using System;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class ScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private LevelTargetScore _levelTargetScore;

    private void Start()
    {
        UpdateScoreText(0);
    }

    public void UpdateScoreText(int score)
    {
        if (_scoreText != null)
            _scoreText.text = _levelTargetScore != null ? $"{score}/{_levelTargetScore.GetTargetScore()}" : score.ToString();
    }
}
