using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Collision
{
    public class AggressiveVehicleCollisionController : VehicleCollisionControllerBase
    {
        protected override bool IsGameOver(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out VehicleBase hitVehicle))
            {
                Debug.Log("hitVehicle: " + hitVehicle);
                if (AreTheyFromAnotherSpawner(hitVehicle))
                {
                    PlayFx(FxTypes.Angry);  

                    if (IsOtherCarSameWithThisVehicle(hitVehicle))
                    {
                        StopAndLetSameTypeCar(hitVehicle);
                        return false;
                    }
                    Debug.Log("Game Is Over");
                }

                VehicleController.SetState<VehicleMovementStopState>();
                return true;
            }

            return false;
        }

        private void StopAndLetSameTypeCar(VehicleBase hitVehicle)
        {
            if (hitVehicle.VehicleController.GetStateCurrentState().GetType() != typeof(VehicleMovementStopState)) 
                VehicleController.SetState<VehicleMovementStopState>();
        }

        private bool IsOtherCarSameWithThisVehicle(VehicleBase hitVehicle) =>
            hitVehicle.GetType() == BasicCar.GetType();

        protected override bool IsItRedLight() =>
            false;
    }
}