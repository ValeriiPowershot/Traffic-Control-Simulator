using UnityEngine;

public class TrafficLightStopWaypoint : MonoBehaviour
{
    [Header("Traffic Light State")]
    public bool isRed = true;
    public bool IsRed => isRed;

    [Header("Distances")]
    public float brakeDistance = 10f;   // начинаем плавно тормозить
    public float stopDistance = 2.5f;   // где должны полностью остановиться
    public float radius = 1f;           // радиус засчёта точки (на зелёный)

    public void SetRed()
    {
        isRed = true;
    }

    public void SetGreen()
    {
        isRed = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, brakeDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = isRed ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
