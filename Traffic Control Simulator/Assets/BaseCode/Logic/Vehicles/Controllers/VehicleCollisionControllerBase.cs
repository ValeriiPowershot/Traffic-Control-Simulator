using BaseCode.Logic.Lights;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers
{
    public class VehicleCollisionControllerBase
    {
        protected VehicleController VehicleController;

        private float _rayDistance;
        private LayerMask _stopLayer = 0;
        
        public void Starter(VehicleBase vehicleBase)
        {
            VehicleController = vehicleBase.VehicleController;
            
            _stopLayer += 1 << 7; //add car layer
            _stopLayer += 1 << 10; //add stop line layer
            _rayDistance = vehicleBase.VehicleScriptableObject.RayLenght;
        }

        public bool CheckForCollision()
        {
            Ray ray = new Ray(BasicCar.RayStartPoint.position, VehicleController.CarTransform.forward);

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
            if (IsGameOver(hit))
                return true;

            if (IsItRedLight())
            {
                VehicleController.SetState<VehicleMovementStopState>();
                return true;
            }

            return false;
        }

        protected virtual bool IsGameOver(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out VehicleBase hitVehicle))
            {
                if (AreTheyInIntersection(hitVehicle) && AreTheyUsingDifferentPath(hitVehicle))
                {
                    Debug.Log("Game Is Over");
                    PlayFx(FxTypes.Angry);  
                }

                VehicleController.SetState<VehicleMovementStopState>();
                return true;
            }

            return false;
        }

        protected virtual bool IsItRedLight()
        {
            return BasicCar.CarLightService.CarLightState == LightState.Red;
        }

        protected bool AreTheyUsingDifferentPath(Vehicles.VehicleBase hitVehicle)
        {
            return hitVehicle.CarLightService.LightPlaceSave !=
                   BasicCar.CarLightService.LightPlaceSave;
        }

        protected bool AreTheyInIntersection(Vehicles.VehicleBase hitVehicle)
        {
            return hitVehicle.CarLightService.LightPlaceSave != LightPlace.None &&
                   BasicCar.CarLightService.LightPlaceSave != LightPlace.None;
        }
        public void PlayFx(FxTypes fxTypes)
        {
            BasicCar.CarManager.gameManager.fxManager.PlayFx(fxTypes, BasicCar.emojiFxSpawnPoint);
        }

        private BasicCar BasicCar => VehicleController.VehicleBase;

    }
}