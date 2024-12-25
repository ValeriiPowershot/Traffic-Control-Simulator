using System;
using BaseCode.Logic;
using BaseCode.Logic.Lights;
using UnityEngine;

namespace Script.Vehicles
{
    //Interface for all cars to be conrolled by the lights
    public class BasicCar : MonoBehaviour 
    {
        private CarManager _manager;

        public event Action LightExited;
        
        private LightState _carLightState;
        public LightState CarLightState { get { return _carLightState; } }

        public LightPlace lightPlaceSave;
        
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

        public void ResetLightPlaceSave()  // i hated with this method
        {
            lightPlaceSave = LightPlace.None;
        }

        public CarManager CarManager
        {
            get;
            set;
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