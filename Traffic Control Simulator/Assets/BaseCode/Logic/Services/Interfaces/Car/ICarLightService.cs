using System;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Vehicles;

namespace BaseCode.Logic.Services.Interfaces.Car
{
    public interface ICarLightService
    {
        void PassLightState(LightState carLightState);
        void PassLightPlaceState(LightPlace lightPlace);
        
        void ExitLightControl();
        void ExitIntersection();
        
        LightState CarLightState { get; }
        LightPlace LightPlaceSave { get; set; }
        
        public event Action LightExited;
    }
}