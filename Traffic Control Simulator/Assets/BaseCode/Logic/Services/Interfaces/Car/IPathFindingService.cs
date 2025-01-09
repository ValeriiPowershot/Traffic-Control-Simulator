using BaseCode.Logic.Ways;
using UnityEngine;

namespace BaseCode.Logic.Services.Interfaces.Car
{
    public interface IPathFindingService
    {
        public void SetNewPathContainerRandomly();
        public WaypointContainer GetPathContainer(int pathIndex);
        public WaypointContainer GetPathContainer();
        public Vector3 GetFirstPosition();
        public WaypointContainer GetPathRandom();
    }
}