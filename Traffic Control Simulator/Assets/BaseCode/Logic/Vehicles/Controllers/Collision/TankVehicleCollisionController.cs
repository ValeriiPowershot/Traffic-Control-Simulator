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

    }
}