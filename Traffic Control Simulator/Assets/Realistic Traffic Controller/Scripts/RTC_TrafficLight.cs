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
/// Traffic light manager.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Traffic Controller/RTC Traffic Light")]
public class RTC_TrafficLight : MonoBehaviour {

    /// <summary>
    /// Trigger collider will be enabled on red / yellow lights to stop traffic vehicles.
    /// </summary>
    public BoxCollider triggerCollider;

    /// <summary>
    /// Trigger ignore collider for opposite side vehicles.
    /// </summary>
    public BoxCollider triggerIgnoreCollider;

    /// <summary>
    /// Mesh renderers of the lights to illuminate.
    /// </summary>
    public MeshRenderer redLight;

    /// <summary>
    /// Mesh renderers of the lights to illuminate.
    /// </summary>
    public MeshRenderer yellowLight;

    /// <summary>
    /// Mesh renderers of the lights to illuminate.
    /// </summary>
    public MeshRenderer greenLight;

    /// <summary>
    /// Light source.
    /// </summary>
    public Light redLightSource;

    /// <summary>
    /// Light source.
    /// </summary>
    public Light yellowLightSource;

    /// <summary>
    /// Light source.
    /// </summary>
    public Light greenLightSource;

    public enum LightState { Red, Yellow, Green }

    /// <summary>
    ///  Light state.
    /// </summary>
    public LightState lightState = LightState.Red;

    /// <summary>
    /// Timers.
    /// </summary>
    [Range(1f, 100f)] public float redTimer = 30f;

    /// <summary>
    /// Timers.
    /// </summary>
    [Range(1f, 10f)] public float yellowTimer = 2f;

    /// <summary>
    /// Timers.
    /// </summary>
    [Range(1f, 100f)] public float greenTimer = 20f;

    /// <summary>
    /// Current timer.
    /// </summary>
    public float timer = 0f;

    /// <summary>
    /// Wait for this light.
    /// </summary>
    public RTC_TrafficLight waitForThisLight;

    private void Awake() {

        //  Setting layer of the traffic light.
        if (RTC_Settings.Instance.trafficLightsLayer != "") {

            transform.gameObject.layer = LayerMask.NameToLayer(RTC_Settings.Instance.trafficLightsLayer);

            foreach (Transform item in transform)
                item.gameObject.layer = LayerMask.NameToLayer(RTC_Settings.Instance.trafficLightsLayer);

            if (triggerIgnoreCollider)
                triggerIgnoreCollider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        }

    }

    private void Update() {

        switch (lightState) {

            case LightState.Red:

                //  Setting emission colors.
                if (redLight) redLight.material.SetColor("_EmissionColor", Color.red);
                if (yellowLight) yellowLight.material.SetColor("_EmissionColor", Color.yellow * 0f);
                if (greenLight) greenLight.material.SetColor("_EmissionColor", Color.green * 0f);

                //  Setting intensity of the light sources.
                if (redLightSource)
                    redLightSource.intensity = 1f;

                if (yellowLightSource)
                    yellowLightSource.intensity = 0f;

                if (greenLightSource)
                    greenLightSource.intensity = 0f;

                break;

            case LightState.Yellow:

                //  Setting emission colors.
                if (redLight) redLight.material.SetColor("_EmissionColor", Color.red * 0f);
                if (yellowLight) yellowLight.material.SetColor("_EmissionColor", Color.yellow);
                if (greenLight) greenLight.material.SetColor("_EmissionColor", Color.green * 0f);

                //  Setting intensity of the light sources.
                if (redLightSource)
                    redLightSource.intensity = 0f;

                if (yellowLightSource)
                    yellowLightSource.intensity = 1f;

                if (greenLightSource)
                    greenLightSource.intensity = 0f;

                break;

            case LightState.Green:

                //  Setting emission colors.
                if (redLight) redLight.material.SetColor("_EmissionColor", Color.red * 0f);
                if (yellowLight) yellowLight.material.SetColor("_EmissionColor", Color.yellow * 0f);
                if (greenLight) greenLight.material.SetColor("_EmissionColor", Color.green);

                //  Setting intensity of the light sources.
                if (redLightSource)
                    redLightSource.intensity = 0f;

                if (yellowLightSource)
                    yellowLightSource.intensity = 0f;

                if (greenLightSource)
                    greenLightSource.intensity = 1f;

                break;

        }

        //  Increasing timer.
        timer += Time.deltaTime;

        //  If timer is higher than all timers, reset it to 0.
        if (timer >= (redTimer + yellowTimer + greenTimer))
            timer = 0f;

        //  Setting state of the light based on timer.
        if (timer < redTimer)
            lightState = LightState.Red;
        else if (timer < (redTimer + greenTimer))
            lightState = LightState.Green;
        else if (timer < (redTimer + greenTimer + yellowTimer))
            lightState = LightState.Yellow;

        //  If target light is selected, wait for it.
        if (waitForThisLight) {

            if (waitForThisLight.lightState == LightState.Red)
                lightState = LightState.Green;

            if (waitForThisLight.lightState == LightState.Green)
                lightState = LightState.Red;

        }

        //  Enable / disable trigger collider depending on the light state.
        if (triggerCollider) {

            if (lightState == LightState.Red || lightState == LightState.Yellow)
                triggerCollider.enabled = true;
            else if (lightState == LightState.Green)
                triggerCollider.enabled = false;

        }
    }
    

}
