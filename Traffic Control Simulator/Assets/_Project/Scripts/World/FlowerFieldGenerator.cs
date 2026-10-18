using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class FlowerFieldGenerator : MonoBehaviour
{
    [Header("Field Size")]
    public float width = 10f;
    public float length = 10f;

    [Header("Flowers")]
    [SerializeField] private Camera _camera;
    public GameObject[] flowerPrefabs;

    [Header("Generation Settings")]
    public int flowerCount = 50;
    public float minDistanceBetweenFlowers = 1.0f;
    public bool randomRotation = true;
    public bool faceCamera = false;

    private List<GameObject> spawnedFlowers = new List<GameObject>();

    public void GenerateFlowers()
    {
        ClearFlowers();

        if (flowerPrefabs == null || flowerPrefabs.Length == 0)
        {
            Debug.LogWarning("No flower prefabs assigned.");
            return;
        }

        Camera sceneCamera = _camera;
        if (faceCamera && sceneCamera == null)
        {
            Debug.LogWarning("No SceneView camera found. Cannot orient flowers toward camera.");
            return;
        }

        HashSet<Vector2> usedPositions = new HashSet<Vector2>();
        int placed = 0;
        int attempts = 0;
        int maxAttempts = flowerCount * 10;

        while (placed < flowerCount && attempts < maxAttempts)
        {
            Vector3 position = new Vector3(
                Random.Range(-width / 2f, width / 2f),
                0f,
                Random.Range(-length / 2f, length / 2f)
            );

            bool tooClose = false;
            foreach (var pos in usedPositions)
            {
                if (Vector2.Distance(new Vector2(position.x, position.z), pos) < minDistanceBetweenFlowers)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                GameObject prefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];
                GameObject flower = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
                flower.transform.localPosition = position;

                if (faceCamera && sceneCamera != null)
                {
                    Vector3 dir = sceneCamera.transform.position - flower.transform.position;
                    dir.y = 0f;
                    flower.transform.rotation = Quaternion.LookRotation(dir);
                }
                else if (randomRotation)
                {
                    flower.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                }

                spawnedFlowers.Add(flower);
                usedPositions.Add(new Vector2(position.x, position.z));
                placed++;
            }

            attempts++;
        }

        if (placed < flowerCount)
        {
            Debug.LogWarning("Not all flowers could be placed. Try increasing the field size or lowering the min distance.");
        }
    }

    public void ClearFlowers()
    {
        foreach (var flower in spawnedFlowers)
        {
            if (flower != null)
                DestroyImmediate(flower);
        }
        spawnedFlowers.Clear();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FlowerFieldGenerator))]
public class FlowerFieldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FlowerFieldGenerator generator = (FlowerFieldGenerator)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Flowers"))
        {
            generator.GenerateFlowers();
        }

        if (GUILayout.Button("Clear Flowers"))
        {
            generator.ClearFlowers();
        }
    }
}
#endif
