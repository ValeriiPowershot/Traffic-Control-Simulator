using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.So
{
    [CreateAssetMenu(fileName = "VehiclesSo", menuName = "So/VehiclesSo", order = 0)]
    public class VehiclesSo : ScriptableObject
    {
        // later I can change it to a vehicle type that has vehicles. 
        public List<VehicleSo> vehicles = new List<VehicleSo>();
    }
}