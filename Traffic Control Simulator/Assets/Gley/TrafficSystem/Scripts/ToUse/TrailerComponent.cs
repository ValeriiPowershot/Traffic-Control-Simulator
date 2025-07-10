using Gley.TrafficSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class TrailerComponent : MonoBehaviour, ITrafficParticipant
    {
        [Header("Object References")]
        [Tooltip("RigidBody of the vehicle")]
        public Rigidbody rb;
        [Tooltip("Empty GameObject used to rotate the vehicle from the correct point")]
        public Transform trailerHolder;
        [Tooltip("The point where the trailer attaches to the truck")]
        public Transform truckConnectionPoint;
        [Tooltip("The joint that will connect to the truck")]
        public ConfigurableJoint joint;
        [Tooltip("All trailer wheels and their properties")]
        public Wheel[] allWheels;
        [Tooltip("If suspension is set to 0, the value of suspension will be half of the wheel radius")]
        public float maxSuspension = 0f;
        [Tooltip("How rigid the suspension will be. Higher the value -> more rigid the suspension")]
        public float springStiffness = 5;


        [HideInInspector]
        public float width;
        [HideInInspector]
        public float height;
        [HideInInspector]
        public float length;


        private VehicleComponent associatedVehicle;
        private float springForce;

        internal void Initialize(VehicleComponent associatedVehicle)
        {
            this.associatedVehicle = associatedVehicle;
            springForce = ((rb.mass * -Physics.gravity.y) / allWheels.Length);
            Vector3 centerOfMass = Vector3.zero;
            for (int i = 0; i < allWheels.Length; i++)
            {
                allWheels[i].wheelTransform.Translate(Vector3.up * (allWheels[i].maxSuspension / 2 + allWheels[i].wheelRadius));
                centerOfMass += allWheels[i].wheelTransform.position;
            }
            rb.centerOfMass = centerOfMass / allWheels.Length;
            DeactivateVehicle();
        }


        public float GetCurrentSpeedMS()
        {
            return associatedVehicle.GetCurrentSpeed();
        }


        internal void DeactivateVehicle()
        {
            rb.transform.localPosition = Vector3.zero;
            rb.transform.localRotation = Quaternion.identity;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }


        internal int GetNrOfWheels()
        {
            return allWheels.Length;
        }


        internal float GetSpringForce()
        {
            return springForce;
        }


        internal float GetSpringStiffness()
        {
            return springStiffness;
        }
    }
}