using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Vehicles.States;
using UnityEngine;

namespace Script.Vehicles.Controllers
{
    [Serializable]
    public class VehicleController 
    {
        private VehicleMovementStateController _movementStateController;
        private VehiclePathController _vehiclePathController;

        public List<Transform> targets = new();
        public Tween MoveTween; 
 
        public void Starter(Vehicle vehicle)
        {
            Vehicle = vehicle;
            _movementStateController = new VehicleMovementStateController(this);
            _vehiclePathController = new VehiclePathController(this);

            _vehiclePathController.DefinePath();
            StartEngine();
        }

        public void StartEngine()
        {
            SetState<VehicleGoState>(); // Start in the stopped state
        }

        public void Update()
        {
            _movementStateController.Update();
        }

        public bool IsTweenWorking()
        {
            if (MoveTween != null && MoveTween.IsPlaying() == false)
            {
                MoveTween.Play();
                return false;
            }

            return true;
        }
        public void CleanUp()
        {
            MoveTween?.Kill();
        }

        public void SetState<T>() where T : IVehicleState
        {
            _movementStateController.SetState<T>(); // Start in the stopped state
        }
        
        public Vehicle Vehicle { get; private set; }
    }
}













