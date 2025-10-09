//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright � 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Realistic_Traffic_Controller.Scripts;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Main class for lanes, waypoints, and traffic vehicles.
/// </summary>
public class RTC {

    /// <summary>
    /// Creates a scene manager.
    /// </summary>
    public static GameObject CreateSceneManager() {

        RTC_SceneManager sceneManager = RTC_SceneManager.Instance;
        return sceneManager.gameObject;

    }

    /// <summary>
    /// Creates a new lane.
    /// </summary>
    public static RTC_Lane CreateNewLane() {

        GameObject newLane = new GameObject("Lane_" + (LaneContainer().childCount).ToString());
        newLane.transform.position = Vector3.zero;
        newLane.transform.rotation = Quaternion.identity;
        newLane.transform.SetParent(LaneContainer(), true);
        newLane.transform.SetAsLastSibling();
        return newLane.AddComponent<RTC_Lane>();

    }

    /// <summary>
    /// Creates a new lane with given waypoints.
    /// </summary>
    /// <param name="waypointPositions"></param>
    /// <param name="radius"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static RTC_Lane CreateNewLaneWithWaypoints(Vector3[] waypointPositions, float radius, float speed) {

        RTC_Lane newLane = CreateNewLane();

        for (int i = 0; i < waypointPositions.Length; i++) {

            GameObject newWaypoint = new GameObject("Waypoint" + newLane.transform.childCount.ToString(), typeof(RTC_Waypoint));

            newWaypoint.transform.SetParent(newLane.transform, false);
            newWaypoint.transform.position = waypointPositions[i];
            newWaypoint.transform.position += Vector3.up * (newWaypoint.GetComponent<RTC_Waypoint>().radius / 2f);

            RTC_Waypoint newWaypointComponent = newWaypoint.GetComponent<RTC_Waypoint>();
            newWaypointComponent.connectedLane = newLane;
            newWaypointComponent.radius = radius;
            newWaypointComponent.targetSpeed = speed;

        }

        newLane.UpdateLane();
        ReOrderWaypoints(newLane);
        newLane.UpdateLane();

