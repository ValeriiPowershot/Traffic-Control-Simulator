using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Stop Settings")]
    public bool stopHere = false;       // Нужно ли останавливаться на этой точке

    [Header("Waypoint Settings")]
    public float radius = 1f;           // Радиус, когда точка считается достигнутой
    public float brakeDistance = 10f;   // Расстояние перед точкой, где начинаем тормозить

    void OnDrawGizmos()
    {
        Gizmos.color = stopHere ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (stopHere)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, brakeDistance); // где начинается торможение
        }
    }
}
