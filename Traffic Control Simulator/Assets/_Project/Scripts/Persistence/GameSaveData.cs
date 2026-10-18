using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int CurrentDay;

    public int Reputation;

    public List<DayResult> Results = new();
}


[Serializable]
public class DayResult
{
    public int CarsPassed;

    public int Accidents;

    public int Score;

    public int ReputationChange;
}
