using System;
using BaseCode.Logic;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Ways;
using Script.ScriptableObject;
using Script.Vehicles.Controllers;
using Script.Vehicles.States;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace Script.Vehicles
{
    public class Vehicle : BasicCar
    {
        [SerializeField] private VehicleController vehicleController;
        [SerializeField] private GameObject _turnLight;
        [SerializeField] private Transform _rightTurn, _leftTurn;
        //public AllWaysContainer allWaysContainer;
        public Transform rayStartPoint;
        public Transform arrowIndicatorEndPoint;
        public WaypointContainer WaypointContainer { get; set; }
        public VehicleScriptableObject VehicleScriptableObject { get; set; }

        public void Starter(CarManager Manager, AllWaysContainer Container, VehicleScriptableObject currentCar)
        {
            //allWaysContainer = Container;
            CarManager = Manager;
            VehicleScriptableObject = currentCar;
            
            vehicleController.Starter(this);
        }
        
        public virtual void Update() => vehicleController.Update();

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
                    _turnLight.SetActive(false);
                    break;
                case TurnType.Right:
                    SetTurnLight(_rightTurn.position);
                    break;
                case TurnType.Left:
                    SetTurnLight(_leftTurn.position);
                    break;
            }
        }

        private void SetTurnLight(Vector3 pos)
        {
            _turnLight.transform.position = pos;
            _turnLight.SetActive(true);
        }
    }

    public enum TurnType
    {
        None,
        Right,
        Left,
    }
}