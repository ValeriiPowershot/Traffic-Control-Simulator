#if GLEY_TRAFFIC_SYSTEM
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Gley.UrbanAssets.Internal;
using Debug = UnityEngine.Debug;
using System.Diagnostics;
using UnityEngine.Events;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// This is the core class of the system, it controls everything else
    /// </summary>
    public class TrafficManager : UrbanManager
    {
        #region Variables

        //transforms to update
        private TransformAccessArray vehicleTrigger;
        private TransformAccessArray suspensionConnectPoints;
        private TransformAccessArray wheelsGraphics;

        private NativeArray<float3> activeCameraPositions;

        //properties for each vehicle
        private NativeArray<TrafficSystem.DriveActions> vehicleSpecialDriveAction;
        private NativeArray<VehicleTypes> vehicleType;
        private Rigidbody[] vehicleRigidbody;
        private Dictionary<int, Rigidbody> trailerRigidbody;

        private NativeArray<float3> vehicleDownDirection;
        private NativeArray<float3> vehicleForwardDirection;
        private NativeArray<float3> trailerForwardDirection;
        private NativeArray<float3> triggerForwardDirection;
        private NativeArray<float3> vehicleRightDirection;
        private NativeArray<float3> trailerRightDirection;
        private NativeArray<float3> vehicleTargetWaypointPosition;
        private NativeArray<float3> vehiclePosition;
        private NativeArray<float3> vehicleGroundDirection;
        private NativeArray<float3> vehicleForwardForce;
        private NativeArray<float3> trailerForwardForce;


        private NativeArray<float3> vehicleVelocity;
        private NativeArray<float3> trailerVelocity;
        private NativeArray<float3> closestObstacle;
        private NativeArray<float> wheelSpringForce;
        private NativeArray<float> vehicleMaxSteer;
        private NativeArray<float> vehicleRotationAngle;
        private NativeArray<float> vehiclePowerStep;
        private NativeArray<float> vehicleBrakeStep;
        private NativeArray<float> vehicleActionValue;
        private NativeArray<float> vehicleDrag;
        private NativeArray<float> massDifference;
        private NativeArray<float> trailerDrag;
        private NativeArray<float> vehicleMaxSpeed;
        private NativeArray<float> vehicleLength;
        private NativeArray<float> vehicleWheelDistance;
        private NativeArray<float> vehicleSteeringStep;
        private NativeArray<float> vehicleDistanceToStop;
        private NativeArray<int> vehicleStartWheelIndex;//start index for the wheels of car i (dim nrOfCars)
        private NativeArray<int> vehicleEndWheelIndex; //number of wheels that car with index i has (nrOfCars)
        private NativeArray<int> vehicleNrOfWheels;
        private NativeArray<int> vehicleListIndex;
        private NativeArray<int> vehicleGear;
        private NativeArray<int> trailerNrWheels;
        private NativeArray<bool> vehicleReadyToRemove;
        private NativeArray<bool> vehicleIsBraking;
        private NativeArray<bool> vehicleNeedWaypoint;
        private NativeArray<bool> ignoreVehicle;

        //properties for each wheel
        private NativeArray<RaycastHit> wheelRaycatsResult;
        private NativeArray<RaycastCommand> wheelRaycastCommand;
        private NativeArray<float3> wheelSuspensionPosition;
        private NativeArray<float3> wheelGroundPosition;
        private NativeArray<float3> wheelVelocity;
        private NativeArray<float3> wheelRightDirection;
        private NativeArray<float3> wheelNormalDirection;
        private NativeArray<float3> wheelSuspensionForce;
        private NativeArray<float3> wheelSideForce;
        private NativeArray<float> wheelRotation;
        private NativeArray<float> wheelRadius;
        private NativeArray<float> wheelRaycatsDistance;
        private NativeArray<float> wheelMaxSuspension;
        private NativeArray<float> wheelSpringStiffness;

        private NativeArray<int> wheelSign;
        private NativeArray<int> wheelAssociatedCar; //index of the car that contains the wheel
        private NativeArray<bool> wheelCanSteer;

        //properties that should be on each wheel
        private NativeArray<float> turnAngle;
        private NativeArray<float> raycastLengths;
        private NativeArray<float> wCircumferences;

        //jobs
        private UpdateWheelJob updateWheelJob;
        private UpdateTriggerJob updateTriggerJob;
        private DriveJob driveJob;
        private WheelJob wheelJob;
        private JobHandle raycastJobHandle;
        private JobHandle updateWheelJobHandle;
        private JobHandle updateTriggerJobHandle;
        private JobHandle driveJobHandle;
        private JobHandle wheelJobHandle;

        //additional properties
        private LayerMask roadLayers;
        private Transform[] activeCameras;
        private Vector3 forward;
        private Vector3 up;
        private float distanceToRemove;
        private float minDistanceToAdd;
        private int nrOfVehicles;
        private int nrOfJobs;
        private int indexToRemove;
        private int totalWheels;
        private int activeSquaresLevel;
        private int activeCameraIndex;
        private bool initialized;
#pragma warning disable 0649
        private bool drawBodyForces;
        private bool debugDensity;
        private bool debugWaypoints;
        private bool debugDisabledWaypoints;
        private bool debug;
        private bool debugSpeed;
        private bool debugIntersections;
        private bool debugPathFinding;
        private bool clearPath;
        private RoadSide side;
#pragma warning restore 0649
        #endregion

        private static TrafficManager instance;
        public static TrafficManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(Constants.trafficManager).AddComponent<TrafficManager>();
                }
                return instance;
            }
        }


        private TrafficVehicles trafficVehicles;
        internal TrafficVehicles TrafficVehicles
        {
            get
            {
                if (trafficVehicles != null)
                {
                    return trafficVehicles;
                }

                return ReturnError<TrafficVehicles>();
            }
            private set
            {
                trafficVehicles = value;
            }
        }

        private DensityManager densityManager;
        internal DensityManager DensityManager
        {
            get
            {
                if (densityManager != null)
                {
                    return densityManager;
                }
                return ReturnError<DensityManager>();
            }
            private set
            {
                densityManager = value;
            }
        }

        private DrivingAI drivingAI;
        internal DrivingAI DrivingAI
        {
            get
            {
                if (drivingAI != null)
                {
                    return drivingAI;
                }
                return ReturnError<DrivingAI>();
            }
            private set
            {
                drivingAI = value;
            }
        }

        private WaypointManager waypointManager;
        internal WaypointManager WaypointManager
        {
            get
            {
                if (waypointManager != null)
                {
                    return waypointManager;
                }
                return ReturnError<WaypointManager>();
            }
            private set
            {
                waypointManager = value;
            }
        }

        private IntersectionManager intersectionManager;

        internal IntersectionManager IntersectionManager
        {
            get
            {
                if (intersectionManager != null)
                {
                    return intersectionManager;
                }
                return ReturnError<IntersectionManager>();
            }
            private set
            {
                intersectionManager = value;
            }

        }

        private VehiclePositioningSystem vehiclePositioningSystem;
        internal VehiclePositioningSystem VehiclePositioningSystem
        {
            get
            {
                if (vehiclePositioningSystem != null)
                {
                    return vehiclePositioningSystem;
                }
                return ReturnError<VehiclePositioningSystem>();
            }
            private set
            {
                vehiclePositioningSystem = value;
            }
        }

        private PathFindingManager pathFindingManager;
        internal PathFindingManager PathFindingManager
        {
            get
            {
                if (pathFindingManager != null)
                {
                    return pathFindingManager;
                }
                if (CurrentSceneData.GetSceneInstance().GetComponent<PathFindingData>() == null)
                {
                    Debug.LogError("Path Finding Waypoints not found, Go to Tools -> Gley -> Traffic System -> Path Finding and enable Path Finding");
                    return null;
                }
                return ReturnError<PathFindingManager>();
            }
            private set
            {
                pathFindingManager = value;
            }
        }


        T ReturnError<T>()
        {
            StackTrace stackTrace = new StackTrace();
            string callingMethodName = string.Empty;
            if (stackTrace.FrameCount >= 3)
            {
                StackFrame callingFrame = stackTrace.GetFrame(2);
                callingMethodName = callingFrame.GetMethod().Name;
            }
            Debug.LogError($"Mobile Traffic System is not initialized. Call Gley.TraficSystem.Initialize() before calling {callingMethodName}");
            return default(T);
        }


        #region TrafficInitialization
        /// <summary>
        /// Initialize the traffic 
        /// </summary>
        /// <param name="activeCameras">camera that follows the player</param>
        /// <param name="nrOfVehicles">max number of traffic vehicles active in the same time</param>
        /// <param name="carPool">available vehicles asset</param>
        /// <param name="minDistanceToAdd">min distance from the player to add new vehicle</param>
        /// <param name="distanceToRemove">distance at which traffic vehicles can be removed</param>
        /// <param name="masterVolume">[-1,1] used to control the engine sound from your master volume</param>
        /// <param name="greenLightTime">roads green light duration in seconds</param>
        /// <param name="yelloLightTime">roads yellow light duration in seconds</param>
        public void Initialize(Transform[] activeCameras, int nrOfVehicles, VehiclePool vehiclePool, TrafficOptions trafficOptions)
        {
            //safety checks
            LayerSetup layerSetup = Resources.Load<LayerSetup>(Constants.layerSetupData);
            if (layerSetup == null)
            {
                Debug.LogError("Layers are not configured. Go to Tools->Gley->Traffic System->Scene Setup->Layer Setup");
                return;
            }

            CurrentSceneData currentSceneData = CurrentSceneData.GetSceneInstance();
            if (currentSceneData.grid == null)
            {
                Debug.LogError("Scene data is null. Go to Tools->Gley->Traffic System and Apply Settings");
                return;
            }

            if (currentSceneData.grid.Length == 0)
            {
                Debug.LogError("Scene grid is not set up correctly. Go to Tools->Gley->Traffic System->Scene Setup->Grid Setup");
                return;
            }

            if (currentSceneData.allWaypoints.Length <= 0)
            {
                Debug.LogError("No waypoints found. Go to Tools->Gley->Traffic System->Road Setup and create a road or make sure your settings are applied");
                return;
            }

            if (vehiclePool.trafficCars.Length <= 0)
            {
                Debug.LogError("No cars available to instantiate. make sure you car pool has at least one car");
                return;
            }

            if (nrOfVehicles <= 0)
            {
                Debug.LogError("Nr of vehicles needs to be greater than 1");
                return;
            }


            this.nrOfVehicles = nrOfVehicles;
            this.activeCameras = activeCameras;

            activeSquaresLevel = trafficOptions.activeSquaresLevel;

            roadLayers = layerSetup.roadLayers;
            up = Vector3.up;

            //compute total wheels
            TrafficVehicles = gameObject.AddComponent<TrafficVehicles>().Initialize(vehiclePool, nrOfVehicles, layerSetup.buildingsLayers, layerSetup.obstaclesLayers, layerSetup.playerLayers, layerSetup.roadLayers, trafficOptions.masterVolume, trafficOptions.lightsOn, trafficOptions.ModifyTriggerSize);
            List<VehicleComponent> traffic = TrafficVehicles.GetVehicleList();
            for (int i = 0; i < traffic.Count; i++)
            {
                totalWheels += traffic[i].allWheels.Length;
                if (traffic[i].trailer != null)
                {
                    totalWheels += traffic[i].trailer.allWheels.Length;
                }
            }

            //initialize arrays
            wheelSuspensionPosition = new NativeArray<float3>(totalWheels, Allocator.Persistent);
            wheelVelocity = new NativeArray<float3>(totalWheels, Allocator.Persistent);
            wheelGroundPosition = new NativeArray<float3>(totalWheels, Allocator.Persistent);
            wheelNormalDirection = new NativeArray<float3>(totalWheels, Allocator.Persistent);
            wheelRightDirection = new NativeArray<float3>(totalWheels, Allocator.Persistent);
            wheelRaycatsDistance = new NativeArray<float>(totalWheels, Allocator.Persistent);
            wheelRadius = new NativeArray<float>(totalWheels, Allocator.Persistent);
            wheelAssociatedCar = new NativeArray<int>(totalWheels, Allocator.Persistent);
            wheelCanSteer = new NativeArray<bool>(totalWheels, Allocator.Persistent);
            wheelSuspensionForce = new NativeArray<float3>(totalWheels, Allocator.Persistent);
            wheelMaxSuspension = new NativeArray<float>(totalWheels, Allocator.Persistent);
            wheelSpringStiffness = new NativeArray<float>(totalWheels, Allocator.Persistent);
            wheelRaycatsResult = new NativeArray<RaycastHit>(totalWheels, Allocator.Persistent);
            wheelRaycastCommand = new NativeArray<RaycastCommand>(totalWheels, Allocator.Persistent);
            wheelSpringForce = new NativeArray<float>(totalWheels, Allocator.Persistent);
            wheelSideForce = new NativeArray<float3>(totalWheels, Allocator.Persistent);

            vehicleTrigger = new TransformAccessArray(nrOfVehicles);
            vehicleSpecialDriveAction = new NativeArray<TrafficSystem.DriveActions>(nrOfVehicles, Allocator.Persistent);
            vehicleType = new NativeArray<VehicleTypes>(nrOfVehicles, Allocator.Persistent);
            vehicleForwardForce = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            trailerForwardForce = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);

            vehiclePosition = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleGroundDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleDownDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleRightDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            trailerRightDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleForwardDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            trailerForwardDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            triggerForwardDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleTargetWaypointPosition = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleVelocity = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            trailerVelocity = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            closestObstacle = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);

            wheelRotation = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            turnAngle = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleDrag = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            massDifference = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            trailerDrag = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleSteeringStep = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleDistanceToStop = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleMaxSpeed = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleLength = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleWheelDistance = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehiclePowerStep = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleBrakeStep = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);

            raycastLengths = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            wCircumferences = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleRotationAngle = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleMaxSteer = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleActionValue = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            wheelSign = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleListIndex = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleEndWheelIndex = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleStartWheelIndex = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleNrOfWheels = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            trailerNrWheels = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleReadyToRemove = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
            vehicleNeedWaypoint = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
            vehicleIsBraking = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
            ignoreVehicle = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
            vehicleGear = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleRigidbody = new Rigidbody[nrOfVehicles];
            trailerRigidbody = new Dictionary<int, Rigidbody>();

            //initialize debug settings for editor
