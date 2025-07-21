//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Traffic spawner and manager.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Traffic Controller/RTC Traffic Spawner")]
public class RTC_TrafficSpawner : RTC_Core {

    [System.Serializable]
    public class TrafficVehicle {

        /// <summary>
        /// Traffic vehicle.
        /// </summary>
        public RTC_CarController trafficVehicle;

        /// <summary>
        /// Amount.
        /// </summary>
        [Range(1, 100)] public int amount = 100;

    }

    /// <summary>
    /// Spawnable traffic vehicles.
    /// </summary>
    public TrafficVehicle[] trafficVehicles;

    /// <summary>
    /// Maximum acceptable traffic vehicles count.
    /// </summary>
    public int maximumActiveVehicles = 6;

    /// <summary>
    /// Traffic vehicles will be spawned at this radius.
    /// </summary>
    [Range(0f, 1000f)] public float radius = 300f;

    /// <summary>
    /// Traffic vehicles will not be spawned at this radius.
    /// </summary>
    [Range(0f, 1000f)] public float closeRadius = 150f;

    /// <summary>
    /// Auto calculates the close radius.
    /// </summary>
    public bool autoCalculateCloseRadius = true;

    /// <summary>
    ///  Ignores close radius on enable, because we want to populate the scene first.
    /// </summary>
    public bool ignoreCloseRadiusOnEnable = true;

    /// <summary>
    /// Main camera.
    /// </summary>
    public Camera mainCamera;

    public enum FollowType {

        FollowCamera,
        ParentCamera,
        Off

    }

    /// <summary>
    /// Follow type for main camera.
    /// </summary>
    public FollowType followType = FollowType.FollowCamera;

    /// <summary>
    /// Won't spawn / transports traffic vehicles behind the camera.
    /// </summary>
    public bool dontSpawnBehindCamera = false;

    /// <summary>
    /// Disables traffic vehicles behind the camera within distance.
    /// </summary>
    public bool disableVehiclesBehindCamera = false;

    /// <summary>
    /// Maximum distance to disable traffic vehicles behind the camera. If above bool is enabled.
    /// </summary>
    [Range(0f, 1000f)] public float distanceToDisable = 50f;

    //  Spawned traffic vehicles list and container for them.
    public List<RTC_CarController> spawnedTrafficVehicles = new List<RTC_CarController>();
    private Transform container;

    /// <summary>
    /// Checks all traffic vehicles once in per second.
    /// </summary>
    [Range(1, 5)] public int checkTrafficPerSecond = 1;

    /// <summary>
    /// When spawner spawned.
    /// </summary>
    /// <param name="trafficSpawner"></param>
    public delegate void onSpawnerSpawned(RTC_TrafficSpawner trafficSpawner);
    public static event onSpawnerSpawned OnSpawnerSpawned;

    //  Creating a list for waypoints in radius.
    public List<RTC_Waypoint> waypointsInRadius = new List<RTC_Waypoint>();

    private void Awake() {

        //  Creating or finding container for spawned traffic vehicles.
        if (GameObject.Find("RTC_TrafficVehiclesContainer"))
            container = GameObject.Find("RTC_TrafficVehiclesContainer").transform;
        else
            container = new GameObject("RTC_TrafficVehiclesContainer").transform;

        container.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        CreateAllVehicles();

    }

    /// <summary>
    /// Creates all traffic vehicles.
    /// </summary>
    private void CreateAllVehicles() {

        //  Spawning all traffic vehicles at once and adding them to the list. And make sure they are disabled as well.
        if (trafficVehicles != null) {

            for (int i = 0; i < trafficVehicles.Length; i++) {

                for (int k = 0; k < trafficVehicles[i].amount; k++) {

                    if (trafficVehicles[i].trafficVehicle != null) {

                        RTC_CarController spawned = Instantiate(trafficVehicles[i].trafficVehicle.gameObject, Vector3.zero, Quaternion.identity, container).GetComponent<RTC_CarController>();
                        spawned.gameObject.SetActive(false);
                        spawnedTrafficVehicles.Add(spawned);

                    }

                }

            }

        }

        //  Shuffle the list for random order.
        spawnedTrafficVehicles = spawnedTrafficVehicles.OrderBy(x => Random.value).ToList();

    }

