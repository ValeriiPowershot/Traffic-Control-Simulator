using CarControllerScripts;
using UnityEngine;

[DisallowMultipleComponent]
public class CarDetection : MonoBehaviour
{
    [Header("Car Components")]
    [SerializeField] private CarMovement carMovement;
    [SerializeField] private CarSpeedometr carSpeedometr;
    [SerializeField] private CarController _carController;

    [Header("Detection Settings")]
    public Transform rayOrigin;
    public float rayDistance = 10f;
    public LayerMask targetLayer;

    [Header("Brake Settings")]
    public float safeDistance = 5f;     // Начало торможения
    public float minDistance = 1f;      // Минимальное безопасное расстояние
    public float maxBrakeForce = 3000f;
    public float brakeSmoothness = 5f;

    void Update()
    {
        if (rayOrigin == null || carMovement == null || carSpeedometr == null) return;

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        float currentSpeed = carSpeedometr.GetCarSpeed();

        if (Physics.Raycast(ray, out hit, rayDistance, targetLayer))
        {
            float distanceToObject = hit.distance;

            // Желаемая скорость пропорционально расстоянию
            float targetSpeed = Mathf.Lerp(0f, carMovement.maxSpeed, (distanceToObject - minDistance) / (safeDistance - minDistance));
            targetSpeed = Mathf.Clamp(targetSpeed, 0f, carMovement.maxSpeed);

            // Разница скорости
            float speedDifference = currentSpeed - targetSpeed;

            float targetBrake = 0f;

            if (speedDifference > 0f)
            {
                // Пропорциональное торможение
                targetBrake = Mathf.Lerp(0f, maxBrakeForce, speedDifference / carMovement.maxSpeed);
            }

            carMovement.SetBrake(targetBrake);

            Debug.Log($"Объект впереди: {hit.collider.name}, расстояние: {distanceToObject:F2}, текущая скорость: {currentSpeed:F2}, цель: {targetSpeed:F2}, brake: {targetBrake:F0}");
        }
    }

    void OnDrawGizmos()
    {
        if (rayOrigin == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayOrigin.position, rayOrigin.position + rayOrigin.forward * rayDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(rayOrigin.position, 0.1f);
    }
}
