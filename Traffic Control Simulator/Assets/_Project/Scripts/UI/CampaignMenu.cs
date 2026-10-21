using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class CampaignMenu : MonoBehaviour
{
    private TrafficGameSettings settings;
    private SaveSystem save;
    private bool confirmNew;
    private string error;
    private Vector2 scroll;
    private readonly List<Canvas> hiddenCanvases = new List<Canvas>();
    public void Configure(TrafficGameSettings configuration) { settings = configuration; }
    private void Start()
    {
        Time.timeScale = 1f;
        save = gameObject.AddComponent<SaveSystem>(); save.Initialize(settings);
        foreach (var root in gameObject.scene.GetRootGameObjects())
            foreach (var canvas in root.GetComponentsInChildren<Canvas>(true))
                if (canvas.enabled) { hiddenCanvases.Add(canvas); canvas.enabled = false; }
    }
    private bool SceneReady(string path)
    {
        if (SceneUtility.GetBuildIndexByScenePath(path) >= 0) return true;
        error = "Сцена не добавлена в Build Settings: " + path; return false;
    }
    private void OnGUI()
    {
        if (settings == null || save == null) return;
        float w = Mathf.Min(570f, Screen.width - 30f), h = Mathf.Min(480f, Screen.height - 30f);
        Rect area = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);
        GUI.Box(area, ""); GUILayout.BeginArea(new Rect(area.x + 25, area.y + 25, w - 50, h - 50));
        var style = new GUIStyle(GUI.skin.label) { fontSize = 19, wordWrap = true };
        scroll = GUILayout.BeginScrollView(scroll);
        GUILayout.Label("Traffic Control Simulator", new GUIStyle(style) { fontSize = 27 });
        GUILayout.Label("Станьте мэром и сохраните доверие жителей до выборов.", style);
        GUILayout.Space(15);
        if (confirmNew)
        {
            GUILayout.Label("Заменить текущее сохранение новой кампанией? Предыдущий файл сохранится как резервная копия.", style);
            if (GUILayout.Button("Начать новую кампанию", GUILayout.Height(42)) && SceneReady(settings.GameplayScenePath))
            { if (save.TryNewGame()) SceneManager.LoadScene(settings.GameplayScenePath); else error = save.LastError; }
            if (GUILayout.Button("Отмена", GUILayout.Height(35))) confirmNew = false;
        }
        else
        {
            if (GUILayout.Button("Новая кампания", GUILayout.Height(45))) confirmNew = true;
            GUI.enabled = save.HasSave;
            if (GUILayout.Button(save.Data.CampaignComplete ? "Результат кампании" : "Продолжить: день " + save.Data.CurrentDay, GUILayout.Height(45)) && SceneReady(settings.GameplayScenePath)) SceneManager.LoadScene(settings.GameplayScenePath);
            GUI.enabled = true;
            if (GUILayout.Button("Обучение", GUILayout.Height(38)) && SceneReady(settings.TutorialScenePath)) SceneManager.LoadScene(settings.TutorialScenePath);
            if (GUILayout.Button("Выбор уровней (старый экран)", GUILayout.Height(35)) && SceneReady(settings.LevelSelectionScenePath)) SceneManager.LoadScene(settings.LevelSelectionScenePath);
        }
        if (!string.IsNullOrEmpty(save.LastError)) GUILayout.Label(save.LastError, style);
        if (!string.IsNullOrEmpty(error)) GUILayout.Label(error, style);
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
    private void OnDestroy() { foreach (var canvas in hiddenCanvases) if (canvas != null) canvas.enabled = true; }
}
