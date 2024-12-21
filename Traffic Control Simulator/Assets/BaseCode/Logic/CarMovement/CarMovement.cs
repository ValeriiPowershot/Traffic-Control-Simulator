using System.Collections.Generic;
using BaseCode.Logic.Ways;
using Script.ScriptableObject;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BaseCode.Logic.CarMovement
{
    public class CarMovement : MonoBehaviour
    {
        [SerializeField] private VehicleScriptableObject carData; 
        [SerializeField] private AllWaysContainer _allWaysContainer;
    
        public WaypointContainer waypointContainer;
        private List<Transform> _waypoints = new List<Transform>();

        private int _currentWaypointIndex;
        private float _speed;

        private void OnEnable()
        {
           // _waypointContainer = _allWaysContainer.AllWays[Random.Range(0, _allWaysContainer.AllWays.Length)];
            _waypoints = waypointContainer.Waypoints; 
        }

        private void FixedUpdate()
        {
            if (!HasWaypoints()) return;

            if (IsAtFinalWaypoint()) return;

            AdjustSpeed();

            MoveTowardsWaypoint();
            RotateTowardsWaypoint();

            if (IsCloseToWaypoint())
                ProceedToNextWaypoint();
        }

        private void AdjustSpeed()
        {
            _speed = carData.Speed;
        }

        private bool HasWaypoints()
        {
            return _waypoints.Count > 0;
        }

        private bool IsAtFinalWaypoint()
        {
            return _currentWaypointIndex == _waypoints.Count;
        }

        private void MoveTowardsWaypoint()
        {
            Transform targetWaypoint = _waypoints[_currentWaypointIndex];
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetWaypoint.position, 
                _speed * Time.fixedDeltaTime
            );
        }

        private void RotateTowardsWaypoint()
        {
            Transform targetWaypoint = _waypoints[_currentWaypointIndex];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                carData.RotationSpeed * Time.fixedDeltaTime
            );
        }

        private bool IsCloseToWaypoint()
        {
            Transform targetWaypoint = _waypoints[_currentWaypointIndex];
            return Vector3.Distance(transform.position, targetWaypoint.position) < 0.5f;
        }

        private void ProceedToNextWaypoint()
        {
            _currentWaypointIndex++;

            if (_currentWaypointIndex >= _waypoints.Count)
            {
                Debug.Log("Final waypoint reached");
            }
        }
    }
}
