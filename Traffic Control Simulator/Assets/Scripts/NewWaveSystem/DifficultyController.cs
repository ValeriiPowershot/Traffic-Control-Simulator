using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultyController : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stages;
    [SerializeField] private TextMeshProUGUI _timeText;

    private float _time;
    private int _currentStageIndex;

    public StageConfig CurrentStage => _stages[_currentStageIndex];

    private void Update()
    {
        _time += Time.deltaTime;

        UpdateStage();
        UpdateTimeUI();
    }

    private void UpdateStage()
    {
        if (_currentStageIndex >= _stages.Count - 1)
            return;

        if (_time >= _stages[_currentStageIndex + 1].StartTime)
        {
            _currentStageIndex++;
        }
    }

    private void UpdateTimeUI()
    {
        if (_timeText == null)
            return;

        int minutes = Mathf.FloorToInt(_time / 60f);
        int seconds = Mathf.FloorToInt(_time % 60f);

        _timeText.text = $"{minutes:00}:{seconds:00}";
    }
}
