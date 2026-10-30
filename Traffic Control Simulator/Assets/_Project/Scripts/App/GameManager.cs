using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Realistic_Traffic_Controller.Scripts;

namespace BaseCode.Logic.City
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        private static readonly Dictionary<int, GameManager> directors = new Dictionary<int, GameManager>();
        [SerializeField] private TrafficGameSettings settings;
        public GameState CurrentState;
        public bool IsPaused { get; private set; }
        public bool IsPlaying => CurrentState == GameState.Playing && !IsPaused;
        public bool IsTraining => gameObject.scene.name == "Tutorial";
        public TrafficGameSettings Settings => settings;
        public string Error { get; private set; }
        public StatisticsTracker Tracker { get; private set; }
        private SaveSystem save;
        private DayManager days;
        private NewsManager news;
        private bool initialized, saved;
        private int sessionDay;
        private float previousScale;
        private Vector2 scroll;
        private readonly Dictionary<int, DayVehicleTracker> vehicles = new Dictionary<int, DayVehicleTracker>();
        private readonly HashSet<long> collisions = new HashSet<long>();

        public static T FindInScene<T>(Scene scene) where T : Component
        {
            foreach (var root in scene.GetRootGameObjects())
                foreach (var item in root.GetComponentsInChildren<T>(true))
                    if (item.gameObject.activeInHierarchy && (!(item is Behaviour) || ((Behaviour)(object)item).enabled)) return item;
            return null;
        }
        public static GameManager ForScene(Scene scene)
        {
            if (directors.TryGetValue(scene.handle, out var existing) && existing != null && existing.isActiveAndEnabled) return existing;
            var found = FindInScene<GameManager>(scene);
            if (found != null) directors[scene.handle] = found;
            return found;
        }
        private void Awake()
        { Instance = this; directors[gameObject.scene.handle] = this; previousScale = Time.timeScale > 0 ? Time.timeScale : 1f; CurrentState = GameState.Preparing; Time.timeScale = 0f; }
        public void SetConfiguration(TrafficGameSettings configuration) { if (!initialized) settings = configuration; }
        private T Ensure<T>() where T : Component
        { return FindInScene<T>(gameObject.scene) ?? gameObject.AddComponent<T>(); }
        private void Start()
        {
            if (settings == null) { Error = "Не назначен GameSettings. Откройте уровень через Production или шаблон."; return; }
            Tracker = Ensure<StatisticsTracker>(); Tracker.Configure(settings);
            save = Ensure<SaveSystem>(); save.Initialize(settings);
            days = Ensure<DayManager>(); days.Configure(save, settings);
            news = Ensure<NewsManager>();
            Ensure<ElectionSystem>().Configure(settings);
            sessionDay = save.Data.CurrentDay;
            initialized = true;
            if (!IsTraining && save.Data.CampaignComplete) { saved = true; CurrentState = GameState.Finished; }
            else if (!IsTraining && sessionDay > settings.CampaignLengthDays)
                Error = "Сохранение относится к другому сроку кампании. Начните новую кампанию из меню.";
            if (BaseCode.ScoreData.Instance != null) BaseCode.ScoreData.Instance.Score = 0;
        }
        private void Update()
        {
            if (CurrentState == GameState.Playing && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            { IsPaused = !IsPaused; Time.timeScale = IsPaused ? 0f : previousScale; }
        }
        public void StartGame()
        {
            if (!initialized || Error != null || CurrentState != GameState.Preparing) return;
            if (!IsTraining && !save.HasSave && save.LastError != null) { Error = save.LastError; return; }
            Tracker.ResetStatistics(); vehicles.Clear(); collisions.Clear();
            CurrentState = GameState.Playing; IsPaused = false; Time.timeScale = previousScale;
            foreach (var root in gameObject.scene.GetRootGameObjects())
                foreach (var car in root.GetComponentsInChildren<RTC_CarController>(false)) RegisterVehicle(car.gameObject);
        }
        public DayVehicleTracker RegisterVehicle(GameObject car)
        {
            if (car == null || !IsPlaying) return null;
            int id = car.GetInstanceID();
            if (vehicles.TryGetValue(id, out var known) && known != null) return known;
            var token = car.GetComponent<DayVehicleTracker>() ?? car.AddComponent<DayVehicleTracker>();
            token.Initialize(this); vehicles[id] = token; Tracker.RegisterVehicle(); return token;
        }
        public bool RecordExit(GameObject car)
        {
            var token = RegisterVehicle(car);
            if (token == null || !token.TryExit()) return false;
            Tracker.AddCarPassed();
            if (BaseCode.ScoreData.Instance != null) BaseCode.ScoreData.Instance.Score = Tracker.Statistics.Score;
            return true;
        }
        public void RecordCollision(GameObject first, GameObject second)
        {
            if (!IsPlaying || !settings.CountVehicleCollisions || first == null || second == null || first == second) return;
            uint a = unchecked((uint)first.GetInstanceID()), b = unchecked((uint)second.GetInstanceID());
            long key = unchecked((long)(((ulong)System.Math.Min(a, b) << 32) | System.Math.Max(a, b)));
            if (collisions.Add(key)) Tracker.AddAccident();
        }
        public void FinishGame()
        {
            if (!initialized || (CurrentState != GameState.Playing && !(CurrentState == GameState.Results && !saved))) return;
            Time.timeScale = 0f; IsPaused = false; CurrentState = GameState.Results;
            foreach (var root in gameObject.scene.GetRootGameObjects())
            {
                foreach (var spawner in root.GetComponentsInChildren<BaseCode.CarSpawnerSystem>(true)) spawner.StopAllCoroutines();
                foreach (var timer in root.GetComponentsInChildren<DifficultyController>(true)) timer.PauseTimer();
            }
            Tracker.CalculateResult();
            if (IsTraining) { saved = true; return; }
            saved = days.TryFinishDay(Tracker.Statistics);
            Error = saved ? null : days.LastError;
            if (saved && save.Data.CampaignComplete) CurrentState = GameState.Finished;
        }
        private void Load(string path)
        {
            if (string.IsNullOrEmpty(path) || SceneUtility.GetBuildIndexByScenePath(path) < 0)
            { Error = "Сцена не добавлена в Build Settings: " + path; return; }
            Time.timeScale = previousScale; SceneManager.LoadScene(path);
        }
        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (directors.TryGetValue(gameObject.scene.handle, out var owner) && owner == this) directors.Remove(gameObject.scene.handle);
            Time.timeScale = previousScale;
        }
        private void OnGUI()
        {
            if (CurrentState == GameState.Playing && !IsPaused)
            {
                GUI.Box(new Rect(15, 15, 410, 85), "");
                GUI.Label(new Rect(28, 25, 390, 65), (IsTraining ? "Обучение" : "День " + sessionDay + " / " + settings.CampaignLengthDays + "   Доверие: " + save.Data.Reputation)
                    + "\nПроехали: " + Tracker.Statistics.CarsPassed + "   Очки: " + Tracker.Statistics.Score + "\nEsc: пауза");
                return;
            }
            float width = Mathf.Min(620f, Screen.width - 30f), height = Mathf.Min(570f, Screen.height - 30f);
            Rect area = new Rect((Screen.width - width) / 2f, (Screen.height - height) / 2f, width, height);
            GUI.Box(area, ""); GUILayout.BeginArea(new Rect(area.x + 24, area.y + 20, area.width - 48, area.height - 40));
            var title = new GUIStyle(GUI.skin.label) { fontSize = 25, wordWrap = true };
            var body = new GUIStyle(GUI.skin.label) { fontSize = 17, wordWrap = true };
            if (!initialized) { GUILayout.Label("Не удалось подготовить смену", title); GUILayout.Label(Error ?? "Подготовка…", body); GUILayout.EndArea(); return; }
            scroll = GUILayout.BeginScrollView(scroll);
            if (IsPaused)
            {
                GUILayout.Label("Пауза", title);
                GUILayout.Label("Выход в меню вернёт вас к началу незавершённой смены.", body);
                if (GUILayout.Button("Продолжить", GUILayout.Height(42))) { IsPaused = false; Time.timeScale = previousScale; }
            }
            else if (CurrentState == GameState.Preparing)
            {
                GUILayout.Label(IsTraining ? "Обучение" : "Смена мэра: день " + sessionDay, title);
                GUILayout.Label("Переключайте светофоры и помогайте машинам добраться до конца маршрута. В конце смены жители оценят результат.", body);
                GUILayout.Space(12);
                GUILayout.Label("Смена: " + settings.DayDurationSeconds + " сек.\nДоверие: " + save.Data.Reputation + ". Для переизбрания нужно " + settings.ElectionThreshold + ".\nВыборы после дня " + settings.CampaignLengthDays + ".", body);
                if (Error == null && (IsTraining || save.HasSave || save.LastError == null))
                    if (GUILayout.Button("Начать смену", GUILayout.Height(45))) StartGame();
            }
            else
            {
                GUILayout.Label(CurrentState == GameState.Finished ? (save.Data.ElectionWon ? "Вы переизбраны!" : "Выборы проиграны") : (IsTraining ? "Обучение завершено" : "Итоги дня " + sessionDay), title);
                var result = saved && !IsTraining && save.Data.Results.Count > 0 ? save.Data.Results[save.Data.Results.Count - 1] : null;
                var stat = Tracker.Statistics;
                GUILayout.Label("Проехали: " + (result != null ? result.CarsPassed : stat.CarsPassed) + "\nАварии: " + (result != null ? result.Accidents : stat.Accidents)
                    + "\nОчки: " + (result != null ? result.Score : stat.Score) + "\nСреднее время остановок: " + (result != null ? result.AverageWaitingTime : stat.AverageWaitingTime).ToString("F1") + " сек.", body);
                if (saved && !IsTraining)
                {
                    GUILayout.Label("Изменение доверия: " + (result != null ? result.ReputationChange.ToString("+0;-0;0") : "0") + "\nТекущее доверие: " + save.Data.Reputation, body);
                    GUILayout.Label(news.GetNews(save.Data.Reputation), body);
                    if (!save.Data.CampaignComplete && GUILayout.Button("К следующему дню", GUILayout.Height(42))) Load(gameObject.scene.path);
                }
                if (!saved && GUILayout.Button("Повторить сохранение", GUILayout.Height(42))) FinishGame();
            }
            if (!string.IsNullOrEmpty(Error)) GUILayout.Label(Error, body);
            if (!save.HasSave && save.LastError != null) GUILayout.Label(save.LastError, body);
            GUILayout.Space(12);
            if (GUILayout.Button(saved || CurrentState == GameState.Preparing ? "В главное меню" : "В меню без сохранения смены", GUILayout.Height(38))) Load(settings.MainMenuScenePath);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
