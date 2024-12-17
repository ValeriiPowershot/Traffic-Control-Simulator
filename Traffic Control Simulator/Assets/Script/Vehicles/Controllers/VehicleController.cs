using System;
using Script.Vehicles.States;
using UnityEngine;

namespace Script.Vehicles.Controllers
{
    public class VehicleController
    {
        private readonly VehicleStateController _vehicleStateController;

        public VehicleController(Vehicle vehicle)
        {
            _vehicleStateController = new VehicleStateController(this);
            Vehicle = vehicle;
        }
        
        public void StartEngine()
        {
            _vehicleStateController.SetState<VehicleStopState>();
        }

        public void Update()
        {
            _vehicleStateController.Update();
        }

        public Vehicle Vehicle { get; set; }
    }
}













