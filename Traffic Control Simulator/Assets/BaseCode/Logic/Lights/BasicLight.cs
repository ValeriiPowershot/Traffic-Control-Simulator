using System.Collections.Generic;
using UnityEngine;

//Basic class for all lights
namespace BaseCode.Logic.Lights
{
    public class BasicLight : MonoBehaviour
    {
        //temporary light indicator
        [SerializeField] private Material[] _lightMats;
        [SerializeField] private MeshRenderer _lightMesh;
        
        public LightState LightState { get; private set; } = (LightState)1;
        private int _lightIndex = 1;
        private int MAX_LIGHT_INDEX = 2;

        private List<ICar> _controlledCars = new List<ICar>();

        //updates and sets light state in order: green, yellow, red
        public void ChangeLight()
        {
            SetLight(++_lightIndex);
            PassStates(LightState);
        }
        //Sets state by index
        public void ChangeLight(LightState State)
        {
            SetLight((int)State);
            PassStates(LightState);
        }

        public void AddNewCar(ICar NewCar)
        {
            NewCar.PassLightState(LightState);
            _controlledCars.Add(NewCar);
        }

        public void RemoveCar(ICar OldCar)
        {
            _controlledCars.Remove(OldCar);
        }

        //Inform cars about state changes
        private void PassStates(LightState State)
        {
            foreach(var Car in _controlledCars)
            {
                Car.PassLightState(State);
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
