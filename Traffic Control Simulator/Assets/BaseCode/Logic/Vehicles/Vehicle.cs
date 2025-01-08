using System;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.States;
using BaseCode.Logic.Ways;
using UnityEngine;
using UnityEngine.Serialization;


namespace BaseCode.Logic.Vehicles
{
    public class Vehicle : BasicCar
    {
        [SerializeField] private VehicleController vehicleController;
        public GameObject TurnLight;
        public Transform RightTurn;
        public Transform LeftTurn;
        //public AllWaysContainer allWaysContainer;
        public Transform RayStartPoint;
        public Transform ArrowIndicatorEndPoint;
        public WaypointContainer WaypointContainer { get; set; }
        public VehicleScriptableObject VehicleScriptableObject { get; set; }

        public void Starter(CarManager Manager, AllWaysContainer Container, VehicleScriptableObject currentCar)
        {
            //allWaysContainer = Container;
            CarManager = Manager;
            VehicleScriptableObject = currentCar;
            
            vehicleController.Starter(this);
        }
        
        public virtual void Update()
            => vehicleController.Update();

        public override void PassLightState(LightState state)
        {
            base.PassLightState(state);

            switch (state)
            {
                case LightState.Green:
                    vehicleController.SetState<VehicleGoState>();
                    break;
                case LightState.Red:
                    //vehicleController.SetState<VehicleStopState>();
                    break;
                case LightState.Yellow:
                    vehicleController.SetState<VehicleSlowDownState>();
                    break;
                case LightState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        public virtual void AssignNewPathContainer()
        {
            vehicleController.StateController.InitializePath();
        }
        
        public virtual void DestinationReached()
        {
            CarManager.CarDestinationReached(this);
        }

        public void ShowTurn(TurnType TurnType)
        {
            switch (TurnType)
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