using System;
using BaseCode.Logic.Lights;
using UnityEngine;

namespace Script.Vehicles
{
    //Interface for all cars to be conrolled by the lights
    public class BasicCar : MonoBehaviour 
    {
        public event Action LightExited;
        
        private LightState _carLightState;
        public LightState CarLightState { get { return _carLightState; } }

        public LightPlace lightPlaceSave;
        //passes light state
        //could be called multiple times to inform about state changes
        public virtual void PassLightState(LightState State, LightPlace lightPlace)
        {
            _carLightState = State;
            lightPlaceSave = lightPlace;
        }

        public virtual void ExitLightControl()
        {
            _carLightState = LightState.None;
            LightExited?.Invoke(); //necessary for scoring system - (looked by tolga, its ok :D)
        }

        // call from exit of intersection
        public void ResetLightPlaceSave()  // i hated with this method
        {
            lightPlaceSave = LightPlace.None;
        }
    }

    public enum LightState
    {
        None,
        Green,
        Red,
        Yellow,
    }
    
}