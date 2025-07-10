#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    public class DebugOptions
    {
        public static DebugSettings LoadOrCreateDebugSettings()
        {
            return LoadOrCreateDataAsset<DebugSettings>(GetRootFolder(Constants.folderName, Constants.parentFolder), Constants.DebugOptionsPath, Constants.DebugOptionsName);
        }


        public static bool GetDebug()
        {
            DebugSettings debugSettings = LoadOrCreateDebugSettings();
            return debugSettings.debug;
        }


        public static bool GetSpeedDebug()
        {
            DebugSettings debugSettings = LoadOrCreateDebugSettings();
            return debugSettings.debugSpeed;
        }


        public static bool GetIntersectionDebug()
        {
            DebugSettings debugSettings = LoadOrCreateDebugSettings();
            return debugSettings.debugIntersections;
        }


        public static bool GetWaypointsDebug()
        {
            DebugSettings debugSettings = LoadOrCreateDebugSettings();
            return debugSettings.debugWaypoints;
        }


        private static T LoadOrCreateDataAsset<T>(string rootFolder, string path, string name) where T : ScriptableObject
        {
            string assetPath = ($"{rootFolder}/{path}/{name}.asset");
            T result = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (result == null)
            {
                T asset = ScriptableObject.CreateInstance<T>();
                CreateFolder($"{rootFolder}/{path}");
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            return result;
        }


        private static void CreateFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string[] folders = path.Split('/');
                string tempPath = "";
                for (int i = 0; i < folders.Length - 1; i++)
                {
                    tempPath += folders[i];
                    if (!AssetDatabase.IsValidFolder(tempPath + "/" + folders[i + 1]))
                    {
                        AssetDatabase.CreateFolder(tempPath, folders[i + 1]);
                        AssetDatabase.Refresh();
                    }
                    tempPath += "/";
                }
            }
        }


        private static string GetRootFolder(string folderName, string parentFolder)
        {
            string rootFolder = FindFolder(folderName, parentFolder);
            if (rootFolder == null)
            {
                throw new Exception($"Folder Not Found: '{parentFolder}/{folderName}'");
            }
            return rootFolder;
        }


        private static string FindFolder(string folderName, string parent)
        {
            string result = null;
            var folders = AssetDatabase.GetSubFolders("Assets");
            foreach (var folder in folders)
            {
                result = Recursive(folder, folderName, parent);
                if (result != null)
                {
                    return result;
                }
            }
            return result;
        }


        private static string Recursive(string currentFolder, string folderToSearch, string parent)
        {
            if (currentFolder.EndsWith($"{parent}/{folderToSearch}"))
            {
                return currentFolder;
            }
            var folders = AssetDatabase.GetSubFolders(currentFolder);
            foreach (var fld in folders)
            {
                string result = Recursive(fld, folderToSearch, parent);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
#endif