using System.Collections.Generic;
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
            bool result = IsCarInLayer(other);
            
            if (result && other.TryGetComponent(out BasicCar car))
            {
                cars.Add(car); 
                car.SpawnIndex = carDetectorSpawnIndex;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsCarInLayer(other) && other.TryGetComponent(out BasicCar car))
            {
                cars.Remove(car);
            }
        }
        
        private bool IsCarInLayer(Collider other)
        {
            return _carLayer.value == (1 << other.gameObject.layer);
        }

        public void ResetDetector()
        {
            cars.Clear();
        }
    }
}