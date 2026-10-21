using System;
using System.IO;

// Standalone smoke tests for pure campaign rules and real filesystem transactions.
public static class CampaignTests
{
    private static int passed;
    private static void Check(bool condition, string name)
    { if (!condition) throw new Exception(name); Console.WriteLine("PASS " + name); passed++; }
    private static GameSaveData Finish(GameSaveData data, int cars = 300, int accidents = 0, int? day = null, int term = 7)
        => CampaignRules.Complete(data, new CityStatistics { CarsPassed = cars, Accidents = accidents }, day ?? data.CurrentDay, term, 60, 50, 100, 5);
    public static int Main()
    {
        var initial = new GameSaveData();
        var next = Finish(initial, 100, 1);
        Check(next.Reputation == 45 && next.Results[0].Score == 50, "Existing score and reputation formula");
        Check(initial.CurrentDay == 1 && initial.Reputation == 50 && initial.Results.Count == 0, "Calculations never mutate unsaved live progress");
        Check(next.CurrentDay == 2 && next.LastCompletedDay == 1 && next.Results.Count == 1, "Advance and append once");
        bool rejected = false;
        try { Finish(next, day: 1); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected && next.Results.Count == 1, "Reject a duplicate completion from the same session");
        var won = new GameSaveData(); for (int i = 0; i < 7; i++) won = Finish(won);
        Check(won.CampaignComplete && won.ElectionWon && won.Reputation == 71 && won.Results.Count == 7, "Seven-day winning campaign");
        rejected = false; try { Finish(won); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "Completed campaign cannot accrue more rewards");
        var lost = new GameSaveData(); for (int i = 0; i < 7; i++) lost = Finish(lost, 0);
        Check(lost.CampaignComplete && !lost.ElectionWon && lost.Reputation == 50, "Seven-day losing campaign");
        Check(Finish(new GameSaveData { Reputation = 60 }, 0, term: 1).ElectionWon && !Finish(new GameSaveData { Reputation = 59 }, 0, term: 1).ElectionWon, "Election threshold at 59 and 60");
        Check(Finish(new GameSaveData { Reputation = 99 }, 10000).Reputation == 100 && Finish(new GameSaveData { Reputation = 1 }, 0, 100).Reputation == 0, "Reputation bounds");
        var historical = Finish(next);historical.Results[0].Score = 999;
        Check(next.Results[0].Score == 50, "Candidate history does not alias live save history");
        var legacy = new GameSaveData { Version = 0, CurrentDay = 2 };
        legacy.Results.Add(new DayResult { CarsPassed = 10 });
        Check(CampaignRules.NormalizeLoadedSave(legacy) && legacy.Version == 1 && legacy.LastCompletedDay == 1 && legacy.Results[0].Day == 1, "Migrate unversioned save history");
        Check(!CampaignRules.NormalizeLoadedSave(new GameSaveData { Version = 99 }), "Reject future save versions");
        Check(!CampaignRules.NormalizeLoadedSave(new GameSaveData { Reputation = 101 }), "Reject corrupt reputation");
        Check(!CampaignRules.NormalizeLoadedSave(new GameSaveData { CampaignComplete = true }), "Reject campaign outcome without completed days");
        string root = Path.Combine(Path.GetTempPath(), "traffic-campaign-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            string file = Path.Combine(root, "save.json"); string error;
            Check(AtomicSaveFile.TryWrite(file, "v1", out error) && File.ReadAllText(file) == "v1", "Initial save publication");
            Check(AtomicSaveFile.TryWrite(file, "v2", out error) && File.ReadAllText(file) == "v2" && File.ReadAllText(file + ".bak") == "v1", "Atomic replacement retains previous save");
            Directory.CreateDirectory(file + ".tmp");
            Check(!AtomicSaveFile.TryWrite(file, "v3", out error) && !string.IsNullOrEmpty(error), "Write failure is reported");
            Check(File.ReadAllText(file) == "v2" && File.ReadAllText(file + ".bak") == "v1", "Write failure preserves current and backup saves");
        }
        finally { Directory.Delete(root, true); }
        Console.WriteLine(passed + " tests passed"); return 0;
    }
}

namespace UnityEngine {}
