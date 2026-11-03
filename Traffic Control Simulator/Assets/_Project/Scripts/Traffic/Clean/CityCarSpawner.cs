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
        [Header("Driver variation")]
        [Range(0, 0.2f)] public float DriverVariation = 0.08f;
        public bool StartAutomatically = true;
        public int Completed { get; private set; }
        private readonly List<CityCarController> spawned = new List<CityCarController>();
        private float timer;
        private int sequence;
        private readonly Collider[] occupancy = new Collider[64];
        private void Update()
        {
            if (!Road || !StartAutomatically) return;
            spawned.RemoveAll(car => car == null);
            timer -= Time.deltaTime;
            if (timer > 0 || spawned.Count >= Capacity) return;
            foreach (var car in spawned) if (car.RouteDistance < 6) return;
            if (Spawn()) timer = Mathf.Max(0.2f, Interval);
        }
        public CityCarController Spawn()
        {
            if (!Road || Road.Length < 5) return null;
            spawned.RemoveAll(item => item == null);
            if (spawned.Count >= Mathf.Max(1, Capacity) || !SpawnAreaClear()) return null;
            CityCarController car;
            if (VehiclePrefab)
            {
                var start = Road.At(0);
                car = Instantiate(VehiclePrefab, start.Position + Vector3.up * 0.7f + start.Tangent * 1.2f, Quaternion.LookRotation(start.Tangent), transform);
                car.Prepare(Road, GetInstanceID());
            }
            else car = CityCarFactory.Create(Road, 0, GetInstanceID(), Paint, transform);
            // Local deterministic sequence avoids changing Unity's global random state.
            float style = Mathf.Repeat(++sequence * 0.61803399f, 1) * 2 - 1;
            float factor = 1 + style * DriverVariation;
            car.TopSpeedKph *= factor; car.MaxAcceleration *= factor;
            car.QueueGap *= 1 + style * DriverVariation;
            car.StartReactionSeconds = Mathf.Clamp(0.4f - style * 0.15f, 0.2f, 0.6f);
            spawned.Add(car);
            car.Arrived += OnArrival;
            return car;
        }
        public bool SpawnAreaClear()
        {
            if (!Road) return false;
            var start = Road.At(0);
            Vector3 size = VehiclePrefab && VehiclePrefab.Chassis ? VehiclePrefab.Chassis.size : new Vector3(1.55f, 0.65f, 3.6f);
            Vector3 center = start.Position + Vector3.up * 0.7f + start.Tangent * 1.2f;
            int count = Physics.OverlapBoxNonAlloc(center, size * 0.5f + new Vector3(0.3f, 0.3f, 0.7f), occupancy,
                Quaternion.LookRotation(start.Tangent), ~0, QueryTriggerInteraction.Ignore);
            if (count == occupancy.Length) return false;
            for (int i = 0; i < count; i++)
                if (occupancy[i] && occupancy[i].GetComponentInParent<CityCarController>()) return false;
            return true;
        }
        private void OnArrival(CityCarController car) { Completed++; Destroy(car.gameObject); }
    }
}
