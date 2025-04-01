using System;
using BaseCode.Logic.Managers;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using BaseCode.Logic.Vehicles.States.Movement;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Vehicles.Boss
{
    public class Truck : AggressiveCar
    {
        public Animator truckAnimator;
        public Transform handHoldCarPoint;
        public Transform loadPos;
        public override void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            base.Starter(manager, currentCar);
            
            var dict = VehicleController.VehicleStateController.GetStatesDict();
            dict[typeof(VehicleMovementTruckStopState)] = new VehicleMovementTruckStopState(VehicleController);
        }

        public override void AssignCollisionController()
        {
            VehicleController.VehicleCollisionController = new TruckVehicleCollisionController();
            VehicleController.VehicleCollisionController.Starter(this);
        }

        public override void DestinationReached( bool isDied = false)
        {
            ((TruckVehicleCollisionController)VehicleController.VehicleCollisionController).ReleaseLoad();
            base.DestinationReached(isDied);
        }
        public void OnSlappingCarHandAnimAction()
        {
            Debug.Log("OnSlappingCarHandAnimAction");
             
            var scale = CurrentVehicle.transform.localScale;
            var originalLocalScale = CurrentVehicle.transform.localScale;
            scale.x = 0.01f;
            CurrentVehicle.transform.localScale = scale;

            Vector3 localScale = new Vector3(originalLocalScale.x, originalLocalScale.y, originalLocalScale.z);
            TruckVehicleCollisionController.LoadedVehicleBases.
                Add(new Tuple<Vector3, VehicleBase>(localScale,CurrentVehicle));
            
            VehicleController.VehicleCollisionController.PlayFx(FxTypes.StarCarCrash, CurrentVehicle.transform);
        }
        
        public void OnPickTheCarHandAnimAction()
        {
            Debug.Log("OnPickTheCarHandAnimAction");
            CurrentVehicle.transform.parent = handHoldCarPoint;
        }
        public void OnDropTheCarHandAnimAction()
        {
            Debug.Log("OnDropTheCarHandAnimAction");
            
            CurrentVehicle.transform.parent = loadPos;
            CurrentVehicle.transform.localPosition= Vector3.zero;
            CurrentVehicle = null;
            VehicleController.SetState<VehicleMovementGoState>();
        }
        
        private TruckVehicleCollisionController TruckVehicleCollisionController =>
            (TruckVehicleCollisionController)VehicleController.VehicleCollisionController;

        private VehicleBase CurrentVehicle
        {
            get => TruckVehicleCollisionController.CurrentVehicle;
            set => TruckVehicleCollisionController.CurrentVehicle = value;
        }
    }
}