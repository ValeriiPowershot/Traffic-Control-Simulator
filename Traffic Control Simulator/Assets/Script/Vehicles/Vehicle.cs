using Script.So;
using Script.Vehicles.Controllers;
using UnityEngine;

namespace Script.Vehicles
{
    public class Vehicle : MonoBehaviour
    {
        public VehicleController vehicleController;
        public VehicleSo vehicleSo;

        private void Start() // this will be called by spawn manager
        {
            vehicleController.Starter(this);
        }

        public void Update()
        {
            // every car had a update but then i added dotween and it was not needed anymore
            vehicleController.Update();
        }

        public void OnDestroy()
        {
            vehicleController.CleanUp();
        }
    }
}