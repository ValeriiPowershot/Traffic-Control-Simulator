using BaseCode.Interfaces;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Services.Handler;
using BaseCode.Logic.Vehicles.Controllers;
using Unity.VisualScripting;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class BasicCar : VehicleBase
    {
        public Transform emojiFxSpawnPoint;
        
        public GameObject TurnLight;
        public Transform RightTurn;
        public Transform LeftTurn;
        public Transform RayStartPoint;
        public Transform ArrowIndicatorEndPoint;
        public VehicleBlinker blinker;
        public override void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            base.Starter(manager, currentCar);
            VehicleController.Starter(this);
        }

        public virtual void Update()
        {
            VehicleController.Update();
        }

        public override void AssignNewPathContainer()
        {
            PathContainerService.SetNewPathContainerRandomly();
            
            transform.SetPositionAndRotation(PathContainerService.GetFirstPosition(), Quaternion.identity);
            
            VehicleController.StateController.InitializePath();
        }

        public void ShowTurn(TurnType turnType)
        {
            switch (turnType)
            {
                case TurnType.None:
                    TurnLight.SetActive(false);
                    blinker.isNone = true;
                    break;
                case TurnType.Right:
                    SetTurnLight(RightTurn.position);
                    break;
                case TurnType.Left:
                    SetTurnLight(LeftTurn.position);
                    break;
            }
        }
        private void SetTurnLight(Vector3 pos)
        {
            TurnLight.transform.position = pos;
            Debug.Log("Hel");
            blinker.isNone = false;
        }
    }

    public enum TurnType
    {
        None,
        Right,
        Left,
    }
}