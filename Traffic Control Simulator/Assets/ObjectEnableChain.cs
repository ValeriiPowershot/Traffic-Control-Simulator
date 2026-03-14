using UnityEngine;

[DisallowMultipleComponent]
public class ObjectEnableChain : MonoBehaviour
{
    [SerializeField] private SpawnParts[] _spawnParts;

    private int _currentPartIndex;

    private void Start()
    {
        foreach (SpawnParts part in _spawnParts)
        {
            if (part != null)
                part.StartSpawn();
        }
    }
}
