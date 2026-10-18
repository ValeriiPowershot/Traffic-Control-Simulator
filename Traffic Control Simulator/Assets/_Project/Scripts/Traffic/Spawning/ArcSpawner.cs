using UnityEngine;

public class ArcSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _pointPrefab;

    [Header("Straight Ingress (въезд до дуги)")]
    [SerializeField] private int _straightCount = 3;
    [SerializeField] private float _straightSpacing = 2f;

    [Header("Arc Section")]
    [SerializeField] private int _arcCount = 5;
    [SerializeField] private float _radius = 5f;
    [SerializeField] private float _angleDegrees = 90f;

    [SerializeField] private Vector3 _direction = Vector3.forward;

    public void SpawnArc()
    {
        ClearChildren();

        if (_pointPrefab == null)
            return;

        Vector3 dir = _direction.normalized;
        int index = 0;

        // ▶️ 1. Прямая часть (въезд)
        Vector3 lastStraightPos = transform.position;
        if (_straightCount > 0)
        {
            for (int i = _straightCount; i > 0; i--)
            {
                Vector3 pos = transform.position - dir * _straightSpacing * i;
                GameObject straightPoint = Instantiate(_pointPrefab, pos, Quaternion.identity, transform);
                straightPoint.name = $"Straight_{index++}";
                if (i == 1)
                    lastStraightPos = pos;
            }
        }

        // 📍 2. Сместим центр дуги от последней прямой
        // дуга начинается от конца прямой — сдвигаем центр дуги назад
        float angleRad = _angleDegrees * Mathf.Deg2Rad;
        float startAngle = -angleRad / 2f;

        // ЦЕНТР дуги = конец прямой + (вектор напротив начала дуги) * радиус
        Vector3 startDir = Quaternion.AngleAxis(Mathf.Rad2Deg * startAngle, Vector3.up) * dir;
        Vector3 arcCenter = lastStraightPos - startDir * _radius;

        // 🔁 3. Дуга
        if (_arcCount > 1)
        {
            for (int i = 0; i < _arcCount; i++)
            {
                float t = i / (_arcCount - 1f);
                float angle = startAngle + t * angleRad;

                Vector3 rotated = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.up) * dir;
                Vector3 arcPos = arcCenter + rotated * _radius;

                GameObject arcPoint = Instantiate(_pointPrefab, arcPos, Quaternion.identity, transform);
                arcPoint.name = $"Arc_{index++}";
            }
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
