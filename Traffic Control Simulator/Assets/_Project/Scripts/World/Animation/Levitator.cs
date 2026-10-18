using UnityEngine;

public class Levitator : MonoBehaviour
{
    [Header("Levitation Settings")]
    [SerializeField] private float amplitude = 0.5f; // Высота подъёма
    [SerializeField] private float frequency = 1f;   // Скорость колебаний

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.localPosition;
    }

    private void Update()
    {
        float offsetY = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * amplitude;

        transform.localPosition = _startPosition + Vector3.up * offsetY;
    }
}
