using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private DifficultyController _difficulty;
    [SerializeField] private TrafficManager _traffic;

    [Header("Pools")]
    [SerializeField] private CarPool _normalPool;
    [SerializeField] private CarPool _heavyPool;

    [Header("Settings")]
    [SerializeField] private float _baseSpeed = 5f;

    private float _nextSpawnTime;

    private void Update()
    {
        if (Time.time < _nextSpawnTime)
            return;

        TrySpawn();
        ScheduleNextSpawn();
    }

    private void TrySpawn()
    {
        var stage = _difficulty.CurrentStage;

        if (_traffic.ActiveCars >= stage.MaxCarsOnScreen)
            return;
    }

    private void ScheduleNextSpawn()
    {
        var stage = _difficulty.CurrentStage;
    }
}
