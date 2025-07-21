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
/// RTC_TrafficSpawner won't spawn traffic vehicles closer to this.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Traffic Controller/RTC Dont Spawn Around")]
public class RTC_DontSpawnAround : MonoBehaviour {

    /// <summary>
    ///  Radius.
    /// </summary>
    public float radius = 25f;      // 

    /// <summary>
    /// Event when don't spawn enabled or spawned.
    /// </summary>
    /// <param name="dontSpawnAround"></param>
    public delegate void onDontSpawnAroundSpawned(RTC_DontSpawnAround dontSpawnAround);
    public static event onDontSpawnAroundSpawned OnDontSpawnAroundSpawned;

    /// <summary>
    /// Event when don't spawn disabled or destoyed.
    /// </summary>
    /// <param name="dontSpawnAround"></param>
    public delegate void onDontSpawnAroundDisabled(RTC_DontSpawnAround dontSpawnAround);
    public static event onDontSpawnAroundDisabled OnDontSpawnAroundDisabled;

    private void OnEnable() {

        if (OnDontSpawnAroundSpawned != null)
            OnDontSpawnAroundSpawned(this);

    }

    private void OnDisable() {

        if (OnDontSpawnAroundDisabled != null)
            OnDontSpawnAroundDisabled(this);

    }

    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

    }

}