        return newLane;

    }

    /// <summary>
    /// Creates a new waypoint with given lane, position, radius, and target speed.
    /// </summary>
    /// <param name="lane"></param>
    /// <param name="position"></param>
    /// <param name="createWithRadius"></param>
    /// <param name="createWithSpeed"></param>
    public static void CreateNewWaypoint(RTC_Lane lane, Vector3 position, float createWithRadius, float createWithSpeed) {

        GameObject newWaypoint = new GameObject("Waypoint" + lane.transform.childCount.ToString(), typeof(RTC_Waypoint));

        newWaypoint.transform.SetParent(lane.transform, false);
        newWaypoint.transform.position = position;
        newWaypoint.transform.position += Vector3.up * (newWaypoint.GetComponent<RTC_Waypoint>().radius / 2f);

        RTC_Waypoint newWaypointComponent = newWaypoint.GetComponent<RTC_Waypoint>();
        newWaypointComponent.connectedLane = lane;
        newWaypointComponent.radius = createWithRadius;
        newWaypointComponent.targetSpeed = createWithSpeed;

        lane.UpdateLane();

    }

    /// <summary>
    /// Creates a previous waypoint with given lane, position, radius, and target speed.
    /// </summary>
    /// <param name="lane"></param>
    /// <param name="position"></param>
    /// <param name="createWithRadius"></param>
    /// <param name="createWithSpeed"></param>
    public static void CreatePreviousWaypoint(RTC_Lane lane, Vector3 position, float createWithRadius, float createWithSpeed) {

        GameObject newWaypoint = new GameObject("Waypoint" + lane.transform.childCount.ToString(), typeof(RTC_Waypoint));

        newWaypoint.transform.SetParent(lane.transform, false);
        newWaypoint.transform.position = position;
        newWaypoint.transform.position += Vector3.up * (newWaypoint.GetComponent<RTC_Waypoint>().radius / 2f);

        RTC_Waypoint newWaypointComponent = newWaypoint.GetComponent<RTC_Waypoint>();
        newWaypointComponent.connectedLane = lane;
        newWaypointComponent.radius = createWithRadius;
        newWaypointComponent.targetSpeed = createWithSpeed;

        RTC_Waypoint firstWaypoint = lane.waypoints[0];

        if (firstWaypoint.previousWaypoint) {

            newWaypointComponent.previousWaypoint = firstWaypoint.previousWaypoint;
            firstWaypoint.previousWaypoint.nextWaypoint = newWaypointComponent;

        }

        newWaypointComponent.nextWaypoint = firstWaypoint;
        firstWaypoint.previousWaypoint = newWaypointComponent;

        newWaypointComponent.transform.SetSiblingIndex(firstWaypoint.transform.GetSiblingIndex());

        lane.UpdateLane();

    }

    /// <summary>
    /// Creates a next waypoint with given lane, position, radius, and target speed.
    /// </summary>
    /// <param name="lane"></param>
    /// <param name="position"></param>
    /// <param name="createWithRadius"></param>
    /// <param name="createWithSpeed"></param>
    public static void CreateNextWaypoint(RTC_Lane lane, Vector3 position, float createWithRadius, float createWithSpeed) {

        GameObject newWaypoint = new GameObject("Waypoint" + lane.transform.childCount.ToString(), typeof(RTC_Waypoint));
        newWaypoint.transform.SetParent(lane.transform, false);
        newWaypoint.transform.position = position;
        newWaypoint.transform.position += Vector3.up * (newWaypoint.GetComponent<RTC_Waypoint>().radius / 2f);

        RTC_Waypoint newWaypointComponent = newWaypoint.GetComponent<RTC_Waypoint>();
        newWaypointComponent.connectedLane = lane;
        newWaypointComponent.radius = createWithRadius;
        newWaypointComponent.targetSpeed = createWithSpeed;

        RTC_Waypoint lastWaypoint = lane.waypoints[lane.waypoints.Length - 1].gameObject.GetComponent<RTC_Waypoint>();
        newWaypointComponent.previousWaypoint = lastWaypoint;

        if (lastWaypoint.nextWaypoint) {

            lastWaypoint.nextWaypoint.previousWaypoint = newWaypointComponent;
            newWaypointComponent.nextWaypoint = lastWaypoint.nextWaypoint;

        }

        lastWaypoint.nextWaypoint = newWaypointComponent;
        newWaypointComponent.transform.SetAsLastSibling();

        lane.UpdateLane();

    }

    /// <summary>
    /// Creates Waypoints between two given Waypoints. Special thanks to "Daniel Furer (daniel.furer@quick-line.ch)!"
    /// </summary>
    public static void AddWaypointsBetween(RTC_Waypoint wp1, RTC_Waypoint wp2, float minimumDistance = 15f) {

        // Horizontal distance between the two Waypoints
        float horizontalDistance = Vector3.Distance(new Vector3(wp1.transform.position.x, 0f, wp1.transform.position.z), new Vector3(wp2.transform.position.x, 0f, wp2.transform.position.z));

        // Check if distance is big enough
        if (horizontalDistance > (minimumDistance * 2f)) {

            // Midpoint between the two Waypoints
            Vector3 midpoint = (wp1.transform.position + wp2.transform.position) / 2f;

            // Create new Waypoint in the middle
            RTC_Waypoint newWaypoint = new GameObject().AddComponent<RTC_Waypoint>();
            newWaypoint.transform.position = midpoint;
            newWaypoint.transform.SetParent(wp1.transform.parent);

            // Connect new Waypoint
            wp1.nextWaypoint = newWaypoint;
            newWaypoint.nextWaypoint = wp2;
            wp2.previousWaypoint = newWaypoint;
            newWaypoint.previousWaypoint = wp1;

            // Try to add more Waypoints between previous Waypoints and created Waypoint
            AddWaypointsBetween(wp1, newWaypoint, minimumDistance);
            AddWaypointsBetween(newWaypoint, wp2, minimumDistance);

            wp1.connectedLane.UpdateLane();
            ReOrderWaypoints(wp1.connectedLane);
            wp1.connectedLane.UpdateLane();

        }

    }

    /// <summary>
    /// Reorders all waypoints of the lane.
    /// </summary>
    /// <param name="lane"></param>
    public static void ReOrderWaypoints(RTC_Lane lane) {

        if (lane.waypoints == null)
            return;

        RTC_Waypoint[] waypoints = lane.waypoints;

        for (int i = 0; i < waypoints.Length; i++) {

            if (waypoints[i] != null) {

                if (i < waypoints.Length - 1)
                    waypoints[i].nextWaypoint = waypoints[i + 1];

                if (i > 0)
                    waypoints[i].previousWaypoint = waypoints[i - 1];

            }

        }

        lane.UpdateLane();

    }

    /// <summary>
    /// Closes the circuit by connecting first and last waypoints together.
    /// </summary>
    /// <param name="lane"></param>
    public static void CloseCircuit(RTC_Lane lane) {

        if (lane.waypoints == null)
            return;

        RTC_Waypoint firstWaypoint = FindFirstWaypointOfLane(lane);
        RTC_Waypoint lastWaypoint = FindLastWaypointOfLane(lane);

        if (firstWaypoint != null && lastWaypoint != null) {

            firstWaypoint.previousWaypoint = lastWaypoint;
            lastWaypoint.nextWaypoint = firstWaypoint;

            lane.UpdateLane();

            Debug.Log("Closed circuit of lane  " + lane.transform.name + ".");

        }

    }

    /// <summary>
    /// Breaks the circuit by disconnecting first and last waypoints from each other.
    /// </summary>
    /// <param name="lane"></param>
    public static void BreakCircuit(RTC_Lane lane) {

        if (lane.waypoints == null)
            return;

        RTC_Waypoint firstWaypoint = FindFirstWaypointOfLane(lane);
        RTC_Waypoint lastWaypoint = FindLastWaypointOfLane(lane);

        if (firstWaypoint != null && lastWaypoint != null) {

            firstWaypoint.previousWaypoint = null;
            lastWaypoint.nextWaypoint = null;

            lane.UpdateLane();

        }

        Debug.Log("Breaked circuit of lane  " + lane.transform.name + ".");

    }

    /// <summary>
    /// Inverse direction.
    /// </summary>
    /// <param name="lane"></param>
    public static void ReverseCircuit(RTC_Lane lane) {

        if (lane.waypoints == null)
            return;

        RTC_Waypoint[] allWaypoints = lane.waypoints;

        for (int i = 0; i < allWaypoints.Length; i++) {

            if (allWaypoints[i] != null) {

                allWaypoints[i].transform.SetAsFirstSibling();
                allWaypoints[i].previousWaypoint = null;
                allWaypoints[i].nextWaypoint = null;

            }

        }

        for (int i = 0; i < lane.transform.childCount; i++) {

            if (i < lane.transform.childCount - 1)
                lane.transform.GetChild(i).GetComponent<RTC_Waypoint>().nextWaypoint = lane.transform.GetChild(i + 1).GetComponent<RTC_Waypoint>();

            if (i != 0)
                lane.transform.GetChild(i).GetComponent<RTC_Waypoint>().previousWaypoint = lane.transform.GetChild(i - 1).GetComponent<RTC_Waypoint>();

        }

        lane.UpdateLane();

        Debug.Log("Reversed lane  " + lane.transform.name + ".");

    }

    /// <summary>
    /// Aligns all waypoints by X axis.
    /// </summary>
    /// <param name="lane"></param>
    public static void AlignX(RTC_Lane lane) {

        if (lane.waypoints == null)
            return;

        float averageX = 0f;
        int count = 0;

        for (int i = 0; i < lane.waypoints.Length; i++) {

            if (lane.waypoints[i] != null) {

                averageX += lane.waypoints[i].transform.localPosition.x;
                count++;

            }

        }

        averageX /= Mathf.Clamp(count, 1, Mathf.Infinity);

        for (int i = 0; i < lane.waypoints.Length; i++) {

            if (lane.waypoints[i] != null)
                lane.waypoints[i].transform.localPosition = new Vector3(averageX, lane.waypoints[i].transform.localPosition.y, lane.waypoints[i].transform.localPosition.z);

        }

        lane.UpdateLane();

        Debug.Log("Aligned all waypoints X positions of the lane " + lane.transform.name + ".");

    }

    /// <summary>
    /// Aligns all waypoints by Y axis.
    /// </summary>
    /// <param name="lane"></param>
    public static void AlignY(RTC_Lane lane) {

        if (lane.waypoints == null)
            return;

        float averageY = 0f;
        int count = 0;

        for (int i = 0; i < lane.waypoints.Length; i++) {

            if (lane.waypoints[i] != null) {

                averageY += lane.waypoints[i].transform.localPosition.y;
                count++;

            }

        }

        averageY /= Mathf.Clamp(lane.waypoints.Length, 1, Mathf.Infinity);

        for (int i = 0; i < lane.waypoints.Length; i++) {

            if (lane.waypoints[i] != null)
                lane.waypoints[i].transform.localPosition = new Vector3(lane.waypoints[i].transform.localPosition.x, averageY, lane.waypoints[i].transform.localPosition.z);

        }

        lane.UpdateLane();

        Debug.Log("Aligned all waypoints Y positions of the lane " + lane.transform.name + ".");

    }

    /// <summary>
    /// Aligns all waypoints by Z axis.
    /// </summary>
    /// <param name="lane"></param>
    public static void AlignZ(RTC_Lane lane) {

        if (lane.waypoints == null)
            return;

        float averageZ = 0f;
        int count = 0;


        for (int i = 0; i < lane.waypoints.Length; i++) {

            if (lane.waypoints[i] != null) {

                averageZ += lane.waypoints[i].transform.localPosition.z;
                count++;

            }

        }

        averageZ /= Mathf.Clamp(lane.waypoints.Length, 1, Mathf.Infinity);

        for (int i = 0; i < lane.waypoints.Length; i++) {

            if (lane.waypoints[i] != null)
                lane.waypoints[i].transform.localPosition = new Vector3(lane.waypoints[i].transform.localPosition.x, lane.waypoints[i].transform.localPosition.y, averageZ);

        }

        lane.UpdateLane();

        Debug.Log("Aligned all waypoints Z positions of the lane " + lane.transform.name + ".");

    }

    /// <summary>
    /// Inserts a new waypoint at given index.
    /// </summary>
    /// <param name="lane"></param>
    /// <param name="waypoint"></param>
    /// <param name="index"></param>
    public static void InsertWaypoint(RTC_Lane lane, RTC_Waypoint waypoint, int index) {

        waypoint.transform.SetSiblingIndex(index + 1);
        List<RTC_Waypoint> waypoints = new List<RTC_Waypoint>();

        for (int i = 0; i < lane.waypoints.Length; i++) {

            if (lane.waypoints[i] != null)
                waypoints.Add(lane.waypoints[i]);

        }

        waypoints.Insert(index, waypoint);

        lane.waypoints = waypoints.ToArray();
        lane.UpdateLane();

        Debug.Log("Inserted a new waypoint on lane " + lane.transform.name + ".");

    }

    /// <summary>
    /// Finds nearest point on the line.
    /// </summary>
    /// <param name="linePnt"></param>
    /// <param name="lineDir"></param>
    /// <param name="pnt"></param>
    /// <returns></returns>
    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt) {

        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;

    }

    /// <summary>
    /// Gets the container for lanes or creates it.
    /// </summary>
    /// <returns></returns>
    public static Transform LaneContainer() {

        Transform laneManager;

        if (GameObject.Find("RTC_Lanes"))
            laneManager = GameObject.Find("RTC_Lanes").transform;
        else
            laneManager = null;

        if (laneManager) {

            laneManager.transform.position = Vector3.zero;
            laneManager.transform.rotation = Quaternion.identity;
            laneManager.transform.localScale = Vector3.one;

        } else {

            laneManager = new GameObject("RTC_Lanes").transform;

        }

        return laneManager;

    }

    /// <summary>
    /// Creates the wheel colliders.
    /// </summary>
    public static void CreateWheelColliders(RTC_CarController carController) {

        carController.gameObject.SetActive(true);

        // Creating a list for all wheel models.
        List<Transform> allWheelModels = new List<Transform>();

        for (int i = 0; i < carController.wheels.Length; i++) {

            if (carController.wheels[i] != null)
                allWheelModels.Add(carController.wheels[i].wheelModel);

        }

        // If we don't have any wheelmodels, throw an error.
        bool missingWheelFound = false;

        for (int i = 0; i < allWheelModels.Count; i++) {

            if (allWheelModels[i] == null) {

                missingWheelFound = true;
                break;

            }

        }

        if (missingWheelFound) {

            Debug.LogError("You haven't choosen your wheel models properly. Please select all of your wheel models before creating wheel colliders. Script needs to know their sizes and positions, aye?");
            return;

        }

        // Holding default rotation.
        Quaternion currentRotation = carController.transform.rotation;

        // Resetting rotation.
        carController.transform.rotation = Quaternion.identity;

        // Creating a new gameobject called Wheel Colliders for all Wheel Colliders, and parenting it to this gameobject.
        GameObject WheelColliders = new GameObject("Wheel Colliders");
        WheelColliders.transform.SetParent(carController.transform, false);
        WheelColliders.transform.localRotation = Quaternion.identity;
        WheelColliders.transform.localPosition = Vector3.zero;
        WheelColliders.transform.localScale = Vector3.one;

        // Creating WheelColliders.
        foreach (Transform wheel in allWheelModels) {

            GameObject wheelcollider = new GameObject(wheel.transform.name);

            wheelcollider.transform.SetPositionAndRotation(RTC_GetBounds.GetBoundsCenter(wheel.transform), carController.transform.rotation);
            wheelcollider.transform.name = wheel.transform.name;
            wheelcollider.transform.SetParent(WheelColliders.transform);
            wheelcollider.transform.localScale = Vector3.one;
            wheelcollider.AddComponent<WheelCollider>();

            Bounds biggestBound = new Bounds();
            Renderer[] renderers = wheel.GetComponentsInChildren<Renderer>();

            foreach (Renderer render in renderers) {
                if (render != carController.GetComponent<Renderer>()) {
                    if (render.bounds.size.z > biggestBound.size.z)
                        biggestBound = render.bounds;
                }
            }

            wheelcollider.GetComponent<WheelCollider>().radius = (biggestBound.extents.y) / carController.transform.localScale.y;
            JointSpring spring = wheelcollider.GetComponent<WheelCollider>().suspensionSpring;

            spring.spring = 45000f;
            spring.damper = 2500f;
            spring.targetPosition = .5f;

            wheelcollider.GetComponent<WheelCollider>().suspensionSpring = spring;
            wheelcollider.GetComponent<WheelCollider>().suspensionDistance = .2f;
            wheelcollider.GetComponent<WheelCollider>().forceAppPointDistance = .1f;
            wheelcollider.GetComponent<WheelCollider>().mass = 40f;
            wheelcollider.GetComponent<WheelCollider>().wheelDampingRate = 1f;

            WheelFrictionCurve sidewaysFriction;
            WheelFrictionCurve forwardFriction;

            sidewaysFriction = wheelcollider.GetComponent<WheelCollider>().sidewaysFriction;
            forwardFriction = wheelcollider.GetComponent<WheelCollider>().forwardFriction;

            forwardFriction.extremumSlip = .375f;
            forwardFriction.extremumValue = 1f;
            forwardFriction.asymptoteSlip = .8f;
            forwardFriction.asymptoteValue = .5f;
            forwardFriction.stiffness = 1.25f;

            sidewaysFriction.extremumSlip = .275f;
            sidewaysFriction.extremumValue = 1f;
            sidewaysFriction.asymptoteSlip = .5f;
            sidewaysFriction.asymptoteValue = .75f;
            sidewaysFriction.stiffness = 1.25f;

            wheelcollider.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction;
            wheelcollider.GetComponent<WheelCollider>().forwardFriction = forwardFriction;

        }

        WheelCollider[] allWheelColliders = carController.GetComponentsInChildren<WheelCollider>();

        for (int i = 0; i < carController.wheels.Length; i++) {

            if (carController.wheels[i] != null)
                carController.wheels[i].wheelCollider = allWheelColliders[i];

        }

        carController.transform.rotation = currentRotation;

        Debug.Log("Created wheel colliders for " + carController.transform.name + ".");

    }

    /// <summary>
    /// Updates all lanes and waypoints.
    /// </summary>
    public static void UpdateEverything() {

        RTC_SceneManager.Instance.UpdateEverything();
        Debug.Log("Updated all lanes and their waypoints.");

    }

    /// <summary>
    /// Clamps the vector with minimum and maximum vectors.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static Vector3 ClampVector(Vector3 target, Vector3 vector1, Vector3 vector2) {

        float minX = vector1.x;
        float maxX = vector2.x;

        float minY = vector1.y;
        float maxY = vector2.y;

        float minZ = vector1.z;
        float maxZ = vector2.z;

        float minX1, maxX1, minY1, maxY1, minZ1, maxZ1;

        if (minX > maxX) {

            minX1 = maxX;
            maxX1 = minX;

        } else {

            minX1 = minX;
            maxX1 = maxX;

        }

        if (minY > maxY) {

            minY1 = maxY;
            maxY1 = minY;

        } else {

            minY1 = minY;
            maxY1 = maxY;

        }

        if (minZ > maxZ) {

            minZ1 = maxZ;
            maxZ1 = minZ;

        } else {

            minZ1 = minZ;
            maxZ1 = maxZ;

        }

        Vector3 clampedVector = new Vector3(Mathf.Clamp(target.x, minX1, maxX1), Mathf.Clamp(target.y, minY1, maxY1), Mathf.Clamp(target.z, minZ1, maxZ1));
        return clampedVector;

    }

    public static bool EqualVehicleType(RTC_VehicleTypes.VehicleType vehicleType, RTC_Lane lane) {

        bool equal = false;

        switch (vehicleType) {

            case RTC_VehicleTypes.VehicleType.Bus:

                if (lane.supportedBus)
                    equal = true;

                break;

            case RTC_VehicleTypes.VehicleType.Exotic:

                if (lane.supportedExotic)
                    equal = true;

                break;

            case RTC_VehicleTypes.VehicleType.Heavy:

                if (lane.supportedHeavy)
                    equal = true;

                break;

            case RTC_VehicleTypes.VehicleType.Light:

                if (lane.supportedLight)
                    equal = true;

                break;

            case RTC_VehicleTypes.VehicleType.Medium:

                if (lane.supportedMedium)
                    equal = true;

                break;

            case RTC_VehicleTypes.VehicleType.Police:

                if (lane.supportedPolice)
                    equal = true;

                break;

            case RTC_VehicleTypes.VehicleType.Sedan:

                if (lane.supportedSedan)
                    equal = true;

                break;

            case RTC_VehicleTypes.VehicleType.Taxi:

                if (lane.supportedTaxi)
                    equal = true;

                break;

            case RTC_VehicleTypes.VehicleType.Van:

                if (lane.supportedVan)
                    equal = true;

                break;

        }

        return equal;

    }

    /// <summary>
    /// Creates new audiosource with specified settings.
    /// </summary>
    public static AudioSource NewAudioSource(GameObject go, Vector3 localPosition, string audioName, float minDistance, float maxDistance, float volume, AudioClip audioClip, bool loop, bool playNow) {

        GameObject audioSourceObject = new GameObject(audioName);

        if (go.transform.Find("All Audio Sources")) {

            audioSourceObject.transform.SetParent(go.transform.Find("All Audio Sources"));

        } else {

            GameObject allAudioSources = new GameObject("All Audio Sources");
            allAudioSources.transform.SetParent(go.transform, false);
            audioSourceObject.transform.SetParent(allAudioSources.transform, false);

        }

        audioSourceObject.transform.SetPositionAndRotation(go.transform.position, go.transform.rotation);
        audioSourceObject.transform.localPosition = localPosition;

        audioSourceObject.AddComponent<AudioSource>();
        AudioSource source = audioSourceObject.GetComponent<AudioSource>();

        //audioSource.GetComponent<AudioSource>().priority =1;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.volume = volume;
        source.clip = audioClip;
        source.loop = loop;
        source.dopplerLevel = 0f;

        if (minDistance == 0 && maxDistance == 0)
            source.spatialBlend = 0f;
        else
            source.spatialBlend = 1f;

        if (playNow) {

            source.playOnAwake = true;
            source.Play();

        } else {

            source.playOnAwake = false;

        }

        return source;

    }

    public static RTC_Waypoint FindFirstWaypointOfLane(RTC_Lane lane) {

        if (lane.waypoints == null)
            return null;

        for (int i = 0; i < lane.waypoints.Length; i++) {

            if (lane.waypoints[i] != null)
                return lane.waypoints[i];

        }

        return null;

    }

    public static RTC_Waypoint FindLastWaypointOfLane(RTC_Lane lane) {

        if (lane.waypoints == null)
            return null;

        for (int i = lane.waypoints.Length - 1; i > 0; i++) {

            if (lane.waypoints[i] != null)
                return lane.waypoints[i];

        }

        return null;

    }

}

