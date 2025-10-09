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

public class RTC_Core : MonoBehaviour {

    /// <summary>
    /// Car controller component.
    /// </summary>
    public RTC_CarController carController;

    private RTC_SceneManager sceneManager;

    /// <summary>
    /// RTC Scene Manager.
    /// </summary>
    public RTC_SceneManager RTCSceneManager {

        get {

            if (sceneManager == null)
                sceneManager = RTC_SceneManager.Instance;

            return sceneManager;

        }

    }

    public void ResetVehicle() {

        //  Resetting inputs.
        carController.throttleInput = 0f;
        carController.brakeInput = 0f;
        carController.steerInput = 0f;
        carController.clutchInput = 1f;
        carController.throttleInputRaw = 0f;
        carController.brakeInputRaw = 0f;
        carController.steerInputRaw = 0f;
        carController.clutchInputRaw = 1f;
        carController.currentGear = 0;
        carController.gearShiftingNow = false;
        carController.lastTimeShifted = 0f;

        //  Resetting velocity.
        carController.Rigid.velocity = Vector3.zero;
        carController.Rigid.angularVelocity = Vector3.zero;

        //  Resetting wheels.
        for (int i = 0; i < carController.wheels.Length; i++) {

            if (carController.wheels[i] != null && carController.wheels[i].wheelCollider != null) {

                carController.wheels[i].wheelCollider.motorTorque = 0f;
                carController.wheels[i].wheelCollider.steerAngle = 0f;
                carController.wheels[i].wheelCollider.brakeTorque = 0f;

            }

        }

        carController.crashed = false;
        carController.raycastedVehicle = null;
        carController.stoppedForReason = false;
        carController.stoppedTime = 0f;
        carController.useSideRaycasts = false;
        carController.passingObstacle = false;
        carController.overtakingTimer = 0f;
        carController.ignoreInputs = false;
        carController.direction = 1;
        carController.waitingAtWaypoint = 0f;
        carController.stopNow = false;
        carController.m_disableAfterCrash = 0f;
        carController.m_checkUpsideDown = 0f;
        carController.wantedEngineRPMRaw = 0;
        carController.tractionWheelRPM2EngineRPM = 0;
        carController.wheelRPM2Speed = 0;
        carController.targetWheelSpeedForCurrentGear = 0f;
        carController.raycastOrder = -1;
        carController.desiredSpeed = 0f;
        carController.currentSpeed = 0f;
        carController.raycastDistance = 0f;
        carController.raycastHitDistance = 0f;

        if (carController.engineRunning) {

            carController.wantedEngineRPMRaw = carController.minEngineRPM;
            carController.currentEngineRPM = carController.minEngineRPM;

        }

        //  Resetting audio.
        if (carController.engineSoundOnSource)
            carController.engineSoundOnSource.volume = 0f;

        if (carController.engineSoundOffSource)
            carController.engineSoundOffSource.volume = 0f;

    }

    public void ResetVehicleOnEnable() {

        ResetVehicle();

    }

    public void ResetVehicleOnDisable() {

        ResetVehicle();

    }

    /// <summary>
    /// Finds eligible gear depends on the speed.
    /// </summary>
    /// <returns></returns>
    public float[] FindTargetSpeed() {

        float[] targetSpeeds = new float[carController.gearRatios.Length];

        float partition = carController.maximumSpeed / carController.gearRatios.Length;

        //  Assigning target speeds.
        for (int i = targetSpeeds.Length - 1; i >= 0; i--)
            targetSpeeds[i] = partition * (i + 1) * carController.gearShiftThreshold;

        return targetSpeeds;

    }

    /// <summary>
    /// Finds eligible gear depends on the speed.
    /// </summary>
    /// <returns></returns>
    public int FindEligibleGear() {

        float[] targetSpeeds = FindTargetSpeed();
        int eligibleGear = 0;

        for (int i = 0; i < targetSpeeds.Length; i++) {

            if (carController.currentSpeed < targetSpeeds[i]) {

                eligibleGear = i;
                break;

            }

        }

        return eligibleGear;

    }

    /// <summary>
    /// Returns -1 when to the left, 1 to the right, and 0 for forward/backward
    /// </summary>
    /// <param name="fwd"></param>
    /// <param name="targetDir"></param>
    /// <param name="up"></param>
    /// <returns></returns>
    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {

        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        return dir;

    }

    public void DisableAllAudioSources() {

        if (carController.engineSoundOnSource)
            Destroy(carController.engineSoundOnSource.gameObject);

        if (carController.engineSoundOffSource)
            Destroy(carController.engineSoundOffSource.gameObject);

    }

}
