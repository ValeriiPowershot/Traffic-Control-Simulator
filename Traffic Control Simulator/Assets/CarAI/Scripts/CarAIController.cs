using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CarAIController : MonoBehaviour
{
    //Wheel transforms

    [Header("Wheels")]
    public Transform frontRight;
    public Transform frontLeft;
    public Transform rearRight;
    public Transform rearLeft;

    //Wheel colliders

    public WheelCollider frontRightCollider;
    public WheelCollider frontLeftCollider;
    public WheelCollider rearRightCollider;
    public WheelCollider rearLeftCollider;

    [Header("Checkpoints And Detections")]
    //Checkpoints
    [Tooltip("The checkpoint transform that the ai checks for. Every time the car enters a checkpoint, this variable changes to the next connected checkpoint to it, or will choose one randomly if it haves multiple checkpoints connected.")]
    public Transform nextCheckpoint;
    [Tooltip("A list of the positions where the ai will shoot rays that will detect objects. Placing them inside of the car's collider is ideal.")]
    public List<Transform> checks = new List<Transform> {null};
    [Tooltip("If its true, the AI will check for checkpoints.")]
    public bool CheckPointSearch = true;
    [Tooltip("If its true, it means an object is in front of the car.")]
    public bool objectDetected = false;
    [Tooltip("If its true, the vehicle will be controlled by the ai.")]
    public bool isCarControlledByAI = true;
    [Tooltip("Layers seen by the car. If you uncheck a layer the car won't react to objects from that layer.")]
    public LayerMask seenLayers = Physics.AllLayers;

    [Header("Car Settings")]

    //Speed
    [Tooltip("The speed of the vehicle measured in km/h. This is only for reading, changing it won't affect the speed of the car.")]
    public int kmh;
    [Tooltip("The speed limit applied to the ai to drive the car in km/h.")]
    public int speedLimit;
    [Tooltip("Distance to keep away from other objects.")]
    public float distanceFromObjects = 2f;
    [Tooltip("The number of km/h that the car will go above/under the speed limit. For example 0=it will respect the speed limit, 10=it will go with 10 km/h above the speed limit, -10=it will go with 10 km/h below the speed limit.")]
    public int recklessnessThreshold = 0;
    [Tooltip("If true the car will despawn if it flips over on the Z axis.")]
    public bool despawnForFlippingOver = true;
    [Tooltip("If true the car will switch to taxi mode, meaning that using the TaxiScript, it will go from a start checkpoint to an end checkpoint. These checkpoints need to be connected in a checkpoint network.")]
    public bool taxiMode = false;

    //Car values

    [Tooltip("Acceleration threshold.")]
    public float acceleration = 100f;
    [Tooltip("Breaking threshold. Tip: make it bigger than the acceleration threshold so that the car can break faster.")]
    public float breaking = 1000f;

    //Private variables
    private Stopwatch stopwatch = new Stopwatch();
    private Vector3 lastPos;
    private float steerAngle = 0f;
    private bool flipOverCheck = false;

    private void FixedUpdate()
    {
        WheelUpdate(frontRight, frontRightCollider);
        WheelUpdate(frontLeft, frontLeftCollider);
        WheelUpdate(rearRight, rearRightCollider);
        WheelUpdate(rearLeft, rearLeftCollider);

        //Calculate speed
        CalculateKMH();

        //Search for checkpoints

        SearchForCheckpoints();

        if(despawnForFlippingOver && !flipOverCheck)
        {
            flipOverCheck = true;
            StartCoroutine(CheckForFlippingOver());
        }

    }

    IEnumerator CheckForFlippingOver()
    {
        bool deleteCar = isCarFlipedOver();

        if (deleteCar)
        {
            for(int i = 0; i < 10; i++)
            {
                if(!isCarFlipedOver())
                {
                    deleteCar = false;
                }
                yield return new WaitForSeconds(1);
            }

            if(deleteCar)
            {
                UnityEngine.Debug.Log("Car " + gameObject.name + " destroyed for flipping over.");
                Destroy(gameObject);
            }
        }

        yield return new WaitForSeconds(10);

        flipOverCheck = false;

        yield return null;
    }

    private bool isCarFlipedOver()
    {

        if(transform.rotation.eulerAngles.z > 30f || transform.rotation.eulerAngles.z < -30f)
        {
            return true;
        }

        return false;
    }

    private void WheelUpdate(Transform transform, WheelCollider collider)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        transform.position = pos;
        transform.rotation = rot;
    }

    /// <summary>
    /// Accelerates the vehicle by the value given.
    /// </summary>
    public void Accelerate(float value)
    {
        frontRightCollider.motorTorque = value;
        frontLeftCollider.motorTorque = value;
    }

    /// <summary>
    /// Breaks the vehicle by the value given.
    /// </summary>
    public void Break(float value)
    {
        frontRightCollider.brakeTorque = value;
        frontLeftCollider.brakeTorque = value;
        rearRightCollider.brakeTorque = value;
        rearLeftCollider.brakeTorque = value;
    }

    /// <summary>
    /// Turns the front wheels at the angle given.
    /// </summary>
    public void Turn(float angle)
    {
        frontRightCollider.steerAngle = angle;
        frontLeftCollider.steerAngle = angle;
    }

    private void CalculateKMH()
    {
        if(stopwatch.IsRunning)
        {
            stopwatch.Stop();

            float distance = (transform.position - lastPos).magnitude;
            float time = stopwatch.Elapsed.Milliseconds / (float)1000;

            kmh = (int)(3600 * distance / time / 1000);

            lastPos = transform.position;
            stopwatch.Reset();
            stopwatch.Start();

        }
        else
        {
            lastPos = transform.position;
            stopwatch.Reset();
            stopwatch.Start();
        }
    }

    /// <summary>
    /// Sets the speed of the vehicle to the one given in the parameter.
    /// </summary>
    public void SetSpeed(int speedLimit)
    {
        if (kmh > speedLimit)
        {
            Break(breaking);
            Accelerate(0);
        }
        else if (kmh < speedLimit)
        {
            Accelerate(acceleration);
            Break(0);
        }
    }

    private void SearchForCheckpoints()
    {
        if (CheckPointSearch && isCarControlledByAI)
        {
            Vector3 nextCheckpointRelative = transform.InverseTransformPoint(nextCheckpoint.position);

            steerAngle = nextCheckpointRelative.x / nextCheckpointRelative.magnitude;
            float xangle = nextCheckpointRelative.y / nextCheckpointRelative.magnitude;

            steerAngle = Mathf.Asin(steerAngle) * 180f / 3.14f;
            xangle = Mathf.Asin(xangle) * 180f / 3.14f;

            Turn(steerAngle);

            float maxDistance = kmh * kmh / 100f + distanceFromObjects;

            RaycastHit carHit = new RaycastHit();

            int objectInFront = 0;

            for(int i = 0; i < checks.Count; i++)
            {
                checks[i].localRotation = Quaternion.Euler(-xangle, steerAngle, 0);
                bool isObjectInFront = Physics.Raycast(checks[i].position, checks[i].forward, out carHit, maxDistance, seenLayers, QueryTriggerInteraction.Ignore);

                #if UNITY_EDITOR
                UnityEngine.Debug.DrawRay(checks[i].position, checks[i].forward * maxDistance, Color.green);
                #endif
                
                if(isObjectInFront == true)
                    objectInFront++;
            }
           
            if (objectInFront > 0)
            {
                SetSpeed(0);
                objectDetected = true;
            }
            else
            {
                objectDetected = false;
                int speed = speedLimit + recklessnessThreshold;
                if(speedLimit == 0)
                {
                    speed = 0;
                }
                if(speed == 0)
                {
                    speed = speedLimit;
                }
                SetSpeed(speed);
            }
        }
    }
}