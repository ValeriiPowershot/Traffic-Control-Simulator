using UnityEngine;

public class SelfRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 90f;

    [Tooltip("Positive and negative angle limits in degrees")]
    public float positiveAngleLimit = 90f;
    public float negativeAngleLimit = -85f;

    public bool clockwise = true;

    [Header("Optional")]
    public bool useFixedUpdate = false;

    private Vector3 _center;
    private float _currentAngle = 0f;
    private int _direction = 1; // 1 = вращение в положительную сторону, -1 = в обратную

    private void Start()
    {
        _center = CalculateVisualCenter();
    }

    private void Update()
    {
        if (!useFixedUpdate) Rotate();
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate) Rotate();
    }

    private void Rotate()
    {
        float baseDirection = clockwise ? 1f : -1f;
        float angleStep = rotationSpeed * Time.deltaTime * baseDirection * _direction;
        float nextAngle = _currentAngle + angleStep;

        // Проверка на превышение лимитов
        if ((_direction == 1 && nextAngle >= positiveAngleLimit) ||
            (_direction == -1 && nextAngle <= negativeAngleLimit))
        {
            // Корректируем шаг, чтобы ровно попасть в лимит
            angleStep = (_direction == 1 ? positiveAngleLimit : negativeAngleLimit) - _currentAngle;
            _direction *= -1; // Меняем направление
        }

        transform.RotateAround(_center, rotationAxis.normalized, angleStep);
        _currentAngle += angleStep;
    }

    private Vector3 CalculateVisualCenter()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("RotateAroundVisualCenter: No renderers found.");
            return transform.position;
        }

        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds.center;
    }
}
