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
/// Shared RTC Settings.
/// </summary>
public class RTC_Settings : ScriptableObject {

    private static RTC_Settings instance;

    /// <summary>
    /// Instance of the RTC Settings class.
    /// </summary>
    public static RTC_Settings Instance {

        get {

            if (instance == null)
                instance = (RTC_Settings)Resources.Load("RTC_Settings", typeof(RTC_Settings));

            return instance;

        }

    }

    /// <summary>
    /// Traffic spawner prefab as resource.
    /// </summary>
    public RTC_TrafficSpawner trafficSpawner;

    /// <summary>
    /// Demo camera prefab as resource.
    /// </summary>
    public RTC_DemoCamera demoCamera;

    /// <summary>
    /// Demo UI canvas prefab as resource.
    /// </summary>
    public RTC_Demo demoUICanvas;

    //  Gizmo colors.
    public Color unselectedWaypointColor = Color.yellow;
    public Color selectedWaypointColor = Color.red;
    public Color firstWaypointColor = Color.green;
    public Color lastWaypointColor = Color.blue;
    public Color arrowColor = Color.green;
    public Color textColor = Color.blue;
    public Color unselectedLaneColor = Color.yellow;
    public Color selectedLaneColor = Color.blue;
    public float textDistance = 100f;

    /// <summary>
    /// Sets the layer of all children gameobjects.
    /// </summary>
    public bool layerChildren = true;

    /// <summary>
    /// Layer to be used on traffic vehicles.
    /// </summary>
    public string trafficVehiclesLayer = "RTC_TrafficVehicle";

    /// <summary>
    /// Layer to be used on traffic lights.
    /// </summary>
    public string trafficLightsLayer = "RTC_TrafficLight";

}
