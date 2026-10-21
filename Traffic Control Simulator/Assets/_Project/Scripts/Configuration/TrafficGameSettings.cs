using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Traffic Control/Game Settings")]
public sealed class TrafficGameSettings : ScriptableObject
{
    [Header("Day")]
    [Min(1f), Tooltip("Duration of a shift in seconds.")]
    public float DayDurationSeconds = 300f;
    [Header("Traffic")]
    [Min(0.1f), Tooltip("Multiplies each spawner's existing interval. Larger means less traffic.")]
    public float SpawnIntervalMultiplier = 1f;
    [Min(0), Tooltip("Override the no-stage car limit. Zero preserves each spawner's current limit.")]
    public int FallbackCarLimit = 0;
    [Header("Campaign")]
    [Range(0, 100), Tooltip("Used for a new save, not an existing campaign.")]
    public int StartingReputation = 50;
    [Range(0, 100)] public int ElectionThreshold = 60;
    [Header("Current scoring model")]
    [Min(1)] public int ScorePerReputationPoint = 100;
    [Min(0)] public int ReputationPenaltyPerAccident = 5;
    [Min(0)] public int ScorePenaltyPerAccident = 50;

    private void OnValidate()
    {
        DayDurationSeconds = Mathf.Max(1f, DayDurationSeconds);
        SpawnIntervalMultiplier = Mathf.Max(0.1f, SpawnIntervalMultiplier);
        FallbackCarLimit = Mathf.Max(0, FallbackCarLimit);
        ScorePerReputationPoint = Mathf.Max(1, ScorePerReputationPoint);
        ReputationPenaltyPerAccident = Mathf.Max(0, ReputationPenaltyPerAccident);
        ScorePenaltyPerAccident = Mathf.Max(0, ScorePenaltyPerAccident);
    }
}
