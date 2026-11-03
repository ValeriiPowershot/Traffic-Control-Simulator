using UnityEngine;

namespace TrafficControl.CleanVehicles
{
    // Sandbox controls deliberately have no campaign or persistence dependencies.
    public sealed class CityTrafficPreview : MonoBehaviour
    {
        public CityCarSpawner[] Spawners;
        public TrafficLight[] Signals;
        private bool running;
        private float previousScale;
        private void Awake() { previousScale = Time.timeScale > 0 ? Time.timeScale : 1; Time.timeScale = 1; }
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12,12,310,430), GUI.skin.box);
            GUILayout.Label("LEVEL 1: НОВОЕ ДВИЖЕНИЕ");
            if (GUILayout.Button(running ? "Остановить спавн" : "Запустить поток", GUILayout.Height(34)))
            { running = !running; foreach (var spawn in Spawners) if (spawn) spawn.StartAutomatically = running; }
            foreach (var light in Signals) if (light && GUILayout.Button("Поток " + light.TrafficLightSpawnIndex + ": " + light.LightState)) light.SwitchRedGreen();
            if (GUILayout.Button(Time.timeScale == 0 ? "Продолжить" : "Пауза")) Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            if (GUILayout.Button("Убрать машины"))
                for (int i = CityCarController.All.Count - 1; i >= 0; i--)
                    if (CityCarController.All[i].gameObject.scene == gameObject.scene) Destroy(CityCarController.All[i].gameObject);
            int waiting = 0, crashed = 0, completed = 0; float error = 0;
            foreach (var car in CityCarController.All)
            {
                if (car.gameObject.scene != gameObject.scene) continue;
                if (car.State == CityCarController.DrivingState.Waiting) waiting++;
                if (car.State == CityCarController.DrivingState.Crashed) crashed++;
                error = Mathf.Max(error, Mathf.Abs(car.LaneError));
            }
            foreach (var spawn in Spawners) if (spawn) completed += spawn.Completed;
            GUILayout.Label("Ждут: " + waiting + "   Аварии: " + crashed + "\nПроехали: " + completed + "\nОтклонение оси: " + error.ToString("F2") + " м");
            GUILayout.Label("Тестовая копия. Прогресс кампании не сохраняется.");
            GUILayout.EndArea();
        }
        private void OnDestroy() { Time.timeScale = previousScale; }
    }
}
