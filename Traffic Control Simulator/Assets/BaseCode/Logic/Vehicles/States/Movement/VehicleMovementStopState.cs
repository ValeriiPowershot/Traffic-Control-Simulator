using System.Collections;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public class VehicleMovementStopState : IVehicleMovementState
    {
        private readonly float _rayDistance = 0.2f; // Adjust distance as needed
        private readonly LayerMask _carLayer = LayerMask.GetMask("Car"); // Ensure cars are on a "Car" layer
        
        public bool IsWaiting;
        
        public VehicleController VehicleController { get; set; }
        
        public VehicleMovementStopState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        
        public void MovementEnter()
        {
            IsWaiting = false;
        }

        public void MovementUpdate() 
        {
            var ray = new Ray(VehicleController.VehicleBase.RayStartPoint.position, VehicleController.VehicleBase.transform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance,_carLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.red);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                if (IsWaiting == false)
                {
                    IsWaiting = true;
                    VehicleController.VehicleBase.StartCoroutine(WaitForSeconds());
                }
            }
        }

        private IEnumerator WaitForSeconds()
        {
            yield return new WaitForSeconds(1f);
            IsWaiting = false;
            VehicleController.SetState<VehicleMovementGoState>();
        }

        public void MovementExit()
        {
        }
    }
}