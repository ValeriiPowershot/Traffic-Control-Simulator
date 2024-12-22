using System;
using Script.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Lights
{
    public class LightControlPoint : MonoBehaviour
    {
        private BasicLight _parentLight;

        public void SetParentLight(BasicLight parentLight)
        {
            _parentLight = parentLight;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BasicCar collidedCar))
                _parentLight.AddNewCar(collidedCar);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out BasicCar collidedCar))
                _parentLight.RemoveCar(collidedCar);
        }
    }

    public enum LightPointType
    {
        None,
        Entry,
        Exit,
    }
}