    private void Start() {

        //  Calling an event when spawner spawned.
        if (OnSpawnerSpawned != null)
            OnSpawnerSpawned(this);

        //  Checking active traffic vehicles.
        InvokeRepeating(nameof(Check), .04f, checkTrafficPerSecond);

    }

    private void OnEnable() {

        //  Ignoring close radius to populate the scene at start.
        if (ignoreCloseRadiusOnEnable)
            StartCoroutine(IgnoreCloseRadius());

    }

    /// <summary>
    /// Ignoring close radius to populate the scene at start.
    /// </summary>
    /// <returns></returns>
    private IEnumerator IgnoreCloseRadius() {

        //  Saving current close radius as temp value, setting it to 0, checking the vehicles, and set close radius back to original value.
        float currentCloserRadius = closeRadius;
        bool currentAutoCalculateCloseRadius = autoCalculateCloseRadius;
        autoCalculateCloseRadius = false;
        closeRadius = 50f;

        yield return new WaitForSeconds(checkTrafficPerSecond);
        autoCalculateCloseRadius = currentAutoCalculateCloseRadius;
        closeRadius = currentCloserRadius;

    }

    private void Update() {

        //  If auto calculate close radius is enabled, divide radius by 2.
        if (autoCalculateCloseRadius)
            closeRadius = radius * .5f;

        //  Close radius can't be higher than the radius.
        if (closeRadius > radius)
            closeRadius = radius;

        //  Close radius can't be higher than the 150.
        if (closeRadius > 150f)
            closeRadius = 150f;

        //  Radius can't be lower than the 0.
        if (radius < 0)
            radius = 0;

        //  Close radius can't be lower than the 0.
        if (closeRadius < 0)
            closeRadius = 0;

        //  If a traffic vehicle is far from radius, disable it.
        if (spawnedTrafficVehicles != null) {

            for (int i = 0; i < spawnedTrafficVehicles.Count; i++) {

                if (spawnedTrafficVehicles[i] != null && Vector3.Distance(spawnedTrafficVehicles[i].transform.position, transform.position) > radius)
                    spawnedTrafficVehicles[i].gameObject.SetActive(false);

            }

        }

    }

    private void LateUpdate() {

        //  Getting main camera if not selected.
        if (!mainCamera)
            mainCamera = Camera.main;

        //  If main camera found, but it's not active, return.
        if (mainCamera && !mainCamera.gameObject.activeSelf)
            return;

        switch (followType) {

            //  Follows the main camera without parenting to it.
            case FollowType.FollowCamera:

                //  If main camera is selected, set position and rotation of the spawner.
                if (mainCamera) {

                    //  Make sure root of the spawner is not child object of the main camera.
                    if (transform.parent == mainCamera.transform)
                        transform.SetParent(null, false);

                    //  Setting position and rotation of the spawner.
                    transform.position = mainCamera.transform.position;
                    transform.forward = mainCamera.transform.forward;

                }

                break;

            //  Parents to the main camera.
            case FollowType.ParentCamera:

                //  If main camera is selected, set parent. Spawner will be a child gameobject of the main camera.
                if (mainCamera) {

                    //  If root of the spawner is not child object of the main camera, set parent.
                    if (transform.parent != mainCamera.transform)
                        transform.SetParent(mainCamera.transform, false);

                    //  Make sure local position and rotation of the spawner is set to 0.
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;

                }

                break;

            //  Don't follow or parent.
            case FollowType.Off:
                break;

        }

    }

