using BaseCode.Logic.ScriptableObject;
using UnityEngine;

namespace BaseCode.Logic.Managers
{
    public class SaveManager : SingletonManagerBase<SaveManager>
    {
        public PlayerSo playerSo;
        public ConfigSo configSo;
        public SceneSo sceneSo;

        private const string PlayerKey = "PlayerData";
        private const string ConfigKey = "ConfigData";
        private const string SceneKey = "SceneData";

        protected override void Awake()
        {
            base.Awake();
            SaveAll(); 
        }

        public void SaveAll()
        {
            SavePlayer();
            SaveConfig();
            SaveScene();

            PlayerPrefs.Save();
        }
        public void SavePlayer()
        {
            string key = CurrentPlayerKey;
            string json = JsonUtility.ToJson(playerSo);
            PlayerPrefs.SetString(key, json);
        }

        public void SaveConfig()
        {
            string key = CurrentConfigKey;
            string json = JsonUtility.ToJson(configSo);
            PlayerPrefs.SetString(key, json);
        }

        public void SaveScene()
        {
            string key = CurrentSceneKey;
            string json = JsonUtility.ToJson(sceneSo);
            PlayerPrefs.SetString(key, json);
        }
        
        public void LoadAll()
        {
            LoadPlayer();
            LoadConfig();
            LoadScene();
        }
        public void LoadPlayer()
        {
            string key = CurrentPlayerKey;
            if (PlayerPrefs.HasKey(key))
                JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), playerSo);
        }

        public void LoadConfig()
        {
            string key = CurrentConfigKey;
            if (PlayerPrefs.HasKey(key))
                JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), configSo);
        }

        public void LoadScene()
        {
            string key = CurrentSceneKey;
            if (PlayerPrefs.HasKey(key))
                JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), sceneSo);
        }
        public string CurrentPlayerKey => PlayerKey;
        public string CurrentConfigKey => ConfigKey;
        public string CurrentSceneKey => SceneKey;
    }
}