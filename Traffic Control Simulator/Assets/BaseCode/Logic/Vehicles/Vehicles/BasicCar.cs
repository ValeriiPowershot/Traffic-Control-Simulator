using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.States.Movement;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class BasicCar : VehicleBase
    {
        public Transform emojiFxSpawnPoint;

        public Transform RayStartPoint;
        public Transform ArrowIndicatorEndPoint;
        public VechicleTurnLights VechicleTurnLights;

        private bool _needToCheck;
        
        public override void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            base.Starter(manager, currentCar);
            VehicleController.Starter(this);
        }

        public virtual void Update()
        {
            VehicleController.Update();

            CheckTurnLightState();
        }
        
        private void CheckTurnLightState()
        {
            if (NeedToTurn)
            {
                float rotationY = ArrowIndicatorEndPoint.localRotation.eulerAngles.y;
                
                if (rotationY > 180) 
                    rotationY -= 360;
                
                switch (rotationY)
                {
                    case > 20:
                        VehicleController.VehicleBase.VechicleTurnLights.ShowTurnLight(Indicator.Right);
                        break;
                    case < -20:
                        VehicleController.VehicleBase.VechicleTurnLights.ShowTurnLight(Indicator.Left);
                        break;
                    default:
                        VehicleController.VehicleBase.VechicleTurnLights.StopTurnSignals();
                        break;
                }
            }
        }


        public override void AssignNewPathContainer()
        {
            PathContainerService.SetNewPathContainerRandomly();

            transform.SetPositionAndRotation(PathContainerService.GetFirstPosition(), Quaternion.identity);

            VehicleController.StateController.InitializePath();
        }
    }
}