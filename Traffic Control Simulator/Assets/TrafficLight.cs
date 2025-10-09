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
    [SerializeField] private TrafficLightState _startLightState;
    public TrafficLightState LightState { get; private set; }

    [SerializeField] private TrafficLightColorSwitcher _trafficLightSwitcher;
    [SerializeField] private Collider _collider;
    [SerializeField] private Image _lightUI;
    public Transform StopLine;
    public int LightIndex;

    private TrafficLightMaterials _trafficLightMaterials;
    
    private bool _isRedLight;
    public bool IsRedLight
    {
        get => _isRedLight;
        private set
        {
            _isRedLight = value;
            if (_collider != null)
                _collider.enabled = _isRedLight;
        }
    }

    [SerializeField] private float yellowDuration = 2f;

    private bool _isSwitching = false; // запрещаем новую смену пока не закончена текущая

    private void Start()
    {
        LightState = _startLightState;
        _trafficLightMaterials = GetComponent<TrafficLightMaterials>();
        SetLightState(LightState);
    }

    private void Reset()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();
    }

    public void SwitchRedGreen()
    {
        if (_isSwitching) return; // если уже переключаем, игнорируем нажатие

        if (LightState == TrafficLightState.Red)
        {
            StartCoroutine(YellowThenGreen());
        }
        else if (LightState == TrafficLightState.Green)
        {
            StartCoroutine(YellowThenRed());
        }
    }

    private IEnumerator YellowThenRed()
    {
        _isSwitching = true;
        SetLightState(TrafficLightState.Yellow);
        _lightUI.color = _trafficLightMaterials.YellowColor;
        yield return new WaitForSeconds(yellowDuration);
        SetLightState(TrafficLightState.Red);
        _isSwitching = false;
    }

    private IEnumerator YellowThenGreen()
    {
        _isSwitching = true;
        SetLightState(TrafficLightState.Yellow);
        _lightUI.color = _trafficLightMaterials.YellowColor;
        yield return new WaitForSeconds(yellowDuration);
        SetLightState(TrafficLightState.Green);
        _isSwitching = false;
    }

    private void SetLightState(TrafficLightState newState)
    {
        LightState = newState;
        IsRedLight = (newState == TrafficLightState.Red);

        switch (newState)
        {
            case TrafficLightState.Red:
                _trafficLightSwitcher.ChangeToRed();
                _lightUI.color = _trafficLightMaterials.RedColor;
                break;
            case TrafficLightState.Yellow:
                _trafficLightSwitcher.ChangeToYellow();
                break;
            case TrafficLightState.Green:
                _trafficLightSwitcher.ChangeToGreen();
                _lightUI.color = _trafficLightMaterials.GreenColor;
                break;
        }
    }
}
