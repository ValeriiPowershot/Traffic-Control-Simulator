using System;
using BaseCode.Logic.Lights;
using UnityEngine;

namespace BaseCode.Logic.Vehicles
{
    public class BasicCar : MonoBehaviour 
    {
        private CarManager _manager;
        
        public LightState _carLightState;
        public LightPlace _lightPlaceSave;
        
        public event Action LightExited;

        public virtual void PassLightState(LightState carLightState)
        {
            _carLightState = carLightState;
        }
        public virtual void PassLightPlaceState(LightPlace lightPlace)
        {
            _lightPlaceSave = lightPlace;
        }

        public virtual void ExitLightControl()
        {
            _carLightState = LightState.None;
            LightExited?.Invoke(); //necessary for scoring system - (looked by tolga, its ok :D)
        }

        public void ExitIntersection()  
        {
            _lightPlaceSave = LightPlace.None;
        }
        
        public CarManager CarManager
        {
            get;
            set;
        }
        
        public LightState CarLightState => _carLightState;

        public LightPlace lightPlaceSave
        {
            get => _lightPlaceSave;
            set => _lightPlaceSave = value;
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