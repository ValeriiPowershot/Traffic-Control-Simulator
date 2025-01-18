using BaseCode.Logic.Lights;
using BaseCode.Logic.Vehicles.States.Movement;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers
{
    public class VehicleCollisionController
    {
        private readonly VehicleMovementGoState _vehicleGoState;

        private float _rayDistance;
        private LayerMask _stopLayer = 0;
        
        public VehicleCollisionController(VehicleMovementGoState vehicleGoState)
        {
            _vehicleGoState = vehicleGoState;
            
            _stopLayer += 1 << 7; //add car layer
            _stopLayer += 1 << 10; //add stop line layer
            _rayDistance = _vehicleGoState.CarData.RayLenght;
        }

        public bool CheckForCollision()
        {
            Ray ray = new Ray(BasicCar.RayStartPoint.position, _vehicleGoState.CarTransform.forward);

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
            if (hit.collider.TryGetComponent(out Vehicles.VehicleBase hitVehicle))
            {
                if (AreTheyInIntersection(hitVehicle) && AreTheyUsingDifferentPath(hitVehicle))
                {
                    Debug.Log("Game Is Over");
                }

                _vehicleGoState.VehicleController.SetState<VehicleMovementStopState>();
                return true;
            }

            if (IsItRedLight())
            {
                _vehicleGoState.VehicleController.SetState<VehicleMovementStopState>();
                return true;
            }

            return false;
        }

        private bool IsItRedLight()
        {
            return BasicCar.CarLightService.CarLightState == Vehicles.LightState.Red;
        }

        private bool AreTheyUsingDifferentPath(Vehicles.VehicleBase hitVehicle)
        {
            return hitVehicle.CarLightService.LightPlaceSave !=
                   BasicCar.CarLightService.LightPlaceSave;
        }

        private bool AreTheyInIntersection(Vehicles.VehicleBase hitVehicle)
        {
            return hitVehicle.CarLightService.LightPlaceSave != LightPlace.None &&
                   BasicCar.CarLightService.LightPlaceSave != LightPlace.None;
        }

        private Vehicles.BasicCar BasicCar => _vehicleGoState.VehicleController.BasicCar;

    }
}