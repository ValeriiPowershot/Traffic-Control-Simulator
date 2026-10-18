using UnityEngine;

[DisallowMultipleComponent]
public class SpawnPartHolder : MonoBehaviour
{
    [SerializeField] private SpawnParts[] _spawnParts;
    [SerializeField] private SpawnParts _firstSpawnPart;

    [Header("Settings")]
    [SerializeField] private bool _autoStart;

    private void Start()
    {
        if (_autoStart)
        {
            RestartParts();
        }
    }

    public void TurnOffAllParts()
    {
        for (int i = 0; i < _spawnParts.Length; i++)
        {
            if (_spawnParts[i] == null)
                continue;

            _spawnParts[i].ResetParts();
        }
    }

    public void RestartParts()
    {
        TurnOffAllParts();

        if (_firstSpawnPart != null)
        {
            _firstSpawnPart.StartSpawn();
        }
    }
}
