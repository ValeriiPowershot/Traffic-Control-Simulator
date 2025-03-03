using System;
using System.Collections.Generic;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using BaseCode.Logic.Vehicles.Vehicles.Boss;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Collision
{
    public class TruckVehicleCollisionController : TankVehicleCollisionController
    {
        private static readonly int IsHandSlapper = Animator.StringToHash("IsHandSlapper");
        public readonly List<Tuple<Vector3, VehicleBase>> LoadedVehicleBases = new List<Tuple<Vector3, VehicleBase>>();
        public VehicleBase CurrentVehicle;
        
        protected override bool IsGameOver(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out VehicleBase hitVehicle))
            {
                PlayFx(FxTypes.DiabolicalLaugh);
                PressCar(hitVehicle);
            }

            return false;
        }
        protected override void PressCar(VehicleBase hitVehicle)
        {
            VehicleController.SetState<VehicleMovementTruckStopState>();
            
            CurrentVehicle = hitVehicle;
            hitVehicle.GetComponent<BoxCollider>().enabled = false;
            hitVehicle.enabled = false;
            Truck.truckAnimator.SetTrigger(IsHandSlapper);
        }

        public void ReleaseLoad()
        {
            foreach (Tuple<Vector3, VehicleBase> loadedVehicleBase in LoadedVehicleBases)
            {
                VehicleBase hitVehicle = loadedVehicleBase.Item2;

                hitVehicle.transform.parent = null;
                hitVehicle.enabled = true;
                hitVehicle.GetComponent<BoxCollider>().enabled = true;
                hitVehicle.VehicleController.StateController.GetState<VehicleMovementGoState>().VehiclePathController
                    .SetPathToEndPosition(loadedVehicleBase.Item1);
            }
            LoadedVehicleBases.Clear();
        }
        
        private Truck Truck => (Truck)VehicleController.VehicleBase;
    }
}