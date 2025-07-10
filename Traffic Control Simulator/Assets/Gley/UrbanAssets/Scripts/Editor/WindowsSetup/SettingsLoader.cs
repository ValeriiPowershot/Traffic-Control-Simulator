using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class SettingsLoader
    {
        protected string path;


        public SettingsLoader(string path)
        {
            this.path = path;
        }


        public T LoadSettingsAsset<T>() where T : SettingsWindowData
        {
            T settingsWindowData = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));

            if (settingsWindowData == null)
            {
                SettingsWindowData asset = ScriptableObject.CreateInstance<T>().Initialize();
                string[] pathFolders = path.Split('/');
                string tempPath = pathFolders[0];
                for (int i = 1; i < pathFolders.Length - 1; i++)
                {
                    if (!AssetDatabase.IsValidFolder(tempPath + "/" + pathFolders[i]))
                    {
                        AssetDatabase.CreateFolder(tempPath, pathFolders[i]);
                        AssetDatabase.Refresh();
                    }

                    tempPath += "/" + pathFolders[i];
                }

                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                settingsWindowData = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
            }

            return settingsWindowData;
        }


       
    }
}