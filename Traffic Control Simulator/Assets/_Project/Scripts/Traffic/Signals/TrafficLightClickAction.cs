using UnityEngine;

[DisallowMultipleComponent]
public class TrafficLightClickAction : MonoBehaviour
{
    [SerializeField] private TrafficLight _trafficLight;

    private void OnMouseDown()
    {
        DoSomething();
    }

    private void DoSomething()
    {
        _trafficLight.SwitchRedGreen();
    }
}
