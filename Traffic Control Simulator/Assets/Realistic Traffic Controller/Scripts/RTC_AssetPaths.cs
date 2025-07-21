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
/// Path of the RTC demo assets.
/// </summary>
public class RTC_AssetPaths : ScriptableObject {

    private static RTC_AssetPaths instance;
    public static RTC_AssetPaths Instance {

        get {

            if (instance == null)
                instance = (RTC_AssetPaths)Resources.Load("RTC_AssetPaths", typeof(RTC_AssetPaths));

            return instance;

        }

    }

    public GameObject[] demoVehicles;
    public GameObject trafficLight;

}
