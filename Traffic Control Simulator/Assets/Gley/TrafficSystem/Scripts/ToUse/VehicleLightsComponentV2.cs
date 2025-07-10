using Gley.TrafficSystem.Internal;
using UnityEngine;
namespace Gley.TrafficSystem
{
    /// <summary>
    /// Used to control vehicle lights if needed
    /// Light objects are enabled or disabled based on car actions 
    /// Not all lights are mandatory
    /// </summary>
    public class VehicleLightsComponentV2 : MonoBehaviour, IVehicleLightsComponent
    {
        [Tooltip("Blinking interval")]
        public float blinkTime = 0.5f;
        [Tooltip("A GameObject containing all main lights - will be active based on Manager API calls")]
        public GameObject[] frontLights;
        [Tooltip("A GameObject containing all reverse lights - will be active if a vehicle is reversing")]
        public GameObject[] reverseLights;
        [Tooltip("A GameObject containing all rear lights - will be active if main lights are active")]
        public GameObject[] rearLights;
        [Tooltip("A GameObject containing all brake lights - will be active when a vehicle is braking")]
        public GameObject[] stopLights;
        [Tooltip("A GameObject containing all blinker left lights - will be active when car turns left")]
        public GameObject[] blinkerLeft;
        [Tooltip("A GameObject containing all blinker right lights - will be active when car turns right")]
        public GameObject[] blinkerRight;

        private float currentTime;
        private bool updateLights;
        private bool leftBlink;
        private bool rightBlink;


        /// <summary>
        /// Initialize the component if required
        /// </summary>
        public void Initialize()
        {
            currentTime = 0;
            LightsSetup();
        }


        /// <summary>
        /// Disable all lights
        /// </summary>
        public void DeactivateLights()
        {
            LightsSetup();
            leftBlink = false;
            rightBlink = false;
        }


        /// <summary>
        /// Set lights state
        /// </summary>
        private void LightsSetup()
        {
            if (frontLights.Length != 0)
            {
                foreach (var light in frontLights)
                {
                    light.SetActive(false);
                }

            }
            if (reverseLights.Length != 0)
            {
                foreach (var light in reverseLights)
                {
                    light.SetActive(false);
                }
            }
            if (rearLights.Length != 0)
            {
                foreach (var light in rearLights)
                {
                    light.SetActive(false);
                }
            }
            if (stopLights.Length != 0)
            {
                foreach (var light in stopLights)
                {
                    light.SetActive(false);
                }
            }
            if (blinkerLeft.Length != 0)
            {
                foreach (var light in blinkerLeft)
                {
                    light.SetActive(false);
                }
                updateLights = true;
            }
            if (blinkerRight.Length != 0)
            {
                foreach (var light in blinkerRight)
                {
                    light.SetActive(false);
                }
                updateLights = true;
            }
        }


        /// <summary>
        /// Activate brake lights
        /// </summary>
        /// <param name="active"></param>
        public void SetBrakeLights(bool active)
        {
            if (stopLights.Length != 0)
            {
                foreach (var light in stopLights)
                {
                    if (light.activeSelf != active)
                    {
                        light.SetActive(active);
                    }
                }
            }
        }


        /// <summary>
        /// Activate main lights
        /// </summary>
        /// <param name="active"></param>
        public void SetMainLights(bool active)
        {
            if (frontLights.Length != 0)
            {
                foreach (var light in frontLights)
                {
                    light.SetActive(active);
                }
            }
            if (rearLights.Length != 0)
            {
                foreach (var light in rearLights)
                {
                    light.SetActive(active);
                }
            }
        }


        /// <summary>
        /// Activate reverse lights
        /// </summary>
        /// <param name="active"></param>
        public void SetReverseLights(bool active)
        {
            if (reverseLights.Length != 0)
            {
                foreach (var light in reverseLights)
                {
                    if (light.activeSelf != active)
                    {
                        light.SetActive(active);
                    }
                }
            }
        }


        /// <summary>
        /// Activate blinker lights
        /// </summary>
        /// <param name="blinkType"></param>
        public void SetBlinker(BlinkType blinkType)
        {
            if (blinkerLeft.Length != 0 && blinkerRight.Length != 0)
            {
                switch (blinkType)
                {
                    case BlinkType.Stop:
                        if (leftBlink == true)
                        {
                            leftBlink = false;
                        }
                        if (rightBlink == true)
                        {
                            rightBlink = false;
                        }
                        break;
                    case BlinkType.BlinkLeft:
                        if (leftBlink == false)
                        {
                            leftBlink = true;
                        }
                        if (rightBlink == true)
                        {
                            rightBlink = false;
                        }
                        break;
                    case BlinkType.BlinkRight:
                        if (rightBlink == false)
                        {
                            rightBlink = true;
                        }
                        if (leftBlink == true)
                        {
                            leftBlink = false;
                        }
                        break;

                    case BlinkType.Hazard:
                        if (rightBlink == false)
                        {
                            rightBlink = true;
                        }
                        if (leftBlink == false)
                        {
                            leftBlink = true;
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// Perform blinking
        /// </summary>
        public void UpdateLights(float realtimeSinceStartup)
        {
            if (updateLights)
            {
                if (realtimeSinceStartup - currentTime > blinkTime)
                {
                    currentTime = realtimeSinceStartup;
                    if (leftBlink == false)
                    {
                        foreach (var light in blinkerLeft)
                        {
                            if (light.activeSelf != leftBlink)
                            {
                                light.SetActive(leftBlink);
                            }
                        }
                    }
                    else
                    {
                        foreach (var light in blinkerLeft)
                        {
                            light.SetActive(!light.activeSelf);
                        }
                    }
                    if (rightBlink == false)
                    {
                        foreach (var light in blinkerRight)
                        {
                            if (light.activeSelf != rightBlink)
                            {
                                light.SetActive(rightBlink);
                            }
                        }
                    }
                    else
                    {
                        foreach (var light in blinkerRight)
                        {
                            light.SetActive(!light.activeSelf);
                        }
                    }
                }
            }
        }
    }
}
