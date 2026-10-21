using UnityEngine;
using System.IO;


public class SaveSystem : MonoBehaviour
{
        [SerializeField] private TrafficGameSettings settings;

    private string savePath;


    public GameSaveData Data { get; private set; }


    private void Awake()
    {
        savePath = Application.persistentDataPath + "/save.json";

        Load();
    }


    public void Save()
    {
        string json = JsonUtility.ToJson(Data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved");
        Debug.Log(json);
    }


    public void Load()
    {
        if(File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            Data = JsonUtility.FromJson<GameSaveData>(json);

            Debug.Log("Save Loaded");
        }
        else
        {
            Data = new GameSaveData();

            Data.CurrentDay = 1;
            Data.Reputation = settings != null ? settings.StartingReputation : 50;

            Debug.Log("New Game Created");
        }
    }


    public void ResetSave()
    {
        Data = new GameSaveData();

        Data.CurrentDay = 1;
        Data.Reputation = settings != null ? settings.StartingReputation : 50;

        Save();
    }
}
