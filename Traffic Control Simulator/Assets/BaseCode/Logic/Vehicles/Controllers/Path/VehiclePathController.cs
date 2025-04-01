using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Path
{
    public class VehiclePathController
    {
        private readonly VehicleController _vehicleController;
        private readonly PathPointsContainerController _pathPointController;
        public VehiclePathController(VehicleController vehicleController)
        {
            _vehicleController = vehicleController;
            _pathPointController = new PathPointsContainerController(this);
        }
        public void InitializeNewPath()
        {
            PathContainer.SetNewPathContainerRandomly();
            _pathPointController.AssignFoundPath();
            AssignRotationPosition(PathContainer.GetFirstPosition());
        }
        public void ProceedToNextWaypoint()
        {
            if (_pathPointController.CanProceedToEndCar())
                OnReachDestination();
        }
        private void OnReachDestination()
        {
            SetPathToEndPosition();
            VehicleController.VehicleBase.DestinationReached();
        }
        public void SetPathToEndPosition(Vector3 originalY)
        {
            SetPathToEndPosition();
            CarTransform.localScale= originalY;
        }
        public void SetPathToEndPosition()
        {
            _pathPointController.OnReachDestination();
        }     
        
        private void AssignRotationPosition(Vector3 position, Quaternion rotation = default)
        {
            _vehicleController.CarTransform.SetPositionAndRotation(position,rotation);
        }
        public Transform CarTransform => _vehicleController.CarTransform;
        public PathContainer PathContainer => _pathPointController.PathContainer;
        public PathPointsContainerController PathPointController => _pathPointController;
        public VehicleController VehicleController => _vehicleController;
    }


}