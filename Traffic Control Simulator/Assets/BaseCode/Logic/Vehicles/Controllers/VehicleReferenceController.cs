using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.Vehicles.Controllers
{
    [Serializable]
    public class VehicleReferenceController
    {
        public Transform emojiFxSpawnPoint;
        public Transform rayStartPoint;
        public Transform arrowIndicatorEndPoint;
        
    }
}