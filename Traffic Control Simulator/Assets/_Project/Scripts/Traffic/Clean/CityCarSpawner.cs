using System.Collections.Generic;
using UnityEngine;

namespace TrafficControl.CleanVehicles
{
    public sealed class CityCarSpawner : MonoBehaviour
    {
        public CityRoadPath Road;
        public CityCarController VehiclePrefab;
        [Min(0.2f)] public float Interval = 3.5f;
        [Min(1)] public int Capacity = 10;
        public Color Paint = new Color(0.2f, 0.6f, 0.8f);
        public int Completed { get; private set; }
        private readonly List<CityCarController> spawned = new List<CityCarController>();
        private float timer;
        private void Update()
        {
            if (!Road) return;
            spawned.RemoveAll(car => car == null);
            timer -= Time.deltaTime;
            if (timer > 0 || spawned.Count >= Capacity) return;
            foreach (var car in spawned) if (car.RouteDistance < 6) return;
            Spawn(); timer = Mathf.Max(0.2f, Interval);
        }
        public CityCarController Spawn()
        {
            if (!Road) return null;
            CityCarController car;
            if (VehiclePrefab)
            {
                var start = Road.At(0);
                car = Instantiate(VehiclePrefab, start.Position + Vector3.up * 0.7f + start.Tangent * 1.2f, Quaternion.LookRotation(start.Tangent), transform);
                car.Prepare(Road, GetInstanceID());
            }
            else car = CityCarFactory.Create(Road, 0, GetInstanceID(), Paint, transform);
            spawned.Add(car);
            car.Arrived += OnArrival;
            return car;
        }
        private void OnArrival(CityCarController car) { Completed++; Destroy(car.gameObject); }
    }
}