    /// <summary>
    /// Checks all spawned traffic vehicles. Enables / disables them.
    /// </summary>
    private void Check() {

        //  Getting closer waypoints to the spawner.
        GetClosestWaypoints();

        //  If waypoints in radius is null, return.
        if (waypointsInRadius == null)
            return;

        //  If count of the radius is below 1, return.
        if (waypointsInRadius.Count < 1)
            return;

        if (spawnedTrafficVehicles != null) {

            //  Checking for total active vehicles.
            for (int i = 0; i < spawnedTrafficVehicles.Count; i++) {

                //  Calculating total active vehicles.
                int totalActiveVehicles = 0;

                //  Calculating total active vehicles.
                for (int k = 0; k < spawnedTrafficVehicles.Count; k++) {

                    if (spawnedTrafficVehicles[k] != null && spawnedTrafficVehicles[k].gameObject.activeSelf)
                        totalActiveVehicles++;

                }

                //  If total active vehicles is below limit, activate it.
                if (spawnedTrafficVehicles[i] != null && totalActiveVehicles < maximumActiveVehicles && !spawnedTrafficVehicles[i].gameObject.activeSelf) {

                    RTC_Waypoint closestWaypoint = null;

                    //  Getting closest waypoint to the radius.
                    if (waypointsInRadius.Count >= 1)
                        closestWaypoint = waypointsInRadius[Random.Range(0, waypointsInRadius.Count)];

                    //  Setting position and rotation of the spawned traffic vehicle.
                    if (closestWaypoint) {

                        spawnedTrafficVehicles[i].transform.SetPositionAndRotation(closestWaypoint.transform.position, closestWaypoint.transform.rotation);
                        spawnedTrafficVehicles[i].SetWaypoint(closestWaypoint);

                        //  Activating traffic vehicle if possible.
                        spawnedTrafficVehicles[i].gameObject.SetActive(CheckTraffic(spawnedTrafficVehicles[i]));

                    }

                }

                //  If spawned vehicle is active...
                if (spawnedTrafficVehicles[i] != null && spawnedTrafficVehicles[i].gameObject.activeSelf) {

                    //  If enabled and spawned vehicle is at behind of the spawner, disable the vehicle.
                    if (disableVehiclesBehindCamera) {

                        if (Vector3.Dot((spawnedTrafficVehicles[i].transform.position - transform.position).normalized, mainCamera.transform.forward) < 0f && Vector3.Distance(spawnedTrafficVehicles[i].transform.position, transform.position) >= distanceToDisable)
                            spawnedTrafficVehicles[i].gameObject.SetActive(false);

                    }

                }

            }

        }

    }

    /// <summary>
    /// Gets closest waypoints to the spawner.
    /// </summary>
    /// <returns></returns>
    private void GetClosestWaypoints() {

        //  Creating a list for waypoints in radius.
        if (waypointsInRadius == null)
            waypointsInRadius = new List<RTC_Waypoint>();

        waypointsInRadius.Clear();

        //  Looping all waypoints.
        if (RTCSceneManager.allWaypoints != null) {

            for (int i = 0; i < RTCSceneManager.allWaypoints.Length; i++) {

                //  If waypoint is in radius, add it to the list.
                if (RTCSceneManager.allWaypoints[i] != null && Vector3.Distance(RTCSceneManager.allWaypoints[i].transform.position, transform.position) < (radius * 1f))
                    waypointsInRadius.Add(RTCSceneManager.allWaypoints[i]);

            }

        }

        //  Looping all waypoints in radius.
        for (int i = 0; i < waypointsInRadius.Count; i++) {

            //  Excluding waypoints in close radius to prevent spawning too close vehicles.
            if (waypointsInRadius[i] != null && Vector3.Distance(waypointsInRadius[i].transform.position, transform.position) <= closeRadius)
                waypointsInRadius[i] = null;

        }

        //  Creating final list for proper waypoints.
        List<RTC_Waypoint> properWaypointsInRadius = new List<RTC_Waypoint>();

        for (int i = 0; i < waypointsInRadius.Count; i++) {

            //  If element is not null, add it to the proper list.
            if (waypointsInRadius[i] != null)
                properWaypointsInRadius.Add(waypointsInRadius[i]);

        }

        //  Clear the waypoints in radius list. 
        waypointsInRadius.Clear();

        //  Set the final list.
        waypointsInRadius = properWaypointsInRadius;

    }

