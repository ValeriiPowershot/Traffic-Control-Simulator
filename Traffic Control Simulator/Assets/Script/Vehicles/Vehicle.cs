using System;
using Script.So;
using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles
{
    public class Vehicle : MonoBehaviour
    {
        private VehicleController _vehicleController;
        
        public VehicleSo vehicleSo;

        private void Start()
        {
            _vehicleController = new VehicleController(this);
            _vehicleController.StartEngine();
        }

        public void Update()
        {
            _vehicleController.Update();
        }
    }
}