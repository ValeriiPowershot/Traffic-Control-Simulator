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
/// Single lane.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Traffic Controller/RTC Lane")]
public class RTC_Lane : MonoBehaviour {

    /// <summary>
    /// All waypoints attached to the lane.
    /// </summary>
    public RTC_Waypoint[] waypoints;        //  

    /// <summary>
    /// Lane index.
    /// </summary>
    public int laneIndex = 0;       //  

    //  Supported vehicle types.
    public bool supportedLight = true;
    public bool supportedMedium = true;
    public bool supportedHeavy = true;
    public bool supportedSedan = true;
    public bool supportedTaxi = true;
    public bool supportedBus = true;
    public bool supportedExotic = true;
    public bool supportedVan = true;
    public bool supportedPolice = true;

    /// <summary>
    /// Hides waypoints at middle.
    /// </summary>
    public bool hideMiddleWaypointsGizmos = false;

    /// <summary>
    /// Updates the lane data and all children waypoints.
    /// </summary>
    public void UpdateLane() {

        laneIndex = transform.GetSiblingIndex();        //  Getting index of the lane.
        transform.name = "Lane_" + transform.GetSiblingIndex();     //  Setting name of the lane with proper index string.

        //  Getting all waypoints of this lane.
        waypoints = GetComponentsInChildren<RTC_Waypoint>();

        //  Looping all waypoints. Make sure they are looking at proper directions.
        for (int i = 0; i < waypoints.Length; i++) {

            waypoints[i].waypointPosition = waypoints[i].transform.position;

            if (waypoints[i].nextWaypoint && waypoints[i].firstWaypoint)
                waypoints[i].transform.LookAt(waypoints[i].nextWaypoint.transform);

            if (waypoints[i].previousWaypoint) {

                waypoints[i].transform.LookAt(waypoints[i].previousWaypoint.transform);
                waypoints[i].transform.Rotate(Vector3.up, 180f);

            }

            //  Setting index of the waypoints.
            waypoints[i].index = waypoints[i].transform.GetSiblingIndex();

            //  If index is 0, this is the first waypoint.
            if (waypoints[i].index == 0) {

                waypoints[i].firstWaypoint = true;
                waypoints[i].lastWaypoint = false;

            }

            //  If index is equal to length of the waypoints, this is the last waypoint.
            if (waypoints.Length >= 2 && waypoints[i].index == waypoints.Length - 1) {

                waypoints[i].firstWaypoint = false;
                waypoints[i].lastWaypoint = true;

            }

            //  If index is not first and last, this is not the first or last waypoint.
            if (waypoints[i].index != 0 && (waypoints.Length >= 2 && waypoints[i].index == waypoints.Length - 1)) {

                waypoints[i].firstWaypoint = false;
                waypoints[i].lastWaypoint = false;

            }

            //  Hiding middle waypoints if enabled.
            if (hideMiddleWaypointsGizmos) {

                if (i != 0 && i != waypoints.Length - 1)
                    waypoints[i].ignoreGizmos = true;
                else
                    waypoints[i].ignoreGizmos = false;

            } else {

                waypoints[i].ignoreGizmos = false;

            }

            //  Setting name of the waypoint with proper index string.
            waypoints[i].transform.name = "Waypoint" + i.ToString();

            //  And finally, ppdating all waypoints.
            waypoints[i].UpdateWaypoint();

        }

    }

}
