using UnityEngine;

public class WaypointSystem : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public int currentIndex = 0;

    public Transform CurrentTarget => currentIndex < waypoints.Length ? waypoints[currentIndex] : null;

    public void UpdateWaypoints(Transform carTransform, CarMovement movement)
    {
        if (CurrentTarget == null) return;

        float distance = Vector3.Distance(carTransform.position, CurrentTarget.position);
        float speed = movement.rb.velocity.magnitude;

        Waypoint wp = CurrentTarget.GetComponent<Waypoint>();
        TrafficLightStopWaypoint light = CurrentTarget.GetComponent<TrafficLightStopWaypoint>();

        float desiredBrake = 0f;

        // =========================================================
        // 🔹 1. Проверяем СЛЕДУЮЩУЮ точку (если она светофор)
        // =========================================================

        if (currentIndex + 1 < waypoints.Length)
        {
            Transform nextPoint = waypoints[currentIndex + 1];
            TrafficLightStopWaypoint nextLight = nextPoint.GetComponent<TrafficLightStopWaypoint>();

            if (nextLight != null)
            {
                float distanceToNext = Vector3.Distance(carTransform.position, nextPoint.position);

                if (distanceToNext <= nextLight.brakeDistance)
                {
                    // Если красный — тормозим до нуля
                    // Если зелёный — замедляемся до разрешённой скорости
                    float targetSpeed = nextLight.isRed ? 0f : 5f; // ← можешь менять

                    float speedDiff = speed - targetSpeed;

                    if (speedDiff > 0f)
                    {
                        float ratio = speedDiff / movement.maxSpeed;
                        float brake = Mathf.Clamp(ratio * movement.maxBrakeForce, 0f, movement.maxBrakeForce);
                        desiredBrake = Mathf.Max(desiredBrake, brake);
                    }
                }
            }
        }

        // =========================================================
        // 🔹 2. Обычный StopHere
        // =========================================================

        if (wp != null && wp.stopHere)
        {
            if (distance <= wp.brakeDistance)
            {
                float ratio = Mathf.Clamp01(1f - distance / Mathf.Max(wp.brakeDistance, 0.1f));
                desiredBrake = Mathf.Max(desiredBrake, ratio * movement.maxBrakeForce);
            }
        }

        // =========================================================
        // 🔹 3. Текущий светофор
        // =========================================================

        if (light != null)
        {
            if (distance <= light.brakeDistance)
            {
                float targetSpeed = light.isRed ? 0f : 5f;

                float speedDiff = speed - targetSpeed;

                if (speedDiff > 0f)
                {
                    float ratio = speedDiff / movement.maxSpeed;
                    float brake = Mathf.Clamp(ratio * movement.maxBrakeForce, 0f, movement.maxBrakeForce);
                    desiredBrake = Mathf.Max(desiredBrake, brake);
                }

                if (!light.isRed)
                {
                    movement.SetBrake(0f);
                }
            }
        }

        // =========================================================
        // 🔹 Применяем тормоз
        // =========================================================

        movement.ApplyBrakePriority(desiredBrake);

        // =========================================================
        // 🔹 Переключение waypoint
        // =========================================================

        bool reached = false;

        if (wp != null && distance <= wp.radius)
            reached = true;

        if (light != null && distance <= light.radius && !light.isRed)
            reached = true;

        if (reached)
        {
            currentIndex = Mathf.Min(currentIndex + 1, waypoints.Length - 1);
        }
    }
}
