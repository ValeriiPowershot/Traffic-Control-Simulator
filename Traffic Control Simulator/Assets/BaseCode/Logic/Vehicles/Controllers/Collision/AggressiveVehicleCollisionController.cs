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
                if (AreTheyInIntersection(hitVehicle) && AreTheyUsingDifferentPath(hitVehicle))
                {
                    PlayFx(FxTypes.Angry);  

                    if (IsOtherCarAggressive(hitVehicle))
                    {
                        StopAndLetAggressiveCar(hitVehicle);
                        return false;
                    }
                    Debug.Log("Game Is Over");
                    PressCar(hitVehicle);
                }

                VehicleController.SetState<VehicleMovementStopState>();
                return true;
            }

            return false;
        }

        private void StopAndLetAggressiveCar(VehicleBase hitVehicle)
        {
            if (hitVehicle.vehicleController.GetStateCurrentState().GetType() != typeof(VehicleMovementStopState))
            {
                VehicleController.SetState<VehicleMovementStopState>();
            }
        }

        private bool IsOtherCarAggressive(VehicleBase hitVehicle)
        {
            return hitVehicle.GetType() == typeof(AggressiveCar);
        }

        protected override bool IsItRedLight()
        {
            return false;
        }
        
    }
}