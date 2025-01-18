using System.Collections;
using BaseCode.Logic.Vehicles.Controllers;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public class VehicleMovementStopState : IVehicleMovementState
    {
        private readonly float _rayDistance = 0.2f; // Adjust distance as needed
        private readonly LayerMask _carLayer = LayerMask.GetMask("Car"); // Ensure cars are on a "Car" layer
        
        private bool _isWaiting;
        public VehicleController VehicleController { get; set; }
        public VehicleMovementStopState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        public void MovementEnter()
        {
            _isWaiting = false;
        }

        public void MovementUpdate() 
        {
            var ray = new Ray(VehicleController.BasicCar.RayStartPoint.position, VehicleController.BasicCar.transform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance,_carLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.red);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                if (_isWaiting == false)
                {
                    _isWaiting = true;
                    VehicleController.BasicCar.StartCoroutine(WaitForSeconds());
                }
            }
        }
        
        private IEnumerator WaitForSeconds()
        {
            yield return new WaitForSeconds(1f);
            _isWaiting = false;
            VehicleController.SetState<VehicleMovementGoState>();
        }

        public void MovementExit()
        {
        }
  
         
    }
}