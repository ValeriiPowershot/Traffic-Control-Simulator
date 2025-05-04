using System;
using BaseCode.Interfaces;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using BaseCode.Logic.Vehicles.Controllers.Lights;
using BaseCode.Logic.Vehicles.Controllers.Path;
using BaseCode.Logic.Vehicles.Controllers.Score;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers
{
    [Serializable]
    public class VehicleController 
    {
        private VehicleScriptableObject VehicleSo { get; set; }
        public Transform CarTransform => VehicleBase.transform;

        public VehicleMovementStateController VehicleStateController;
        public VehicleCollisionControllerBase VehicleCollisionController;
        public VehiclePathController VehiclePathController;
        public VehicleScoreController VehicleScoreController;
        public VehicleLightController VehicleLightController;
        public VehicleReferenceController vehicleReferenceController;
        public VehicleTurnLights vehicleTurnLights;
        
        
        
        private IVehicleMovementState _currentMovementState;
        
        public void Starter(BasicCar basicCar)
        {
            VehicleBase = basicCar;
            VehicleSo = VehicleBase.VehicleScriptableObject;

            VehicleStateController = new VehicleMovementStateController(this);
            VehiclePathController = new VehiclePathController(this);
            VehicleScoreController = new VehicleScoreController(this);
            VehicleLightController = new VehicleLightController(this);
            VehicleBase.AssignCollisionController();
                
            StartEngine();
        }
        private void StartEngine() =>
            SetState<VehicleMovementGoState>(); // Start in the stopped state

        public void Update()
        {
            VehicleStateController.Update();
            vehicleTurnLights.CheckTurnLightState();
        }
        public void RestartControllers(bool isDied = false)
        {
            VehicleStateController.RestartVehicleMovementStateController();
            VehiclePathController.SetPathToEndPosition();
            VehicleScoreController.RestartVehicleScore(isDied);
            VehicleLightController.RestartVehicleLightController();
        }

        public T GetCurrentState<T>() where T : class, IVehicleMovementState
        {
            if (_currentMovementState is T typedState)
                return typedState;

            Debug.LogWarning($"Current state is not of type {typeof(T).Name}");
            return null;
        }
        
        public void SetState<T>() where T : IVehicleMovementState =>
            VehicleStateController.SetState<T>(); // Start in the stopped state
        
        public IVehicleMovementState GetStateCurrentState() =>
            VehicleStateController.GetStateCurrentState();
        
        public BasicCar VehicleBase { get; private set; }
        public VehicleMovementGoState GoState => VehicleStateController.GetState<VehicleMovementGoState>();
    }
}