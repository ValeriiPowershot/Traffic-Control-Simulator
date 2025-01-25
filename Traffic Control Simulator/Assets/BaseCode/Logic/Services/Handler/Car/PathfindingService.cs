using System;
using BaseCode.Logic.Services.Interfaces.Car;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Ways;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BaseCode.Logic.Services.Handler.Car
{
    [Serializable]
    public class PathContainerService : IPathFindingService
    {
        private AllWaysContainer _allWaysContainer;
        private WaypointContainer _waypointContainer;
        public void Starter(CarManager carManager)
        {
           _allWaysContainer = carManager.allWaysContainer;
        }

        public void SetNewPathContainerRandomly()
        {
            _waypointContainer = GetPathRandom();
        }
        
        public WaypointContainer GetPathContainer(int pathIndex)
        {
            if (pathIndex < 0 || pathIndex >= _allWaysContainer.allWays.Length)
            {
                Debug.LogError("Invalid path index!");
                return null;
            }

            return _allWaysContainer.allWays[pathIndex];
       
        }
        public WaypointContainer GetPathContainer()
        {
            return _waypointContainer;
        }

        public Vector3 GetFirstPosition()
        {
            return GetPathContainer().roadPoints[0].point.transform.position;
        }

        public WaypointContainer GetPathRandom()
        {
            return GetPathContainer(Random.Range(0, _allWaysContainer.allWays.Length));
        }

        public RoadPoint GetIndexWaypoint(int i)
        {
            return GetPathContainer().roadPoints[i];
        }
    }
}