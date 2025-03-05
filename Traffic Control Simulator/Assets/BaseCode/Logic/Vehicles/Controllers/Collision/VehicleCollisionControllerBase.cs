using BaseCode.Logic.Lights;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Collision
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
            _rayDistance = vehicleBase.VehicleScriptableObject.rayLenght;
        }

        public bool CheckForCollision()
        {
            Ray ray = new Ray(BasicCar.RayStartPoint.position, VehicleController.CarTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, _stopLayer)) // hit to stop or car
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

                return CollisionHappened(hit);
            }
            Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.green);

            return false;
        }

        private bool CollisionHappened(RaycastHit hit)
        {
            if (IsGameOver(hit))
                return true;

            if (!IsItRedLight()) 
                return false;
            
            VehicleController.SetState<VehicleMovementStopState>();
            return true;
        }

        protected virtual bool IsGameOver(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out VehicleBase hitVehicle)) 
                return false;
            
            if (AreTheyFromAnotherSpawner(hitVehicle))
            {
                Debug.Log("Game Is Over");
                PlayFx(FxTypes.Angry);  
                PlayFx(FxTypes.Smoke, VehicleController.VehicleBase.transform);  
            }
            
            VehicleController.SetState<VehicleMovementStopState>();
            return true;
        }

        protected virtual bool IsItRedLight() =>
            BasicCar.CarLightService.CarLightState == LightState.Red;

        protected bool AreTheyFromAnotherSpawner(VehicleBase hitVehicle) =>
            hitVehicle.SpawnIndex != BasicCar.SpawnIndex;

        protected bool AreTheyUsingDifferentPath(VehicleBase hitVehicle)
        {
            return hitVehicle.CarLightService.LightPlaceSave !=
                   BasicCar.CarLightService.LightPlaceSave;
        }

        protected bool AreTheyInIntersection(VehicleBase hitVehicle)
        {
            return hitVehicle.CarLightService.LightPlaceSave != LightPlace.None &&
                   BasicCar.CarLightService.LightPlaceSave != LightPlace.None;
        }

        protected void PlayFx(FxTypes fxTypes) =>
            PlayFx(fxTypes, BasicCar.emojiFxSpawnPoint);

        public void PlayFx(FxTypes fxTypes, Transform spawnPoint) =>
            BasicCar.CarManager.gameManager.fxManager.PlayFx(fxTypes, spawnPoint);

        protected BasicCar BasicCar => 
            VehicleController.VehicleBase;
    }
}