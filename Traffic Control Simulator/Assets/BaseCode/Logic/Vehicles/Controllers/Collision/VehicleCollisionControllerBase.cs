using System.Collections;
using BaseCode.Extensions;
using BaseCode.Extensions.UI;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Managers;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers.Lights;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using DG.Tweening;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Collision
{
    public class VehicleCollisionControllerBase
    {
        protected VehicleController VehicleController;

        private float _rayDistance;
        private LayerMask _stopLayer = 0;
        
        public int spawnIndex;
        public void Starter(VehicleBase vehicleBase)
        {
            VehicleController = vehicleBase.vehicleController;
            
            _stopLayer += 1 << 7; //add car layer
            _stopLayer += 1 << 10; //add stop line layer
            
            _rayDistance = vehicleBase.VehicleScriptableObject.rayLenght;
        }
        public bool CheckForCollision()
        {
            Ray ray = new Ray(ReferenceController.rayStartPoint.position, VehicleController.CarTransform.forward);

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
                OnGameOver(hitVehicle);
                return true;
            }
            
            VehicleController.SetState<VehicleMovementStopState>();
            return true;
        }

        private void OnGameOver(VehicleBase hitVehicle)
        {
            Debug.Log("Game Is Over");
            PlayFx(FxTypes.Angry);  
            PlayFx(FxTypes.Smoke, VehicleController.VehicleBase.transform, new Vector3(2.7f,2.7f,2.7f));
            
            CarCrashed(hitVehicle);
        }
        protected virtual void CarCrashed(VehicleBase hitVehicle)
        {
            hitVehicle.DisableVehicle();
            BasicCar.CarManager.StartCoroutine(PressEffect(hitVehicle));
        }

        private IEnumerator PressEffect(VehicleBase hitVehicle) // this should change!
        {
            Vector3 originalScale = hitVehicle.transform.localScale;
            Vector3 originalPosition = hitVehicle.transform.position;

            PlayFx(FxTypes.StarCarCrash, hitVehicle.transform, new Vector3(10,10,10));
            GameManager.cameraManager.CameraShake();

            hitVehicle.transform.PressedEffect(0.1f);

            yield return new WaitForSeconds(1f);
            
            hitVehicle.transform.DOMoveY(originalPosition.y - 5f, 1f).SetEase(Ease.OutQuad);
    
            yield return new WaitForSeconds(1f);
            
            hitVehicle.transform.position = originalPosition;
            hitVehicle.transform.localScale = originalScale;

            hitVehicle.EnableVehicle();

            hitVehicle.DestinationReached(true);
        }
        
        protected virtual bool IsItRedLight() =>
            VehicleLightController.CarLightState == LightState.Red;

        protected bool AreTheyFromAnotherSpawner(VehicleBase hitVehicle) =>
            hitVehicle.vehicleController.VehicleCollisionController.spawnIndex != spawnIndex;

        protected bool AreTheyUsingDifferentPath(VehicleBase hitVehicle)
        {
            return hitVehicle.vehicleController.VehicleLightController.LightPlaceSave !=
                   VehicleLightController.LightPlaceSave;
        }
        protected bool AreTheyInIntersection(VehicleBase hitVehicle)
        {
            return hitVehicle.vehicleController.VehicleLightController.LightPlaceSave != LightPlace.None &&
                   VehicleLightController.LightPlaceSave != LightPlace.None;
        }
        protected void PlayFx(FxTypes fxTypes) =>
            PlayFx(fxTypes, ReferenceController.emojiFxSpawnPoint, new Vector3(5,5,5)); // sorry for static values :D

        public void PlayFx(FxTypes fxTypes, Transform spawnPoint, Vector3 localScale = default) =>
            GameManager.fxManager.PlayFx(fxTypes, spawnPoint, localScale);
        protected BasicCar BasicCar => VehicleController.VehicleBase;
        protected VehicleLightController VehicleLightController => BasicCar.vehicleController.VehicleLightController;

        protected VehicleReferenceController ReferenceController =>
            BasicCar.vehicleController.vehicleReferenceController;
        protected GameManager GameManager => BasicCar.CarManager.GameManager;
    }
}