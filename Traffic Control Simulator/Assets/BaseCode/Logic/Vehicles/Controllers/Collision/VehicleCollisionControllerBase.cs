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
        public bool isCrashed = false;
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
            if (IsCrashedWithAnotherCar(hit))
                return true;

            if (!IsItRedLight()) 
                return false;
            
            VehicleController.SetState<VehicleMovementStopState>();
            return true;
        }

        protected virtual bool IsCrashedWithAnotherCar(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out VehicleBase hitVehicle)) 
                return false;
            
            if (AreTheyFromAnotherSpawner(hitVehicle))
            {
                OnCrash(hitVehicle);
                return true;
            }
                
            VehicleController.SetState<VehicleMovementStopState>();
            return true;
        }

        private void SetCarCrashed(bool value, VehicleBase hitVehicle)
        {
            isCrashed = value;
            hitVehicle.vehicleController.VehicleCollisionController.isCrashed = value;
        }

        private static float _collisionIntensity = 0f; // Tracks global intensity

        private void OnCrash(VehicleBase hitVehicle)
        {
            if (IsVehicleCrashed(hitVehicle) == false)
            {
                SetCarCrashed(true, hitVehicle);
                PlayerFirstCrasherFx();

                // Increase intensity by 10 and set global FMOD parameter
                _collisionIntensity += 10f;
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName(AudioHelper.ParameterName.Intensity, _collisionIntensity);

                // Call FMOD collision event using AudioHelper
                AudioHelper.Play3DSFXAtPosition(AudioHelper.EventPath.SFX_Collision, BasicCar.transform.position);

                CarCrashed(hitVehicle);
                CarCrashed(BasicCar);
            }
        }
        

        protected virtual void CarCrashed(VehicleBase hitVehicle)
        {
            GameManager.StartCoroutine(CrashedVehicle(hitVehicle));
        }
        
        private IEnumerator CrashedVehicle(VehicleBase hitVehicle) // this should change!
        {
            hitVehicle.vehicleController.vehicleTurnLights.LightsBlinking();
            GameManager.cameraManager.CameraShake();
            hitVehicle.DisableVehicle();
            // PlayFx(FxTypes.StarCarCrash, hitVehicle.transform, new Vector3(10,10,10));
            hitVehicle.transform.DOShakePosition(0.3f, strength: 0.05f, vibrato: 10);

            yield return hitVehicle.FadeOutAndRemove();

            hitVehicle.DestinationReached(true);
            hitVehicle.EnableVehicle();
            SetCarCrashed(false, hitVehicle);
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
        private void PlayerFirstCrasherFx()
        {
            // PlayFx(FxTypes.Angry);  
            PlayFx(FxTypes.Smoke, VehicleController.VehicleBase.transform, new Vector3(2.7f,2.7f,2.7f));
        }

        private bool IsVehicleCrashed(VehicleBase hitVehicle)
        {
            return hitVehicle.vehicleController.VehicleCollisionController.isCrashed;
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