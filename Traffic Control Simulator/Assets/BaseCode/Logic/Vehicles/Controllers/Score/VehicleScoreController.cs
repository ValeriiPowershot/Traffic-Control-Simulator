using BaseCode.Logic.Managers;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;

namespace BaseCode.Logic.Vehicles.Controllers.Score
{
    public class VehicleScoreController
    {
        private VehicleScriptableObject CarData { get; set; }

        private VehicleController _vehicleController;
        private VehicleBase _vehicleBase;

        public float acceptableWaitingTime;
        public float successPoints;

        public VehicleScoreController(VehicleController vehicleController)
        {
            _vehicleController = vehicleController;
            _vehicleBase = _vehicleController.VehicleBase;
            CarData = _vehicleBase.VehicleScriptableObject;
            
            CarScoreCalculator = _vehicleBase.GetComponent<CarScoreCalculatorBase>();
            CarScoreCalculator.Initialize(ScoringManager);
            
            RestartVehicleScore();
        }
        
        public void RestartVehicleScore(bool isDied = false)
        {
            acceptableWaitingTime = CarData.AcceptableWaitingTime;
            successPoints = CarData.SuccessPoints;
            CarScoreCalculator.OnReachedDestination(isDied);
        }
        
        public ScoringManager ScoringManager => _vehicleBase.CarManager.GameManager.scoringManager;
        public CarScoreCalculatorBase CarScoreCalculator { get; set; }
    }
}
