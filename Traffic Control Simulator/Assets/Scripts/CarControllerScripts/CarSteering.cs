using UnityEngine;

public class CarSteering : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft, frontRight;

    [Header("Visual Wheels")]
    public Transform frontLeftVisual;
    public Transform frontRightVisual;
    public Transform rearLeftVisual;
    public Transform rearRightVisual;

    [Header("Steering Settings")]
    public float maxSteerAngle = 30f;
    public float steerSmoothness = 6f;

    [Header("Steering Curve")]
    [Tooltip("X = нормализованный угол (0–1), Y = сила поворота")]
    public AnimationCurve steeringCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Distance Influence")]
    public float fullSteerDistance = 12f;

    private float currentSteerAngle;

    // =========================
    // Steering logic
    // =========================
    public void UpdateSteering(
        Transform carTransform,
        Transform target,
        bool isBraking
    )
    {
        if (target == null) return;

        if (isBraking)
        {
            currentSteerAngle = Mathf.Lerp(
                currentSteerAngle,
                0f,
                Time.fixedDeltaTime * steerSmoothness
            );
        }
        else
        {
            Vector3 localTarget =
                carTransform.InverseTransformPoint(target.position);

            float angleToTarget =
                Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

            float normalizedAngle =
                Mathf.Clamp(angleToTarget / maxSteerAngle, -1f, 1f);

            float abs = Mathf.Abs(normalizedAngle);

            float distance = localTarget.magnitude;
            float distanceFactor =
                Mathf.Clamp01(distance / fullSteerDistance);

            float sharpness = 0f;
            Waypoint wp = target.GetComponent<Waypoint>();
            if (wp != null)
                sharpness = wp.turnSharpness;

            float curveValue =
                steeringCurve.Evaluate(abs);

            float steerStrength =
                Mathf.Lerp(curveValue * distanceFactor, curveValue, sharpness);

            float targetAngle =
                steerStrength * Mathf.Sign(normalizedAngle) * maxSteerAngle;

            currentSteerAngle = Mathf.Lerp(
                currentSteerAngle,
                targetAngle,
                Time.fixedDeltaTime * steerSmoothness
            );
        }

        frontLeft.steerAngle = currentSteerAngle;
        frontRight.steerAngle = currentSteerAngle;

        UpdateVisualWheels();
    }

    // =========================
    // Visual wheels sync
    // =========================
    private void UpdateVisualWheels()
    {
        UpdateWheel(frontLeft, frontLeftVisual);
        UpdateWheel(frontRight, frontRightVisual);
        UpdateWheel(rearLeftVisual != null ? null : null, null); // заглушка
        UpdateWheel(rearRightVisual != null ? null : null, null); // заглушка

        // Реальные задние колёса (без руля)
        if (rearLeftVisual != null)
            UpdateWheelPose(frontLeft.attachedRigidbody, rearLeftVisual, frontLeft.radius);

        if (rearRightVisual != null)
            UpdateWheelPose(frontRight.attachedRigidbody, rearRightVisual, frontRight.radius);
    }

    private void UpdateWheel(WheelCollider collider, Transform visual)
    {
        if (collider == null || visual == null) return;

        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);

        visual.position = pos;
        visual.rotation = rot;
    }

    // fallback если нет WheelCollider (на всякий)
    private void UpdateWheelPose(Rigidbody rb, Transform visual, float radius)
    {
        if (rb == null || visual == null) return;

        visual.Rotate(Vector3.right, rb.velocity.magnitude * Time.fixedDeltaTime * 50f);
    }
}
