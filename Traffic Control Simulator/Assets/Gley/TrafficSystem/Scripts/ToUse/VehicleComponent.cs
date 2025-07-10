using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem
{
    /// <summary>
    /// Add this script on a vehicle prefab and configure the required parameters
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [HelpURL("https://gley.gitbook.io/mobile-traffic-system/setup-guide/vehicle-implementation")]
    public class VehicleComponent : MonoBehaviour, ITrafficParticipant
    {
        [Header("Object References")]
        [Tooltip("RigidBody of the vehicle")]
        public Rigidbody rb;
        [Tooltip("Empty GameObject used to rotate the vehicle from the correct point")]
        public Transform carHolder;
        [Tooltip("Front trigger used to detect obstacle. It is automatically generated")]
        public Transform frontTrigger;
        [Tooltip("Assign this object if you need a hard shadow on your vehicle, leave it black otherwise")]
        public Transform shadowHolder;

        [Header("Wheels")]
        [Tooltip("All vehicle wheels and their properties")]
        public Wheel[] allWheels;
        [Tooltip("Max wheel turn amount in degrees")]
        public float maxSteer = 30;
        [Tooltip("If suspension is set to 0, the value of suspension will be half of the wheel radius")]
        public float maxSuspension = 0f;
        [Tooltip("How rigid the suspension will be. Higher the value -> more rigid the suspension")]
        public float springStiffness = 5;


        [Header("Car Properties")]
        [Tooltip("Vehicle type used for making custom paths")]
        public VehicleTypes vehicleType;
        [Tooltip("Min vehicle speed. Actual vehicle speed is picked random between min and max")]
        public int minPossibleSpeed = 40;
        [Tooltip("Max vehicle speed")]
        public int maxPossibleSpeed = 90;
        [Tooltip("Time in seconds to reach max speed (acceleration)")]
        public float accelerationTime = 10;
        [Tooltip("Distance to keep from an obstacle/vehicle")]
        public float distanceToStop = 3;
        [Tooltip("Car starts braking when an obstacle enters trigger. Total length of the trigger = distanceToStop+minTriggerLength")]
        public float triggerLength = 4;

        [HideInInspector]
        public bool updateTrigger = false;
        [HideInInspector]
        public float maxTriggerLength = 10;

        [HideInInspector]
        public TrailerComponent trailer;
        [HideInInspector]
        public Transform trailerConnectionPoint;

        [HideInInspector]
        public float length = 0;
        [HideInInspector]
        public float coliderHeight = 0;
        [HideInInspector]
        public float wheelDistance;

        [HideInInspector]
        public VisibilityScript visibilityScript;
        [HideInInspector]
        public bool excluded;

        private struct Obstacle
        {
            Collider collider;
            bool isConvex;

            public Obstacle(Collider collider, bool isConvex)
            {
                this.collider = collider;
                this.isConvex = isConvex;
            }

            internal Collider GetCollider()
            {
                return collider;
            }

            internal bool IsConvex()
            {
                return isConvex;
            }
        }

        public List<ITrafficParticipant> vehiclesToFollow;

        internal float colliderWidth;

        private Collider[] allColliders;
        private List<Obstacle> obstacleList;     
        private Transform frontAxle;
        private LayerMask buildingLayers;
        private LayerMask obstacleLayers;
        private LayerMask playerLayers;
        private LayerMask roadLayers;
        private EngineSoundComponent engineSound;
        private IVehicleLightsComponent vehicleLights;
        private DriveActions currentAction;
        private float springForce;
        private float maxSpeed;
        private float storedMaxSpeed;
        private float minTriggerLength;
        private int listIndex;
        private bool lightsOn;
        private BoxCollider frontCollider;
        private ModifyTriggerSize modifyTriggerSize;



        /// <summary>
        /// Initialize vehicle
        /// </summary>
        /// <param name="buildingLayers">static colliders to interact with</param>
        /// <param name="obstacleLayers">dynamic colliders to interact with</param>
        /// <param name="playerLayers">player colliders to interact with</param>
        /// <returns>the vehicle</returns>
        public virtual VehicleComponent Initialize(LayerMask buildingLayers, LayerMask obstacleLayers, LayerMask playerLayers, LayerMask roadLayers, bool lightsOn, ModifyTriggerSize modifyTriggerSize)
        {
            this.buildingLayers = buildingLayers;
            this.obstacleLayers = obstacleLayers;
            this.playerLayers = playerLayers;
            this.roadLayers = roadLayers;
            this.modifyTriggerSize = modifyTriggerSize;
            allColliders = GetComponentsInChildren<Collider>();
            springForce = ((rb.mass * -Physics.gravity.y) / allWheels.Length);

            frontCollider = frontTrigger.GetChild(0).GetComponent<BoxCollider>();
            colliderWidth = frontCollider.size.x;
            minTriggerLength = frontCollider.size.z;
            DeactivateVehicle();

            //compute center of mass based on the wheel position
            Vector3 centerOfMass = Vector3.zero;
            for (int i = 0; i < allWheels.Length; i++)
            {
                allWheels[i].wheelTransform.Translate(Vector3.up * (allWheels[i].maxSuspension / 2 + allWheels[i].wheelRadius));
                centerOfMass += allWheels[i].wheelTransform.position;
            }
            rb.centerOfMass = centerOfMass / allWheels.Length;

            //set additional components
            engineSound = GetComponent<EngineSoundComponent>();
            if (engineSound)
            {
                engineSound.Initialize();
            }

            this.lightsOn = lightsOn;
            vehicleLights = GetComponent<VehicleLightsComponent>();
            if (vehicleLights == null)
            {
                vehicleLights = GetComponent<VehicleLightsComponentV2>();
            }
            if (vehicleLights != null)
            {
                vehicleLights.Initialize();
            }

            if (trailer != null)
            {
                trailer.Initialize(this);
            }

            return this;
        }


        /// <summary>
        /// Returns all colliders of the vehicle
        /// </summary>
        /// <returns></returns>
        internal Collider[] GetAllColliders()
        {
            return allColliders;
        }


        /// <summary>
        /// Apply new trigger size delegate
        /// </summary>
        /// <param name="triggerSizeModifier"></param>
        internal void SetTriggerSizeModifierDelegate(ModifyTriggerSize triggerSizeModifier)
        {
            modifyTriggerSize = triggerSizeModifier;
        }


        /// <summary>
        /// Add a vehicle on scene
        /// </summary>
        /// <param name="position"></param>
        /// <param name="vehicleRotation"></param>
        /// <param name="masterVolume"></param>
        public virtual void ActivateVehicle(Vector3 position, Quaternion vehicleRotation, float masterVolume, Quaternion trailerRotation)
        {
            storedMaxSpeed = maxSpeed = Random.Range(minPossibleSpeed, maxPossibleSpeed);

            gameObject.transform.SetPositionAndRotation(position, vehicleRotation);

            //position vehicle with front wheels on the waypoint
            float distance = Vector3.Distance(position, frontTrigger.transform.position);
            transform.Translate(-transform.forward * distance, Space.World);

            if (trailer != null)
            {
                trailer.transform.rotation = trailerRotation;
            }

            gameObject.SetActive(true);


            if (engineSound)
            {
                engineSound.Play(masterVolume);
            }

            SetMainLights(lightsOn);

            AIEvents.onNotifyVehicles += AVehicleChengedState;
        }


        /// <summary>
        /// Remove a vehicle from scene
        /// </summary>
        public virtual void DeactivateVehicle()
        {
            //Debug.Log(trailer);
            gameObject.SetActive(false);
            obstacleList = new List<Obstacle>();
            vehiclesToFollow = new List<ITrafficParticipant>();
            visibilityScript.Reset();
            if (engineSound)
            {
                engineSound.Stop();
            }

            if (vehicleLights != null)
            {
                vehicleLights.DeactivateLights();
            }
            AIEvents.onNotifyVehicles -= AVehicleChengedState;
            if (trailer)
            {
                trailer.DeactivateVehicle();
            }
        }


        /// <summary>
        /// Compute the ground direction vector used to apply forces, and update the shadow
        /// </summary>
        /// <returns>ground direction</returns>
        public Vector3 GetGroundDirection()
        {
            Vector3 frontPoint = Vector3.zero;
            int nrFront = 0;
            Vector3 backPoint = Vector3.zero;
            int nrBack = 0;
            for (int i = 0; i < allWheels.Length; i++)
            {
                if (allWheels[i].wheelPosition == Wheel.WheelPosition.Front)
                {
                    nrFront++;
                    frontPoint += allWheels[i].wheelGraphics.position;
                }
                else
                {
                    nrBack++;
                    backPoint += allWheels[i].wheelGraphics.position;
                }
            }
            Vector3 groundDirection = (frontPoint / nrFront - backPoint / nrBack).normalized;
            if (shadowHolder)
            {
                Vector3 centerPoint = (frontPoint / nrFront + backPoint / nrBack) / 2 - transform.up * (allWheels[0].wheelRadius - 0.1f);
                shadowHolder.rotation = Quaternion.LookRotation(groundDirection);
                shadowHolder.position = new Vector3(shadowHolder.position.x, centerPoint.y, shadowHolder.position.z);

            }
            return groundDirection;
        }


        /// <summary>
        /// Computes the acceleration per frame
        /// </summary>
        /// <returns></returns>
        public float GetPowerStep()
        {
            int nrOfFrames = (int)(accelerationTime / Time.fixedDeltaTime);
            float targetSpeedMS = maxSpeed / 3.6f;
            return targetSpeedMS / nrOfFrames;
        }


        /// <summary>
        /// Computes steering speed per frame
        /// </summary>
        /// <returns></returns>
        public float GetSteeringStep()
        {
            return maxSteer * Time.fixedDeltaTime * 2;
        }


        /// <summary>
        /// Computes brake step per frame
        /// </summary>
        /// <returns></returns>
        public float GetBrakeStep()
        {
            int nrOfFrames = (int)(accelerationTime / 4 / Time.fixedDeltaTime);
            float targetSpeedMS = maxSpeed / 3.6f;
            return targetSpeedMS / nrOfFrames;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>max vehicle speed</returns>
        public float GetMaxSpeed()
        {
            return maxSpeed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>spring force</returns>
        public float GetSpringForce()
        {
            return springForce;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Max RayCast length</returns>
        public float GetRaycastLength()
        {
            return allWheels[0].raycastLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Current vehicle action</returns>
        public DriveActions GetCurrentAction()
        {
            return currentAction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Wheel circumference</returns>
        public float GetWheelCircumference()
        {
            return allWheels[0].wheelCircumference;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Vehicle velocity vector</returns>
        public Vector3 GetVelocity()
        {
            return rb.velocity;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Current speed in kmh</returns>
        public float GetCurrentSpeed()
        {
            return GetVelocity().magnitude * 3.6f;
        }


        /// <summary>
        /// Returns currens speed in m/s
        /// </summary>
        /// <returns></returns>
        public float GetCurrentSpeedMS()
        {
            return GetVelocity().magnitude;
        }


        /// <summary>
        /// Used to verify is the current collider is included in other vehicle trigger
        /// </summary>
        /// <returns>first collider from collider list</returns>
        public Collider GetCollider()
        {
            return allColliders[0];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Trigger orientation</returns>
        public Vector3 GetHeading()
        {
            return frontTrigger.forward;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>vehicle orientation</returns>
        public Vector3 GetForwardVector()
        {
            return transform.forward;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>max steer</returns>
        public float GetMaxSteer()
        {
            return maxSteer;
        }


        /// <summary>
        /// Set the list index for current vehicle
        /// </summary>
        /// <param name="index">new list index</param>
        public void SetIndex(int index)
        {
            listIndex = index;
        }


        /// <summary>
        /// Get list index of the current vehicle
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            return listIndex;
        }


        /// <summary>
        /// Check if the vehicle is not in view
        /// </summary>
        /// <returns></returns>
        public bool CanBeRemoved()
        {
            return visibilityScript.IsNotInView();
        }


        /// <summary>
        /// A vehicle stopped reversing check for new action 
        /// </summary>
        public void CurrentVehicleActionDone()
        {
            if (obstacleList.Count > 0)
            {
                for (int i = 0; i < obstacleList.Count; i++)
                {
                    ObstacleTypes obstacleType = GetObstacleTypes(obstacleList[i].GetCollider());
                    if (obstacleType != ObstacleTypes.Other)
                    {
                        VehicleEvents.TriggeerObjectInTriggerEvent(listIndex, obstacleType, obstacleList[i].GetCollider());
                    }
                }
            }
            else
            {
                VehicleEvents.TriggerTriggerClearedEvent(listIndex);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>current vehicle type</returns>
        public VehicleTypes GetVehicleType()
        {
            return vehicleType;
        }


        /// <summary>
        /// Creates a GameObject that is used to reach waypoints 
        /// </summary>
        /// <returns>the front wheel position of the vehicle</returns>
        public Transform GetFrontAxle()
        {
            if (frontAxle == null)
            {
                frontAxle = new GameObject("FrontAxle").transform;
                frontAxle.transform.SetParent(frontTrigger.parent);
                frontAxle.transform.position = frontTrigger.position;
            }
            return frontAxle;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>number of vehicle wheels</returns>
        public int GetNrOfWheels()
        {
            return allWheels.Length;
        }


        /// <summary>
        /// Returns the nr of wheels of the trailer
        /// </summary>
        /// <returns></returns>
        public int GetTrailerWheels()
        {
            if (trailer == null)
            {
                return 0;
            }
            return trailer.GetNrOfWheels();
        }


        /// <summary>
        /// Set the new vehicle action
        /// </summary>
        /// <param name="currentAction"></param>
        public void SetCurrentAction(DriveActions currentAction)
        {
            this.currentAction = currentAction;
        }


        /// <summary>
        /// Returns the position of the closest obstacle inside the front trigger.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetClosestObstacle()
        {
            if (obstacleList.Count > 0)
            {
                if (!obstacleList[0].IsConvex())
                {
                    return frontTrigger.position;
                }

                Vector3 result = obstacleList[0].GetCollider().ClosestPoint(frontTrigger.position);

                float minDistance = Vector3.SqrMagnitude(result - frontTrigger.position);

                for (int i = 1; i < obstacleList.Count; i++)
                {
                    Vector3 closestPoint = obstacleList[i].GetCollider().ClosestPoint(frontTrigger.position);
                    float distance = Vector3.SqrMagnitude(closestPoint - frontTrigger.position);
                    if (Vector3.SqrMagnitude(closestPoint - frontTrigger.position) < minDistance)
                    {
                        result = closestPoint;
                        minDistance = distance;
                    }
                }
                return result;
            }
            return Vector3.zero;
        }


        /// <summary>
        /// CHeck trigger objects
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger)
            {
                ObstacleTypes obstacleType = GetObstacleTypes(other);
                if (obstacleType == ObstacleTypes.TrafficVehicle || obstacleType == ObstacleTypes.Player)
                {
                    AddVehichleToFollow(other);
                }
                if (obstacleType != ObstacleTypes.Other && obstacleType != ObstacleTypes.Road)
                {
                    NewColliderHit(other);
                    VehicleEvents.TriggeerObjectInTriggerEvent(listIndex, obstacleType, other);
                }
            }
        }


        /// <summary>
        /// Determines which vehicle should be followed
        /// </summary>
        /// <param name="other"></param>
        private void AddVehichleToFollow(Collider other)
        {
            Rigidbody otherRb = other.attachedRigidbody;
            if (otherRb != null)
            {
                if (otherRb.GetComponent<ITrafficParticipant>() != null)
                {
                    vehiclesToFollow.Add(otherRb.GetComponent<ITrafficParticipant>());
                }
            }
        }


        /// <summary>
        /// Check for collisions
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            Events.TriggerVehicleCrashEvent(listIndex, GetObstacleTypes(collision.collider), collision.collider);
        }


        /// <summary>
        /// Returns the type of obstacle that just entered the front trigger
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private ObstacleTypes GetObstacleTypes(Collider other)
        {
            bool carHit = other.gameObject.layer == gameObject.layer;
            //possible vehicle hit
            if (carHit)
            {
                Rigidbody otherRb = other.attachedRigidbody;
                if (otherRb != null)
                {
                    if (otherRb.GetComponent<VehicleComponent>() != null)
                    {
                        return ObstacleTypes.TrafficVehicle;
                    }
                }
                //if it is on traffic layer but it lacks a vehicle component, it is a dynamic object
                return ObstacleTypes.DynamicObject;
            }
            else
            {
                //trigger the corresponding event based on object layer
                if (buildingLayers == (buildingLayers | (1 << other.gameObject.layer)))
                {
                    return ObstacleTypes.StaticObject;
                }
                else
                {
                    if (obstacleLayers == (obstacleLayers | (1 << other.gameObject.layer)))
                    {
                        return ObstacleTypes.DynamicObject;
                    }
                    else
                    {
                        if (playerLayers == (playerLayers | (1 << other.gameObject.layer)))
                        {
                            return ObstacleTypes.Player;
                        }
                        else
                        {
                            if (roadLayers == (roadLayers | (1 << other.gameObject.layer)))
                            {
                                return ObstacleTypes.Road;
                            }
                        }
                    }
                }
            }
            return ObstacleTypes.Other;
        }


        /// <summary>
        /// Every time a new collider is hit it is added inside the list
        /// </summary>
        /// <param name="other"></param>
        void NewColliderHit(Collider other)
        {
            if (!obstacleList.Any(cond => cond.GetCollider() == other))
            {
                bool isConvex = true;
                if (other is MeshCollider)
                {
                    isConvex = ((MeshCollider)other).convex;
                }

                obstacleList.Add(new Obstacle(other, isConvex));
            }
        }


        /// <summary>
        /// Remove a collider from the list
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerExit(Collider other)
        {
            if (!other.isTrigger)
            {
                //TODO this should only trigger if objects of interest are doing trigger exit
                if (other.gameObject.layer == gameObject.layer ||
                    (buildingLayers == (buildingLayers | (1 << other.gameObject.layer))) ||
                    (obstacleLayers == (obstacleLayers | (1 << other.gameObject.layer))) ||
                    (playerLayers == (playerLayers | (1 << other.gameObject.layer))))
                {
                    obstacleList.RemoveAll(cond => cond.GetCollider() == other);
                    if (obstacleList.Count == 0)
                    {
                        VehicleEvents.TriggerTriggerClearedEvent(listIndex);
                    }
                    Rigidbody otherRb = other.attachedRigidbody;
                    if (otherRb != null)
                    {
                        if (otherRb.GetComponent<ITrafficParticipant>() != null)
                        {
                            vehiclesToFollow.Remove(otherRb.GetComponent<ITrafficParticipant>());
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Check if current collider is from a new object
        /// </summary>
        /// <param name="colliders"></param>
        /// <returns></returns>
        internal bool AlreadyCollidingWith(Collider[] colliders)
        {
            for (int i = 0; i < obstacleList.Count; i++)
            {
                for (int j = 0; j < colliders.Length; j++)
                {
                    if (obstacleList[i].GetCollider() == colliders[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// When another vehicle changes his state, check if the current vehicle is affected and respond accordingly
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <param name="collider"></param>
        /// <param name="newAction"></param>
        private void AVehicleChengedState(int vehicleIndex, Collider collider)
        {
            //if that vehicle is in the bot trigger
            if (obstacleList.Any(cond => cond.GetCollider() == collider))
            {
                if (obstacleList.Count > 0)
                {
                    for (int i = 0; i < obstacleList.Count; i++)
                    {
                        ObstacleTypes obstacleType = GetObstacleTypes(obstacleList[i].GetCollider());
                        if (obstacleType != ObstacleTypes.Other)
                        {
                            VehicleEvents.TriggeerObjectInTriggerEvent(listIndex, obstacleType, obstacleList[i].GetCollider());
                        }
                    }
                }
                else
                {
                    VehicleEvents.TriggerTriggerClearedEvent(listIndex);
                }
            }
        }


        /// <summary>
        /// Remove a collider from the trigger if the collider was destroyed
        /// </summary>
        /// <param name="collider"></param>
        public void ColliderRemoved(Collider collider)
        {
            if (obstacleList != null)
            {
                if (obstacleList.Any(cond => cond.GetCollider() == collider))
                {
                    OnTriggerExit(collider);
                }
            }
        }


        /// <summary>
        /// Removed a list of colliders from the trigger if the colliders ware destroyed
        /// </summary>
        /// <param name="colliders"></param>
        public void ColliderRemoved(Collider[] colliders)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (obstacleList.Any(cond => cond.GetCollider() == colliders[i]))
                {
                    OnTriggerExit(colliders[i]);
                }
            }
        }


        //update the lights component if required
        #region Lights
        internal void SetMainLights(bool on)
        {
            if (on != lightsOn)
            {
                lightsOn = on;
            }
            if (vehicleLights != null)
            {
                vehicleLights.SetMainLights(on);
            }
        }

        public void SetReverseLights(bool active)
        {
            if (vehicleLights != null)
            {
                vehicleLights.SetReverseLights(active);
            }
        }

        public void SetBrakeLights(bool active)
        {
            if (vehicleLights != null)
            {
                vehicleLights.SetBrakeLights(active);
            }
        }

        public virtual void SetBlinker(BlinkType blinkType)
        {
            if (vehicleLights != null)
            {
                vehicleLights.SetBlinker(blinkType);
            }
        }

        public void UpdateLights(float realtimeSinceStartup)
        {
            if (vehicleLights!=null)
            {
                vehicleLights.UpdateLights(realtimeSinceStartup);
            }
        }
        #endregion


        //update the sound component if required
        #region Sound
        public void UpdateEngineSound(float masterVolume)
        {
            if (engineSound)
            {
                engineSound.UpdateEngineSound(GetCurrentSpeed(), maxSpeed, masterVolume);
            }
        }
        #endregion


        /// <summary>
        /// Removes active events
        /// </summary>
        private void OnDestroy()
        {
            AIEvents.onNotifyVehicles -= AVehicleChengedState;
        }


        /// <summary>
        /// Returns the size of the trigger
        /// </summary>
        /// <returns></returns>
        internal float GetTriggerSize()
        {
            return frontCollider.size.z - 2;
        }


        /// <summary>
        /// Modify the dimension of the front trigger
        /// </summary>
        internal void UpdateColliderSize()
        {
            if (updateTrigger)
            {
                modifyTriggerSize?.Invoke(GetVelocity().magnitude * 3.6f, frontCollider, storedMaxSpeed, minTriggerLength, maxTriggerLength);
            }
        }


        /// <summary>
        /// Get the gollow speed
        /// </summary>
        /// <returns></returns>
        internal float GetFollowSpeed()
        {
            if (vehiclesToFollow.Count == 0)
            {
                return Mathf.Infinity;
            }
            return vehiclesToFollow.Min(cond => cond.GetCurrentSpeedMS());
        }


        /// <summary>
        /// Set max speed for the current vehicle
        /// </summary>
        /// <param name="speed"></param>
        internal void SetMaxSpeed(float speed)
        {
            maxSpeed = speed;
            if (maxSpeed < 5)
            {
                maxSpeed = 0;
            }
        }


        /// <summary>
        /// Reset max speed to the original one
        /// </summary>
        internal void ResetMaxSpeed()
        {
            SetMaxSpeed(storedMaxSpeed);
        }


        /// <summary>
        /// Returns the stiffness of the springs
        /// </summary>
        /// <returns></returns>
        internal float GetSpringStiffness()
        {
            return springStiffness;
        }
    }
}