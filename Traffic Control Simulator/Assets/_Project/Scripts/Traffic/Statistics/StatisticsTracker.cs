using UnityEngine;

public class StatisticsTracker : MonoBehaviour
{
        [SerializeField] private TrafficGameSettings settings;

    public static StatisticsTracker Instance;

    public CityStatistics Statistics = new();


    private void Awake()
    {
        Instance = this;
    }


    public void AddCarPassed()
    {
        Statistics.CarsPassed++;
    }


    public void AddAccident()
    {
        Statistics.Accidents++;
    }


    public void CalculateResult()
    {
        Statistics.Score =
            Statistics.CarsPassed - (Statistics.Accidents * (settings != null ? settings.ScorePenaltyPerAccident : 50));
    }


    public void ResetStatistics()
    {
        Statistics = new CityStatistics();
    }
}
