//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single waypoint of the lane.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Traffic Controller/RTC Waypoint")]
public class RTC_Waypoint : MonoBehaviour {

    /// <summary>
    /// Position of the waypoint.
    /// </summary>
    public Vector3 waypointPosition = Vector3.zero;

    /// <summary>
    /// Connected lane. All waypoints must be connected to a lane.
    /// </summary>
    public RTC_Lane connectedLane;

    /// <summary>
    /// Next waypoint.
    /// </summary>
    public RTC_Waypoint nextWaypoint;

    /// <summary>
    /// Previous waypoint.
    /// </summary>
    public RTC_Waypoint previousWaypoint;

    /// <summary>
    ///  Interconnection waypoint.
    /// </summary>
    public RTC_Waypoint interConnectionWaypoint;

    /// <summary>
    /// Index of the waypoint.
    /// </summary>
    public int index = 0;

    /// <summary>
    ///  Is this first waypoint?
    /// </summary>
    public bool firstWaypoint = false;

    /// <summary>
    ///   Is this last waypoint?
    /// </summary>
    public bool lastWaypoint = false;

    /// <summary>
    /// Radius.
    /// </summary>
    [Range(.1f, 10f)] public float radius = 1.5f;

    /// <summary>
    ///  Target speed.
    /// </summary>
    [Range(5f, 360f)] public float targetSpeed = 80f;

    /// <summary>
    /// Target wait time.
    /// </summary>
    [Range(0f, 100f)] public float wait = 0f;

    /// <summary>
    /// Desired speed for next waypoint.
    /// </summary>
    public float desiredSpeedForNextWaypoint = 50f;

    /// <summary>
    /// Desired speed for interconnection waypoint.
    /// </summary>
    public float desiredSpeedForInterConnectionWaypoint = 50f;

    /// <summary>
    /// Distance to the next waypoint.
    /// </summary>
    public float distanceToNextWaypoint = 0f;

    /// <summary>
    /// Distance to the interconnection waypoint.
    /// </summary>
    public float distanceToInterConnectionWaypoint = 0f;

    /// <summary>
    /// Angle to next waypoint as degree.
    /// </summary>
    public float angleToNextWaypoint = 0f;

    /// <summary>
    /// Angle to interconnection waypoint as degree.
    /// </summary>
    public float angleToInterConnectionWaypoint = 0f;

    /// <summary>
    /// Ignore and don't use gizmos.
    /// </summary>
    public bool ignoreGizmos = false;

    /// <summary>
    /// Updates the waypoint.
    /// </summary>
    public void UpdateWaypoint() {

        //  Getting connected lane.
        connectedLane = GetComponentInParent<RTC_Lane>();

        //  Calculate distance to the next waypoint.
        if (nextWaypoint)
            distanceToNextWaypoint = Vector3.Distance(transform.position, nextWaypoint.transform.position);
        else
            distanceToNextWaypoint = 0f;

        //  Calculate distance to the interconnection waypoint.
        if (interConnectionWaypoint)
            distanceToInterConnectionWaypoint = Vector3.Distance(transform.position, interConnectionWaypoint.transform.position);
        else
            distanceToInterConnectionWaypoint = 0f;

        //  Calculate angle to next waypoint.
        if (nextWaypoint)
            angleToNextWaypoint = Vector3.Angle(nextWaypoint.transform.position - transform.position, transform.forward);
        else
            angleToNextWaypoint = 0f;

        //  Calculate angle to interconnection waypoint.
        if (interConnectionWaypoint)
            angleToInterConnectionWaypoint = Vector3.Angle(interConnectionWaypoint.transform.position - transform.position, transform.forward);
        else
            angleToInterConnectionWaypoint = 0f;

        //  Calculate desired speeds.
        desiredSpeedForNextWaypoint = targetSpeed * (1f - Mathf.InverseLerp(0, 180, angleToNextWaypoint));
        desiredSpeedForInterConnectionWaypoint = targetSpeed * (1f - Mathf.InverseLerp(0, 180, angleToInterConnectionWaypoint));

    }

    /// <summary>
    /// When a value changed in the inspector panel.
    /// </summary>
    private void OnValidate() {

        if (connectedLane)
            connectedLane.UpdateLane();

    }

}
