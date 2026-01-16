using UnityEngine;

public class TrafficLightNew : MonoBehaviour
{
    [Header("Traffic Light Settings")]
    public bool isRed = true;             // Текущий цвет
    public float stopDistance = 5f;       // Где машина должна остановиться перед светофором
    public float brakeDistance = 10f;     // Начинаем тормозить заранее
    public float radius = 1f;             // Радиус для засчёта waypoint

    public void SetRed() => isRed = true;
    public void SetGreen() => isRed = false;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = isRed ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, brakeDistance);
    }
}
