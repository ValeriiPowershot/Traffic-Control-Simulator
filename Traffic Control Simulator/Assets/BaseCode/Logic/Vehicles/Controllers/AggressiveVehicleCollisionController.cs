using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers
{
    public class AggressiveVehicleCollisionController : VehicleCollisionControllerBase
    {
        protected override bool IsGameOver(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out VehicleBase hitVehicle))
            {
                if (AreTheyInIntersection(hitVehicle) && AreTheyUsingDifferentPath(hitVehicle))
                {
                    Debug.Log("Game Is Over");
                    PlayFx(FxTypes.Angry);  

                    if (IsOtherCarAggressive(hitVehicle))
                    {
                        StopAndLetAggressiveCar(hitVehicle);
                        return false;
                    }
                }

                VehicleController.SetState<VehicleMovementStopState>();
                return true;
            }

            return false;
        }

        private void StopAndLetAggressiveCar(VehicleBase hitVehicle)
        {
            if (hitVehicle.VehicleController.GetStateCurrentState().GetType() != typeof(VehicleMovementStopState))
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