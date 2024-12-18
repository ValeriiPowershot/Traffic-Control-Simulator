using DG.Tweening;
using Script.So;
using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles.States
{
    public class VehicleGoState : IVehicleState
    {
        private readonly float _rayDistance = 5f; // Adjust distance as needed
        private LayerMask _carLayer = LayerMask.GetMask("Car"); // Ensure cars are on a "Car" layer

        public VehicleController VehicleController { get; set; }
        public VehicleGoState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        
        public void MovementEnter()
        {
            VehicleController.MoveTween.Play();
            VehicleController.MoveTween.timeScale = VehicleSo.speed; // Adjust tween speed dynamically
        }
        
        public void MovementUpdate()
        {
            var ray = new Ray(VehicleController.Vehicle.rayStartPoint.position, VehicleController.Vehicle.transform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance,_carLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                VehicleController.SetState<VehicleStopState>();
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.green);
            }
        }


        public void MovementExit()
        {
        }
        

       
        private VehicleSo VehicleSo => VehicleController.Vehicle.vehicleSo;
    }
}