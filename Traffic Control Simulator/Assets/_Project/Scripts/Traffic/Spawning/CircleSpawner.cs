using UnityEngine;
using UnityEngine.Serialization;

public class CircleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _pointPrefab;
    [SerializeField] private int _objectsCount = 8;
    [SerializeField] private float _radius = 5f;

    public void SpawnCircle()
    {
        ClearChildren();

        for (int i = 0; i < _objectsCount; i++)
        {
            float angle = i * Mathf.PI * 2 / _objectsCount;
            Vector3 position = new Vector3(
                Mathf.Cos(angle) * _radius,
                0,
                Mathf.Sin(angle) * _radius
            );

            GameObject child = Instantiate(_pointPrefab, transform.position + position, Quaternion.identity, transform);
            child.name = $"Child_{i}";
        }
    }

    public void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            #if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
            #else
            Destroy(transform.GetChild(i).gameObject);
            #endif
        }
    }
}
