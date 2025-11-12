//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright � 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realistic_Traffic_Controller.Scripts
{
    /// <summary>
    /// Navigates, calculate inputs, and drives the vehicle itself.
    /// </summary>
    [AddComponentMenu("BoneCracker Games/Realistic Traffic Controller/RTC Car Controller")]
    [RequireComponent(typeof(Rigidbody))]
    public class RTC_CarController : RTC_Core {

        private Rigidbody rigid;

        /// <summary>
        /// Rigidbody.
        /// </summary>
        public Rigidbody Rigid {

            get {

                if (rigid == null)
                    rigid = GetComponent<Rigidbody>();

                return rigid;

            }

        }

        /// <summary>
        /// Center of mass.
        /// </summary>
        public Transform COM;

        [System.Serializable]
        public class Wheel {

            /// <summary>
            /// Wheel model.
            /// </summary>
            public Transform wheelModel;

            /// <summary>
            /// Wheel collider.
            /// </summary>
            public WheelCollider wheelCollider;

            /// <summary>
            /// Is traction?
            /// </summary>
            public bool isTraction;

            /// <summary>
            /// Is steering?
            /// </summary>
            public bool isSteering;

            /// <summary>
            /// Is braking?
            /// </summary>
            public bool isBraking;

        }

        /// <summary>
        /// All wheels of the vehicle.
        /// </summary>
        public Wheel[] wheels;

        [System.Serializable]
        public class Bound {

            public float front = 2.5f;
            public float rear = -2.5f;
            public float left = -1f;
            public float right = 1f;
            public float up = .5f;
            public float down = -.5f;

        }

        public RTC_VehicleTypes.VehicleType vehicleType = RTC_VehicleTypes.VehicleType.Light;

        /// <summary>
        /// Front, rear, right, and left sides of the vehicle.
        /// </summary>
        public Bound bounds = new Bound();

        /// <summary>
        /// Smoothed throttle input.
        /// </summary>
        [Range(0f, 1f)] public float throttleInput = 0f;

        /// <summary>
        /// Smoothed brake input.
        /// </summary>
        [Range(0f, 1f)] public float brakeInput = 0f;

        /// <summary>
        /// Smoothed steer input.
        /// </summary>
        [Range(-1f, 1f)] public float steerInput = 0f;

        /// <summary>
        /// Smoothed clutch input.
        /// </summary>
        [Range(0f, 1f)] public float clutchInput = 1f;

        /// <summary>
        /// Fuel and idle throttle inputs.
        /// </summary>
        [Range(0f, 1f)] private float fuelInput = 0f;

        /// <summary>
        /// Fuel and idle throttle inputs.
        /// </summary>
        [Range(0f, 1f)] private float idleInput = 0f;

        /// <summary>
        /// Raw throttle Input.
        /// </summary>
        [Range(0f, 1f)] public float throttleInputRaw = 0f;

        /// <summary>
        /// Raw brake Input.
        /// </summary>
        [Range(0f, 1f)] public float brakeInputRaw = 0f;

        /// <summary>
        /// Raw steer Input.
        /// </summary>
        [Range(-1f, 1f)] public float steerInputRaw = 0f;

        /// <summary>
        /// Raw clutch Input.
        /// </summary>
        [Range(0f, 1f)] public float clutchInputRaw = 1f;

        private float clutchVelocity = 0f;      //  Clutch velocity ref.
        private float engineVelocity = 0f;       //  Engine velocity ref.

        /// <summary>
        /// Engine running right now?
        /// </summary>
        public bool engineRunning = true;       //  Engine is running now?

        /// <summary>
        /// Engine torque curve based on RPM.
        /// </summary>
        public AnimationCurve engineTorqueCurve = new AnimationCurve();

        /// <summary>
        /// Auto create engine torque curve. If min/max engine rpm, engine torque, max engine torque at rpm, or top speed has been changed at runtime, it will generate new curve with them.
        /// </summary>
        public bool autoGenerateEngineRPMCurve = true;

        /// <summary>
        /// Maximum peek of the engine at this RPM.
        /// </summary>
        [Range(0f, 15000f)] public float maxEngineTorqueAtRPM = 4000f;

        /// <summary>
        /// Raw engine rpm.
        /// </summary>
        public float wantedEngineRPMRaw = 0f;

        /// <summary>
        /// Clutch engage rpm.
        /// </summary>
        [Range(100f, 1200f)] public float engageClutchRPM = 600f;

        /// <summary>
        /// Current engine rpm (smoothed).
        /// </summary>
        public float currentEngineRPM = 0f;

        /// <summary>
        /// Minimum engine rpm.
        /// </summary>
        [Range(100f, 1200f)] public float minEngineRPM = 800f;

        /// <summary>
        /// Maximum engine rpm.
        /// </summary>
        [Range(2500f, 12000f)] public float maxEngineRPM = 7000f;

        /// <summary>
        /// RPM of the traction wheels.
        /// </summary>
        public float tractionWheelRPM2EngineRPM = 0f;

        /// <summary>
        /// Wheel speed of the vehicle.
        /// </summary>
        public float wheelRPM2Speed = 0f;

        /// <summary>
        /// Target wheel speed for current gear.
        /// </summary>
        public float targetWheelSpeedForCurrentGear = 0f;

        /// <summary>
        /// Gear ratios. Faster accelerations on higher values, but lower top speeds.
        /// </summary>
        public float[] gearRatios = new float[] { 4.35f, 2.5f, 1.66f, 1.23f, 1.0f, .85f };

        /// <summary>
        /// Current gear.
        /// </summary>
        [Range(0, 10)] public int currentGear = 0;

        /// <summary>
        /// Automatic transmission will shift up late on higher values.
        /// </summary>
        [Range(.1f, .9f)] public float gearShiftThreshold = .8f;

        /// <summary>
        /// Target engine rpm to shift up.
        /// </summary>
        [Range(1200f, 12000f)] public float gearShiftUpRPM = 5000f;

        /// <summary>
        /// Target engine rpm to shift down.
        /// </summary>
        [Range(1200f, 12000f)] public float gearShiftDownRPM = 3250f;

        /// <summary>
        ///  Shifting time.
        /// </summary>
        [Range(0f, 1f)] public float gearShiftingTime = .2f;

        /// <summary>
        /// Shifting now?
        /// </summary>
        public bool gearShiftingNow = false;

        /// <summary>
        /// Don't shift timer if too close to previous one.
        /// </summary>
        public bool dontGearShiftTimer = true;

        /// <summary>
        /// Timer for don't shift.
        /// </summary>
        public float lastTimeShifted = 0f;

        /// <summary>
        /// Differential ratio.
        /// </summary>
        [Range(.2f, 12f)] public float differentialRatio = 3.6f;

        /// <summary>
        /// Maximum engine torque.
        /// </summary>
        [Range(0f, 5000f)] public float engineTorque = 200f;

        /// <summary>
        /// Maximum speed.
        /// </summary>
        [Range(20f, 360f)] public float maximumSpeed = 160f;

        /// <summary>
        /// Calculated and desired speed related to destination point and angle.
        /// </summary>
        public float desiredSpeed = 0f;

        /// <summary>
        /// Maximum brake torque.
        /// </summary>
        [Range(0f, 50000f)] public float brakeTorque = 1000f;

        /// <summary>
        /// Maximum steering angle.
        /// </summary>
        [Range(0f, 90f)] public float steerAngle = 40f;

        /// <summary>
        /// Direction of the vehicle. 1 is forward, -1 is reverse.
        /// </summary>
        [Range(-1, 1)] public int direction = 1;

        /// <summary>
        /// Current speed as kmh.
        /// </summary>
        public float currentSpeed = 0f;

        /// <summary>
        /// Timer when vehicle stops at waypoint.
        /// </summary>
        public float waitingAtWaypoint = 0f;

        /// <summary>
        /// Stop now?
        /// </summary>
        public bool stopNow = false;

        /// <summary>
        /// Current waypoint of the lane.
        /// </summary>
        public RTC_Waypoint currentWaypoint;

        /// <summary>
        /// Current waypoint of the lane.
        /// </summary>
        public RTC_Waypoint nextWaypoint;

        /// <summary>
        /// Current waypoint of the lane.
        /// </summary>
        public RTC_Waypoint pastWaypoint;

        public TurnSignalsSystem turnSignalsSystem;
    
        [Range(.01f, 1f)] public float lookAhead = .125f;

        /// <summary>
        /// Current lane.
        /// </summary>
        public RTC_Lane CurrentLane {

            get {

                if (currentWaypoint && currentWaypoint.connectedLane)
                    return currentWaypoint.connectedLane;
                else
                    return null;

            }

        }

        /// <summary>
        /// Next lane.
        /// </summary>
        public RTC_Lane NextLane {

            get {

                if (nextWaypoint && nextWaypoint.connectedLane)
                    return nextWaypoint.connectedLane;
                else
                    return null;

            }

        }

        /// <summary>
        /// Interconnecting now?
        /// </summary>
        public bool interconnecting = false;

        /// <summary>
        /// Will turn left?
        /// </summary>
        public bool willTurnLeft = false;

        /// <summary>
        /// Will turn right?
        /// </summary>
        public bool willTurnRight = false;

        /// <summary>
        /// Headlights will be enabled on night.
        /// </summary>
        public bool isNight = false;

        /// <summary>
        ///  Resets the vehicle after upside down.
        /// </summary>
        public bool checkUpsideDown = false;
        public float m_checkUpsideDown = 0f;

        /// <summary>
        /// Vehicle won't move after the collision.
        /// </summary>
        public bool canCrash = false;

        /// <summary>
        /// Crash impact limit.
        /// </summary>
        public float crashImpact = 3f;

        /// <summary>
        /// Disables vehicle after crash.
        /// </summary>
        public float disableAfterCrash = 0f;

        /// <summary>
        /// Disables vehicle after crash.
        /// </summary>
        public float m_disableAfterCrash = 0f;

        /// <summary>
        /// Crashed?
        /// </summary>
        public bool crashed = false;

        /// <summary>
        /// Can takeover if there is an obstacle?
        /// </summary>
        public bool canTakeover = false;

        /// <summary>
        ///  Stopped for a reason? For example, stopped for traffic light, or stopped for bus stop, etc...
        /// </summary>
        public bool stoppedForReason = false;

        /// <summary>
        ///  How long the vehicle stops?
        /// </summary>
        public float stoppedTime = 0f;

        /// <summary>
        /// Time needed to takeover.
        /// </summary>
        public float timeNeededToTakeover = 3f;

        /// <summary>
        /// Currently passing the obstacle?
        /// </summary>
        public bool passingObstacle = false;

        /// <summary>
        /// Ignoring throttle, brake, and steer inputs.
        /// </summary>
        public bool ignoreInputs = false;

        /// <summary>
        ///  Using side raycasts to pass the obstacle.
        /// </summary>
        public bool useSideRaycasts = false;

        /// <summary>
        /// Timer for currently overtaking.
        /// </summary>
        public float overtakingTimer = 0f;
        public float waitForHorn = 0f;

        /// <summary>
        /// Use smoothed inputs instead of raw inputs.
        /// </summary>
        public bool smoothInputs = true;

        /// <summary>
        /// Use raycasts to detect obstacles and traffic lights.
        /// </summary>
        public bool useRaycasts = true;

        /// <summary>
        /// Hits collected by the raycast.
        /// </summary>
        public List<RaycastHit> hit = new List<RaycastHit>();

        /// <summary>
        /// Order for casting right / middle / left raycasts.
        /// </summary>
        public int raycastOrder = -1;

        /// <summary>
        ///  Raycast layermask.
        /// </summary>
        public LayerMask raycastLayermask = -1;

        /// <summary>
        ///  Original and default distance of the raycast.
        /// </summary>
        private float raycastDistanceOrg = 3f;

        /// <summary>
        ///  Raycast distance.
        /// </summary>
        public float raycastDistance = 3f;

        /// <summary>
        /// Raycast length multiplier rate related to speed.
        /// </summary>
        public float raycastDistanceRate = 20f;

        /// <summary>
        /// Raycast hit distance.
        /// </summary>
        public float raycastHitDistance = 0f;

        /// <summary>
        /// Raycast pivot origin point.
        /// </summary>
        public Vector3 raycastOrigin = new Vector3(0f, 0f, 0f);

        /// <summary>
        ///  Raycasted vehicle.
        /// </summary>
        public RTC_CarController raycastedVehicle = null;

        /// <summary>
        /// Lights.
        /// </summary>
        public enum LightType { Brake, Indicator_L, Indicator_R, Headlight }

        [System.Serializable]
        public class Lights {

            public Light light;
            public LightType lightType = LightType.Brake;
            public float intensity = 1f;
            public float smoothness = 20f;
            public MeshRenderer meshRenderer;
            public string shaderKeyword = "_EmissionColor";
            public int materialIndex = 0;

        }

        /// <summary>
        /// Lights.
        /// </summary>
        public Lights[] lights;

        /// <summary>
        /// Indicator timer.
        /// </summary>
        private float indicatorTimer = 0f;

        /// <summary>
        /// Spawn position offset will be applied on enable.
        /// </summary>
        public Vector3 spawnPositionOffset = new Vector3(0f, .5f, 0f);

        /// <summary>
        /// Navigator will point direction to the destination.
        /// </summary>
        private Transform navigator;

        /// <summary>
        /// Nearest point to the line.
        /// </summary>
        private Transform navigatorPoint;

        /// <summary>
        /// Projector.
        /// </summary>
        private BoxCollider projection;

        /// <summary>
        /// Optimization will enable / disable heavier processes.
        /// </summary>
        public bool optimization = true;

        /// <summary>
        /// Distance limit for optimization.
        /// </summary>
        public float distanceForLOD = 100f;

        /// <summary>
        /// Aligning wheel models.
        /// </summary>
        public bool wheelAligning = true;

        /// <summary>
        /// Lighting.
        /// </summary>
        public bool lighting = true;

        /// <summary>
        /// Sounding.
        /// </summary>
        public bool sounding = true;

        /// <summary>
        /// Engine on sound audiosource.
        /// </summary>
        public AudioSource engineSoundOnSource;

        /// <summary>
        /// Engine off sound audiosource.
        /// </summary>
        public AudioSource engineSoundOffSource;

        /// <summary>
        /// Horn sound audiosource.
        /// </summary>
        public AudioSource hornSource;

        /// <summary>
        /// Engine on sound clip..
        /// </summary>
        public AudioClip engineSoundOn;

        /// <summary>
        /// Engine off sound clip..
        /// </summary>
        public AudioClip engineSoundOff;

        /// <summary>
        /// Horn sound clip..
        /// </summary>
        public AudioClip horn;

        /// <summary>
        /// Minimum audiosource radius.
        /// </summary>
        public float minAudioRadius = 5f;

        /// <summary>
        /// Maximum audiosource radius.
        /// </summary>
        public float maxAudioRadius = 50f;

        /// <summary>
        /// Minimum audiosource volume.
        /// </summary>
        public float minAudioVolume = .1f;

        /// <summary>
        /// Maximum audiosource volume.
        /// </summary>
        public float maxAudioVolume = .5f;

        /// <summary>
        /// Minimum audiosource pitch.
        /// </summary>
        public float minAudioPitch = .6f;

        /// <summary>
        /// Maximum audiosource pitch.
        /// </summary>
        public float maxAudioPitch = 1.2f;

        public int CarSpawnIndex;
    
        [System.Serializable]
        public class Paint {

            public MeshRenderer meshRenderer;
            public int materialIndex = 0;
            public string colorString = "_Color";

        }

        private bool mustStopAtTrafficLight = false;
        private Transform currentStopLine;
        private float stopBuffer = 2f;
    
        /// <summary>
        /// Paints.
        /// </summary>
        public Paint[] paints;

        /// <summary>
        /// Closer vehicles.
        /// </summary>
        public List<RTC_CarController> closerVehicles = new List<RTC_CarController>();

        /// <summary>
        /// When this vehicle spawned.
        /// </summary>
        /// <param name="trafficVehicle"></param>
        public delegate void onTrafficSpawned(RTC_CarController trafficVehicle);
        public static event onTrafficSpawned OnTrafficSpawned;

        /// <summary>
        /// When this vehicle de-spawned.
        /// </summary>
        /// <param name="trafficVehicle"></param>
        public delegate void onTrafficDeSpawned(RTC_CarController trafficVehicle);
        public static event onTrafficDeSpawned OnTrafficDeSpawned;

        //  Events can be used on enable.
        public RTC_Event_Output outputEvent_OnEnable = new RTC_Event_Output();
        public RTC_Output outputOnEnable = new RTC_Output();

        //  Events can be used on disable.
        public RTC_Event_Output outputEvent_OnDisable = new RTC_Event_Output();
        public RTC_Output outputOnDisable = new RTC_Output();

        //  Events can be used on collision.
        public RTC_Event_Output outputEvent_OnCollision = new RTC_Event_Output();
        public RTC_Output outputOnCollision = new RTC_Output();

        private bool _isTurnSignalWorking;
    
        private void Awake() {

            //  Car controller component.
            carController = this;

            //  Getting rigidbody and setting center of mass position.
            Rigid.centerOfMass = transform.InverseTransformPoint(COM.position);

            //  Getting original raycast distance value.
            raycastDistanceOrg = raycastDistance;

            //  Creating navigator. It will calculate the correct direction angle.
            GameObject navigatorGO = new GameObject("Navigator");
            navigatorGO.transform.SetParent(transform);
            navigator = navigatorGO.transform;
            navigator.localPosition = Vector3.zero;
            navigator.localRotation = Quaternion.identity;

            //  Creating navigator point. It will find the nearest point on the line.
            GameObject navigatorPointGO = new GameObject("NavigatorPoint");
            navigatorPointGO.transform.SetParent(transform);
            navigatorPoint = navigatorPointGO.transform;
            navigatorPoint.localPosition = Vector3.zero;
            navigatorPoint.localRotation = Quaternion.identity;

            //  Creating projector. It will be used to interrupt upcoming vehicles.
            GameObject projectorGO = new GameObject("Projector");
            projectorGO.transform.SetParent(transform);
            projection = projectorGO.AddComponent<BoxCollider>();
            projection.isTrigger = true;
            projection.transform.localPosition = new Vector3(0f, 0f, bounds.front);
            projection.transform.localRotation = Quaternion.identity;
            projection.transform.localScale = new Vector3(bounds.right * 2f, bounds.up * 2f, 1f);

            //  Creating engine sounds.
            if (engineSoundOn)
                engineSoundOnSource = RTC.NewAudioSource(gameObject, Vector3.zero, engineSoundOn.name, minAudioRadius, maxAudioRadius, 0f, engineSoundOn, true, true);

            //  Creating engine sounds.
            if (engineSoundOff)
                engineSoundOffSource = RTC.NewAudioSource(gameObject, Vector3.zero, engineSoundOff.name, minAudioRadius, maxAudioRadius, 0f, engineSoundOff, true, true);

            //  Creating horn sound.
            if (horn)
                hornSource = RTC.NewAudioSource(gameObject, Vector3.zero, horn.name, minAudioRadius, maxAudioRadius, .5f, horn, false, false);

        }

        private void OnEnable() {

            //  Calling this event when this vehicle spawned.
            if (OnTrafficSpawned != null)
                OnTrafficSpawned(this);

            //  Adding offset to the transform on enable.
            transform.position += spawnPositionOffset;

            //  If no current waypoint, find the closest one.
            if (!currentWaypoint)
                FindClosest();

            //  Resetting variables of the vehicle on enable.
            ResetVehicleOnEnable();

            //  Painting body renderer if selected.
            if (paints != null && paints.Length >= 1)
                PaintBody();

            //  Invoking event.
            outputEvent_OnEnable.Invoke(outputOnEnable);

        }

        private void Update() {
        
            Inputs();
            ClampInputs();
            Navigation();
            WheelAlign();
            VehicleLights();
            Optimization();
            Interaction();
            Audio();
            Takeover();
            Others();
        
        

            //  Setting timer for last shifting.
            if (lastTimeShifted > 0)
                lastTimeShifted -= Time.deltaTime;

            //  Clamping timer.
            lastTimeShifted = Mathf.Clamp(lastTimeShifted, 0f, 10f);
        }

        private void FixedUpdate() {

            //  Speed of the vehicle.
            currentSpeed = Rigid.velocity.magnitude * 3.6f;

            Throttle();
            Brake();
            Engine();
            Clutch();
            Gearbox();
            Steering();
            Raycasts();
            SideRaycasts();
            WheelColliders();

        }

        /// <summary>
        /// Getting other closer vehicles at front.
        /// </summary>
        private void Interaction() {

            if (closerVehicles == null)
                closerVehicles = new List<RTC_CarController>();

            //  Clearing the list first.
            closerVehicles.Clear();

            //  If all vehicles is not null...
            if (RTCSceneManager.allVehicles != null) {

                //  Looping all vehicles...
                for (int i = 0; i < RTCSceneManager.allVehicles.Length; i++) {

                    if (RTCSceneManager.allVehicles[i] != null) {

                        //  If distance is below 15 meters, and other traffic vehicle is at front side, add it to the list.
                        if (Vector3.Distance(transform.position, RTCSceneManager.allVehicles[i].transform.position) < 15f && RTCSceneManager.allVehicles[i] != this && RTCSceneManager.allVehicles[i].gameObject.activeSelf && Vector3.Dot((RTCSceneManager.allVehicles[i].transform.position - transform.position).normalized, transform.forward) > 0)
                            closerVehicles.Add(RTCSceneManager.allVehicles[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate bounds of the vehicle.
        /// </summary>
        public void CalculateBounds() {

            Quaternion rot = transform.rotation;
            transform.rotation = Quaternion.identity;

            Vector3 boundsSize = RTC_GetBounds.GetBounds(gameObject).size / 2f;

            bounds.front = boundsSize.z;
            bounds.rear = -boundsSize.z;
            bounds.right = boundsSize.x;
            bounds.left = -boundsSize.x;
            bounds.up = boundsSize.y;
            bounds.down = -boundsSize.y;

            transform.rotation = rot;
        }

        /// <summary>
        /// Audio.
        /// </summary>
        private void Audio() {

            //  If sounding is off, stop the audiosources.
            if (!sounding) {

                if (engineSoundOnSource && engineSoundOnSource.isPlaying)
                    engineSoundOnSource.Stop();

                if (engineSoundOffSource && engineSoundOffSource.isPlaying)
                    engineSoundOffSource.Stop();

                if (hornSource && hornSource.isPlaying)
                    hornSource.Stop();

                return;
            }

            //  If engine sound on is selected, adjust volume and pitch based on throttle / speed.
            if (engineSoundOnSource) {

                if (engineRunning) {

                    engineSoundOnSource.volume = Mathf.Lerp(minAudioVolume, maxAudioVolume, throttleInput);
                    engineSoundOnSource.pitch = Mathf.Lerp(minAudioPitch, maxAudioPitch, currentEngineRPM / maxEngineRPM);

                } else {

                    engineSoundOnSource.volume = 0f;
                    engineSoundOnSource.pitch = 0f;

                }

                if (engineSoundOnSource.isActiveAndEnabled && !engineSoundOnSource.isPlaying)
                    engineSoundOnSource.Play();
            }

            //  If engine sound off is selected, adjust volume and pitch based on throttle / speed.
            if (engineSoundOffSource) {

                if (engineRunning) {

                    engineSoundOffSource.volume = Mathf.Lerp(maxAudioVolume, minAudioVolume, throttleInput);
                    engineSoundOffSource.pitch = Mathf.Lerp(minAudioPitch, maxAudioPitch, currentEngineRPM / maxEngineRPM);

                } else {

                    engineSoundOffSource.volume = 0f;
                    engineSoundOffSource.pitch = 0f;

                }

                if (engineSoundOffSource.isActiveAndEnabled && !engineSoundOffSource.isPlaying)
                    engineSoundOffSource.Play();

            }

            if (raycastedVehicle && !raycastedVehicle.stoppedForReason && raycastedVehicle.currentSpeed < 10f)
                waitForHorn += Time.deltaTime;
            else
                waitForHorn = 0f;

            if (waitForHorn > 10f && !hornSource.isPlaying) {

                waitForHorn = 0f;
                hornSource.Play();

            }

        }

        /// <summary>
        /// Takeover.
        /// </summary>
        private void Takeover() {

            //  If crashed, return.
            if (crashed)
                return;

            //  If disabled, or stopNow, return.
            if (!canTakeover || stopNow) {

                stoppedTime = 0f;
                return;

            }

            //  If current speed is below 2, increase timer. Otherwise set timer to 0.
            if (currentSpeed <= 2) {

                if (!stoppedForReason)
                    stoppedTime += Time.deltaTime;
            } 
            else {

                stoppedTime = 0f;

            }

            //  If timer is above the limit, try to pass the obstacle.
            if (stoppedTime >= timeNeededToTakeover) {

                if (!passingObstacle)
                    StartCoroutine("Reverse");

            }

        }

        /// <summary>
        /// Others.
        /// </summary>
        private void Others() {

            if (m_disableAfterCrash > 0)
                m_disableAfterCrash -= Time.deltaTime;

            if (m_disableAfterCrash < 0) {

                m_disableAfterCrash = 0f;
                gameObject.SetActive(false);

            }

            if (checkUpsideDown) {

                if (Vector3.Dot(transform.up, Vector3.down) > -.1f) {

                    m_checkUpsideDown += Time.deltaTime;

                    if (m_checkUpsideDown >= 3f) {

                        m_checkUpsideDown = 0f;

                        Vector3 fwd = transform.forward;
                        transform.rotation = Quaternion.identity;
                        transform.forward = fwd;

                    }

                }

            }

        }

        /// <summary>
        /// Setting inputs smoothed or raw.
        /// </summary>
        private void Inputs() {

            //  Return if ignore inputs are enabled.
            if (ignoreInputs)
                return;

            //  If crashed, return.
            if (crashed) {

                throttleInput = 0f;
                brakeInput = 1f;
                return;

            }

            //  Smoothing inputs or getting directly raw inputs.
            if (smoothInputs) {

                throttleInput = Mathf.MoveTowards(throttleInput, throttleInputRaw, Time.deltaTime * 5f);
                brakeInput = Mathf.MoveTowards(brakeInput, brakeInputRaw, Time.deltaTime * 10f);
                steerInput = Mathf.MoveTowards(steerInput, steerInputRaw, Time.deltaTime * 10f);

            } else {

                throttleInput = throttleInputRaw;
                brakeInput = brakeInputRaw;
                steerInput = steerInputRaw;

            }

            //  If vehicle is stopped now, override throttle input and brake input.
            if (stopNow) {

                throttleInput = 0f;
                brakeInput = 1f;

            }

        }

        /// <summary>
        /// Clamps all inputs.
        /// </summary>
        private void ClampInputs() {

            throttleInput = Mathf.Clamp01(throttleInput);
            brakeInput = Mathf.Clamp01(brakeInput);
            steerInput = Mathf.Clamp(steerInput, -1f, 1f);

            throttleInputRaw = Mathf.Clamp01(throttleInputRaw);
            brakeInputRaw = Mathf.Clamp01(brakeInputRaw);
            steerInputRaw = Mathf.Clamp(steerInputRaw, -1f, 1f);

        }

        /// <summary>
        /// Raycasting and getting hit info.
        /// </summary>
        private void Raycasts()
        {
            TrafficLight trafficLight = null;
        
            //  If raycasting is disabled, return.
            if (!useRaycasts || useSideRaycasts || crashed) {

                raycastHitDistance = 0f;
                raycastedVehicle = null;
                return;

            }

            raycastOrder++;

            if (raycastOrder >= 2)
                raycastOrder = -1;

            //  Calculating length of the raycast based on vehicle speed.
            raycastDistance = Mathf.Lerp(raycastDistance, raycastDistanceOrg * (currentSpeed / 100f) * raycastDistanceRate, Time.fixedDeltaTime * 5f);

            //  Make sure length of the ray is not smaller than the original value.
            if (raycastDistance < raycastDistanceOrg)
                raycastDistance = raycastDistanceOrg;

            RaycastHit[] hits;
            RaycastHit firstHit = new RaycastHit();     //  First raycast hit.
            Vector3 rayDirection = Quaternion.Euler(0f, (steerInput * steerAngle) * .9f, 0f) * transform.forward;       //  Ray direction to forward.
            Vector3 rayOrigin = new Vector3(bounds.right * .85f * Mathf.Clamp(raycastOrder, -1, 1), 0f, 0f);        //  Right / middle / left raycast order.

            //  Drawing rays inside the editor.
            Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0f, 0f, bounds.front) + raycastOrigin + rayOrigin), rayDirection * raycastDistance, Color.white);

            //  Raycasting to direction.
            hits = Physics.RaycastAll(transform.position + transform.TransformDirection(new Vector3(0f, 0f, bounds.front) + raycastOrigin + rayOrigin), rayDirection, raycastDistance, raycastLayermask);

            //  Adding hits to the list.
            for (int i = 0; i < hits.Length; i++) {

                if (!hit.Contains(hits[i]))
                    hit.Add(hits[i]);

            }

            //  Closest hit.
            float closestHit = raycastDistance;

            //  Looping raycast hits, and make sure it's not child object of the vehicle. Finding first and closer hit.
            for (int i = 0; i < hit.Count; i++) {

                if (!hit[i].transform.IsChildOf(transform) && hit[i].distance < closestHit) {

                    closestHit = hit[i].distance;
                    firstHit = hit[i];
                }
            }

            if (firstHit.point != Vector3.zero) {

                //  If raycasted object is another traffic vehicle, take it.
                if (!raycastedVehicle)
                {
                    raycastedVehicle = firstHit.transform.gameObject.GetComponentInParent<RTC_CarController>();
                }

                //  Setting hit distance.
                raycastHitDistance = firstHit.distance;

                //  If raycasted, but raycasted object's layer is ignorable, set distance to 0.
                if (firstHit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
                    raycastHitDistance = 0f;

                //  If raycasted, but raycasted object's layer is traffic light, set stopped for reason to true. Otherwise to false.
                if (firstHit.transform.gameObject.layer == LayerMask.NameToLayer("RTC_TrafficLight"))
                {
                    // Сначала ищем у root
                    trafficLight = firstHit.transform.root.GetComponent<TrafficLight>();

                    // Если не нашли — ищем у детей root-а
                    if (trafficLight == null)
                        trafficLight = firstHit.transform.root.GetComponentInChildren<TrafficLight>();

                    if (trafficLight != null &&
                        trafficLight.LightIndex == CarSpawnIndex &&
                        trafficLight.IsRedLight)
                    {
                        print(trafficLight.name);
                        StopAtTrafficLight(trafficLight, trafficLight.StopLine);
                    }
                }
                else
                {
                    mustStopAtTrafficLight = false;
                    stoppedForReason = false;
                }

                //  Drawing hit ray.
                if (raycastHitDistance != 0)
                    Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0f, 0f, bounds.front) + raycastOrigin + rayOrigin), rayDirection * firstHit.distance, Color.red);

            } else {

                if (raycastOrder == 1) {

                    //  If first hit doesn't exists, set raycastedVehicle to null.
                    raycastedVehicle = null;
                    raycastHitDistance = 0f;
                    stoppedForReason = false;

                }

            }
        
            // if (raycastedVehicle)
            // {
            //     FollowVehicle(raycastedVehicle);
            // }
            // else
            // {
            //     // Если тормоз был очень сильный → быстро убираем
            //     brakeTorque = Mathf.Lerp(brakeInput, 0f, Time.deltaTime * 1f); // быстрее, чем раньше
            //     //engineRunning = true;
            //     stoppedForReason = false;
            //
            //     // Дополнительно: если машина почти стоит, подталкиваем движение
            //     if (currentSpeed < 2f)
            //     {
            //         // Можно сделать плавное восстановление управления движением
            //         // Например, включить флаг resumeCruising
            //     }
            // }

            //  Clearing the hit list.
            if (raycastOrder == 1)
                hit.Clear();

        }
    
        private void FollowVehicle(RTC_CarController targetVehicle)
        {
            // if (!targetVehicle) return;
            //
            // float distanceToTarget = raycastHitDistance;
            // float mySpeed = currentSpeed;
            // float targetSpeed = targetVehicle.currentSpeed;
            //
            // // --- параметры поведения ---
            // float stopDistance = 3f;       // зазор при полной остановке
            // float reactionZone = 15f;      // начинаем тормозить чуть раньше
            // float maxBrake = 2000000f;     // усилен тормоз
            //
            // if (distanceToTarget > reactionZone) return;
            //
            // stoppedForReason = true;
            // engineRunning = true;
            //
            // // --- расчёт силы торможения ---
            // float brakeFactor = 1f - Mathf.InverseLerp(stopDistance, reactionZone, distanceToTarget);
            // float speedFactor = Mathf.Clamp01((mySpeed - targetSpeed) / 3f); // быстрее реагируем на разницу скоростей
            //
            // float targetBrake = Mathf.Lerp(0f, maxBrake, brakeFactor * (0.7f + speedFactor * 0.3f));
            //
            // // 🚀 Резкий отклик
            // brakeInput = Mathf.Lerp(brakeInput, targetBrake, Time.deltaTime * 8f);
            //
            // // --- логика полной остановки ---
            // bool targetStopped = targetSpeed < 0.5f;
            // bool tooClose = distanceToTarget <= stopDistance + 0.3f;
            //
            // if (tooClose && targetStopped)
            // {
            //     brakeInput = maxBrake;
            //     rigid.velocity = Vector3.zero;
            //     currentSpeed = 0f;
            //     mustStopAtTrafficLight = true;
            // }
        }
    
        private void StopAtTrafficLight(TrafficLight trafficLight, Transform stopLine)
        {
            if (!trafficLight.IsRedLight) return;
        
            float distanceToStop = Vector3.Distance(transform.position, stopLine.position);
        
            float reactionZone = 20f;
        
            if (distanceToStop > reactionZone) return;
        
            engineRunning = false;
            stoppedForReason = true;
        
            float stopDistance = 2f;
        
            float brakeFactor = Mathf.InverseLerp(reactionZone, stopDistance, distanceToStop);
        
            brakeFactor = Mathf.Clamp01(brakeFactor);
        
            Debug.Log($"Distance: {distanceToStop:F2}  BrakeFactor: {brakeFactor:F2}");
        
            if (distanceToStop <= stopDistance)
            {
                brakeInput = 1f;
                mustStopAtTrafficLight = true;
            }
        }
    
        /// <summary>
        /// Sideraycasts will be used to takeover. Only for a few seconds.
        /// </summary>
        private void SideRaycasts() {

            //  If using side raycasting is disabled, return.
            if (!useSideRaycasts || crashed)
                return;

            RaycastHit[] hit;     //  Raycast hits.
            RaycastHit rightHit = new RaycastHit();     //  First raycast hit.
            Vector3 rightDirection = Quaternion.Euler(0f, 5f, 0f) * transform.forward;       //  Ray direction to forward.

            //  Drawing rays inside the editor.
            Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(bounds.right, 0f, bounds.rear)), rightDirection * 10f, Color.white);

            //  Raycasting to direction.
            hit = Physics.RaycastAll(transform.position + transform.TransformDirection(new Vector3(bounds.right, 0f, bounds.rear)), rightDirection, 10f, raycastLayermask);

            float closestHit = 10f;

            //  Looping raycast hits, and make sure it's not child object of the vehicle. Finding first and closer hit.
            for (int i = 0; i < hit.Length; i++) {

                if (!hit[i].transform.IsChildOf(transform) && hit[i].distance < closestHit && hit[i].transform.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")) {

                    closestHit = hit[i].distance;
                    rightHit = hit[i];

                }

            }

            //  If first hit, draw ray and calculate the hit distance.
            if (rightHit.point != Vector3.zero) {

                overtakingTimer = 1f;
                steerInputRaw = -(2f - Mathf.InverseLerp(0f, 10f, rightHit.distance));

            }

            //  Drawing hit ray.
            if (rightHit.point != Vector3.zero)
                Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(bounds.right, 0f, bounds.rear)), rightDirection * rightHit.distance, Color.red);

            RaycastHit leftHit = new RaycastHit();     //  First raycast hit.
            Vector3 leftDirection = Quaternion.Euler(0f, -5f, 0f) * transform.forward;       //  Ray direction to forward.

            //  Drawing rays inside the editor.
            Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(bounds.left, 0f, bounds.rear)), leftDirection * 10f, Color.white);

            //  Raycasting to direction.
            hit = Physics.RaycastAll(transform.position + transform.TransformDirection(new Vector3(bounds.left, 0f, bounds.rear)), leftDirection, 10f, raycastLayermask);

            //  Looping raycast hits, and make sure it's not child object of the vehicle. Finding first and closer hit.
            for (int i = 0; i < hit.Length; i++) {

                if (!hit[i].transform.IsChildOf(transform) && hit[i].distance < closestHit) {

                    closestHit = hit[i].distance;
                    leftHit = hit[i];

                }

            }

            //  If first hit, draw ray and calculate the hit distance.
            if (leftHit.point != Vector3.zero) {

                overtakingTimer = 1f;
                steerInputRaw = (2f - Mathf.InverseLerp(0f, 10f, rightHit.distance));

            }

            //  Drawing hit ray.
            if (leftHit.point != Vector3.zero)
                Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(bounds.left, 0f, bounds.rear)), leftDirection * leftHit.distance, Color.red);


        }

        /// <summary>
        /// Visually aligns all wheels.
        /// </summary>
        private void WheelAlign() {

            //  Return if wheelAligning is disabled.
            if (!wheelAligning)
                return;

            //  Looping all wheels.
            for (int i = 0; i < wheels.Length; i++) {

                if (wheels[i] != null && wheels[i].wheelCollider != null && wheels[i].wheelModel != null) {

                    //  Position and rotation.
                    Vector3 wheelPos;
                    Quaternion wheelRot;

                    //  Getting world pose.
                    wheels[i].wheelCollider.GetWorldPose(out wheelPos, out wheelRot);

                    //  And setting position and rotation of the wheel.
                    wheels[i].wheelModel.transform.SetPositionAndRotation(wheelPos, wheelRot);

                }

            }

        }

        /// <summary>
        /// Sets motor torque, steer angle, and brake torque with inputs.
        /// </summary>
        private void WheelColliders() {

            if (wheels != null) {

                for (int i = 0; i < wheels.Length; i++) {

                    if (wheels[i] != null && wheels[i].wheelCollider != null) {

                        if (wheels[i].isTraction && (wheelRPM2Speed * .5f) < targetWheelSpeedForCurrentGear)
                            wheels[i].wheelCollider.motorTorque = (engineTorqueCurve.Evaluate(currentEngineRPM) * gearRatios[currentGear] * differentialRatio) * throttleInput * direction * (1f - clutchInput) * fuelInput;
                        else
                            wheels[i].wheelCollider.motorTorque = 0f;

                        if (wheels[i].isSteering)
                            wheels[i].wheelCollider.steerAngle = steerAngle * steerInput;
                        else
                            wheels[i].wheelCollider.steerAngle = 0f;

                        if (wheels[i].isBraking)
                            wheels[i].wheelCollider.brakeTorque = brakeTorque * brakeInput;
                        else
                            wheels[i].wheelCollider.brakeTorque = 0f;

                    }

                }

            }

        }

        /// <summary>
        /// Calculating throttle input clamped 0f - 1f.
        /// </summary>
        private void Throttle() {

            //  Make sure throttle input is set to 0 before calculation.
            throttleInputRaw = 0f;

            //  If no current waypoint, throttle input would be 0 and vehicle will stop.
            if (!currentWaypoint)
                return;

            //  Adjusting throttle input based on vehicle speed - desired speed relationship.
            throttleInputRaw = 1f - Mathf.InverseLerp(0f, desiredSpeed, currentSpeed);

            //  Decreasing throttle input related to steer input.
            throttleInputRaw -= (Mathf.Abs(steerInput) / 5f);

            //  Make sure throttle input is not lower than 0.1f below 15 kmh.
            if (currentSpeed < 15 && throttleInputRaw < .1f)
                throttleInputRaw = .1f;

            //  If current speed is above desired speed, set it to 0.
            if (currentSpeed > desiredSpeed)
                throttleInputRaw = 0f;

            if (brakeInputRaw > .05f)
                throttleInputRaw = 0f;

            if (gearShiftingNow)
                throttleInputRaw = 0f;

            //  If there is a raycasted object at front of the vehicle, decrease throttle input based on raycast hit distance.
            if (raycastHitDistance != 0)
                throttleInputRaw -= (1f - Mathf.InverseLerp(0f, raycastDistance, raycastHitDistance));

        }

        /// <summary>
        /// Calculating brake input clamped 0f - 1f.
        /// </summary>
        private void Brake() {

            //  Make sure brake input is set to 0 before calculation.
            brakeInputRaw = 0f;

            //  If no current waypoint, brake input would be 1 and vehicle will stop.
            if (!currentWaypoint) {

                brakeInputRaw = 1f;
                return;

            }

            //  If current speed is above desired speed, set it to 1.
            if (currentSpeed > desiredSpeed)
                brakeInputRaw = 1f;

            //  Increase brake input related to steer input.
            brakeInputRaw += (Mathf.Abs(steerInput) / 5f);

            //  Make sure brake input is 0 if speed is below 15.
            if (currentSpeed < 15 && brakeInputRaw != 0f)
                brakeInputRaw = 0f;

            //  If there is a raycasted object at front of the vehicle, increase brake input based on raycast hit distance.
            if (raycastHitDistance != 0f)
                brakeInputRaw = (1f - Mathf.InverseLerp(0f, raycastDistance, raycastHitDistance));

        }

        /// <summary>
        /// Calculating steer input clamped -1f - 1f.
        /// Calculating by angles of the vehicle and waypoint.
        /// </summary>
        private void Steering() {

            //  If navigator is looking at the waypoint...
            if (navigator.transform.localRotation != Quaternion.identity) {

                // get a "forward vector" for each rotation
                var forwardA = transform.rotation * Vector3.forward;
                var forwardB = navigator.rotation * Vector3.forward;

                // get a numeric angle for each vector, on the X-Z plane (relative to world forward)
                var angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
                var angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;

                // get the signed difference in these angles
                var angleDiff = Mathf.DeltaAngle(angleA, angleB);

                //  Set the steer input.
                if (!Mathf.Approximately(angleDiff, 0f))
                    steerInputRaw = angleDiff / 35f;
                else
                    steerInputRaw = 0f;

            } else {

                //  Set the steer input to 0 if navigator is not looking at the waypoint.
                steerInputRaw = 0f;

            }

        }

        /// <summary>
        /// Engine.
        /// </summary>
        private void Engine() {

            //  If engine rpm is below the minimum, raise the idle input.
            if (currentEngineRPM <= (minEngineRPM + (minEngineRPM / 5f)))
                idleInput = Mathf.Clamp01(Mathf.Lerp(1f, 0f, currentEngineRPM / (minEngineRPM + (minEngineRPM / 5f))));
            else
                idleInput = 0f;

            //  Setting fuel input.
            fuelInput = throttleInput + idleInput;

            //  Clamping fuel input.
            fuelInput = Mathf.Clamp01(throttleInput + idleInput);

            //  If engine rpm exceeds the maximum rpm, cut the fuel.
            if (currentEngineRPM >= maxEngineRPM)
                fuelInput = 0f;

            //  If engine is not running, set fuel and idle input to 0.
            if (!engineRunning) {

                fuelInput = 0f;
                idleInput = 0f;

            }

            //  Calculating average traction wheel rpm.
            float averagePowerWheelRPM = 0f;
            int totalTractionWheels = 0;

            if (wheels != null) {

                for (int i = 0; i < wheels.Length; i++) {

                    if (wheels[i] != null && wheels[i].wheelCollider != null && wheels[i].wheelCollider.enabled) {

                        if (wheels[i].isTraction) {

                            totalTractionWheels++;
                            averagePowerWheelRPM += Mathf.Abs(wheels[i].wheelCollider.rpm);

                        }

                    }

                }

                if (averagePowerWheelRPM > .1f)
                    averagePowerWheelRPM /= (float)Mathf.Clamp(totalTractionWheels, 1f, 40f);

            }

            //  Calculating average traction wheel radius.
            float averagePowerWheelRadius = 0f;

            if (wheels != null) {

                for (int i = 0; i < wheels.Length; i++) {

                    if (wheels[i] != null && wheels[i].wheelCollider != null && wheels[i].wheelCollider.enabled) {

                        if (wheels[i].isTraction)
                            averagePowerWheelRadius += wheels[i].wheelCollider.radius;

                    }

                }

                if (averagePowerWheelRadius >= .1f)
                    averagePowerWheelRadius /= (float)Mathf.Clamp(totalTractionWheels, 1f, 40f);

            }

            //  Converting traction wheel rpm to engine rpm.
            tractionWheelRPM2EngineRPM = (averagePowerWheelRPM * differentialRatio * gearRatios[currentGear]) * (1f - clutchInput) * 1f;

            //  Calculating raw engine rpm.
            wantedEngineRPMRaw += clutchInput * (fuelInput * maxEngineRPM) * Time.fixedDeltaTime;
            wantedEngineRPMRaw += (1f - clutchInput) * (tractionWheelRPM2EngineRPM - wantedEngineRPMRaw) * Time.fixedDeltaTime * 5f;
            wantedEngineRPMRaw -= .15f * maxEngineRPM * Time.fixedDeltaTime * 1f;
            wantedEngineRPMRaw = Mathf.Clamp(wantedEngineRPMRaw, 0f, maxEngineRPM);

            //  Smoothing the engine rpm.
            currentEngineRPM = Mathf.SmoothDamp(currentEngineRPM, wantedEngineRPMRaw, ref engineVelocity, .15f);

            //  Converting wheel rpm to speed as km/h unit.
            wheelRPM2Speed = (averagePowerWheelRPM * averagePowerWheelRadius * Mathf.PI * 2f) * 60f / 1000f;

            //  Calculating target max speed for the current gear.
            targetWheelSpeedForCurrentGear = currentEngineRPM / gearRatios[currentGear] / differentialRatio;
            targetWheelSpeedForCurrentGear *= (averagePowerWheelRadius * Mathf.PI * 2f) * 60f / 1000f;

        }

        /// <summary>
        /// Adjusting clutch input based on engine rpm and speed.
        /// </summary>
        private void Clutch() {

            //  Apply input 1 if engine rpm drops below the engage rpm.
            if (currentEngineRPM <= engageClutchRPM) {

                clutchInputRaw = 1f;

                //  If engine rpm is above the engage rpm, but vehicle is on low speeds, calculate the estimated input based on vehicle speed and throttle input.
            } else if (Mathf.Abs(currentSpeed) < 20f) {

                clutchInputRaw = Mathf.Lerp(clutchInputRaw, (Mathf.Lerp(1f, (Mathf.Lerp(.5f, 0f, (Mathf.Abs(currentSpeed)) / 20f)), Mathf.Abs(throttleInput * direction))), Time.fixedDeltaTime * 20f);

                //  If vehicle speed is above the limit, and engine rpm is above the engage rpm, don't apply clutch.
            } else {

                clutchInputRaw = 0f;

            }

            //  If gearbox is shifting now, apply input.
            if (gearShiftingNow)
                clutchInputRaw = 1f;

            //  Smoothing the clutch input with inertia.
            clutchInput = Mathf.SmoothDamp(clutchInput, clutchInputRaw, ref clutchVelocity, .15f);

        }

        /// <summary>
        /// Calculates estimated speeds and rpms to shift up / down.
        /// </summary>
        private void Gearbox() {

            //  Creating float array for target speeds.
            float[] targetSpeeds = FindTargetSpeed();

            //  Creating low and high limits multiplied with threshold value.
            float lowLimit;
            float highLimit;

            //  If current gear is not first gear, there is a low limit.
            if (currentGear > 0)
                lowLimit = targetSpeeds[currentGear - 1];

            //  High limit.
            highLimit = targetSpeeds[currentGear];

            bool canShiftUpNow = false;

            //  If reverse gear is not engaged, engine rpm is above shiftup rpm, and wheel & vehicle speed is above the high limit, shift up.
            if (currentGear < gearRatios.Length && currentEngineRPM >= gearShiftUpRPM && wheelRPM2Speed >= highLimit && currentSpeed >= highLimit)
                canShiftUpNow = true;

            bool canShiftDownNow = false;

            //  If reverse gear is not engaged, engine rpm is below shiftdown rpm, and wheel & vehicle speed is below the low limit, shift down.
            if (currentGear > 0 && currentEngineRPM <= gearShiftDownRPM) {

                if (FindEligibleGear() != currentGear)
                    canShiftDownNow = true;
                else
                    canShiftDownNow = false;

            }

            if (!dontGearShiftTimer)
                lastTimeShifted = 0f;

            if (!gearShiftingNow && lastTimeShifted <= .02f) {

                if (canShiftDownNow)
                    ShiftToGear(FindEligibleGear());

                if (canShiftUpNow)
                    ShiftUp();

            }

        }

        /// <summary>
        /// Shift to specific gear.
        /// </summary>
        /// <param name="gear"></param>
        public void ShiftToGear(int gear) {

            StartCoroutine(ShiftTo(gear));

        }

        /// <summary>
        /// Shift up.
        /// </summary>
        public void ShiftUp() {

            if (currentGear < gearRatios.Length - 1)
                StartCoroutine(ShiftTo(currentGear + 1));

        }

        /// <summary>
        /// Shift to specific gear with delay.
        /// </summary>
        /// <param name="gear"></param>
        /// <returns></returns>
        private IEnumerator ShiftTo(int gear) {

            lastTimeShifted = 1f;
            gearShiftingNow = true;

            yield return new WaitForSeconds(gearShiftingTime);

            gear = Mathf.Clamp(gear, 0, gearRatios.Length - 1);
            currentGear = gear;
            gearShiftingNow = false;

        }

        /// <summary>
        /// Navigates the vehicle.
        /// </summary>
        private void Navigation() {

            //  If crashed or if no current waypoint, reset navigators, lanes, waypoints, and desired speed, and then return.
            if (crashed || !currentWaypoint) {

                navigator.transform.localRotation = Quaternion.identity;
                navigatorPoint.position = transform.position;

                desiredSpeed = 0f;
                nextWaypoint = null;
                pastWaypoint = null;
                passingObstacle = false;
                useSideRaycasts = false;
                willTurnRight = false;
                willTurnLeft = false;

                //  Adjusting projector's size, center, and angle.
                projection.size = new Vector3(projection.size.x, projection.size.y, currentSpeed / 4f);
                projection.center = new Vector3(0f, 0f, currentSpeed / 8f);
                projection.transform.localRotation = Quaternion.identity * Quaternion.Euler(0f, steerAngle * steerInput, 0f);

                return;

            }

            if (overtakingTimer > 0) {

                overtakingTimer -= Time.deltaTime;

                passingObstacle = true;
                useSideRaycasts = true;

            } else {

                passingObstacle = false;
                useSideRaycasts = false;

            }

            if (overtakingTimer < 0)
                overtakingTimer = 0;

            if (waitingAtWaypoint > 0) {

                waitingAtWaypoint -= Time.deltaTime;
                stopNow = true;

            } else {

                stopNow = false;

            }

            if (waitingAtWaypoint < 0)
                waitingAtWaypoint = 0;

            ////  If no current waypoint, reset navigators, lanes, waypoints, and desired speed.
            //if (!currentWaypoint) {

            //    navigator.transform.localRotation = Quaternion.identity;
            //    navigatorPoint.position = transform.position;

            //    desiredSpeed = 0f;
            //    nextWaypoint = null;
            //    pastWaypoint = null;
            //    passingObstacle = false;
            //    useSideRaycasts = false;
            //    willTurnRight = false;
            //    willTurnLeft = false;

            //    //  Adjusting projector's size, center, and angle.
            //    projection.size = new Vector3(projection.size.x, projection.size.y, currentSpeed / 4f);
            //    projection.center = new Vector3(0f, 0f, currentSpeed / 8f);
            //    projection.transform.localRotation = Quaternion.identity * Quaternion.Euler(0f, steerAngle * steerInput, 0f);

            //    return;

            //}

            //  Drawing a line between our current waypoint and past waypoint. Navigator will find the nearest position on the line and vehicle will try to keep in the lane.
            //  If vehicle passed a waypoint, register it as past waypoint to draw the line. If there is no past waypoint, navigator will directly aim to the current waypoint regardless the line direction.
            if (pastWaypoint) {

                navigatorPoint.position = RTC.NearestPointOnLine(currentWaypoint.transform.position, currentWaypoint.transform.position - pastWaypoint.transform.position, transform.position);
                navigatorPoint.position += (currentWaypoint.transform.position - pastWaypoint.transform.position).normalized * ((currentSpeed + 10f) * lookAhead);
                navigatorPoint.position = RTC.ClampVector(navigatorPoint.position, currentWaypoint.transform.position, pastWaypoint.transform.position);

            } else {

                navigatorPoint.position = transform.position;

            }

            //  If we're on the lane, aim for the navigator point. Otherwise aim for the current waypoint.
            if (pastWaypoint)
                navigator.LookAt(navigatorPoint);
            else
                navigator.LookAt(currentWaypoint.transform);

            //  Make sure only Y axis of the navigator is changed. Other axes are set to 0.
            navigator.transform.localEulerAngles = new Vector3(0f, navigator.transform.localEulerAngles.y, 0f);

            //  If distance to the waypoint is below the radius, pass to the next waypoint.
            if (Vector3.Distance(navigatorPoint.position, currentWaypoint.transform.position) <= currentWaypoint.radius)
                PassWaypoint();

            //  Checking the current waypoint from above PassWaypoint method.
            if (!currentWaypoint)
                return;

            //  If current waypoint is far behind, pass to the next waypoint. Otherwise vehicle will try to go reverse.
            if (Vector3.Distance(transform.position, currentWaypoint.transform.position) <= 10f && Vector3.Dot((currentWaypoint.transform.position - transform.position).normalized, transform.forward) < 0f)
                PassWaypoint();

            //  Checking the current waypoint from above PassWaypoint method.
            if (!currentWaypoint)
                return;

            //  If vehicle is interconnecting right now, get desired speed for interconnection.
            //  Otherwise, get desired speed for next waypoint.
            if (!interconnecting)
                desiredSpeed = currentWaypoint.desiredSpeedForNextWaypoint;
            else
                desiredSpeed = currentWaypoint.desiredSpeedForInterConnectionWaypoint;
        
            if (currentWaypoint.LeftTurnSignal && !_isTurnSignalWorking)
            {
                _isTurnSignalWorking = true;
                turnSignalsSystem.TurnOnLeftTurnSignal();
            }
            if(currentWaypoint.RightTurnSignal && !_isTurnSignalWorking)
            {
                _isTurnSignalWorking = true;
                turnSignalsSystem.TurnOnRightTurnSignal();
            }
            if(!currentWaypoint.LeftTurnSignal && !currentWaypoint.RightTurnSignal)
            {
                _isTurnSignalWorking = false;
                turnSignalsSystem.TurnOffAllSignals();
            }

            //  If desired speed is not 0, multiply desired speed from .75f - 1.25f related to distance to the waypoint.
            if (Vector3.Distance(navigatorPoint.position, currentWaypoint.transform.position) > 60f) {

                desiredSpeed = maximumSpeed;

            } else {

                if (desiredSpeed != 0)
                    desiredSpeed *= Mathf.Lerp(.75f, 1.25f, Mathf.InverseLerp(0f, 60f, Vector3.Distance(navigatorPoint.position, currentWaypoint.transform.position)));

            }

            //  Get turn angle by the next waypoints transform.
            if (nextWaypoint) {

                if (AngleDir(transform.forward, nextWaypoint.transform.forward, Vector3.up) >= .5f) {

                    willTurnRight = true;
                    willTurnLeft = false;

                } else if (AngleDir(transform.forward, nextWaypoint.transform.forward, Vector3.up) <= -.5f) {

                    willTurnRight = false;
                    willTurnLeft = true;

                } else {

                    willTurnRight = false;
                    willTurnLeft = false;

                }

            } else {

                willTurnRight = false;
                willTurnLeft = false;

            }

            //  Turn signals on if vehicle is close enough.
            if (currentWaypoint && Vector3.Distance(transform.position, currentWaypoint.transform.position) >= 40f) {

                willTurnRight = false;
                willTurnLeft = false;

            }

            //  Adjusting projector's size, center, and angle.
            projection.size = new Vector3(projection.size.x, projection.size.y, currentSpeed / 4f);
            projection.center = new Vector3(0f, 0f, currentSpeed / 8f);
            projection.transform.localRotation = Quaternion.identity * Quaternion.Euler(0f, steerAngle * steerInput, 0f);

        }

        /// <summary>
        /// Going reverse for 1 second.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Reverse() {

            passingObstacle = true;
            ignoreInputs = true;
            direction = -1;
            throttleInput = 1f;
            brakeInput = 0f;
            steerInput = 1f;

            yield return new WaitForSeconds(1);

            overtakingTimer = 1f;
            ignoreInputs = false;
            direction = 1;

        }

        /// <summary>
        /// Passes to the next waypoint, or to the interconnection waypoint.
        /// </summary>
        private void PassWaypoint() {

            if (currentWaypoint.wait > 0)
                waitingAtWaypoint = currentWaypoint.wait;

            //  Current waypoint would be next waypoint.
            pastWaypoint = currentWaypoint;
            currentWaypoint = nextWaypoint;

            //  If next waypoint has interconnection waypoint...
            if (nextWaypoint && nextWaypoint.interConnectionWaypoint) {

                //  Chance for setting to interconnection waypoint.
                int chance = Random.Range(0, 3);

                //  If we're in luck, interconnecting now. Otherwise, next waypoint will be used.
                if (chance == 1)
                    interconnecting = true;
                else
                    interconnecting = false;

            } else {

                //  If next waypoint doesn't have interconnection waypoint, set interconnecting to false.
                interconnecting = false;

            }

            if (!nextWaypoint)
                return;

            //  Setting next waypoint or interconnection waypoint. If chance is 1, but waypoint doesn't have interconnection waypoint, set next waypoint.
            if (!interconnecting) {

                if (nextWaypoint.nextWaypoint)
                    nextWaypoint = nextWaypoint.nextWaypoint;
                else if (nextWaypoint.interConnectionWaypoint)
                    nextWaypoint = nextWaypoint.interConnectionWaypoint;
                else
                    nextWaypoint = null;

            } else {

                if (nextWaypoint.interConnectionWaypoint) {

                    if (RTC.EqualVehicleType(vehicleType, nextWaypoint.interConnectionWaypoint.connectedLane))
                        nextWaypoint = nextWaypoint.interConnectionWaypoint;
                    else if (nextWaypoint.nextWaypoint)
                        nextWaypoint = nextWaypoint.nextWaypoint;
                    else
                        nextWaypoint = null;

                } else if (nextWaypoint.nextWaypoint) {

                    nextWaypoint = nextWaypoint.nextWaypoint;

                } else {

                    nextWaypoint = null;

                }

            }

        }

        /// <summary>
        /// Finds closest waypoint and lane.
        /// </summary>
        public void FindClosest() {

            //  Getting all waypoints from the scene manager.
            RTC_Waypoint[] allWaypoints = RTCSceneManager.allWaypoints;

            //  Return with null if waypoints not found.
            if (allWaypoints == null || allWaypoints.Length == 0) {

                //  Setting current waypoint and current lane.
                currentWaypoint = null;
                nextWaypoint = null;
                pastWaypoint = null;

                return;

            }

            //  Closest waypoint distance and index.
            float closestWaypoint = Mathf.Infinity;
            int index = 0;

            //  Checking distances to all waypoints.
            for (int i = 0; i < allWaypoints.Length; i++) {

                if (allWaypoints[i] != null) {

                    if (Vector3.Distance(transform.position, allWaypoints[i].transform.position) < closestWaypoint) {

                        closestWaypoint = Vector3.Distance(transform.position, allWaypoints[i].transform.position);
                        index = i;

                    }

                }

            }

            //  Setting current waypoint and current lane.
            currentWaypoint = allWaypoints[index];
            nextWaypoint = currentWaypoint;
            pastWaypoint = currentWaypoint.previousWaypoint;

        }

        /// <summary>
        /// Sets waypoint.
        /// </summary>
        /// <param name="waypoint"></param>
        public void SetWaypoint(RTC_Waypoint waypoint) {

            currentWaypoint = waypoint;
            nextWaypoint = currentWaypoint;

        }

        /// <summary>
        /// Operating vehicle lights based on steer input and brake input.
        /// </summary>
        private void VehicleLights() {

            if (!lighting) {

                if (lights != null) {

                    for (int i = 0; i < lights.Length; i++) {

                        if (lights[i].light != null)
                            lights[i].light.intensity = 0f;

                    }

                }

                return;

            }

            if (lights != null) {

                //  Looping all lights attached to the vehicle and adjusting their intensity values based on responsive inputs.
                for (int i = 0; i < lights.Length; i++) {

                    if (lights[i].light != null) {

                        switch (lights[i].lightType) {

                            case LightType.Headlight:

                                if (isNight)
                                    Lighting(lights[i], lights[i].intensity, lights[i].smoothness);
                                else
                                    Lighting(lights[i], 0f, lights[i].smoothness);

                                break;

                            case LightType.Brake:

                                if (brakeInput > .1f)
                                    Lighting(lights[i], lights[i].intensity, lights[i].smoothness);
                                else
                                    Lighting(lights[i], isNight ? .15f : 0f, lights[i].smoothness);

                                break;

                            case LightType.Indicator_R:

                                if (!crashed) {

                                    if (willTurnRight && indicatorTimer < .5f)
                                        Lighting(lights[i], lights[i].intensity, lights[i].smoothness);
                                    else
                                        Lighting(lights[i], 0f, lights[i].smoothness);

                                } else {

                                    if (indicatorTimer < .5f)
                                        Lighting(lights[i], lights[i].intensity, lights[i].smoothness);
                                    else
                                        Lighting(lights[i], 0f, lights[i].smoothness);

                                }

                                break;

                            case LightType.Indicator_L:

                                if (!crashed) {

                                    if (willTurnLeft && indicatorTimer < .5f)
                                        Lighting(lights[i], lights[i].intensity, lights[i].smoothness);
                                    else
                                        Lighting(lights[i], 0f, lights[i].smoothness);

                                } else {

                                    if (indicatorTimer < .5f)
                                        Lighting(lights[i], lights[i].intensity, lights[i].smoothness);
                                    else
                                        Lighting(lights[i], 0f, lights[i].smoothness);

                                }

                                break;

                        }

                    }

                }

            }

            indicatorTimer += Time.deltaTime;       //  Used on blinkers.

            //  If indicator timer is above 1 second, reset it to 0.
            if (indicatorTimer > 1f)
                indicatorTimer = 0;

        }

        /// <summary>
        /// Optimization.
        /// </summary>
        private void Optimization() {

            //  Return if not enabled.
            if (!optimization) {

                wheelAligning = true;
                lighting = true;
                sounding = true;
                return;

            }

            // Return if no main camera found.
            if (!Camera.main)
                return;

            //  Distance to the main camera.
            float distanceToCam = Vector3.Distance(transform.position, Camera.main.transform.position);

            //  If distance of the main camera is above the limit, disable wheel aligning and lighting. Otherwise enable them.
            if (distanceToCam > distanceForLOD) {

                wheelAligning = false;
                lighting = false;
                sounding = false;

            } else {

                wheelAligning = true;
                lighting = true;
                sounding = true;

            }

        }

        /// <summary>
        /// Paints the body with randomized color.
        /// </summary>
        private void PaintBody() {

            Color randomColor = Random.ColorHSV();

            for (int i = 0; i < paints.Length; i++) {

                if (paints[i] != null && paints[i].meshRenderer != null && paints[i].meshRenderer.materials.Length > 0)
                    paints[i].meshRenderer.materials[paints[i].materialIndex].SetColor(paints[i].colorString, randomColor);

            }

        }

        /// <summary>
        /// On collision enter.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision) {

            outputEvent_OnCollision.Invoke(outputOnCollision);

            //  Return if canCrash is disabled.
            if (!canCrash)
                return;

            //  Return if relative velocity magnitude is below 3.
            if (collision.relativeVelocity.magnitude < crashImpact)
                return;

            //  Crashed.
            crashed = true;

            m_disableAfterCrash = disableAfterCrash;

        }

        public void Lighting(Lights lightSource, float targetIntensity, float smoothness) {

            lightSource.light.intensity = Mathf.Lerp(lightSource.light.intensity, targetIntensity, Time.deltaTime * smoothness);

            if (lightSource.light.intensity < .05f)
                lightSource.light.intensity = 0f;

            if (lightSource.meshRenderer != null && lightSource.meshRenderer.materials.Length > 0 && lightSource.shaderKeyword != "") {

                lightSource.meshRenderer.materials[lightSource.materialIndex].EnableKeyword("_EMISSION");        //  Enabling keyword of the material for emission.
                lightSource.meshRenderer.materials[lightSource.materialIndex].SetColor(lightSource.shaderKeyword, lightSource.light.intensity * lightSource.light.color);

            }

        }

        public void CreateEngineTorqueCurve() {

            engineTorqueCurve = new AnimationCurve();
            engineTorqueCurve.AddKey(minEngineRPM, engineTorque / 2f);                                                               //	First index of the curve.
            engineTorqueCurve.AddKey(maxEngineTorqueAtRPM, engineTorque);        //	Second index of the curve at max.
            engineTorqueCurve.AddKey(maxEngineRPM, engineTorque / 1.5f);         // Last index of the curve at maximum RPM.

        }

        /// <summary>
        /// On disable.
        /// </summary>
        private void OnDisable() {

            //  Calling this event when this vehicle de-spawned.
            if (OnTrafficDeSpawned != null)
                OnTrafficDeSpawned(this);

            //  Resetting variables on disable.
            ResetVehicleOnDisable();

            outputEvent_OnDisable.Invoke(outputOnDisable);

        }

        /// <summary>
        /// Reset.
        /// </summary>
        private void Reset() {

            if (transform.Find("COM"))
                DestroyImmediate(transform.Find("COM").gameObject);

            GameObject COMGO = new GameObject("COM");
            COM = COMGO.transform;
            COM.SetParent(transform, false);

            CreateEngineTorqueCurve();

        }

        /// <summary>
        /// Drawing gizmos.
        /// </summary>
        private void OnDrawGizmos() {

            Gizmos.color = Color.green;

            if (navigatorPoint)
                Gizmos.DrawWireSphere(navigatorPoint.position, .5f);

            if (bounds != null) {

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position + transform.forward * bounds.front, .15f);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position + transform.forward * bounds.rear, .15f);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position + transform.right * bounds.right, .15f);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position + transform.right * bounds.left, .15f);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position + transform.up * bounds.up, .15f);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position + transform.up * bounds.down, .15f);

            }

        }

        /// <summary>
        /// When a value in the inspector changed.
        /// </summary>
        private void OnValidate() {

            Rigidbody rig = GetComponent<Rigidbody>();

            if (autoGenerateEngineRPMCurve)
                CreateEngineTorqueCurve();

            if (rig != null && rig.mass == 1) {

                rig.mass = 1350;
                rig.drag = 0f;
                rig.angularDrag = .1f;
                rig.interpolation = RigidbodyInterpolation.Interpolate;

            }

            if (wheels != null) {

                if (COM.localPosition == Vector3.zero) {

                    Transform frontWheelCollider = null;
                    float zDistance = 0f;
                    int index = -1;

                    for (int i = 0; i < wheels.Length; i++) {

                        if (wheels[i] != null && wheels[i].wheelCollider != null) {

                            if (wheels[i].wheelCollider && zDistance < wheels[i].wheelCollider.transform.localPosition.z) {

                                zDistance = wheels[i].wheelCollider.transform.localPosition.z;
                                index = i;

                            }

                        }

                    }

                    if (index != -1)
                        frontWheelCollider = wheels[index].wheelCollider.transform;

                    Transform rearWheelCollider = null;
                    zDistance = 0f;
                    index = -1;

                    for (int i = 0; i < wheels.Length; i++) {

                        if (wheels[i] != null && wheels[i].wheelCollider != null) {

                            if (wheels[i].wheelCollider && zDistance > wheels[i].wheelCollider.transform.localPosition.z) {

                                zDistance = wheels[i].wheelCollider.transform.localPosition.z;
                                index = i;

                            }

                        }

                    }

                    if (index != -1)
                        rearWheelCollider = wheels[index].wheelCollider.transform;

                    if (frontWheelCollider && rearWheelCollider)
                        COM.transform.localPosition = Vector3.Lerp(frontWheelCollider.transform.localPosition, rearWheelCollider.transform.localPosition, .5f);

                    COM.transform.localPosition = new Vector3(0f, COM.transform.localPosition.y, COM.transform.localPosition.z);

                }

            }

            if (paints != null && paints.Length > 0) {

                for (int i = 0; i < paints.Length; i++) {

                    if (paints[i] != null && paints[i].colorString == "")
                        paints[i].colorString = "_Color";

                }

            }

            if (lights != null && lights.Length >= 1) {

                for (int i = 0; i < lights.Length; i++) {

                    if (lights[i] != null) {

                        if (lights[i].intensity == 0)
                            lights[i].intensity = 1f;

                        if (lights[i].smoothness == 0)
                            lights[i].smoothness = 20f;

                        if (lights[i].shaderKeyword == "")
                            lights[i].shaderKeyword = "_EmissionColor";

                    }

                }

            }

        }

    }
}