    /// <summary>
    /// Checks the traffic vehicles. If a vehicle is too close to any other vehicles, will return false.
    /// </summary>
    /// <param name="vehicle"></param>
    /// <returns></returns>
    private bool CheckTraffic(RTC_CarController vehicle) {

        //  Looping spawned traffic vehicles.
        if (spawnedTrafficVehicles != null) {

            for (int i = 0; i < spawnedTrafficVehicles.Count; i++) {

                if (spawnedTrafficVehicles[i] != null) {

                    //  If spawned traffic vehicle is active and distance to it is above 25 meters, return false. Otherwise return true.
                    if (spawnedTrafficVehicles[i].gameObject.activeSelf && Vector3.Distance(spawnedTrafficVehicles[i].transform.position, vehicle.transform.position) < 25)
                        return false;

                    //  If spawned traffic vehicle is active and distance to it is above 25 meters, return false. Otherwise return true.
                    switch (vehicle.vehicleType) {

                        case RTC_VehicleTypes.VehicleType.Light:

                            if (vehicle.CurrentLane && !vehicle.CurrentLane.supportedLight)
                                return false;

                            break;

                        case RTC_VehicleTypes.VehicleType.Medium:

                            if (vehicle.CurrentLane && !vehicle.CurrentLane.supportedMedium)
                                return false;

                            break;

                        case RTC_VehicleTypes.VehicleType.Heavy:

                            if (vehicle.CurrentLane && !vehicle.CurrentLane.supportedHeavy)
                                return false;

                            break;

                        case RTC_VehicleTypes.VehicleType.Sedan:

                            if (vehicle.CurrentLane && !vehicle.CurrentLane.supportedSedan)
                                return false;

                            break;

                        case RTC_VehicleTypes.VehicleType.Taxi:

                            if (vehicle.CurrentLane && !vehicle.CurrentLane.supportedTaxi)
                                return false;

                            break;

                        case RTC_VehicleTypes.VehicleType.Bus:

                            if (vehicle.CurrentLane && !vehicle.CurrentLane.supportedBus)
                                return false;

                            break;

                        case RTC_VehicleTypes.VehicleType.Exotic:

                            if (vehicle.CurrentLane && !vehicle.CurrentLane.supportedExotic)
                                return false;

                            break;

                        case RTC_VehicleTypes.VehicleType.Van:

                            if (vehicle.CurrentLane && !vehicle.CurrentLane.supportedVan)
                                return false;

                            break;

                        case RTC_VehicleTypes.VehicleType.Police:

                            if (vehicle.CurrentLane && !vehicle.CurrentLane.supportedPolice)
                                return false;

                            break;

                    }

                    if (RTCSceneManager.dontSpawnArounds != null) {

                        for (int k = 0; k < RTCSceneManager.dontSpawnArounds.Length; k++) {

                            //  If spawned traffic vehicle is active and distance to it is above 25 meters, return false. Otherwise return true.
                            if (Vector3.Distance(spawnedTrafficVehicles[i].transform.position, RTCSceneManager.dontSpawnArounds[k].transform.position) < (RTCSceneManager.dontSpawnArounds[k].radius))
                                return false;

                        }

                    }

                }

            }

        }

        //  If enabled, and spawned vehicle is at behind of the camera, return false. Otherwise return true.
        if (mainCamera != null && dontSpawnBehindCamera) {

            if (Vector3.Dot((vehicle.transform.position - transform.position).normalized, mainCamera.transform.forward) < 0f)
                return false;

        }

        return true;

    }

    private void OnValidate() {

        if (trafficVehicles != null) {

            for (int i = 0; i < trafficVehicles.Length; i++) {

                if (trafficVehicles[i] != null) {

                    if (trafficVehicles[i].amount == 0)
                        trafficVehicles[i].amount = 100;

                }

            }

        }

    }

}
