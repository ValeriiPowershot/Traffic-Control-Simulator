using System;
using System.IO;
using System.Linq;
using BaseCode.Logic.City;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TrafficControl.EditorTools
{
    public sealed class TrafficWorkflowTools : EditorWindow
    {
        private SaveSystem save;
        private DayManager day;
        private StatisticsTracker stats;
        private ElectionSystem elections;
        private int finishedDay = -1;
        private int finishedSaveId;
        private string electionResult = "Not checked";

        [MenuItem("Traffic Control/Debug Panel")]
        private static void ShowPanel() => GetWindow<TrafficWorkflowTools>("Traffic Debug");
        [MenuItem("Traffic Control/Select/Game Settings")]
        private static void Settings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/_Project/Data/Production/GameSettings.asset");
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
        [MenuItem("Traffic Control/Create Level From Template")]
        private static void CreateLevel()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            const string template = "Assets/_Project/Scenes/Templates/LevelTemplate.unity";
            if (!File.Exists(template)) { Debug.LogError("Level template is missing."); return; }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string path = EditorUtility.SaveFilePanelInProject("Create level", "Level_New", "unity", "Choose a new level scene.", "Assets/_Project/Scenes/Production/Game");
            if (string.IsNullOrEmpty(path)) return;
            if (File.Exists(path)) { Debug.LogError("Choose a new filename. Existing scenes are never replaced."); return; }
            if (!AssetDatabase.CopyAsset(template, path)) { Debug.LogError("Could not copy template."); return; }
            AssetDatabase.ImportAsset(path);
            EditorSceneManager.OpenScene(path);
            Debug.Log("Created " + path + ". Configure routes and demand before adding it to Build Settings.");
        }
        private static T Unique<T>() where T : Component
        {
            var objects = Resources.FindObjectsOfTypeAll<T>().Where(x => x.gameObject.scene.IsValid() && x.gameObject.scene.isLoaded && !EditorUtility.IsPersistent(x)).ToArray();
            return objects.Length == 1 ? objects[0] : null;
        }
        private void OnInspectorUpdate() { if (EditorApplication.isPlaying) Repaint(); }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Runtime diagnostics", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Controls are available only in Play Mode. Select systems explicitly if multiple copies are loaded. Finish day writes the current save after confirmation. Changes to settings assets persist after Play Mode.", MessageType.Info);
            if (!EditorApplication.isPlaying)
            { save = null; day = null; stats = null; elections = null; finishedDay = -1; EditorGUILayout.LabelField("Enter Play Mode to inspect the session."); return; }
            if (save == null) save = Unique<SaveSystem>();
            if (day == null) day = Unique<DayManager>();
            if (stats == null) stats = Unique<StatisticsTracker>();
            if (elections == null) elections = Unique<ElectionSystem>();
            save = (SaveSystem)EditorGUILayout.ObjectField("Save", save, typeof(SaveSystem), true);
            day = (DayManager)EditorGUILayout.ObjectField("Day manager", day, typeof(DayManager), true);
            stats = (StatisticsTracker)EditorGUILayout.ObjectField("Statistics", stats, typeof(StatisticsTracker), true);
            elections = (ElectionSystem)EditorGUILayout.ObjectField("Elections", elections, typeof(ElectionSystem), true);
            int cars = Resources.FindObjectsOfTypeAll<Realistic_Traffic_Controller.Scripts.RTC_CarController>()
                .Count(x => x.gameObject.scene.IsValid() && x.gameObject.scene.isLoaded && x.gameObject.activeInHierarchy && !EditorUtility.IsPersistent(x));
            EditorGUILayout.LabelField("Active RTC cars", cars.ToString());
            var game = Unique<GameManager>();
            EditorGUILayout.LabelField("Game state", game != null ? game.CurrentState.ToString() : "No unique GameManager");
            if (save != null && save.Data != null)
            {
                EditorGUILayout.LabelField("Day", save.Data.CurrentDay.ToString());
                EditorGUILayout.LabelField("Reputation", save.Data.Reputation.ToString());
            }
            if (stats != null)
            {
                EditorGUILayout.LabelField("Cars passed", stats.Statistics.CarsPassed.ToString());
                EditorGUILayout.LabelField("Accidents", stats.Statistics.Accidents.ToString());
                EditorGUILayout.LabelField("Score", stats.Statistics.Score.ToString());
            }
            using (new EditorGUI.DisabledScope(save == null || save.Data == null || elections == null))
                if (GUILayout.Button("Preview election result")) electionResult = elections.CheckElection(save.Data.Reputation) ? "Re-elected" : "Not re-elected";
            EditorGUILayout.LabelField("Election preview", electionResult);
            bool canFinish = save != null && save.Data != null && day != null && stats != null;
            if (canFinish)
            {
                var dayData = new SerializedObject(day);
                canFinish = dayData.FindProperty("saveSystem").objectReferenceValue == save;
                if (!canFinish) EditorGUILayout.HelpBox("Selected Save must match DayManager's SaveSystem reference.", MessageType.Warning);
            }
            if (canFinish && finishedSaveId == save.GetInstanceID() && save.Data.CurrentDay == finishedDay) canFinish = false;
            using (new EditorGUI.DisabledScope(!canFinish))
            {
                if (GUILayout.Button("Finish day (writes save)"))
                {
                    if (EditorUtility.DisplayDialog("Finish debug day", "This calculates the current statistics, updates reputation, advances the day and writes save.json. Continue?", "Finish day", "Cancel"))
                    {
                        stats.CalculateResult();
                        day.FinishDay(stats.Statistics);
                        finishedSaveId = save.GetInstanceID(); finishedDay = save.Data.CurrentDay;
                        if (game != null) game.CurrentState = GameState.Results;
                        foreach (var spawner in Resources.FindObjectsOfTypeAll<BaseCode.CarSpawnerSystem>())
                            if (spawner.gameObject.scene.IsValid() && spawner.gameObject.scene.isLoaded) spawner.StopAllCoroutines();
                        foreach (var timer in Resources.FindObjectsOfTypeAll<DifficultyController>())
                            if (timer.gameObject.scene.IsValid() && timer.gameObject.scene.isLoaded) timer.PauseTimer();
                        Debug.Log("Debug day saved. Restart Play Mode for a fresh session.");
                        EditorApplication.isPaused = true;
                    }
                }
            }
        }
    }
}
