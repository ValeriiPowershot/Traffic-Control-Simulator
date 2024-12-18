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
        private VehicleMovementStateController _movementStateController;
        public Tween MoveTween; 
        
        // temporary target place
        public List<Transform> targets = new List<Transform>();
        public Vehicle Vehicle { get; private set; }
 
        public void Starter(Vehicle vehicle)
        {
            Vehicle = vehicle;
            _movementStateController = new VehicleMovementStateController(this);

            DefinePath();
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
 
        private void DefinePath()
        {
            var path = targets.ConvertAll(t => t.position).ToArray();
            var tweenSpeed = Vehicle.vehicleSo.speed; // Speed-based movement

            MoveTween = Vehicle.transform.DOPath(path, tweenSpeed, PathType.CatmullRom)
                .SetEase(Ease.InOutSine)
                .SetSpeedBased(true)
                .SetLoops(-1, LoopType.Restart) // Infinite loop
                .OnWaypointChange(OnWaypointReached); // Handle direction change

            MoveTween.Pause();
        }

        private void OnWaypointReached(int waypointIndex)
        {
            waypointIndex--; // it is counting its spawn point
            
            if (waypointIndex < targets.Count - 1)
            {
                var currentWaypoint = targets[waypointIndex];
                var nextWaypoint = targets[waypointIndex + 1];
                var direction = (nextWaypoint.position - currentWaypoint.position).normalized;
                var targetRotation = Quaternion.LookRotation(direction);
                
                Vehicle.transform.rotation = targetRotation;
            }
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
    }
}













