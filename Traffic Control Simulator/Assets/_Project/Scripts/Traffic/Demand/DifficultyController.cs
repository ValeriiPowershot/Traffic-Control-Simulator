using System.Collections.Generic;
using BaseCode.Logic.City;
using UnityEngine;
using TMPro;

public class DifficultyController : MonoBehaviour
{
    [SerializeField] private TrafficGameSettings settings;
    [SerializeField] private List<StageConfig> _stages;
    [SerializeField] private float _startTime = 300f;
    [SerializeField] private TextMeshProUGUI _timeText;
    private float _time;
    private int _currentStageIndex;
    private bool _isRunning;
    public float RemainingSeconds => _time;
    public StageConfig CurrentStage => _stages == null || _stages.Count == 0 ? null : _stages[Mathf.Clamp(_currentStageIndex, 0, _stages.Count - 1)];
    private void Start() { ResetTimer(); }
    private void Update()
    {
        var flow = GameManager.ForScene(gameObject.scene);
        if (!_isRunning || (flow != null && !flow.IsPlaying)) return;
        _time = Mathf.Max(0f, _time - Time.deltaTime);
        if (_stages != null)
            while (_currentStageIndex + 1 < _stages.Count)
            {
                var next = _stages[_currentStageIndex + 1];
                if (next != null && _time > next.StartTime) break;
                _currentStageIndex++;
            }
        UpdateTimeUI();
        if (_time <= 0f)
        {
            _isRunning = false;
            if (flow != null) flow.FinishGame();
            else Debug.LogWarning("Timer ended outside a campaign scene.", this);
        }
    }
    private void UpdateTimeUI()
    { if (_timeText != null) _timeText.text = $"{Mathf.FloorToInt(_time / 60f):00}:{Mathf.FloorToInt(_time % 60f):00}"; }
    public void AddTime(float seconds) { _time = Mathf.Max(0f, _time + seconds); UpdateTimeUI(); }
    public void PauseTimer() { _isRunning = false; }
    public void ResumeTimer() { if (_time > 0) _isRunning = true; }
    public void ResetTimer()
    { _currentStageIndex = 0; _time = settings != null ? Mathf.Max(1f, settings.DayDurationSeconds) : Mathf.Max(1f, _startTime); _isRunning = true; UpdateTimeUI(); }
}
