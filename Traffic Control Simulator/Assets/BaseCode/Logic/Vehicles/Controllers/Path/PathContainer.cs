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
        private AllWaysContainer _allWaysContainer;
        private WaypointContainer _waypointContainer;

        public PathContainer(VehicleBase vehicleBase)
        {
           _allWaysContainer = vehicleBase.CarManager.allWaysContainer;
        }

        public void SetNewPathContainerRandomly()
        {
            _waypointContainer = GetPathRandom();
        }
        
        public WaypointContainer GetPathContainer(int pathIndex)
        {
            if (pathIndex < 0 || pathIndex >= _allWaysContainer.allWays.Length)
            {
                Debug.Log("No Path Found!");
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

        public CarDetector GetCarDetector()
        {
            return GetIndexWaypoint(0).point.GetChild(0).GetComponent<CarDetector>();
        }
    }
}