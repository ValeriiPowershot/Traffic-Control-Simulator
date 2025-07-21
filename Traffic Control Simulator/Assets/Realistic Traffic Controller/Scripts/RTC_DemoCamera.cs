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
/// Demo camera. It will follow the target vehicle for demo purposes.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Traffic Controller/RTC Demo Camera")]
public class RTC_DemoCamera : MonoBehaviour {

    /// <summary>
    /// Target traffic vehicle we're tracking.
    /// </summary>
    public Transform targetVehicle;

    /// <summary>
    /// Distance to the camera.
    /// </summary>
    public float distance = 5f;

    /// <summary>
    /// Height to the camera.
    /// </summary>
    public float height = 1.5f;

    /// <summary>
    /// Auto calculate distance and height values by calculating bounds of the target vehicle.
    /// </summary>
    public bool autoCalculateDistanceAndHeight = true;

    /// <summary>
    /// Picks random traffic vehicle on start.
    /// </summary>
    public bool pickRandomVehicleOnStart = false;

    private void Start() {

        Invoke(nameof(PickRandom), 2f);

    }

    private void PickRandom() {

        if (pickRandomVehicleOnStart) {

            if (RTC_SceneManager.Instance.allVehicles != null && RTC_SceneManager.Instance.allVehicles.Length > 0) {

                RTC_CarController randomVehicle = RTC_SceneManager.Instance.allVehicles[Random.Range(0, RTC_SceneManager.Instance.allVehicles.Length)];

                if (randomVehicle != null)
                    SetTarget(randomVehicle.transform);

            }

        }

    }

    private void LateUpdate() {

        //  If no target vehicle, return.
        if (!targetVehicle)
            return;

        //  Setting position and rotation of the camera with given settings.
        transform.position = targetVehicle.position;
        transform.position -= targetVehicle.forward * distance;
        transform.position += transform.up * height;

        //  Always look at the target vehicle.
        transform.LookAt(targetVehicle);

    }

    /// <summary>
    /// Sets the new target vehicle.
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target) {

        targetVehicle = target;

        if (autoCalculateDistanceAndHeight) {

            distance = RTC_GetBounds.GetBounds(targetVehicle.gameObject).size.magnitude * 1f;
            height = RTC_GetBounds.GetBounds(targetVehicle.gameObject).size.magnitude * .35f;

        }

    }

}
