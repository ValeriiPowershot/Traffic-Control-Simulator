using BaseCode.Logic.EntityHandler.Lights;
using BaseCode.Logic.EntityHandler.Vehicles.States;
using UnityEngine;

namespace BaseCode.Logic.EntityHandler.Vehicles.Controllers
{
    public class VehicleCollisionController
    {
        private readonly VehicleGoState _vehicleGoState;

        private float _rayDistance;
        private LayerMask _stopLayer = 0;
        
        public VehicleCollisionController(VehicleGoState vehicleGoState)
        {
            _vehicleGoState = vehicleGoState;
            
            _stopLayer += 1 << 7; //add car layer
            _stopLayer += 1 << 10; //add stop line layer
            _rayDistance = _vehicleGoState.CarData.rayDistance;
        }

        public bool CheckForCollision()
        {
            Ray ray = new Ray(BasicVehicle.rayStartPoint.position, _vehicleGoState.CarTransform.forward);

            if (Physics.Raycast(ray, out var hit, _rayDistance, _stopLayer)) // hit to stop or car
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

                return CollisionHappened(hit);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.green);
            }

            return false;
        }

        private bool CollisionHappened(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out VehicleBase hitVehicle))
            {
                if (AreTheyInIntersection(hitVehicle) && AreTheyUsingDifferentPath(hitVehicle))
                {
                    Debug.Log("Game Is Over");
                }

                _vehicleGoState.VehicleController.SetState<VehicleStopState>();
                return true;
            }

            if (IsItRedLight())
            {
                _vehicleGoState.VehicleController.SetState<VehicleStopState>();
                return true;
            }

            return false;
        }

        private bool IsItRedLight()
        {
            return BasicVehicle.CarLightService.CarLightState == LightState.Red;
        }

        private bool AreTheyUsingDifferentPath(VehicleBase hitVehicle)
        {
            return hitVehicle.CarLightService.LightPlaceSave !=
                   BasicVehicle.CarLightService.LightPlaceSave;
        }

        private bool AreTheyInIntersection(VehicleBase hitVehicle)
        {
            return hitVehicle.CarLightService.LightPlaceSave != LightPlace.None &&
                   BasicVehicle.CarLightService.LightPlaceSave != LightPlace.None;
        }

        private BasicVehicle BasicVehicle => _vehicleGoState.VehicleController.BasicVehicle;

    }
}