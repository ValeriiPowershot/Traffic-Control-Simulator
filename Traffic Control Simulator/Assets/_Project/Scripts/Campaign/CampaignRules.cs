using System;
using System.Collections.Generic;

// Pure rules: the returned snapshot can be written before replacing live progress.
public static class CampaignRules
{
    public static bool NormalizeLoadedSave(GameSaveData data)
    {
        if (data == null || data.Version < 0 || data.Version > 1 || data.CurrentDay < 1 || data.Reputation < 0 || data.Reputation > 100) return false;
        if (data.Results == null) data.Results = new List<DayResult>();
        bool legacy = data.Version == 0 || data.LastCompletedDay == 0;
        int previous = 0;
        foreach (var result in data.Results)
        {
            if (result == null || result.CarsPassed < 0 || result.Accidents < 0) return false;
            if (legacy && result.Day == 0) result.Day = previous + 1;
            if (result.Day <= previous || result.Day >= data.CurrentDay) return false;
            previous = result.Day;
        }
        if (legacy) data.LastCompletedDay = previous;
        if (data.LastCompletedDay != previous || data.LastCompletedDay >= data.CurrentDay || data.LastCompletedDay < 0) return false;
        if (data.CampaignComplete && data.LastCompletedDay == 0) return false;
        data.Version = 1; return true;
    }
    public static GameSaveData Complete(GameSaveData before, CityStatistics stats, int expectedDay,
        int termDays, int threshold, int scorePenalty, int reputationDivisor, int accidentPenalty)
    {
        if (before == null || stats == null) throw new ArgumentNullException();
        if (before.CampaignComplete || before.CurrentDay != expectedDay || before.LastCompletedDay >= expectedDay)
            throw new InvalidOperationException("This session has already finished or is out of date.");
        if (expectedDay < 1 || termDays < 1) throw new ArgumentOutOfRangeException();
        int cars = Math.Max(0, stats.CarsPassed), accidents = Math.Max(0, stats.Accidents);
        long rawScore = (long)cars - (long)accidents * Math.Max(0, scorePenalty);
        int score = (int)Math.Max(int.MinValue, Math.Min(int.MaxValue, rawScore));
        long change = (long)score / Math.Max(1, reputationDivisor) - (long)accidents * Math.Max(0, accidentPenalty);
        int reputation = (int)Math.Max(0L, Math.Min(100L, before.Reputation + change));
        var result = new DayResult { Day = expectedDay, CarsPassed = cars, Accidents = accidents, Score = score,
            AverageWaitingTime = float.IsNaN(stats.AverageWaitingTime) || float.IsInfinity(stats.AverageWaitingTime) ? 0f : Math.Max(0f, stats.AverageWaitingTime), ReputationChange = reputation - before.Reputation };
        var results = new List<DayResult>();
        if (before.Results != null)
            foreach (var old in before.Results)
                results.Add(new DayResult { Day = old.Day, CarsPassed = old.CarsPassed, Accidents = old.Accidents,
                    Score = old.Score, ReputationChange = old.ReputationChange, AverageWaitingTime = old.AverageWaitingTime });
        results.Add(result);
        return new GameSaveData { CurrentDay = expectedDay + 1, LastCompletedDay = expectedDay, Reputation = reputation,
            CampaignComplete = expectedDay >= termDays, ElectionWon = expectedDay >= termDays && reputation >= threshold, Results = results };
    }
}
