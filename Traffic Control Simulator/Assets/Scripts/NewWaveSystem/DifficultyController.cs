using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultyController : MonoBehaviour
{
    [Header("Stages")]
    [SerializeField] private List<StageConfig> _stages;

    [Header("Timer")]
    [SerializeField] private float _startTime = 300f; // стартовое время (в секундах)

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _timeText;

    private float _time;
    private int _currentStageIndex;
    private bool _isRunning = true;

    public StageConfig CurrentStage => _stages[_currentStageIndex];

    private void Start()
    {
        _time = _startTime;
    }

    private void Update()
    {
        if (!_isRunning)
            return;

        if (_time <= 0f)
        {
            _time = 0f;
            _isRunning = false;
            OnTimerEnd();
            return;
        }

        _time -= Time.deltaTime;

        UpdateStage();
        UpdateTimeUI();
    }

    private void UpdateStage()
    {
        if (_currentStageIndex >= _stages.Count - 1)
            return;

        // Переход на следующую стадию при уменьшении времени
        if (_time <= _stages[_currentStageIndex + 1].StartTime)
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

    private void OnTimerEnd()
    {
        Debug.Log("Timer ended!");
        // TODO: сюда можно добавить Game Over / победу / остановку игры
    }

    // --- Дополнительно ---

    public void AddTime(float value)
    {
        _time += value;
    }

    public void PauseTimer()
    {
        _isRunning = false;
    }

    public void ResumeTimer()
    {
        if (_time > 0f)
            _isRunning = true;
    }

    public void ResetTimer()
    {
        _time = _startTime;
        _currentStageIndex = 0;
        _isRunning = true;
    }
}
