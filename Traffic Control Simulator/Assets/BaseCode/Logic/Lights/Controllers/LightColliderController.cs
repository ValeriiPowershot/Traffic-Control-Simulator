using BaseCode.Logic.Lights.Handler.Inheriteds.LightControlPointHandler;
using BaseCode.Logic.Lights.Services;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Lights.Controllers
{
    public class LightColliderController : MonoBehaviour
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