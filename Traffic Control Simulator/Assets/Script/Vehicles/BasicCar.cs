using System;
using UnityEngine;

//Interface for all cars to be conrolled by the lights
public class BasicCar : MonoBehaviour 
{
    private LightState _carLightState;
    public event Action LightExited;

    public LightState CarLightState { get { return _carLightState; } }

    //passes light state
    //could be called multiple times to inform about state changes
    public virtual void PassLightState(LightState State)
    {
        _carLightState = State;
    }

    public virtual void ExitLightControl()
    {
        _carLightState = LightState.None;
        LightExited?.Invoke(); //necessary for scoring system
    }
}

public enum LightState
{
    None,
    Green,
    Red,
    Yellow,
}
