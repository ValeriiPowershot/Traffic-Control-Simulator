using DG.Tweening;
using Script.ScriptableObject;
using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles.States
{
    public class VehicleGoState : IVehicleState
    {
        private readonly float _rayDistance = 5f; 
        private readonly LayerMask _carLayer = LayerMask.GetMask("Car"); // Ensure cars are on a "Car" layer

        public VehicleController VehicleController { get; set; }
        public VehicleGoState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        
        public void MovementEnter()
        {
            VehicleController.MoveTween.Play();
            VehicleController.MoveTween.timeScale = VehicleScriptableObjects.speed; // Adjust tween speed dynamically
        }
        
        public void MovementUpdate()
        {
            Ray ray = new Ray(VehicleController.Vehicle.RayStartPoint.position, VehicleController.Vehicle.transform.forward);

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
       
        private VehicleScriptableObjects VehicleScriptableObjects =>
            VehicleController.Vehicle.VehicleScriptableObject;
    }
}