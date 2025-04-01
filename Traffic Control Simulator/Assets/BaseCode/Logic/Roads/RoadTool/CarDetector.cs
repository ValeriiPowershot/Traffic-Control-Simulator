using System.Collections.Generic;
using BaseCode.Extensions;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.Roads.RoadTool
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarDetector : MonoBehaviour
    {
        public List<BasicCar> cars = new();
        private LayerMask _carLayer;

        public int carDetectorSpawnIndex;
        public void Start()
        {
            _carLayer = LayerMask.GetMask("Car"); 
        }
        public bool IsThereCarInSpawnPoint()
        {
            return cars.Count > 0;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetSameLayerComponent(_carLayer, out BasicCar car))
            {
                cars.Add(car); 
                car.VehicleController.VehicleCollisionController.spawnIndex = carDetectorSpawnIndex;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetSameLayerComponent(_carLayer, out BasicCar car))
            {
                cars.Remove(car);
            }
        }
        public void ResetDetector()
        {
            cars.Clear();
        }
    }
}