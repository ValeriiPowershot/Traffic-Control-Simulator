using UnityEngine;
using System.Collections.Generic;
namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// This class is for testing purpose only
    /// It is the car controller provided by Unity:
    /// https://docs.unity3d.com/Manual/WheelColliderTutorial.html
    /// </summary>
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
    }


    public class PlayerCar : MonoBehaviour
    {
        public List<AxleInfo> axleInfos;
        public Transform centerOfMass;
        public float maxMotorTorque;
        public float maxSteeringAngle;
        IVehicleLightsComponent lightsComponent;
        bool mainLights;
        bool brake;
        bool reverse;
        bool blinkLeft;
        bool blinkRifgt;
        float realtimeSinceStartup;
        Rigidbody rb;

        UIInput inputScript;


        private void Start()
        {
            GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
            inputScript = gameObject.AddComponent<UIInput>().Initialize();
            lightsComponent = gameObject.GetComponent<VehicleLightsComponent>();
            lightsComponent.Initialize();
            rb = GetComponent<Rigidbody>();
        }

        // finds the corresponding visual wheel
        // correctly applies the transform
        public void ApplyLocalPositionToVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0)
            {
                return;
            }

            Transform visualWheel = collider.transform.GetChild(0);

            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);

            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }

        public void FixedUpdate()
        {
            float motor = maxMotorTorque * inputScript.GetVerticalInput();
            float steering = maxSteeringAngle * inputScript.GetHorizontalInput();

            float localVelocity = transform.InverseTransformDirection(rb.velocity).z+0.1f;
            reverse = false;
            brake = false;
            if (localVelocity < 0)
            {
                reverse = true;
            }

            if (motor < 0)
            {
                if (localVelocity > 0)
                {
                    brake = true;
                }
            }
            else
            {
                if (motor > 0)
                {
                    if (localVelocity < 0)
                    {
                        brake = true;
                    }
                }
            }

            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        }

        private void Update()
        {
            realtimeSinceStartup += Time.deltaTime;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                mainLights = !mainLights;
                lightsComponent.SetMainLights(mainLights);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
            {
                blinkLeft = !blinkLeft;
                if (blinkLeft == true)
                {
                    blinkRifgt = false;
                    lightsComponent.SetBlinker(BlinkType.BlinkLeft);
                }
                else
                {
                    lightsComponent.SetBlinker(BlinkType.Stop);
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                blinkRifgt = !blinkRifgt;
                if (blinkRifgt == true)
                {
                    blinkLeft = false;
                    lightsComponent.SetBlinker(BlinkType.BlinkRight);
                }
                else
                {
                    lightsComponent.SetBlinker(BlinkType.Stop);
                }
            }

            lightsComponent.SetBrakeLights(brake);
            lightsComponent.SetReverseLights(reverse);
            lightsComponent.UpdateLights(realtimeSinceStartup);
        }
    }
}