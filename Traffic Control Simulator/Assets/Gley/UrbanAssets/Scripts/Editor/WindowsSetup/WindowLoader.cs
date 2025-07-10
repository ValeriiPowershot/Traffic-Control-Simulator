using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Gley.UrbanAssets.Editor
{
    public class WindowLoader : EditorWindow
    {
        internal static T LoadWindow<T>(string PATH, string WINDOW_NAME, int MIN_WIDTH, int MIN_HEIGHT) where T : SettingsWindowBase
        {
            if (!string.IsNullOrEmpty(PATH))
            {
                StreamReader reader = new StreamReader(PATH);
                string longVersion = JsonUtility.FromJson<Gley.Common.AssetVersion>(reader.ReadToEnd()).longVersion;
                T window = (T)GetWindow(typeof(T));
                window.titleContent = new GUIContent(WINDOW_NAME + longVersion);
                window.minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);
                window.Show();
                return (T)Convert.ChangeType(window, typeof(T));
            }
            return null;
        }
    }
}
