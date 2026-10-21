using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrafficControl.EditorTools
{
    public sealed class TrafficProjectTools : EditorWindow
    {
        private const string Root = "Assets/_Project/";
        private readonly List<Finding> findings = new List<Finding>();
        private Vector2 scroll;
        private bool scanned;
        private sealed class Finding
        {
            public string Message;
            public string Path;
            public UnityEngine.Object Context;
        }

        [MenuItem("Traffic Control/Open/Bootstrap")]
        private static void Bootstrap() => OpenScene(Root + "Scenes/Bootstrap/BootstrapScene.unity");
        [MenuItem("Traffic Control/Open/Main Menu")]
        private static void MainMenu() => OpenScene(Root + "Scenes/Menu/MainMenuScene.unity");
        [MenuItem("Traffic Control/Open/Level 1")]
        private static void LevelOne() => OpenScene(Root + "Scenes/Game/Level1.unity");
        [MenuItem("Traffic Control/Select/Traffic Scripts")]
        private static void Traffic() => SelectPath(Root + "Scripts/Traffic");
        [MenuItem("Traffic Control/Select/Campaign Scripts")]
        private static void Campaign() => SelectPath(Root + "Scripts/Campaign");
        [MenuItem("Traffic Control/Select/Game Data")]
        private static void Data() => SelectPath(Root + "Data");
        [MenuItem("Traffic Control/Project Check")]
        private static void ShowCheck() => GetWindow<TrafficProjectTools>("Traffic Project Check");

        private static void SelectPath(string path)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            if (asset == null) { Debug.LogWarning("Not found: " + path); return; }
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
        private static void OpenScene(string path)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            { Debug.LogWarning("Stop Play Mode before opening another scene."); return; }
            if (!File.Exists(path)) { Debug.LogError("Scene not found: " + path); return; }
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(path);
        }
        private void Add(string message, string path, UnityEngine.Object context = null)
            => findings.Add(new Finding { Message = message, Path = path, Context = context });
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Traffic Control project check", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Read-only checks: metadata, merge markers, source type names, build scene paths, and components in currently loaded scenes. Prefab assets and unopened scenes are not inspected for missing components. No automatic fixes.", MessageType.Info);
            if (GUILayout.Button("Run checks")) Scan();
            if (scanned) EditorGUILayout.LabelField(findings.Count + " findings");
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var item in findings)
            {
                EditorGUILayout.HelpBox(item.Message + "\n" + item.Path, MessageType.Warning);
                if (GUILayout.Button("Locate", GUILayout.Width(90)))
                {
                    if (item.Context != null) { Selection.activeObject = item.Context; EditorGUIUtility.PingObject(item.Context); }
                    else SelectPath(item.Path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)
                        ? item.Path.Substring(0, item.Path.Length - 5) : item.Path);
                }
            }
            EditorGUILayout.EndScrollView();
        }
        private void Scan()
        {
            findings.Clear(); scanned = true;
            var guids = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var types = new Dictionary<string, string>();
            var absoluteRoot = Directory.GetParent(Application.dataPath).FullName;
            var paths = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            try
            {
                for (int index = 0; index < paths.Length; index++)
                {
                    if (index % 100 == 0 && EditorUtility.DisplayCancelableProgressBar("Traffic Control", "Checking assets", (float)index / paths.Length))
                    { Add("Scan cancelled. Results are incomplete.", "Assets"); return; }
                    var file = paths[index];
                    var path = file.Substring(absoluteRoot.Length + 1).Replace('\\', '/');
                    // Unity ignores hidden files such as macOS ._ resource forks.
                    if (path.Split('/').Any(part => part.StartsWith(".", StringComparison.Ordinal))) continue;
                    if (file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                    {
                        string content = File.ReadAllText(file);
                        if (Regex.IsMatch(content, @"(?m)^(<{7,}|={7,}|>{7,})")) Add("Unresolved merge markers", path);
                        var matches = Regex.Matches(content, @"(?m)^guid: ([a-fA-F0-9]{32})\s*$");
                        if (matches.Count != 1) Add("Metadata must contain exactly one valid GUID", path);
                        else
                        {
                            string guid = matches[0].Groups[1].Value;
                            if (guids.TryGetValue(guid, out string previous)) Add("Duplicate GUID also used by " + previous, path);
                            else guids.Add(guid, path);
                        }
                        string owner = file.Substring(0, file.Length - 5);
                        if (!File.Exists(owner) && !Directory.Exists(owner)) Add("Metadata has no matching asset", path);
                        continue;
                    }
                    if (!File.Exists(file + ".meta")) Add("Asset has no metadata", path);
                    if (path.StartsWith(Root, StringComparison.Ordinal) && file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    {
                        string content = File.ReadAllText(file);
                        if (Regex.IsMatch(content, @"(?m)^(<{7,}|={7,}|>{7,})")) Add("Unresolved source merge markers", path);
                        // A source heuristic, not a C# parser. Partial types and conditional builds need manual review.
                        string ns = Regex.Match(content, @"\bnamespace\s+([\w.]+)").Groups[1].Value;
                        foreach (Match match in Regex.Matches(content, @"(?m)^\s*public\s+(?:(?:sealed|abstract|static|partial)\s+)*class\s+(\w+)"))
                        {
                            string key = ns + "." + match.Groups[1].Value;
                            if (types.TryGetValue(key, out string previous)) Add("Possible repeated type " + key + " in " + previous + ". Review partial classes and compilation conditions.", path);
                            else types.Add(key, path);
                        }
                    }
                }
                foreach (var scene in EditorBuildSettings.scenes.Where(scene => scene.enabled))
                    if (!File.Exists(scene.path)) Add("Enabled build scene is missing", scene.path);
                for (int index = 0; index < SceneManager.sceneCount; index++)
                {
                    var scene = SceneManager.GetSceneAt(index);
                    if (!scene.isLoaded) continue;
                    foreach (var root in scene.GetRootGameObjects())
                    foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                    {
                        var go = transform.gameObject;
                        if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                            Add("Missing script on " + go.name, scene.path, go);
                        foreach (var component in go.GetComponents<MonoBehaviour>())
                            if (component != null && (component.GetType().Name == "SystemsTester" || component.GetType().Name == "SaveTester"))
                                Add("Manual test component in loaded scene: " + component.GetType().Name, scene.path, go);
                    }
                }
            }
            catch (Exception exception) { Add("Check could not finish: " + exception.Message, "Assets"); }
            finally { EditorUtility.ClearProgressBar(); Repaint(); }
        }
    }
}
