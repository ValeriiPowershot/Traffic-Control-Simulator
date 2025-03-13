using System.Collections;
using BaseCode.Logic.Lights;
using BaseCode.Logic.ScriptableObject;
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
            PressCar(hitVehicle);
        }
        protected virtual void PressCar(VehicleBase hitVehicle)
        {
            Debug.Log("Press Effect");
            DisableVehicle(hitVehicle);
            BasicCar.CarManager.StartCoroutine(PressEffect(hitVehicle));
        }

        private IEnumerator PressEffect(VehicleBase hitVehicle)
        {
            Vector3 originalScale = hitVehicle.transform.localScale;
            Vector3 originalPosition = hitVehicle.transform.position;

            Vector3 pressedScale = new Vector3(originalScale.x, originalScale.y * 0.1f, originalScale.z);

            PlayFx(FxTypes.StarCarCrash, hitVehicle.transform, new Vector3(10,10,10));
            GameManager.cameraManager.CameraShake();
            
            hitVehicle.transform.DOScaleY(pressedScale.y, 0.5f).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(1f);

            hitVehicle.transform.DOMoveY(originalPosition.y - 5f, 1f).SetEase(Ease.OutQuad);
    
            yield return new WaitForSeconds(1f);
            
            hitVehicle.transform.position = originalPosition;
            hitVehicle.transform.localScale = originalScale;

            EnableVehicle(hitVehicle);
            hitVehicle.VehicleController.StateController.GetState<VehicleMovementGoState>()
                .VehiclePathController.SetPathToEndPosition(originalScale);
            hitVehicle.VehicleController.VehicleBase.SetCarDiedOnCollision();
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

        private void DisableVehicle(VehicleBase vehicle)
        {
            vehicle.GetComponent<BoxCollider>().enabled = false;
            vehicle.enabled = false;
        }

        private void EnableVehicle(VehicleBase vehicle)
        {
            vehicle.enabled = true;
            vehicle.GetComponent<BoxCollider>().enabled = true;
        }
        protected void PlayFx(FxTypes fxTypes) =>
            PlayFx(fxTypes, BasicCar.emojiFxSpawnPoint, new Vector3(5,5,5)); // sorry for static values :D

        public void PlayFx(FxTypes fxTypes, Transform spawnPoint, Vector3 localScale = default) =>
            GameManager.fxManager.PlayFx(fxTypes, spawnPoint, localScale);

        protected BasicCar BasicCar => 
            VehicleController.VehicleBase;

        protected GameManager GameManager => BasicCar.CarManager.gameManager;
    }
}