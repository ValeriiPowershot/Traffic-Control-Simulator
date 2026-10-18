using UnityEngine;

public class SelfRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [SerializeField] private float rotationSpeed = 90f;

    [Tooltip("Максимальный угол вправо/вверх")]
    [SerializeField] private float positiveAngleLimit = 90f;

    [Tooltip("Максимальный угол влево/вниз")]
    [SerializeField] private float negativeAngleLimit = -85f;

    [SerializeField] private bool clockwise = true;

    [Header("Optional")]
    [SerializeField] private bool useFixedUpdate = false;

    private float _currentAngle = 0f;

    // 1 = вперёд, -1 = назад
    private int _direction = 1;

    // локальный центр объекта
    private Vector3 _localCenter;

    private void Start()
    {
        _localCenter = CalculateLocalVisualCenter();
    }

    private void Update()
    {
        if (!useFixedUpdate)
            Rotate();
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate)
            Rotate();
    }

    private void Rotate()
    {
        float baseDirection = clockwise ? 1f : -1f;

        float angleStep =
            rotationSpeed *
            Time.deltaTime *
            baseDirection *
            _direction;

        float nextAngle = _currentAngle + angleStep;

        // Проверка лимитов
        if ((_direction == 1 && nextAngle >= positiveAngleLimit) ||
            (_direction == -1 && nextAngle <= negativeAngleLimit))
        {
            angleStep =
                (_direction == 1
                    ? positiveAngleLimit
                    : negativeAngleLimit)
                - _currentAngle;

            _direction *= -1;
        }

        // переводим локальный центр в мировые координаты
        Vector3 worldCenter = transform.TransformPoint(_localCenter);

        // вращение вокруг LOCAL center
        transform.RotateAround(
            worldCenter,
            transform.TransformDirection(rotationAxis.normalized),
            angleStep
        );

        _currentAngle += angleStep;
    }

    private Vector3 CalculateLocalVisualCenter()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found.");
            return Vector3.zero;
        }

        Bounds combinedBounds = renderers[0].localBounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            Bounds b = renderers[i].localBounds;

            // переводим bounds child в local space родителя
            b.center = transform.InverseTransformPoint(
                renderers[i].transform.TransformPoint(b.center)
            );

            combinedBounds.Encapsulate(b);
        }

        return combinedBounds.center;
    }

    public void ResetRotation()
    {
        _currentAngle = 0f;
        _direction = 1;
    }
}
