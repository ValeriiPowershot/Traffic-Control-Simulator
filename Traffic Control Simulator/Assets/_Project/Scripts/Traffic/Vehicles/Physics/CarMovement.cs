using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft, frontRight, rearLeft, rearRight;

    [Header("Engine & Brake Settings")]
    public float maxMotorTorque = 1500f;
    public float maxBrakeForce = 3000f;
    public float maxSpeed = 20f;
    public float brakeSmoothness = 5f;

    public Vector3 centerOfMassOffset = new Vector3(0f, -0.6f, 0f);

    public bool IsBraking = false;
    public float currentBrake = 0f;

    public Rigidbody rb;

    public void Initialize(Rigidbody rigidbody)
    {
        rb = rigidbody;
        rb.centerOfMass = centerOfMassOffset;
    }

    public void UpdateMovement()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            SetBrake(maxBrakeForce / 2);
        }

        // Плавное применение тормозов
        frontLeft.brakeTorque = currentBrake;
        frontRight.brakeTorque = currentBrake;
        rearLeft.brakeTorque = currentBrake;
        rearRight.brakeTorque = currentBrake;

        if (IsBraking)
        {
            rearLeft.motorTorque = 0f;
            rearRight.motorTorque = 0f;
        }
        else
        {
            ApplyMotor();
        }
    }

    public void ApplyBrakePriority(float targetBrake)
    {
        // Если новый тормоз сильнее текущего — применяем его
        if (targetBrake > currentBrake)
        {
            currentBrake = targetBrake;
            IsBraking = currentBrake > 0f;
        }
    }

    public void SetBrake(float targetBrake)
    {
        if (targetBrake == 0f)
        {
            // мгновенно снимаем тормоз, чтобы машина сразу тронулась
            currentBrake = 0f;
            IsBraking = false;
        }
        else
        {
            // плавное нарастание тормоза
            currentBrake = Mathf.Lerp(currentBrake, targetBrake, Time.fixedDeltaTime * brakeSmoothness);
            IsBraking = currentBrake > 0f;
        }
    }

    private void ApplyMotor()
    {
        float speed = rb.velocity.magnitude;
        float torque = (speed < maxSpeed) ? maxMotorTorque : 0f;
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    public void StopCar()
    {
        currentBrake = maxBrakeForce;
        IsBraking = true;

        rearLeft.motorTorque = 0f;
        rearRight.motorTorque = 0f;
        frontLeft.steerAngle = 0f;
        frontRight.steerAngle = 0f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
