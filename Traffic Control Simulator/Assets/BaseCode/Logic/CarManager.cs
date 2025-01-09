using BaseCode.Logic.ScoringSystem;
using BaseCode.Logic.Ways;
using System.Collections.Generic;
using BaseCode.Interfaces;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles;
using UnityEngine;

namespace BaseCode.Logic
{
    public class CarManager : MonoBehaviour
    {
        [SerializeField] private Transform carHandler;
        [SerializeField] private VehicleScriptableObject[] carSoObjects;
        [SerializeField] private AllWaysContainer _allWaysContainer;
        [Space]
        [SerializeField] private int _activeCarsCount= 5, _maxCarsCount = 7;
        [SerializeField] private float _timeToSpawn;
        [SerializeField] private ScoringManager _scoreManager;

        private float _spawnTimer;
        private List<BasicCar> _active = new();
        private List<BasicCar> _hided = new();

        private void Update()
        {
            if((_active.Count + _hided.Count) < _maxCarsCount)
            {
                if(_active.Count < _activeCarsCount && Time.time >= _spawnTimer)
                {
                   // print(_active.Count + _hided.Count); // spent 10 min to find this print 
                    SpawnNewCar();
                }
            }
        }

        private void SpawnNewCar()
        {
            _spawnTimer = Time.time + _timeToSpawn;
            BasicCar newCar;

            if(_hided.Count > 0)
            {
                newCar = _hided[0];
                _hided.RemoveAt(0);
                newCar.gameObject.SetActive(true);
            }
            else
            {
                newCar = CreateNewCar();
            }

            //every car gets new position and path after recycle
            WaypointContainer container = _allWaysContainer.AllWays[Random.Range(0, _allWaysContainer.AllWays.Length)];
            newCar.transform.SetPositionAndRotation(container.roadPoints[0].point.transform.position, Quaternion.identity);
            newCar.WaypointContainer = container;

            _active.Add(newCar);
            newCar.AssignNewPathContainer();
        }

        private BasicCar CreateNewCar()
        {
            VehicleScriptableObject currentCar = carSoObjects[Random.Range(0, carSoObjects.Length)];

            GameObject createdCar = Instantiate(currentCar.vehiclePrefab,
                carHandler);
                
            var newCar = createdCar.GetComponent<BasicCar>();

            _scoreManager.AddCar(newCar.GetComponent<IScoringObject>());
            newCar.Starter(this, _allWaysContainer, currentCar);

            return newCar;
        }

        public void CarDestinationReached(BasicCar basicCar)
        {
            _active.Remove(basicCar);
            basicCar.gameObject.SetActive(false);
            _hided.Add(basicCar);
        }
    }
}
