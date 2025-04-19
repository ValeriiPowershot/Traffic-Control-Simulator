using UnityEngine;

public class SelfRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 90f;
    public bool clockwise = true;

    [Header("Optional")]
    public bool useFixedUpdate = false;

    private Vector3 _center;

    void Start()
    {
        _center = CalculateVisualCenter();
    }

    void Update()
    {
        if (!useFixedUpdate) Rotate();
    }

    void FixedUpdate()
    {
        if (useFixedUpdate) Rotate();
    }

    void Rotate()
    {
        float direction = clockwise ? 1f : -1f;
        float angle = rotationSpeed * direction * Time.deltaTime;
        transform.RotateAround(_center, rotationAxis.normalized, angle);
    }

    /// <summary>
    /// Вычисляет центр объекта с учетом всех рендереров.
    /// </summary>
    Vector3 CalculateVisualCenter()
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
