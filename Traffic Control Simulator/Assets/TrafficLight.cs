using System.Collections;
using System.Collections.Generic;
using Realistic_Traffic_Controller.Scripts;
using UnityEngine;
using UnityEngine.UI;

public enum TrafficLightState
{
    Red,
    Yellow,
    Green
}

[DisallowMultipleComponent]
public class TrafficLight : MonoBehaviour
{
    [Header("Start State")]
    [SerializeField] private TrafficLightState startLightState = TrafficLightState.Red;
    public TrafficLightState LightState { get; private set; }
    public int TrafficLightSpawnIndex;

    [Header("Visuals")]
    [SerializeField] private TrafficLightColorSwitcher trafficLightSwitcher;
    [SerializeField] private Image lightUI;
    [SerializeField] private TrafficLightMaterials trafficLightMaterials;

    [Header("Logic")]
    [SerializeField] private TrafficLightStopWaypoint stopWaypoint;
    [SerializeField] private GameObject _stopCollider;

    [Header("Timings")]
    [SerializeField] private float yellowDuration = 2f;

    private List<RTC_CarController> _waitingCars = new();
    private bool isSwitching = false;

    private void Start()
    {
        SetLightState(startLightState);
    }

    public void AddCarToWaitingList(RTC_CarController carController)
    {
        _waitingCars.Add(carController);
    }

    public void SwitchRedGreen()
    {
        if (isSwitching) return;

        if (LightState == TrafficLightState.Red)
            StartCoroutine(YellowThenGreen());
        else if (LightState == TrafficLightState.Green)
            StartCoroutine(YellowThenRed());
    }

    private IEnumerator YellowThenRed()
    {
        isSwitching = true;
        SetLightState(TrafficLightState.Yellow);
        yield return new WaitForSeconds(yellowDuration);
        SetLightState(TrafficLightState.Red);
        isSwitching = false;
    }

    private IEnumerator YellowThenGreen()
    {
        isSwitching = true;
        SetLightState(TrafficLightState.Yellow);
        yield return new WaitForSeconds(yellowDuration);
        SetLightState(TrafficLightState.Green);
        isSwitching = false;
        WaitingCarsAllowToGo();
    }

    private void WaitingCarsAllowToGo()
    {
        foreach (var car in _waitingCars)
        {
            //StartCoroutine(car.TimeThrottle(0.5f, 2));
            car.stoppedForTrafficLight = false;
            car.startBoostActive = true;
        }
    }

    private void SetLightState(TrafficLightState newState)
    {
        LightState = newState;

        switch (newState)
        {
            case TrafficLightState.Red:
                trafficLightSwitcher.ChangeToRed();
                lightUI.color = trafficLightMaterials.RedColor;
                _stopCollider.SetActive(true);
                if (stopWaypoint != null) stopWaypoint.SetRed();
                break;

            case TrafficLightState.Yellow:
                trafficLightSwitcher.ChangeToYellow();
                lightUI.color = trafficLightMaterials.YellowColor;
                break;

            case TrafficLightState.Green:
                trafficLightSwitcher.ChangeToGreen();
                lightUI.color = trafficLightMaterials.GreenColor;
                _stopCollider.SetActive(false);
                if (stopWaypoint != null) stopWaypoint.SetGreen();
                break;
        }
    }
}
