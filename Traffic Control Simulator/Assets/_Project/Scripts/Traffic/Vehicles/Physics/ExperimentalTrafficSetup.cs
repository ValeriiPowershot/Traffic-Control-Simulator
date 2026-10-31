using UnityEngine;
using BaseCode;
using BaseCode.Logic.City;
using Realistic_Traffic_Controller.Scripts;

[DefaultExecutionOrder(-100), DisallowMultipleComponent]
public sealed class ExperimentalTrafficSetup : MonoBehaviour
{
    private void Awake()
    {
        foreach (var root in gameObject.scene.GetRootGameObjects())
        {
            foreach (var spawner in root.GetComponentsInChildren<CarSpawnerSystem>(true))
                spawner.UseExperimentalController = true;
            foreach (var director in root.GetComponentsInChildren<GameManager>(true))
                director.EnableTrainingSession();
        }
    }
    private void Start()
    {
        // All RTC Awake calls have completed before takeover can disable them.
        foreach (var root in gameObject.scene.GetRootGameObjects())
        {
            foreach (var director in root.GetComponentsInChildren<GameManager>(true))
                director.EnableTrainingSession();
            foreach (var source in root.GetComponentsInChildren<RTC_CarController>(false))
                if (source.enabled)
                    ArcadeTrafficCar.TakeOver(source, source.currentWaypoint ? source.currentWaypoint : source.nextWaypoint, source.CarSpawnIndex);
        }
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(12, Screen.height - 32, 900, 28), "ТЕСТ АВТО: свой поток — тормозим, чужой — сталкиваемся. Прогресс кампании не сохраняется.");
    }
}
