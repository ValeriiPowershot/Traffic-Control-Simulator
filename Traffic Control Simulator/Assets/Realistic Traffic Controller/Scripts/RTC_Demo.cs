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
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Includes public methods to set intensity, radius of the traffic for demo purposes only.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Traffic Controller/RTC Demo")]
public class RTC_Demo : MonoBehaviour {

    /// <summary>
    /// Current car controller we're focusing on.
    /// </summary>
    private RTC_CarController carController;

    /// <summary>
    /// Crreut demo camera we're focusing on.
    /// </summary>
    private RTC_DemoCamera demoCamera;

    /// <summary>
    /// Stress UI button to increase traffic amount.
    /// </summary>
    public GameObject stressButton;

    /// <summary>
    /// Traffic amount text.
    /// </summary>
    public Text trafficCounter;

    /// <summary>
    /// Throttle slider of the reference car controller.
    /// </summary>
    public Slider throttle;

    /// <summary>
    /// Brake slider of the reference car controller.
    /// </summary>
    public Slider brake;

    /// <summary>
    /// Clutch slider of the reference car controller.
    /// </summary>
    public Slider clutch;

    /// <summary>
    /// Timescale slider.
    /// </summary>
    public Slider timeScaleSlider;

    private void Start() {

        //  Find a random traffic vehicle with two seconds delay.
        Invoke(nameof(Change), 2f);

        if (timeScaleSlider)
            timeScaleSlider.SetValueWithoutNotify(Time.timeScale);

    }

    private void LateUpdate() {

        // Displaying UI texts with delay and refreshing them one per second.
        DisplayUI();

    }

    /// <summary>
    /// Sets the maximum active vehicles of the RTC Traffic Spawner on the scene.
    /// </summary>
    /// <param name="slider"></param>
    public void SetIntensity(Slider slider) {

        //  If stress button is choosen, enable it only if slider at maximum value.
        if (stressButton) {

            if (slider.value >= slider.maxValue)
                stressButton.SetActive(true);
            else
                stressButton.SetActive(false);

        }

        // If there is no spawner on the scene, return.
        if (!RTC_SceneManager.Instance.spawner)
            return;

        //  Set maximum active vehicles of the spawner.
        RTC_SceneManager.Instance.spawner.maximumActiveVehicles = (int)slider.value;

    }

    /// <summary>
    /// Sets the radius of the RTC Traffic Spawner on the scene.
    /// </summary>
    /// <param name="slider"></param>
    public void SetRadius(Slider slider) {

        // If there is no spawner on the scene, return.
        if (!RTC_SceneManager.Instance.spawner)
            return;

        //  Set radius of the spawner.
        RTC_SceneManager.Instance.spawner.radius = (int)slider.value;

    }

    /// <summary>
    /// Changes the vehicle with random vehicle.
    /// </summary>
    public void Change() {

        //  Find demo camera first.
        if (!demoCamera)
            demoCamera = FindObjectOfType<RTC_DemoCamera>();

        //  Creating a list for all vehicles on the scene and collecting them. If demo camera found, don't add the demo camera's target car controller to the list.
        List<RTC_CarController> allVehicles = new List<RTC_CarController>();

        for (int i = 0; i < RTC_SceneManager.Instance.allVehicles.Length; i++) {

            if (RTC_SceneManager.Instance.allVehicles[i] != null) {

                if (demoCamera) {

                    if (demoCamera.targetVehicle != null) {

                        if (RTC_SceneManager.Instance.allVehicles[i].transform != demoCamera.targetVehicle.transform)
                            allVehicles.Add(RTC_SceneManager.Instance.allVehicles[i]);

                    } else {

                        allVehicles.Add(RTC_SceneManager.Instance.allVehicles[i]);

                    }

                } else {

                    allVehicles.Add(RTC_SceneManager.Instance.allVehicles[i]);

                }

            }

        }

        //  If all vehicles has length, set target of the demo camera with random vehicle.
        if (allVehicles.Count > 0) {

            carController = allVehicles[Random.Range(0, allVehicles.Count)];
            demoCamera.SetTarget(carController.transform);

        }

    }

    /// <summary>
    /// Resets the scene by reloading the same scene.
    /// </summary>
    public void ResetScene() {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    /// <summary>
    /// Sets the spawn behind camera option on RTC Traffic Spawner.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetSpawnBehindCamera(Toggle toggle) {

        // If there is no spawner on the scene, return.
        if (!RTC_SceneManager.Instance.spawner)
            return;

        //  Seting the option.
        RTC_SceneManager.Instance.spawner.dontSpawnBehindCamera = toggle.isOn;

    }

    /// <summary>
    /// Sets the disable vehicles at behind camera option on RTC Traffic Spawner.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetDisableBehindCamera(Toggle toggle) {

        // If there is no spawner on the scene, return.
        if (!RTC_SceneManager.Instance.spawner)
            return;

        //  Seting the option.
        RTC_SceneManager.Instance.spawner.disableVehiclesBehindCamera = toggle.isOn;

    }

    /// <summary>
    /// Sets the time scale value in the project settings.
    /// </summary>
    /// <param name="slider"></param>
    public void SetTimescale(Slider slider) {

        Time.timeScale = slider.value;

    }

    /// <summary>
    /// Stress test with 200 vehicles.
    /// </summary>
    public void SetStressTest() {

        // If there is no spawner on the scene, return.
        if (!RTC_SceneManager.Instance.spawner)
            return;

        bool state = RTC_SceneManager.Instance.spawner.maximumActiveVehicles == 200 ? true : false;

        RTC_SceneManager.Instance.spawner.maximumActiveVehicles = state ? 20 : 200;

    }

    /// <summary>
    /// Sets the optimization option on all traffic vehicles.
    /// </summary>
    /// <param name="toggle"></param>
    public void SetOptimization(Toggle toggle) {

        // If there are no any active traffic vehicles, return.
        if (RTC_SceneManager.Instance.allVehicles == null)
            return;

        //  Looping all traffic vehicles and setting their optimization options.
        for (int i = 0; i < RTC_SceneManager.Instance.allVehicles.Length; i++) {

            if (RTC_SceneManager.Instance.allVehicles[i] != null)
                RTC_SceneManager.Instance.allVehicles[i].optimization = toggle.isOn;

        }

    }

    /// <summary>
    /// Displaying UI texts and sliders.
    /// </summary>
    private void DisplayUI() {

        //  Feeding sliders with reference car controller values.
        if (carController) {

            if (throttle)
                throttle.SetValueWithoutNotify(carController.throttleInput);

            if (brake)
                brake.SetValueWithoutNotify(carController.brakeInput);

            if (clutch)
                clutch.SetValueWithoutNotify(carController.clutchInput);

        } else {

            if (throttle)
                throttle.SetValueWithoutNotify(0);

            if (brake)
                brake.SetValueWithoutNotify(0);

            if (clutch)
                clutch.SetValueWithoutNotify(0);

        }

        //  Traffic counter.
        if (trafficCounter)
            trafficCounter.text = "Vehicles: " + RTC_SceneManager.Instance.allVehicles.Length.ToString();

    }

}
