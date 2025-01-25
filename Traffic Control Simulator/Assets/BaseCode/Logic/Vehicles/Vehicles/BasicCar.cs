using BaseCode.Logic.ScriptableObject;
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

        public override void Starter(CarManager manager, VehicleScriptableObject currentCar)
        {
            base.Starter(manager, currentCar);
            VehicleController.Starter(this);
        }
        
        public virtual void Update()
            => VehicleController.Update();
        
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
            TurnLight.SetActive(true);
        }
    }

    public enum TurnType
    {
        None,
        Right,
        Left,
    }
}