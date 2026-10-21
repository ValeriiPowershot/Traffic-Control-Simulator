using UnityEngine;

public class StatisticsTracker : MonoBehaviour
{
    [SerializeField] private TrafficGameSettings settings;
    public static StatisticsTracker Instance;
    public CityStatistics Statistics = new CityStatistics();
    private int trackedVehicles;
    private float stoppedSeconds;
    private void Awake() { Instance = this; }
    private void OnDestroy() { if (Instance == this) Instance = null; }
    public void Configure(TrafficGameSettings configuration) { settings = configuration; ResetStatistics(); }
    public void RegisterVehicle() { trackedVehicles++; }
    public void AddStoppedTime(float seconds)
    { stoppedSeconds += Mathf.Max(0f, seconds); Statistics.AverageWaitingTime = stoppedSeconds / Mathf.Max(1, trackedVehicles); }
    public void AddCarPassed() { Statistics.CarsPassed++; CalculateResult(); }
    public void AddAccident() { Statistics.Accidents++; CalculateResult(); }
    public void CalculateResult()
    { Statistics.Score = Statistics.CarsPassed - Statistics.Accidents * (settings != null ? settings.ScorePenaltyPerAccident : 50); }
    public void ResetStatistics() { Statistics = new CityStatistics(); stoppedSeconds = 0; trackedVehicles = 0; }
}
