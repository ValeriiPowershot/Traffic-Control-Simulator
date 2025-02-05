using System.Collections;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using DG.Tweening;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Collision
{
    public class TankVehicleCollisionController : AggressiveVehicleCollisionController
    {
        protected override bool IsGameOver(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out VehicleBase hitVehicle))
            {
                PlayFx(FxTypes.DiabolicalLaugh);
                PressCar(hitVehicle);
            }

            return false;
        }
  
        protected virtual void PressCar(VehicleBase hitVehicle)
        {
            hitVehicle.GetComponent<BoxCollider>().enabled = false;
            hitVehicle.enabled = false;
            
            BasicCar.CarManager.StartCoroutine(PressEffect(hitVehicle));
        }

        private IEnumerator PressEffect(VehicleBase hitVehicle)
        {
            Vector3 originalScale = hitVehicle.transform.localScale;
            Vector3 pressedScale = new Vector3(originalScale.x, originalScale.y * 0.01f, originalScale.z);
            hitVehicle.transform.DOScaleY(pressedScale.y, 1f).SetEase(Ease.OutQuad);
            PlayFx(FxTypes.StarCarCrash, hitVehicle.transform);

            yield return new WaitForSeconds(1f);

            hitVehicle.enabled = true;
            hitVehicle.GetComponent<BoxCollider>().enabled = true;
            hitVehicle.VehicleController.StateController.GetState<VehicleMovementGoState>().
                VehiclePathController.SetPathToEndPosition(originalScale);
        }

    }
}