using System.Collections.Generic;
using BaseCode.Logic.Ways;
using Script.ScriptableObject;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BaseCode.Logic.CarMovement
{
    // i moved implementation to go state
    public class CarMovement : MonoBehaviour
    {
        [SerializeField] private AllWaysContainer _allWaysContainer;
        [SerializeField] private List<Transform> _waypoints;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;
    
        private WaypointContainer _waypointContainer;
        private int _currentSlowdownPointIndex;
        private int _currentAccelerationPointIndex;
        private int _currentWaypointIndex;
        
        private void OnEnable()
        {
            _waypointContainer = _allWaysContainer.AllWays[Random.Range(0, _allWaysContainer.AllWays.Length)];
            _waypoints = _waypointContainer.waypoints;
           // _currentSlowdownPointIndex = _waypointContainer.SlowdownPointIndex;
           // _currentAccelerationPointIndex = _waypointContainer.AccelerationPointIndex;
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
            if (_currentWaypointIndex == _currentSlowdownPointIndex + 1)
                _speed = 4;
            else if (_currentWaypointIndex == _currentAccelerationPointIndex + 1) 
                _speed = 6;
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
                _rotationSpeed * Time.fixedDeltaTime
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
