using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Script.Vehicles.Controllers
{
    [Serializable]
    public class VehiclePathController
    {
        private VehicleController _vehicleController;
        public VehiclePathController(VehicleController vehicleController)
        {
            _vehicleController = vehicleController;
        }

        public void DefinePath() // Define the path for the vehicle to follow
        {
            var vehicle = _vehicleController.Vehicle;
            var targets = _vehicleController.targets;
            
            var path = targets.ConvertAll(t => t.position).ToArray();
            var tweenSpeed = vehicle.vehicleSo.speed; // Speed-based movement

            _vehicleController.MoveTween = vehicle.transform.DOPath(path, tweenSpeed, PathType.CatmullRom)
                .SetEase(Ease.InOutSine)
                .SetSpeedBased(true)
                .SetLoops(-1, LoopType.Restart) // Infinite loop
                .OnWaypointChange(OnWaypointReached); // Handle direction change

            _vehicleController.MoveTween.Pause();
        }

        private void OnWaypointReached(int waypointIndex)
        {
            var targets = _vehicleController.targets;
            
            waypointIndex--; // it is counting its spawn point
            
            if (waypointIndex < targets.Count - 1)
            {
                var vehicle = _vehicleController.Vehicle;
                var currentWaypoint = targets[waypointIndex];
                var nextWaypoint = targets[waypointIndex + 1];
                var direction = (nextWaypoint.position - currentWaypoint.position).normalized;
                var targetRotation = Quaternion.LookRotation(direction);
                
                vehicle.transform.rotation = targetRotation;
            }
        }
        
    }
}