using System.Collections;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public class VehicleMovementCrashState : IVehicleMovementState
    {
        private readonly float _rayDistance = 0.2f; // Adjust distance as needed
        private readonly LayerMask _carLayer = LayerMask.GetMask("Car"); // Ensure cars are on a "Car" layer
        
        private bool _isWaiting;
        public VehicleController VehicleController { get; set; }
        public VehicleMovementCrashState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        public void MovementEnter()
        {
            _isWaiting = false;
        }

        public void MovementUpdate() 
        {
            var ray = new Ray(ReferenceController.rayStartPoint.position, VehicleController.VehicleBase.transform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance,_carLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.red);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                
            }
        }
        
        private IEnumerator WaitForSeconds()
        {
            VehicleController.SetState<VehicleMovementGoState>();
            
            IVehicleMovementState state = VehicleController.GetStateCurrentState();

            if (state is VehicleMovementGoState goState)
            {
                Debug.Log("Slowing down by amount");

                goState.SmoothSlowdownByAmount(5f, 1f);
                yield return new WaitForSeconds(1f);
                goState.SmoothRestoreSpeed();
            }
        }


        public void MovementExit()
        {
        }
        protected VehicleReferenceController ReferenceController =>VehicleController.vehicleReferenceController; 
         
    }
}