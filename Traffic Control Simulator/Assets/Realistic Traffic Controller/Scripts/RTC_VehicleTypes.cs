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
/// Vehicle types.
/// </summary>
public class RTC_VehicleTypes : ScriptableObject {

    private static RTC_VehicleTypes instance;

    /// <summary>
    /// Instance of the class.
    /// </summary>
    public static RTC_VehicleTypes Instance {

        get {

            if (instance == null)
                instance = (RTC_VehicleTypes)Resources.Load("RTC_VehicleTypes", typeof(RTC_VehicleTypes));

            return instance;

        }

    }

    public enum VehicleType { Light, Medium, Heavy, Sedan, Taxi, Bus, Exotic, Van, Police }

    /// <summary>
    /// Vehicle type.
    /// </summary>
    public VehicleType vehicleType = VehicleType.Light;

}
