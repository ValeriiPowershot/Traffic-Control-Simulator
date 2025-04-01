using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Extensions
{
    public static class VehicleBaseExtension
    {
        public static void DisableVehicle(this VehicleBase vehicle)
        {
            vehicle.GetComponent<BoxCollider>().enabled = false;
            vehicle.enabled = false;
        }
        
        public static void EnableVehicle(this VehicleBase vehicle)
        {
            vehicle.enabled = true;
            vehicle.GetComponent<BoxCollider>().enabled = true;
        }
        
        
    }
}