using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Stop Settings")]
    public bool stopHere = false;

    [Header("Waypoint Settings")]
    public float radius = 1f;
    public float brakeDistance = 10f;

    [Header("Max Speed")]
    public int maxSpeed = 10;

    [Header("Steering Hint (optional)")]
    [Tooltip("0 = нет поворота, 1 = резкий поворот (90°)")]
    [Range(0f, 1f)]
    public float turnSharpness = 0f;

    void OnDrawGizmos()
    {
        Gizmos.color = stopHere ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (stopHere)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, brakeDistance);
        }
    }
}
