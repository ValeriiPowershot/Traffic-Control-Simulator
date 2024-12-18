using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Vehicles.States;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Vehicles.Controllers
{
    [Serializable]
    public class VehicleController
    {
        private VehicleMovementStateController _vehicleMovementStateController;
        public Tween MoveTween; 
        
        // temporary target place
        [FormerlySerializedAs("Targets")] public List<Transform> targets = new List<Transform>();
        public Vehicle Vehicle { get; private set; }
 
        public void Starter(Vehicle vehicle)
        {
            Vehicle = vehicle;
            _vehicleMovementStateController = new VehicleMovementStateController(this);

            DefinePath();
            StartEngine();
        }

        public void StartEngine()
        {
            _vehicleMovementStateController.SetState<VehicleGoState>(); // Start in the stopped state
        }

        public void Update()
        {
            _vehicleMovementStateController.Update();
        }
 
        private void DefinePath()
        {
            var path = targets.ConvertAll(t => t.position).ToArray();
            var tweenSpeed = Vehicle.vehicleSo.speed; // 1000 / speed = time to reach target
            
            MoveTween = Vehicle.transform.DOPath(path, tweenSpeed, PathType.CatmullRom)
                .SetEase(Ease.InOutSine)
                .SetSpeedBased(true).SetLoops(-1, LoopType.Restart);    
            
            MoveTween.Pause();
        }
        
        public void CleanUp()
        {
            MoveTween?.Kill();
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


    }
}













