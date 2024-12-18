using DG.Tweening;
using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles.States
{
    public class VehicleStopState : IVehicleState
    {
        private readonly float _rayDistance = 5f; // Adjust distance as needed
        private readonly LayerMask _carLayer = LayerMask.GetMask("Car"); // Ensure cars are on a "Car" layer

        public VehicleController VehicleController { get; set; }
        public VehicleStopState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        public void MovementEnter()
        {
            VehicleController.MoveTween.Pause();
        }

        public void MovementUpdate() 
        {
            if(VehicleController.Vehicle.CarLightState == LightState.Red)
                return;
            
            var ray = new Ray(VehicleController.Vehicle.RayStartPoint.position, VehicleController.Vehicle.transform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance,_carLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.red);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                VehicleController.SetState<VehicleGoState>();
            }
        }

        public void MovementExit()
        {
        }
  
         
    }
}