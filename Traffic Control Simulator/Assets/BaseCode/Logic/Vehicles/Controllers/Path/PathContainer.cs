using System.Collections.Generic;
using BaseCode.Logic.Managers;
using BaseCode.Logic.Roads.RoadTool;
using BaseCode.Logic.Vehicles.States.Movement;
using BaseCode.Logic.Vehicles.Vehicles;
using BaseCode.Logic.Ways;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BaseCode.Logic.Vehicles.Controllers.Path
{
    public class PathContainer
    {
        private WaypointContainer _waypointContainer;
        private readonly CarManager _carManager;

        public PathContainer(VehicleBase vehicleBase)
        {
            _carManager = vehicleBase.CarManager;
        }

        public void SetNewPathContainerRandomly()
        {
            _waypointContainer = GetPathRandom();
        }
        
        public WaypointContainer GetPathContainer(int pathIndex)
        {
            if (pathIndex < 0 || pathIndex >= GetContainerList().Count)
            {
                Debug.Log("No Path Found!");
                return null;
            }

            return GetContainerList()[pathIndex];
       
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
            return GetPathContainer(Random.Range(0, GetContainerList().Count));
        }

        public RoadPoint GetIndexWaypoint(int i)
        {
            return GetPathContainer().roadPoints[i];
        }

        public CarDetector GetCarDetector()
        {
            return GetIndexWaypoint(0).point.GetChild(0).GetComponent<CarDetector>();
        }

        public List<WaypointContainer> GetContainerList()
        {
            int currentIndex = _carManager.CarSpawnServiceHandler.CarWaveController.CurrentLevelIndex;
            return _carManager.allWaysContainer.GetWayGroupByIndex(currentIndex);
        }
    }
}