using BaseCode.Logic.EntityHandler.Lights.LightControlPointHandler;
using BaseCode.Logic.EntityHandler.Vehicles;
using BaseCode.Logic.Services.Interfaces.Light;
using UnityEngine;

namespace BaseCode.Logic.EntityHandler.Lights
{
    public class LightControlPoint : MonoBehaviour
    { 
        [SerializeField] private LightPointType pointType;

        private ILightControlPoint _controlPoint;
        private BasicLight _parentLight;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _controlPoint = pointType switch
            {
                LightPointType.Entry => new EntryLightControlPoint(),
                LightPointType.Exit => new ExitLightControlPoint(),
                _ => null
            };

            _parentLight = GetComponentInParent<BasicLight>();
            _controlPoint?.Initialize(_parentLight);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out VehicleBase vehicle))
            {
                _controlPoint?.OnVehicleEnter(vehicle);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out VehicleBase vehicle))
            {
                _controlPoint?.OnVehicleExit(vehicle);
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