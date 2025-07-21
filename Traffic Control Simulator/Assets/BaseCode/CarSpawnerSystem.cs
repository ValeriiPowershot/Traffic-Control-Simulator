using System.Collections;
using BaseCode;
using UnityEngine;

[DisallowMultipleComponent]
public class CarSpawnerSystem : MonoBehaviour
{
    [Tooltip("Link to ScriptableObject with spawn config.")]
    public CarPoolScriptableObject config;

    private void Start()
    {
        if (config != null)
        {
            foreach (var spawner in config.spawners)
            {
                if (spawner.spawnPoint != null)
                {
                    StartCoroutine(RunSpawner(spawner));
                }
                else
                {
                    Debug.LogWarning("Spawner has no assigned spawn point!", this);
                }
            }
        }
        else
        {
            Debug.LogError("CarSpawnerSystem: No config assigned!", this);
        }
    }

    private IEnumerator RunSpawner(CarPoolScriptableObject.SpawnerEntry spawner)
    {
        foreach (var carData in spawner.carsToSpawn)
        {
            yield return new WaitForSeconds(carData.initialDelay);

            for (int i = 0; i < carData.count; i++)
            {
                if (carData.car != null && carData.car.Prefab != null)
                {
                    Instantiate(
                        carData.car.Prefab,
                        spawner.spawnPoint,
                        Quaternion.identity
                    );
                }
                else
                {
                    Debug.LogWarning("Car or its prefab is missing in config!", this);
                }

                yield return new WaitForSeconds(carData.delayBetweenSpawns);
            }
        }
    }
}
