using UnityEngine;

public class WaypointSystem : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public int currentIndex = 0;

    public Transform CurrentTarget => currentIndex < waypoints.Length ? waypoints[currentIndex] : null;

    /// <summary>
    /// Обновление waypoint и управление торможением
    /// </summary>
    /// <param name="carTransform">Transform машины</param>
    /// <param name="movement">CarMovement компонент</param>
    public void UpdateWaypoints(Transform carTransform, CarMovement movement)
    {
        if (CurrentTarget == null) return;

        float distance = Vector3.Distance(carTransform.position, CurrentTarget.position);
        float speed = movement.rb.velocity.magnitude;

        Waypoint wp = CurrentTarget.GetComponent<Waypoint>();
        TrafficLightNew light = CurrentTarget.GetComponent<TrafficLightNew>();

        float desiredBrake = 0f;

        // --- StopHere торможение ---
        if (wp != null && wp.stopHere)
        {
            float stoppingDistance = (speed * speed) / (2f * movement.maxBrakeForce);
            float brakeDist = Mathf.Max(stoppingDistance, wp.brakeDistance);

            if (distance <= brakeDist)
            {
                float ratio = Mathf.Clamp01(1f - distance / Mathf.Max(brakeDist, 0.1f));
                desiredBrake = ratio * movement.maxBrakeForce;
            }
        }

        // --- TrafficLight торможение ---
        if (light != null)
        {
            float stoppingDistance = (speed * speed) / (2f * movement.maxBrakeForce);
            float brakeDist = Mathf.Max(stoppingDistance, light.brakeDistance);

            if (light.isRed && distance <= brakeDist)
            {
                float ratio = Mathf.Clamp01(1f - distance / Mathf.Max(brakeDist, 0.1f));
                desiredBrake = Mathf.Max(desiredBrake, ratio * movement.maxBrakeForce);

            }
            else if (!light.isRed)
            {
                // Зеленый свет → мгновенно снимаем тормоз
                desiredBrake = 0f;
            }
        }

        // Применяем торможение
        movement.SetBrake(desiredBrake);

        // --- Достижение точки с учётом TrafficLight ---
        bool reached = false;

// Обычный waypoint
        if (wp != null && distance <= wp.radius)
            reached = true;

// TrafficLight
        if (light != null && distance <= light.radius)
        {
            if (!light.isRed)
            {
                // Зеленый свет → можно засчитать
                reached = true;
            }
            else
            {
                // Красный свет → не засчитываем, пока свет красный
                reached = false;
            }
        }

        if (reached)
        {
            // Переходим к следующей точке
            currentIndex = Mathf.Min(currentIndex + 1, waypoints.Length - 1);

            // Сбрасываем тормоза после прохождения точки
            movement.SetBrake(0f);
        }
    }
}
