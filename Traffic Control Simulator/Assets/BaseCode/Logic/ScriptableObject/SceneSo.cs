using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaseCode.Logic.ScriptableObject
{
    public enum SceneID
    {
        Splash,
        Loading,
        MainMenu,
        Levels
    }

    [CreateAssetMenu(fileName = "SceneSo", menuName = "So/SceneSo", order = 0)]
    public class SceneSo : UnityEngine.ScriptableObject
    {
        public List<SceneIDData> scenes = new List<SceneIDData>();
        
        public SceneIDData GetScenesFromId(SceneID id)
        {
            foreach (var sceneData in scenes)
            {
                if (sceneData.id == id)
                {
                    return sceneData;
                }
            }
            Debug.Log("Scene Not Found!");
            return null;
        }

        public void UnlockScene(int nextLevelIndex)
        {
            var sceneId = GetScenesFromId(SceneID.Levels);
            sceneId.sceneNames[nextLevelIndex].IsUnLocked = true;
        }
    }
    
    [System.Serializable] // Make the inner class serializable
    public class SceneIDData
    {
        public SceneID id;
        public SceneAsset sceneAsset;
        public List<SceneLocalData> sceneNames = new List<SceneLocalData>();
    }
    
    [Serializable]
    public class SceneLocalData
    {
        public int AmountOfStar;
        public bool IsUnLocked;
    }
}
