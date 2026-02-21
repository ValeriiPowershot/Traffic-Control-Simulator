using System.Collections;
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

    [Header("Visuals")]
    [SerializeField] private TrafficLightColorSwitcher trafficLightSwitcher;
    [SerializeField] private Image lightUI;
    [SerializeField] private TrafficLightMaterials trafficLightMaterials;

    [Header("Logic")]
    [SerializeField] private TrafficLightStopWaypoint stopWaypoint;

    [Header("Timings")]
    [SerializeField] private float yellowDuration = 2f;

    private bool isSwitching = false;

    private void Start()
    {
        SetLightState(startLightState);
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
    }

    private void SetLightState(TrafficLightState newState)
    {
        LightState = newState;

        switch (newState)
        {
            case TrafficLightState.Red:
                trafficLightSwitcher.ChangeToRed();
                lightUI.color = trafficLightMaterials.RedColor;
                if (stopWaypoint != null) stopWaypoint.SetRed();
                break;

            case TrafficLightState.Yellow:
                trafficLightSwitcher.ChangeToYellow();
                lightUI.color = trafficLightMaterials.YellowColor;
                break;

            case TrafficLightState.Green:
                trafficLightSwitcher.ChangeToGreen();
                lightUI.color = trafficLightMaterials.GreenColor;
                if (stopWaypoint != null) stopWaypoint.SetGreen();
                break;
        }
    }
}
