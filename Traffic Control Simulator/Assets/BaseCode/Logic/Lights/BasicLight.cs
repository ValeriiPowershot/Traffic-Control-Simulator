using System;
using System.Collections.Generic;
using Script.ScriptableObject;
using Script.Vehicles;
using UnityEngine;

//Basic class for all lights
namespace BaseCode.Logic.Lights
{
    public enum LightPlace
    {
        None,
        Forward,
        Left,
        Right,
        Back
    }
    public class BasicLight : MonoBehaviour
    {
        //temporary light indicator
        [SerializeField] private Material[] _lightMats;
        [SerializeField] private MeshRenderer _lightMesh;
        
        public LightState LightState { get; private set; } = (LightState)1;
        private int _lightIndex = 1;
        private const int MAX_LIGHT_INDEX = 2;

        private List<BasicCar> _controlledCars = new List<BasicCar>();

        public LightControlPoint lightControlPoint;
        public LightScriptableObject lightData;

        public LightPlace lightPlace;
        private void Awake()
        {
            lightControlPoint.SetParentLight(this);
        }

        public void SetChangeoverState()
        {
            _lightMesh.material = _lightMats[^1];
            PassStates(LightState.Yellow);
        }

        //updates and sets light state in order: green, red
        public void ChangeLight()
        {
            SetLight(++_lightIndex);
            PassStates(LightState);
        }
        
        public void AddNewCar(BasicCar NewCar)
        {
            if (!_controlledCars.Contains(NewCar))
            {
                _controlledCars.Add(NewCar);
                NewCar.PassLightState(LightState,lightPlace);
            }
        }

        public void RemoveCar(BasicCar OldCar)
        {
            if (_controlledCars.Contains(OldCar))
            {
                _controlledCars.Remove(OldCar);
                OldCar.ExitLightControl();
            }
        }

        //Inform cars about state changes
        private void PassStates(LightState State)
        {
            foreach(var Car in _controlledCars)
            {
                Car.PassLightState(State,lightPlace);
            }
        }

        //The only way _lightIndex and _lightState should be managed
        private void SetLight(int NewIndex)
        {
            if (NewIndex > MAX_LIGHT_INDEX)
                NewIndex = 1;

            _lightIndex = NewIndex;
            LightState = (LightState)_lightIndex;

            //temporary light indication
            _lightMesh.material = _lightMats[_lightIndex - 1];
        }

    }
}
