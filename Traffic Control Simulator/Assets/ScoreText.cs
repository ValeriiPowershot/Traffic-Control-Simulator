using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class ScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void UpdateScoreText(int score)
    {
        _scoreText.text = $"Score: {score}";
    }
}
