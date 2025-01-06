using System;
using BaseCode.Logic.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Lights
{
    public class LightControlPoint : MonoBehaviour
    { 
        [SerializeField] private LightPointType pointType;
        private BasicLight _parentLight;

        private void Awake()
        {
            _parentLight = GetComponentInParent<BasicLight>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if ( pointType == LightPointType.Exit)
            {
                if (other.TryGetComponent(out BasicCar collidedCar))
                    collidedCar.PassLightPlaceState(_parentLight.lightPlace);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if(pointType == LightPointType.Entry)
            {
                if (other.TryGetComponent(out BasicCar collidedCar))
                    _parentLight.AddNewCar(collidedCar);
            }
            else if ( pointType == LightPointType.Exit)
            {
                if (other.TryGetComponent(out BasicCar collidedCar))
                    _parentLight.RemoveCar(collidedCar);
            }
        }
    }

    public enum LightPointType
    {
        None,
        Entry,
        Exit,
    }
}