#if UNITY_EDITOR
            DebugSettings debugSettings = DebugOptions.LoadOrCreateDebugSettings();
            drawBodyForces = debugSettings.drawBodyForces;
            debugDensity = debugSettings.debugDensity;
            debugWaypoints = debugSettings.debugWaypoints;
            debugDisabledWaypoints = debugSettings.debugDisabledWaypoints;
            debug = debugSettings.debug;
            debugSpeed = debugSettings.debugSpeed;
            debugIntersections = debugSettings.debugIntersections;
            debugPathFinding = debugSettings.debugPathFinding;
#endif

            //initialize other managers
            activeCameraPositions = new NativeArray<float3>(activeCameras.Length, Allocator.Persistent);
            for (int i = 0; i < activeCameraPositions.Length; i++)
            {
                activeCameraPositions[i] = activeCameras[i].position;
            }
            AddGridManager(currentSceneData, activeCameraPositions);

            if (trafficOptions.distanceToRemove < 0)
            {
                float cellSize = GridManager.GetCellSize();
                trafficOptions.distanceToRemove = 2 * cellSize + cellSize / 2;
            }
            if (trafficOptions.minDistanceToAdd < 0)
            {
                float cellSize = GridManager.GetCellSize();
                trafficOptions.minDistanceToAdd = cellSize + cellSize / 2;
            }

            distanceToRemove = trafficOptions.distanceToRemove * trafficOptions.distanceToRemove;
            minDistanceToAdd = trafficOptions.minDistanceToAdd;

            PositionValidator positionValidator = gameObject.AddComponent<PositionValidator>().Initialize(this.activeCameras, layerSetup.trafficLayers, layerSetup.playerLayers, layerSetup.buildingsLayers, minDistanceToAdd, debugDensity);
            WaypointManager = gameObject.AddComponent<WaypointManager>().Initialize(GridManager,currentSceneData.allWaypoints, nrOfVehicles, debugWaypoints, debugDisabledWaypoints);
            WaypointManager.SetSpawnWaypointSelector(trafficOptions.SpawnWaypointSelector);
            VehiclePositioningSystem = gameObject.AddComponent<VehiclePositioningSystem>().Initialize(nrOfVehicles, WaypointManager);
            DrivingAI = gameObject.AddComponent<DrivingAI>().Initialize(nrOfVehicles, WaypointManager, TrafficVehicles, VehiclePositioningSystem, positionValidator, debug, debugSpeed, debugPathFinding,
                trafficOptions.PlayerInTrigger, trafficOptions.DynamicObstacleInTrigger, trafficOptions.BuildingInTrigger, trafficOptions.VehicleCrash);

            //initialize all vehicles
            Transform[] tempWheelOrigin = new Transform[totalWheels];
            Transform[] tempWheelGraphic = new Transform[totalWheels];
            int wheelIndex = 0;
            for (int i = 0; i < nrOfVehicles; i++)
            {
                VehicleComponent vehicle = traffic[i];
                VehiclePositioningSystem.AddCar(vehicle.GetFrontAxle());
                vehicleTrigger.Add(vehicle.frontTrigger);
                vehicleRigidbody[i] = vehicle.rb;
                vehicleSteeringStep[i] = vehicle.GetSteeringStep();
                vehicleDistanceToStop[i] = vehicle.distanceToStop;
                vehicleWheelDistance[i] = vehicle.wheelDistance;
                vehicleDrag[i] = vehicle.rb.drag;

                raycastLengths[i] = vehicle.GetRaycastLength();
                wCircumferences[i] = vehicle.GetWheelCircumference();
                vehicleMaxSteer[i] = vehicle.GetMaxSteer();
                vehicleStartWheelIndex[i] = wheelIndex;
                vehicleNrOfWheels[i] = vehicle.GetNrOfWheels();
                vehicleEndWheelIndex[i] = vehicleStartWheelIndex[i] + vehicleNrOfWheels[i];
                trailerNrWheels[i] = vehicle.GetTrailerWheels();

                vehicleLength[i] = vehicle.length;

                for (int j = 0; j < vehicleNrOfWheels[i]; j++)
                {
                    tempWheelOrigin[wheelIndex] = vehicle.allWheels[j].wheelTransform;
                    tempWheelGraphic[wheelIndex] = vehicle.allWheels[j].wheelTransform.GetChild(0);
                    wheelCanSteer[wheelIndex] = vehicle.allWheels[j].wheelPosition == Wheel.WheelPosition.Front;
                    wheelRadius[wheelIndex] = vehicle.allWheels[j].wheelRadius;
                    wheelMaxSuspension[wheelIndex] = vehicle.allWheels[j].maxSuspension;
                    wheelSpringStiffness[wheelIndex] = vehicle.GetSpringStiffness();
                    wheelAssociatedCar[wheelIndex] = i;
                    wheelSpringForce[wheelIndex] = vehicle.GetSpringForce();
                    wheelIndex++;
                }
                if (vehicle.trailer != null)
                {
                    TrailerComponent trailer = vehicle.trailer;
                    for (int j = 0; j < vehicle.trailer.GetNrOfWheels(); j++)
                    {
                        tempWheelOrigin[wheelIndex] = trailer.allWheels[j].wheelTransform;
                        tempWheelGraphic[wheelIndex] = trailer.allWheels[j].wheelTransform.GetChild(0);
                        wheelCanSteer[wheelIndex] = false;
                        wheelRadius[wheelIndex] = trailer.allWheels[j].wheelRadius;
                        wheelMaxSuspension[wheelIndex] = trailer.allWheels[j].maxSuspension;
                        wheelSpringStiffness[wheelIndex] = trailer.GetSpringStiffness();
                        wheelAssociatedCar[wheelIndex] = i;
                        wheelSpringForce[wheelIndex] = trailer.GetSpringForce();
                        wheelIndex++;
                    }
                    vehicleEndWheelIndex[i] += trailer.GetNrOfWheels();
                    trailerDrag[i] = trailer.rb.drag;
                    massDifference[i] = (trailer.rb.mass / vehicle.rb.mass) * (trailer.joint.connectedMassScale / trailer.joint.massScale);
                    trailerRigidbody.Add(i, trailer.rb);
                }

                vehicleListIndex[i] = vehicle.GetIndex();
                vehicleType[i] = vehicle.GetVehicleType();
            }

            suspensionConnectPoints = new TransformAccessArray(tempWheelOrigin);
            wheelsGraphics = new TransformAccessArray(tempWheelGraphic);

            //set the number of jobs based on processor count
            if (SystemInfo.processorCount != 0)
            {
                nrOfJobs = totalWheels / SystemInfo.processorCount + 1;
            }
            else
            {
                nrOfJobs = nrOfVehicles / 4;
            }

            //add events
            AIEvents.onChangeDrivingState += UpdateDrivingState;
            AIEvents.onChangeDestination += DestinationChanged;
            Events.onVehicleAdded += NewVehicleAdded;

            //initialize the remaining managers

            DensityManager = gameObject.AddComponent<DensityManager>().Initialize(TrafficVehicles, WaypointManager, GridManager, positionValidator, activeCameraPositions, nrOfVehicles, activeCameras[0].position, activeCameras[0].forward, activeSquaresLevel, trafficOptions.useWaypointPriority, trafficOptions.initialDensity, trafficOptions.disableWaypointsArea);

            IntersectionManager = gameObject.AddComponent<IntersectionManager>().Initialize(GridManager.GetAllIntersections(), GridManager.GetActiveIntersections(), WaypointManager, trafficOptions.greenLightTime, trafficOptions.yellowLightTime, debugIntersections, trafficOptions.TrafficLightsBehaviour);

            PathFindingData data = CurrentSceneData.GetSceneInstance().GetComponent<PathFindingData>();
            if (data != null)
            {
                PathFindingManager = gameObject.AddComponent<PathFindingManager>().Initialize(TrafficVehicles, WaypointManager, GridManager, debugPathFinding);
            }

            initialized = true;
        }


        internal bool IsInitialized()
        {
            return initialized;
        }


        internal bool IsDebugWaypointsEnabled()
        {
            return debugWaypoints;
        }


        #endregion

        #region API Methods
        public void ClearPathForSpecialVehicles(bool active, RoadSide side)
        {
            clearPath = active;
            if(side == RoadSide.Any)
            {
                side = RoadSide.Right;
            }
            this.side = side;
            for (int i = 0; i < nrOfVehicles; i++)
            {
                if (vehicleRigidbody[i].gameObject.activeSelf)
                {
                    DrivingAI.ChangeLane(active, i, side);

                    if (clearPath == false)
                    {
                        TrafficVehicles.ResetMaxSpeed(i);
                        AIEvents.TriggerChangeDestinationEvent(i);
                    }
                }

            }
        }
        /// <summary>
        /// Removes the vehicles on a given circular area
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void ClearTrafficOnArea(Vector3 center, float radius)
        {
            if (!initialized)
                return;

            float sqrRadius = radius * radius;
            for (int i = 0; i < vehiclePosition.Length; i++)
            {
                if (vehicleRigidbody[i].gameObject.activeSelf)
                {
                    //uses math because of the float3 array
                    if (math.distancesq(center, vehiclePosition[i]) < sqrRadius)
                    {
                        RemoveVehicle(i, true);
                    }
                }
            }
        }


        /// <summary>
        /// Remove a specific vehicle from the scene
        /// </summary>
        /// <param name="index">index of the vehicle to remove</param>
        public void RemoveVehicle(GameObject vehicle)
        {
            if (!initialized)
                return;

            int index = TrafficVehicles.GetVehicleIndex(vehicle);
            if (index != -1)
            {
                RemoveVehicle(index, true);
            }
            else
            {
                Debug.Log("Vehicle not found");
            }
        }


        /// <summary>
        /// Update active camera that is used to remove vehicles when are not in view
        /// </summary>
        /// <param name="activeCamera">represents the camera or the player prefab</param>
        public void UpdateCamera(Transform[] activeCameras)
        {
            if (!initialized)
                return;

            if (activeCameras.Length != activeCameraPositions.Length)
            {
                activeCameraPositions = new NativeArray<float3>(activeCameras.Length, Allocator.Persistent);
            }

            this.activeCameras = activeCameras;
            DensityManager.UpdateCameraPositions(activeCameras);

        }


        public void SetActiveSquaresLevel(int activeSquaresLevel)
        {
            if (!initialized)
                return;

            this.activeSquaresLevel = activeSquaresLevel;
            DensityManager.UpdateActiveSquares(activeSquaresLevel);
        }

        public void StopVehicleDriving(GameObject vehicle)
        {
            if (!initialized)
                return;

            int vehicleIndex = TrafficVehicles.GetVehicleIndex(vehicle);
            if (vehicleIndex >= 0)
            {
                ignoreVehicle[vehicleIndex] = true;
            }
        }




        #endregion


        #region EventHandlers
        /// <summary>
        /// Called every time a new vehicle is enabled
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="targetWaypointPosition">target position</param>
        /// <param name="maxSpeed">max possible speed</param>
        /// <param name="powerStep">acceleration power</param>
        /// <param name="brakeStep">brake power</param>
        private void NewVehicleAdded(int vehicleIndex)
        {
            //set new vehicle parameters
            vehicleTargetWaypointPosition[vehicleIndex] = WaypointManager.GetTargetWaypointPosition(vehicleIndex);

            vehiclePowerStep[vehicleIndex] = TrafficVehicles.GetPowerStep(vehicleIndex);
            vehicleBrakeStep[vehicleIndex] = TrafficVehicles.GetBrakeStep(vehicleIndex);

            vehicleIsBraking[vehicleIndex] = false;
            vehicleNeedWaypoint[vehicleIndex] = false;
            ignoreVehicle[vehicleIndex] = false;
            vehicleGear[vehicleIndex] = 1;
            turnAngle[vehicleIndex] = 0;

            //reset AI
            DrivingAI.VehicleActivated(vehicleIndex);
            vehicleMaxSpeed[vehicleIndex] = DrivingAI.GetMaxSpeedMS(vehicleIndex);
            vehicleLength[vehicleIndex] = TrafficVehicles.GetVehicleLength(vehicleIndex);

            //set initial velocity
            vehicleRigidbody[vehicleIndex].velocity = VehiclePositioningSystem.GetForwardVector(vehicleIndex) * vehicleMaxSpeed[vehicleIndex] / 2;
            if (trailerNrWheels[vehicleIndex] != 0)
            {
                trailerRigidbody[vehicleIndex].velocity = vehicleRigidbody[vehicleIndex].velocity;
            }
            //vehicleRigidbody[vehicleIndex].velocity = Vector3.zero;
        }

        /// <summary>
        /// Remove a specific vehicle from the scene
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle to remove</param>
        public void RemoveVehicle(int vehicleIndex, bool force)
        {
            if (!initialized)
                return;
            if (WaypointManager.HasPath(vehicleIndex) && force == false)
            {
                return;
            }
            vehicleReadyToRemove[indexToRemove] = false;
            int index = vehicleListIndex[vehicleIndex];
            IntersectionManager.RemoveVehicle(index);
            WaypointManager.RemoveAgent(index);
            DrivingAI.RemoveVehicle(index);
            DensityManager.RemoveVehicle(index);
            TrafficVehicles.RemoveVehicle(index);
            closestObstacle[index] = Vector3.zero;
            Events.TriggerVehicleRemovedEvent(index);
        }


        /// <summary>
        /// Called every time a vehicle state changes
        /// </summary>
        /// <param name="vehicleIndex">vehicle index</param>
        /// <param name="action">new action</param>
        /// <param name="actionValue">time to execute the action</param>
        private void UpdateDrivingState(int vehicleIndex, TrafficSystem.DriveActions action, float actionValue)
        {
            TrafficVehicles.SetCurrentAction(vehicleIndex, action);
            vehicleSpecialDriveAction[vehicleIndex] = action;
            vehicleActionValue[vehicleIndex] = actionValue;
            if (action == TrafficSystem.DriveActions.AvoidReverse)
            {
                wheelSign[vehicleIndex] = (int)Mathf.Sign(turnAngle[vehicleIndex]);
            }
        }


        /// <summary>
        /// Called when waypoint changes
        /// </summary>
        /// <param name="vehicleIndex">vehicle index</param>
        /// <param name="targetWaypointPosition">new waypoint position</param>
        /// <param name="maxSpeed">new possible speed</param>
        /// <param name="blinkType">blinking is required or not</param>
        private void DestinationChanged(int vehicleIndex)
        {
            vehicleNeedWaypoint[vehicleIndex] = false;
            vehicleTargetWaypointPosition[vehicleIndex] = GetTargetWaypointPosition(vehicleIndex, clearPath, side);
            vehicleMaxSpeed[vehicleIndex] = DrivingAI.GetMaxSpeedMS(vehicleIndex);
        }
        #endregion


        private Vector3 GetTargetWaypointPosition(int vehicleIndex, bool clearPath, RoadSide side)
        {
            if (!clearPath)
            {
                return WaypointManager.GetTargetWaypointPosition(vehicleIndex);
            }
            else
            {
                //offset target position
                Waypoint waypoint = WaypointManager.GetTargetWaypointOfAgent<Waypoint>(vehicleIndex);
                if (waypoint == null)
                {
                    return WaypointManager.GetTargetWaypointPosition(vehicleIndex);
                }
                Vector3 direction;
                if (waypoint.neighbors.Count > 0)
                {
                    direction = WaypointManager.GetWaypointPosition(waypoint.neighbors[0]) - waypoint.position;
                }
                else
                {
                    direction = waypoint.position - WaypointManager.GetWaypointPosition(waypoint.prev[0]);
                }

                Vector3 offsetDirection = Vector3.Cross(direction.normalized, Vector3.up).normalized;

                Vector3 position = WaypointManager.GetTargetWaypointPosition(vehicleIndex);

                
                float laneWidth = WaypointManager.GetLaneWidth(vehicleIndex);
                if(laneWidth==0)
                {
                    laneWidth = 4;
                }
                float halfCarWidth = TrafficVehicles.GetVehicleWidth(vehicleIndex) / 2;

                float offset = laneWidth / 2 - halfCarWidth;

                if (side == RoadSide.Left)
                {
                    return position + offsetDirection * offset;
                }
                else
                {
                    return position + offsetDirection * -offset;
                }
            }
        }


        private void FixedUpdate()
        {
            if (!initialized)
                return;

            #region Suspensions

            //for each wheel check where the ground is by performing a RayCast downwards using job system
            for (int i = 0; i < totalWheels; i++)
            {
                wheelSuspensionPosition[i] = suspensionConnectPoints[i].position;
                wheelVelocity[i] = vehicleRigidbody[wheelAssociatedCar[i]].GetPointVelocity(wheelSuspensionPosition[i]);
            }

            for (int i = 0; i < nrOfVehicles; i++)
            {
                if (trailerNrWheels[i] > 0)
                {
                    trailerVelocity[i] = trailerRigidbody[i].velocity;
                    trailerForwardDirection[i] = trailerRigidbody[i].transform.forward;
                    trailerRightDirection[i] = trailerRigidbody[i].transform.right;

                }

                vehicleVelocity[i] = vehicleRigidbody[i].velocity;

                vehicleDownDirection[i] = -VehiclePositioningSystem.GetUpVector(i);
                forward = VehiclePositioningSystem.GetForwardVector(i);
                forward.y = 0;
                vehicleForwardDirection[i] = forward;
                vehicleRightDirection[i] = VehiclePositioningSystem.GetRightVector(i);
                vehiclePosition[i] = VehiclePositioningSystem.GetPosition(i);
                vehicleGroundDirection[i] = TrafficVehicles.GetGroundDirection(i);
                triggerForwardDirection[i] = vehicleTrigger[i].transform.forward;
                closestObstacle[i] = TrafficVehicles.GetClosestObstacle(i);
                if (debug)
                {
                    if (!closestObstacle[i].Equals(float3.zero))
                    {
                        Debug.DrawLine(closestObstacle[i], vehiclePosition[i], Color.magenta);
                    }
                }

                //adapt speed to the front vehicle
                if (vehicleSpecialDriveAction[i] == TrafficSystem.DriveActions.Overtake || vehicleSpecialDriveAction[i] == TrafficSystem.DriveActions.Follow)
                {
                    vehicleMaxSpeed[i] = DrivingAI.GetMaxSpeedMS(i);
                    if (vehicleMaxSpeed[i] < 2)
                    {
                        DrivingAI.AddDriveAction(i, TrafficSystem.DriveActions.StopInDistance);
                    }
                }
            }

            for (int i = 0; i < totalWheels; i++)
            {
#if UNITY_2022_2_OR_NEWER
                wheelRaycastCommand[i] = new RaycastCommand(wheelSuspensionPosition[i], vehicleDownDirection[wheelAssociatedCar[i]], new QueryParameters(layerMask: roadLayers), raycastLengths[wheelAssociatedCar[i]]);
#else
                wheelRaycastCommand[i] = new RaycastCommand(wheelSuspensionPosition[i], vehicleDownDirection[wheelAssociatedCar[i]], raycastLengths[wheelAssociatedCar[i]], roadLayers);
#endif
            }
            raycastJobHandle = RaycastCommand.ScheduleBatch(wheelRaycastCommand, wheelRaycatsResult, nrOfJobs, default);
            raycastJobHandle.Complete();

            for (int i = 0; i < totalWheels; i++)
            {
                wheelRaycatsDistance[i] = wheelRaycatsResult[i].distance;
                wheelNormalDirection[i] = wheelRaycatsResult[i].normal;
                wheelGroundPosition[i] = wheelRaycatsResult[i].point;
            }
            #endregion

            #region Driving

            //execute job for wheel turn and driving
            wheelJob = new WheelJob()
            {
                wheelSuspensionForce = wheelSuspensionForce,
                springForces = wheelSpringForce,
                wheelMaxSuspension = wheelMaxSuspension,
                wheelRaycastDistance = wheelRaycatsDistance,
                wheelRadius = wheelRadius,
                wheelNormalDirection = wheelNormalDirection,
                nrOfCarWheels = vehicleEndWheelIndex,
                startWheelIndex = vehicleStartWheelIndex,
                wheelAssociatedCar = wheelAssociatedCar,
                wheelSideForce = wheelSideForce,
                vehicleNrOfWheels = vehicleNrOfWheels,
                wheelVelocity = wheelVelocity,
                wheelRightDirection = wheelRightDirection,
                springStiffness = wheelSpringStiffness,
            };

            driveJob = new DriveJob()
            {
                wheelCircumferences = wCircumferences,
                carVelocity = vehicleVelocity,
                fixedDeltaTime = Time.fixedDeltaTime,
                targetWaypointPosition = vehicleTargetWaypointPosition,
                allBotsPosition = vehiclePosition,
                maxSteer = vehicleMaxSteer,
                forwardDirection = vehicleForwardDirection,
                worldUp = up,
                wheelRotation = wheelRotation,
                turnAngle = turnAngle,
                vehicleRotationAngle = vehicleRotationAngle,
                readyToRemove = vehicleReadyToRemove,
                needsWaypoint = vehicleNeedWaypoint,
                distanceToRemove = distanceToRemove,
                cameraPositions = activeCameraPositions,
                bodyForce = vehicleForwardForce,
                downDirection = vehicleDownDirection,
                rightDirection = vehicleRightDirection,
                powerStep = vehiclePowerStep,
                brakeStep = vehicleBrakeStep,
                specialDriveAction = vehicleSpecialDriveAction,
                actionValue = vehicleActionValue,
                wheelSign = wheelSign,
                isBraking = vehicleIsBraking,
                drag = vehicleDrag,
                maxSpeed = vehicleMaxSpeed,
                gear = vehicleGear,
                groundDirection = vehicleGroundDirection,
                steeringStep = vehicleSteeringStep,
                wheelDistance = vehicleWheelDistance,
                closestObstacle = closestObstacle,
                vehicleLength = vehicleLength,
                nrOfWheels = vehicleNrOfWheels,
                trailerVelocity = trailerVelocity,
                trailerForce = trailerForwardForce,
                trailerForwardDirection = trailerForwardDirection,
                trailerRightDirection = trailerRightDirection,
                trailerNrOfWheels = trailerNrWheels,
                massDifference = massDifference,
                trailerDrag = trailerDrag,
                triggerForwardDirection = triggerForwardDirection,
                distanceToStop = vehicleDistanceToStop,

            };

            wheelJobHandle = wheelJob.Schedule(totalWheels, nrOfJobs);
            driveJobHandle = driveJob.Schedule(nrOfVehicles, nrOfJobs);
            wheelJobHandle.Complete();
            driveJobHandle.Complete();

            //store job values
            wheelSuspensionForce = wheelJob.wheelSuspensionForce;
            wheelSideForce = wheelJob.wheelSideForce;
            wheelRotation = driveJob.wheelRotation;
            turnAngle = driveJob.turnAngle;
            vehicleRotationAngle = driveJob.vehicleRotationAngle;
            vehicleReadyToRemove = driveJob.readyToRemove;
            vehicleNeedWaypoint = driveJob.needsWaypoint;
            vehicleForwardForce = driveJob.bodyForce;
            vehicleActionValue = driveJob.actionValue;
            vehicleIsBraking = driveJob.isBraking;
            vehicleGear = driveJob.gear;
            trailerForwardForce = driveJob.trailerForce;


            //make vehicle actions based on job results
            for (int i = 0; i < nrOfVehicles; i++)
            {
                if (!vehicleRigidbody[i].IsSleeping())
                {
                    int groundedWheels = 0;
                    for (int j = vehicleStartWheelIndex[i]; j < vehicleEndWheelIndex[i] - trailerNrWheels[i]; j++)
                    {
                        if (wheelRaycatsDistance[j] != 0)
                        {
                            groundedWheels++;

                            //apply suspension
                            vehicleRigidbody[i].AddForceAtPosition(wheelSuspensionForce[j], wheelGroundPosition[j]);

                            //apply friction
                            vehicleRigidbody[i].AddForceAtPosition(wheelSideForce[j], wheelSuspensionPosition[j], ForceMode.VelocityChange);
                            if (ignoreVehicle[i] == false)
                            {
                                //apply traction
                                vehicleRigidbody[i].AddForceAtPosition(vehicleForwardForce[i], wheelGroundPosition[j], ForceMode.VelocityChange);
                            }
                        }
                        else
                        {
                            //if the wheel is not grounded apply additional gravity to stabilize the vehicle for a more realistic movement
                            vehicleRigidbody[i].AddForceAtPosition(Physics.gravity * vehicleRigidbody[i].mass / (vehicleEndWheelIndex[i] - vehicleStartWheelIndex[i]), wheelSuspensionPosition[j]);
                        }
                    }

                    //TODO Change this
                    if (trailerNrWheels[i] > 0)
                    {
                        for (int j = vehicleEndWheelIndex[i] - trailerNrWheels[i]; j < vehicleEndWheelIndex[i]; j++)
                        {
                            if (wheelRaycatsDistance[j] != 0)
                            {
                                //if wheel is grounded apply suspension force
                                trailerRigidbody[i].AddForceAtPosition(wheelSuspensionForce[j], wheelGroundPosition[j]);

                                //apply side friction
                                trailerRigidbody[i].AddForceAtPosition(trailerForwardForce[i], wheelSuspensionPosition[j], ForceMode.VelocityChange);

                                if (vehicleIsBraking[i])
                                {
                                    trailerRigidbody[i].AddForceAtPosition(vehicleForwardForce[i], wheelSuspensionPosition[j], ForceMode.VelocityChange);
                                }
                            }
                            else
                            {
                                //if the wheel is not grounded apply additional gravity to stabilize the vehicle for a more realistic movement
                                trailerRigidbody[i].AddForceAtPosition(Physics.gravity * trailerRigidbody[i].mass / trailerNrWheels[i], wheelSuspensionPosition[j]);
                            }
                        }
                    }

                    if (ignoreVehicle[i] == true)
                        continue;

                    //apply rotation 
                    if (groundedWheels != 0)
                    {
                        vehicleRigidbody[i].MoveRotation(vehicleRigidbody[i].rotation * Quaternion.Euler(0, vehicleRotationAngle[i], 0));
                    }
                    //request new waypoint if needed
                    if (vehicleNeedWaypoint[i] == true)
                    {
                        if (clearPath)
                        {
                            DrivingAI.AddDriveAction(i, TrafficSystem.DriveActions.ChangeLane, false, side);
                        }
                        DrivingAI.WaypointRequested(i, vehicleType[i], clearPath);
                    }

                    //if current action is finished set a new action
                    if (vehicleActionValue[i] < 0)
                    {
                        DrivingAI.TimedActionEnded(i);
                    }
                    //update reverse lights
                    if (vehicleGear[i] < 0)
                    {
                        TrafficVehicles.SetReverseLights(i, true);
                    }
                    else
                    {
                        TrafficVehicles.SetReverseLights(i, false);
                    }

                    //update engine and lights components
                    TrafficVehicles.UpdateVehicleScripts(i);
                }
            }
            #endregion
        }


        private void Update()
        {
            if (!initialized)
                return;

            //update brake lights
            for (int i = 0; i < nrOfVehicles; i++)
            {
                TrafficVehicles.SetBrakeLights(i, vehicleIsBraking[i]);
            }

            #region WheelUpdate
            //update wheel graphics
            for (int i = 0; i < totalWheels; i++)
            {
                wheelSuspensionPosition[i] = suspensionConnectPoints[i].position;
                wheelRightDirection[i] = suspensionConnectPoints[i].right;
            }

            updateWheelJob = new UpdateWheelJob()
            {
                wheelsOrigin = wheelSuspensionPosition,
                downDirection = vehicleDownDirection,
                wheelRotation = wheelRotation,
                turnAngle = turnAngle,
                wheelRadius = wheelRadius,
                maxSuspension = wheelMaxSuspension,
                raycastDistance = wheelRaycatsDistance,
                nrOfCars = nrOfVehicles,
                canSteer = wheelCanSteer,
                carIndex = wheelAssociatedCar
            };
            updateWheelJobHandle = updateWheelJob.Schedule(wheelsGraphics);
            updateWheelJobHandle.Complete();
            #endregion

            #region TriggerUpdate
            //update trigger orientation
            updateTriggerJob = new UpdateTriggerJob()
            {
                turnAngle = turnAngle,
                specialDriveAction = vehicleSpecialDriveAction
            };
            updateTriggerJobHandle = updateTriggerJob.Schedule(vehicleTrigger);
            updateTriggerJobHandle.Complete();
            #endregion

            #region RemoveVehicles
            //remove vehicles that are too far away and not in view
            indexToRemove++;
            if (indexToRemove == nrOfVehicles)
            {
                indexToRemove = 0;
            }
            activeCameraIndex = UnityEngine.Random.Range(0, activeCameraPositions.Length);
            DensityManager.UpdateVehicleDensity(activeCameras[activeCameraIndex].position, activeCameras[activeCameraIndex].forward, activeCameraIndex);


            if (vehicleReadyToRemove[indexToRemove] == true)
            {

                if (vehicleRigidbody[indexToRemove].gameObject.activeSelf)
                {
                    if (TrafficVehicles.CanBeRemoved(vehicleListIndex[indexToRemove]) == true)
                    {
                        RemoveVehicle(indexToRemove, false);
                    }
                }
            }
            #endregion

            //update additional managers
            for (int i = 0; i < activeCameras.Length; i++)
            {
                activeCameraPositions[i] = activeCameras[i].transform.position;
            }
            IntersectionManager.UpdateIntersections();
            GridManager.UpdateGrid(activeSquaresLevel, activeCameraPositions);

            #region Debug
#if UNITY_EDITOR
            //draw debug forces if requested
            if (drawBodyForces)
            {
                for (int i = 0; i < nrOfVehicles; i++)
                {
                    Debug.DrawRay(vehicleRigidbody[i].transform.TransformPoint(vehicleRigidbody[i].centerOfMass), TrafficVehicles.GetVelocity(i), Color.red);
                    //Debug.DrawRay(vehicleRigidbody[i].transform.TransformPoint(vehicleRigidbody[i].centerOfMass), vehicleForwardForce[i] * 100, Color.green, Time.deltaTime, false);
                    //Debug.DrawRay(vehicleRigidbody[i].transform.TransformPoint(vehicleRigidbody[i].centerOfMass), vehicleForwardDirection[i], Color.yellow, Time.deltaTime, false);
                    //Debug.DrawRay(vehicleRigidbody[i].transform.TransformPoint(vehicleRigidbody[i].centerOfMass), vehicleRightDirection[i], Color.blue, Time.deltaTime, false);
                    if (TrafficVehicles.HasTrailer(i))
                    {
                        Vector3 localVelocity = trailerRigidbody[i].transform.InverseTransformVector(trailerRigidbody[i].velocity);
                        Debug.DrawRay(trailerRigidbody[i].transform.TransformPoint(trailerRigidbody[i].centerOfMass), new Vector3(-localVelocity.x, 0, 0) * 100, Color.green, Time.deltaTime, false);
                        Debug.DrawRay(trailerRigidbody[i].transform.TransformPoint(trailerRigidbody[i].centerOfMass), trailerRigidbody[i].velocity, Color.red);
                    }
                }

                for (int j = 0; j < totalWheels; j++)
                {
                    Debug.DrawRay(wheelSuspensionPosition[j], wheelSuspensionForce[j] / TrafficVehicles.GetSpringForce(wheelAssociatedCar[j]), Color.yellow);
                }
            }
#endif
            #endregion
        }


        #region Cleanup
        /// <summary>
        /// Cleanup
        /// </summary>
        private void OnDestroy()
        {
            try
            {
                wheelSpringForce.Dispose();
                raycastLengths.Dispose();
                wCircumferences.Dispose();
                wheelRadius.Dispose();
                vehicleVelocity.Dispose();
                trailerVelocity.Dispose();
                vehicleMaxSteer.Dispose();
                suspensionConnectPoints.Dispose();
                wheelsGraphics.Dispose();
                wheelGroundPosition.Dispose();
                wheelVelocity.Dispose();
                wheelRotation.Dispose();
                turnAngle.Dispose();
                wheelRaycatsResult.Dispose();
                wheelRaycastCommand.Dispose();
                wheelCanSteer.Dispose();
                wheelAssociatedCar.Dispose();
                vehicleEndWheelIndex.Dispose();
                vehicleStartWheelIndex.Dispose();
                vehicleNrOfWheels.Dispose();
                trailerNrWheels.Dispose();
                vehicleDownDirection.Dispose();
                vehicleForwardDirection.Dispose();
                trailerForwardDirection.Dispose();
                vehicleRotationAngle.Dispose();
                vehicleRightDirection.Dispose();
                vehicleTargetWaypointPosition.Dispose();
                vehiclePosition.Dispose();
                vehicleGroundDirection.Dispose();
                vehicleReadyToRemove.Dispose();
                vehicleListIndex.Dispose();
                vehicleNeedWaypoint.Dispose();
                wheelRaycatsDistance.Dispose();
                wheelRightDirection.Dispose();
                wheelNormalDirection.Dispose();
                wheelMaxSuspension.Dispose();
                wheelSuspensionForce.Dispose();
                vehicleForwardForce.Dispose();
                wheelSideForce.Dispose();
                vehicleSteeringStep.Dispose();
                vehicleGear.Dispose();
                vehicleDrag.Dispose();
                vehicleMaxSpeed.Dispose();
                vehicleLength.Dispose();
                vehicleWheelDistance.Dispose();
                vehiclePowerStep.Dispose();
                vehicleBrakeStep.Dispose();
                vehicleTrigger.Dispose();
                vehicleSpecialDriveAction.Dispose();
                vehicleType.Dispose();
                vehicleActionValue.Dispose();
                wheelSign.Dispose();
                vehicleIsBraking.Dispose();
                ignoreVehicle.Dispose();
                activeCameraPositions.Dispose();
                closestObstacle.Dispose();
                trailerForwardForce.Dispose();
                trailerRightDirection.Dispose();
                trailerDrag.Dispose();
                massDifference.Dispose();
                triggerForwardDirection.Dispose();
                wheelSuspensionPosition.Dispose();
                vehicleDistanceToStop.Dispose();
                wheelSpringStiffness.Dispose();
            }
            catch { }

            AIEvents.onChangeDrivingState -= UpdateDrivingState;
            AIEvents.onChangeDestination -= DestinationChanged;
            Events.onVehicleAdded -= NewVehicleAdded;
        }

        internal void AddVehicleWithPath(Vector3 position, VehicleTypes vehicleType, Vector3 destination, UnityAction<VehicleComponent, int> completeMethod)
        {
            List<int> path = PathFindingManager.GetPath(position, destination, vehicleType);
            if (path != null)
            {
                //aici tre sa vina un callback
                DensityManager.AddVehicleAtPosition(position, vehicleType, completeMethod, path);
            }
        }

        internal void SetPriorityCrossingState(string crossingName, bool stop, bool stopUpdate)
        {
            var data = GridManager.GetCurrentSceneData();
            for(int i=0;i<data.allPriorityCrossings.Length;i++)
            {
                data.allPriorityCrossings[i].SetPriorityCrossingState(crossingName, stop, stopUpdate);
            }
        }

        internal bool GetPriorityCrossingState(string crossingName)
        {
            var data = GridManager.GetCurrentSceneData();
            for (int i = 0; i < data.allPriorityCrossings.Length; i++)
            {
                if(data.allPriorityCrossings[i].name == crossingName)
                {
                    return data.allPriorityCrossings[i].GetPriorityCrossingState();
                }
            }
            Debug.LogWarning($"{crossingName} not found");
            return false;
        }

        internal TrafficLightsColor GetTrafficLightsCrossingState(string crossingName)
        {
            var data = GridManager.GetCurrentSceneData();
            for (int i = 0; i < data.allLightsCrossings.Length; i++)
            {
                if (data.allLightsCrossings[i].name == crossingName)
                {
                    return data.allLightsCrossings[i].GetCrossingState();
                }
            }
            Debug.LogWarning($"{crossingName} not found");
            return TrafficLightsColor.Red;
        }
        #endregion
    }
}
#endif