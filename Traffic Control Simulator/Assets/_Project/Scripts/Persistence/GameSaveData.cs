using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int Version = 1;
    public int CurrentDay = 1;
    public int Reputation = 50;
    public int LastCompletedDay;
    public bool CampaignComplete;
    public bool ElectionWon;
    public List<DayResult> Results = new List<DayResult>();
}

[Serializable]
public class DayResult
{
    public int Day;
    public int CarsPassed;
    public int Accidents;
    public int Score;
    public int ReputationChange;
    public float AverageWaitingTime;
}
