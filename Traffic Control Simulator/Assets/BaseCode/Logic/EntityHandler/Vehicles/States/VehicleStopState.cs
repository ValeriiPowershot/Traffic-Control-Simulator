using System.Collections;
using BaseCode.Logic.EntityHandler.Vehicles.Controllers;
using UnityEngine;

namespace BaseCode.Logic.EntityHandler.Vehicles.States
{
    public class VehicleStopState : IVehicleMovementState
    {
        private readonly float _rayDistance = 5f; // Adjust distance as needed
        private readonly LayerMask _carLayer = LayerMask.GetMask("Car"); // Ensure cars are on a "Car" layer
        
        private bool _isWaiting;
        public VehicleController VehicleController { get; set; }
        public VehicleStopState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        public void MovementEnter()
        {
            _isWaiting = false;
        }

        public void MovementUpdate() 
        {
            var ray = new Ray(VehicleController.BasicVehicle.rayStartPoint.position, VehicleController.BasicVehicle.transform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance,_carLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.red);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                // wait 1 second
                if (_isWaiting == false)
                {
                    _isWaiting = true;
                    VehicleController.BasicVehicle.StartCoroutine(WaitForSeconds());
                }
            }
        }
        
        private IEnumerator WaitForSeconds()
        {
            yield return new WaitForSeconds(1);
            _isWaiting = false;
            VehicleController.SetState<VehicleGoState>();
        }

        public void MovementExit()
        {
        }
  
         
    }
}