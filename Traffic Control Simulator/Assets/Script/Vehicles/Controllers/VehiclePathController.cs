using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// this file is not used in the project
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
            Vehicle vehicle = _vehicleController.Vehicle;
            List<Transform> targets = _vehicleController.targets;
            
            Vector3[] path = targets.ConvertAll(t => t.position).ToArray();
            float tweenSpeed = vehicle.VehicleScriptableObject.Speed; // Speed-based movement

            _vehicleController.MoveTween = vehicle.transform.DOPath(path, tweenSpeed, PathType.CatmullRom)
                .SetEase(Ease.InOutSine)
                .SetSpeedBased(true)
                .SetLoops(-1, LoopType.Restart) // Infinite loop
                .OnWaypointChange(OnWaypointReached); // Handle direction change

            _vehicleController.MoveTween.Pause();
        }

        private void OnWaypointReached(int waypointIndex)
        {
            List<Transform> targets = _vehicleController.targets;
            
            waypointIndex--; // it is counting its spawn point
            
            if (waypointIndex < targets.Count - 1)
            {
                Vehicle vehicle = _vehicleController.Vehicle;
                Transform currentWaypoint = targets[waypointIndex];
                Transform nextWaypoint = targets[waypointIndex + 1];
                Vector3 direction = (nextWaypoint.position - currentWaypoint.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                vehicle.transform.rotation = targetRotation;
            }
        }
        
    }
}