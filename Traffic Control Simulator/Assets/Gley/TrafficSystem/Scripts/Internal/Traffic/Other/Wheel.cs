using UnityEngine;
namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Used to store wheel properties for vehicle 
    /// </summary>
    [System.Serializable]
    public class Wheel
    {
        public enum WheelPosition
        {
            Front,
            Back,
            Other
        }

        public Transform wheelTransform;
        public Transform wheelGraphics;
        public WheelPosition wheelPosition;
        public float wheelRadius;
        [HideInInspector]
        public float wheelCircumference;
        [HideInInspector]
        public float raycastLength;
        public float maxSuspension = 0f;
    }
}
