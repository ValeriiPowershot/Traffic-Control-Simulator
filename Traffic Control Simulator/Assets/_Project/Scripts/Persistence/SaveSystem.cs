using System;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] private TrafficGameSettings settings;
    private string savePath;
    private bool invalidSave;
    public GameSaveData Data { get; private set; }
    public string LastError { get; private set; }
    public bool HasSave { get; private set; }
    public bool RecoveredFromBackup { get; private set; }
    private void Awake() { Initialize(settings); }
    public void Initialize(TrafficGameSettings configuration)
    {
        if (configuration != null) settings = configuration;
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Load();
    }
    public void Load()
    {
        LastError = null; HasSave = false; invalidSave = false; RecoveredFromBackup = false;
        GameSaveData loaded;
        if (TryRead(savePath, out loaded)) { Data = loaded; HasSave = true; return; }
        if (TryRead(savePath + ".bak", out loaded))
        { Data = loaded; HasSave = true; RecoveredFromBackup = true; LastError = "Прогресс восстановлен из резервной копии."; return; }
        invalidSave = File.Exists(savePath) || File.Exists(savePath + ".bak");
        Data = Fresh();
        if (invalidSave) LastError = "Не удалось прочитать сохранение. Оно сохранено на диске. Начните новую кампанию, чтобы заменить его.";
    }
    private GameSaveData Fresh() => new GameSaveData { Reputation = settings != null ? settings.StartingReputation : 50 };
    private static bool TryRead(string path, out GameSaveData data)
    {
        data = null;
        try
        {
            if (!File.Exists(path)) return false;
            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json) || !json.Contains("\"CurrentDay\"") || !json.Contains("\"Reputation\"")) return false;
            var candidate = JsonUtility.FromJson<GameSaveData>(json);
            if (!CampaignRules.NormalizeLoadedSave(candidate)) return false;
            data = candidate; return true;
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
        { return false; }
    }
    public bool TryCommit(GameSaveData next, bool replaceInvalid = false)
    {
        if (invalidSave && !replaceInvalid) { LastError = "Сначала явно начните новую кампанию: существующее сохранение повреждено."; return false; }
        try
        {
            string writeError;
            if (next == null || !AtomicSaveFile.TryWrite(savePath, JsonUtility.ToJson(next, true), out writeError))
            { LastError = "Не удалось записать сохранение."; return false; }
            Data = next; HasSave = true; invalidSave = false; LastError = null; return true;
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is NotSupportedException || exception is ArgumentException)
        { LastError = "Не удалось сохранить прогресс: " + exception.Message; Debug.LogError(LastError); return false; }
    }
    public bool TryNewGame() => TryCommit(Fresh(), true);
    public void Save() { TryCommit(Data); }
    public void ResetSave() { TryNewGame(); }
}
