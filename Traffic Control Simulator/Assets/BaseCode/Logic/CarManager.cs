using BaseCode.Logic.ScoringSystem;
using BaseCode.Logic.Ways;
using Script.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace BaseCode.Logic
{
    public class CarManager : MonoBehaviour
    {
        [SerializeField] private Transform carHandler;
        [SerializeField] private Vehicle[] _carPrefabs;
        [SerializeField] private AllWaysContainer _allWaysContainer;
        [Space]
        [SerializeField] private int ACTIVE_CARS_COUNT = 5, MAX_CARS_COUNT = 7;
        [SerializeField] private float TIME_TOSPAWN = 12f;
        [SerializeField] private ScoringManager _scoreManager;

        private float _spawnTimer;
        private List<Vehicle> _active = new();
        private List<Vehicle> _hided = new();

        private void Update()
        {
            if((_active.Count + _hided.Count) < MAX_CARS_COUNT)
            {
                if(_active.Count < ACTIVE_CARS_COUNT && Time.time >= _spawnTimer)
                {
                    print(_active.Count + _hided.Count);
                    SpawnNewCar();
                }
            }
        }

        private void SpawnNewCar()
        {
            _spawnTimer = Time.time + TIME_TOSPAWN;
            Vehicle newCar;

            if(_hided.Count > 0)
            {
                newCar = _hided[0];
                _hided.RemoveAt(0);
                newCar.gameObject.SetActive(true);
            }
            else
            {
                //instatiate car and put in first waypoint position on a random path;
                newCar = Instantiate(_carPrefabs[Random.Range(0, _carPrefabs.Length)], _allWaysContainer.AllWays[Random.Range(0, _allWaysContainer.AllWays.Length)].waypoints[0].transform.position, Quaternion.identity, carHandler);
                _scoreManager.AddCar(newCar.GetComponent<IScoringObject>());
                newCar.Starter(this, _allWaysContainer);
            }

            _active.Add(newCar);
            newCar.InitializePath();
        }

        public void CarDestinationReched(Vehicle Vehicle)
        {
            _active.Remove(Vehicle);
            Vehicle.gameObject.SetActive(false);
            _hided.Add(Vehicle);
        }
    }
}
