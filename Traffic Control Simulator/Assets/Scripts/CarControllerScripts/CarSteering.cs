using UnityEngine;

public class CarSteering : MonoBehaviour
{
    public WheelCollider frontLeft, frontRight;
    public float maxSteerAngle = 30f;

    public void UpdateSteering(Transform carTransform, Transform target, bool isBraking)
    {
        if (target == null) return;

        Vector3 localTarget = carTransform.InverseTransformPoint(target.position);
        float steer = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steerAngle = isBraking ? 0f : Mathf.Clamp(steer, -maxSteerAngle, maxSteerAngle);
        frontLeft.steerAngle = steerAngle;
        frontRight.steerAngle = steerAngle;
    }
}
